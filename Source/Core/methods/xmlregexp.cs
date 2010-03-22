using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using linqtoweb.Core.datacontext;

using System.Diagnostics;
using System.Xml;
using HtmlAgilityPack;

namespace linqtoweb.Core.methods
{
    /// <summary>
    /// Methods of data context.
    /// </summary>
    public partial class ForeachMethods
    {
        /// <summary>
        /// RegExp enumerator.
        /// </summary>
        public class XmlMatchEnumerator : IEnumerable<LocalVariables>
        {
            /// <summary>
            /// Root node of the document child elements.
            /// </summary>
            private readonly HtmlNode root;

            /// <summary>
            /// Pattern to match with document child elements.
            /// </summary>
            private readonly XmlNodeList pattern;

            /// <summary>
            /// Perform deep search.
            /// </summary>
            private readonly bool deepSearch;

            /// <summary>
            /// Start matching from these indexes.
            /// </summary>
            private readonly int firstRootChild, firstPatternNode;

            /// <summary>
            /// Indexes of root child nodes to be skipped - they was already matched with previoud pattern nodes.
            /// </summary>
            private readonly HashSet<int> rootSkip;

            /// <summary>
            /// Empty HashSet.
            /// </summary>
            private static HashSet<int> rootSkipEmpty = new HashSet<int>();

            public XmlMatchEnumerator(HtmlNode root, XmlNodeList pattern )
                :this(root, 0, null, pattern, 0, true)
            {
            }

            protected XmlMatchEnumerator(HtmlNode root, int firstRootChild, HashSet<int> rootSkip, XmlNodeList pattern, int firstPatternNode, bool deepSearch)
            {
                Debug.Assert(root != null, "XML cannot be null.");
                Debug.Assert(pattern != null, "Given pattern cannot be null.");
                Debug.Assert(firstRootChild >= 0);
                Debug.Assert(firstPatternNode >= 0 && firstPatternNode < pattern.Count);

                this.root = root;
                this.pattern = pattern;
                this.deepSearch = deepSearch;
                this.firstRootChild = firstRootChild;
                this.firstPatternNode = firstPatternNode;
                this.rootSkip = rootSkip ?? rootSkipEmpty;
            }

            #region helper

            /// <summary>
            /// Create copy of rootSkip and add specified element.
            /// </summary>
            /// <param name="i">Element to add to the collection of indexes to skip.</param>
            /// <returns>New collection of indexes.</returns>
            private HashSet<int> Unify(int i)
            {
                HashSet<int> nums = new HashSet<int>(rootSkip);
                nums.Add(i);

                return nums;
            }

            /// <summary>
            /// Try to match the document node and pattern node. Does not match child nodes.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="patternNode"></param>
            /// <param name="loc"></param>
            /// <returns></returns>
            private static bool IsMatch(HtmlNode node, XmlNode patternNode, LocalVariables loc, bool patternHasSiblings)
            {
                if (node == null || patternNode == null || node.Name != patternNode.Name)
                    return false;

                // fill loc with matched variables

                // attributes
                if (patternNode.Attributes != null)
                foreach (XmlAttribute patAttr in patternNode.Attributes)
                {
                    string val = node.GetAttributeValue(patAttr.Name, (string)null);

                    if (val == null) return false;

                    var vars = new RegExpEnumerator(val, new Regex(PatternToRegexp(patAttr.Value, true), RegexOptions.Multiline | RegexOptions.IgnoreCase)).FirstOrDefault();
                    if (vars == null) return false;

                    loc.AddVariables(vars);
                }

                // InnerText of Text node
                if (node.Name == HtmlNode.HtmlNodeTypeNameText)
                {
                    string nodeInnerText = (patternHasSiblings ? node.InnerText : node.ParentNode.InnerHtml);   // take all the inner text if pattern has not specified another elements on the same level

                    var vars = new RegExpEnumerator(nodeInnerText, new Regex(PatternToRegexp(patternNode.InnerText, true), RegexOptions.Multiline | RegexOptions.IgnoreCase)).FirstOrDefault();
                    if (vars == null) return false;

                    loc.AddVariables(vars);
                }

                return true;
            }

            #endregion

            #region IEnumerable<LocalVariables> Members

            public IEnumerator<LocalVariables> GetEnumerator()
            {
                if (firstRootChild < root.ChildNodes.Count)
                {
                    XmlNode patternNode = pattern[firstPatternNode];

                    for (int i = firstRootChild; i < root.ChildNodes.Count; ++i)
                    {
                        if (rootSkip.Contains(i))
                            continue;

                        HtmlNode rootNode = root.ChildNodes[i];

                        LocalVariables loc = new LocalVariables();

                        if (IsMatch(rootNode, patternNode, loc, pattern.Count > 1))  // TODO: for each match
                        {
                            // child variations
                            List<LocalVariables> childVariations = new List<LocalVariables>();

                            if (patternNode.HasChildNodes)
                            {   // child variations
                                foreach (var x in new XmlMatchEnumerator(rootNode, 0, null, patternNode.ChildNodes, 0, false)) // TODO: allow deep search for child elements ?
                                    childVariations.Add(x);
                            }
                            else
                            {
                                childVariations.Add(new LocalVariables());
                            }

                            if (childVariations.Count > 0)
                            {
                                // rest of the pattern variations
                                List<LocalVariables> patternRestVariations = new List<LocalVariables>();

                                if (firstPatternNode + 1 < pattern.Count)   // rest of the pattern on the same level
                                {
                                    foreach (var x in new XmlMatchEnumerator(root, 0/*(i+1) to keep order too as it is in pattern*/, Unify(i), pattern, firstPatternNode + 1, false))
                                        patternRestVariations.Add(x);
                                }
                                else
                                {
                                    patternRestVariations.Add(new LocalVariables());
                                }

                                // combine variations,
                                // if any of lists is empty, nothing is yield_returned
                                foreach (var y in patternRestVariations)    // can be empty, then returns nothing immediately
                                {
                                    loc.AddVariables(y);

                                    foreach (var x in childVariations)
                                    {
                                        loc.AddVariables(x);
                                        yield return loc;
                                    }
                                }
                            }
                        }

                        // perform deep search
                        if (deepSearch && rootNode.HasChildNodes)
                        {
                            foreach (var x in new XmlMatchEnumerator(rootNode, 0, null, pattern, firstPatternNode, true))
                                yield return x;
                        }
                    }
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var x in this)
                {
                    yield return (object)x;
                }
            }

            #endregion
        }

        /// <summary>
        /// Matches given XML pattern onto the current Content.
        /// </summary>
        /// <param name="context">Data context.</param>
        /// <param name="exp">XML extractor pattern with.</param>
        /// <returns>Enumerator of dictionary of matched groups (variables).</returns>
        public static XmlMatchEnumerator xmlmatch(DataContext context, string exp)
        {
            XmlDocument pattrnXml = new XmlDocument();
            HtmlDocument dataXml = new HtmlDocument();
            
            try
            {
                pattrnXml.LoadXml(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><pattern>" +
                    exp +
                    "</pattern>"
                    );

                dataXml.LoadHtml(context.Content);
            }
            catch
            {
                throw;
            }

            // get matched variables
            return new XmlMatchEnumerator(dataXml.DocumentNode, pattrnXml.DocumentElement.ChildNodes);
        }
    }

}