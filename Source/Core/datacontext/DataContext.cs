using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace linqtoweb.Core.datacontext
{
    /// <summary>
    /// Page source data context, where the extraction method is called.
    /// Used by extraction method.
    /// Contains
    /// - information about the page (data source).
    /// - cache object
    /// - extraction techniques (methods like RegExp, DOM, ...)
    /// - methods for creating new DataContexts
    /// - downloading and processing methods
    /// </summary>
    public class DataContext
    {
        // TODO: internal cache object

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext(/*cache*/)
        {
            
        }

        /// <summary>
        /// Create new action and adds it into the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="method"></param>
        internal void AddAction( MethodsContainer.ExtractionMethod method, LocalVariables parameters )
        {
            // create the action
            ActionItem newAction = new ActionItem(method, this, parameters);

            // add the action to all objects specified within the action parameters
            parameters.AddActionToParameters(newAction);
        }

        #region new data contexts

        /// <summary>
        /// New HTML page context.
        /// </summary>
        /// <param name="relativeurl"></param>
        /// <returns></returns>
        public DataContext OpenHtml( string relativeurl )
        {
            return new HtmlContext(this, relativeurl);
        }

        #endregion

        #region data source information

        /// <summary>
        /// The data content URI identifier.
        /// </summary>
        public virtual Uri URI
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Data content.
        /// </summary>
        public virtual string Content
        {
            get
            {
                throw new NotExtractedDataException("Content is not available in the current context.");
            }
        }

        #endregion

        #region extraction methods (RegExp, DOM, ...)

        public class RegExpEnumerator:IEnumerable< LocalVariables >
        {
            private readonly Regex exp;
            private readonly DataContext datacontext;

            public RegExpEnumerator( DataContext datacontext, Regex exp )
            {
                Debug.Assert(datacontext != null, "Regular expression must be executed on some data context.");
                Debug.Assert(exp != null, "Given regular expression cannot be null.");

                this.datacontext = datacontext;
                this.exp = exp;
            }

            #region IEnumerable<Dictionary<string,string>> Members

            public IEnumerator<LocalVariables> GetEnumerator()
            {
                // enumerate the matches
                foreach (Match m in exp.Matches(datacontext.Content))
                {
                    // collection of matched variables
                    LocalVariables values = new LocalVariables();

                    for (int i = 0; i < m.Groups.Count; ++i)
                    {
                        string groupname = exp.GroupNameFromNumber(i);

                        values[groupname] = m.Groups[i].Value;
                    }

                    yield return values;
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
        /// Matches given regular expression onto the current Content.
        /// </summary>
        /// <param name="exp">Regular expression with .NET group names (?&lt;GroupName&gt;\w*).</param>
        /// <returns>Enumerator of dictionary of matched groups (variables).</returns>
        public RegExpEnumerator regexp(string exp)
        {
            // regular expression to be matched
            Regex regularExpression = new Regex(exp, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return regexp(regularExpression);
        }

        /// <summary>
        /// Matches given regular expression onto the current Content.
        /// </summary>
        /// <param name="exp">Regular expression with .NET group names (?&lt;GroupName&gt;\w*). </param>
        /// <returns>Enumerator of dictionary of matched groups (variables).</returns>
        public RegExpEnumerator regexp(Regex exp)
        {
            return new RegExpEnumerator(this, exp);
        }

        #endregion

        #region helper methods (downloading, processing)

        protected string ReadWebRequest( Uri uri )
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            req.Timeout = 15000;
            req.AllowAutoRedirect = true;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; YPC 3.0.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            req.Referer = "";
            req.Headers.Add("Accept-Language", "en,cs");

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd();
        }

        #endregion
    }
}
