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
            private readonly XmlNodeList pattern;
            private readonly HtmlNode root;
            private readonly bool deepSearch;
            private readonly int firstRootChild, firstPatternNode;

            public XmlMatchEnumerator(HtmlNode root, XmlNodeList pattern )
                :this(root, 0, pattern, 0, true)
            {
            }

            protected XmlMatchEnumerator(HtmlNode root, int firstRootChild, XmlNodeList pattern, int firstPatternNode, bool deepSearch)
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
            }

            #region helper

            private static bool IsMatch(HtmlNode node, XmlNode patternNode, LocalVariables loc)
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
                if (node.Name == "#text")
                {
                    var vars = new RegExpEnumerator(node.InnerText, new Regex(PatternToRegexp(patternNode.InnerText, true), RegexOptions.Multiline | RegexOptions.IgnoreCase)).FirstOrDefault();
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
                        HtmlNode rootNode = root.ChildNodes[i];

                        LocalVariables loc = new LocalVariables();

                        if (IsMatch(rootNode, patternNode, loc))  // TODO: for each match
                        {
                            // child variations
                            List<LocalVariables> childVariations = new List<LocalVariables>();

                            if (patternNode.HasChildNodes)
                            {   // child variations
                                foreach (var x in new XmlMatchEnumerator(rootNode, 0, patternNode.ChildNodes, 0, false))
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
                                    foreach (var x in new XmlMatchEnumerator(root, i + 1, pattern, firstPatternNode + 1, false))
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
                            foreach (var x in new XmlMatchEnumerator(rootNode, 0, pattern, firstPatternNode, true))
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