using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using linqtoweb.Core.datacontext;

using System.Diagnostics;
using System.Net;
using System.IO;

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
        public class RegExpEnumerator : IEnumerable<LocalVariables>
        {
            private readonly Regex exp;
            private readonly string content;

            public RegExpEnumerator(string content, Regex exp)
            {
                Debug.Assert(content != null, "Regular expression must be executed on some data context.");
                Debug.Assert(exp != null, "Given regular expression cannot be null.");

                this.content = content.Replace("\r", string.Empty);
                this.exp = exp;
            }

            #region IEnumerable<Dictionary<string,string>> Members

            public IEnumerator<LocalVariables> GetEnumerator()
            {
                //IDataContextContent content = datacontext as IDataContextContent;

                // enumerate the matches
                foreach (Match m in exp.Matches(content))
                {
                    if (m.Index > 0 && m.Index == content.Length)
                        continue;   // empty match

                    // collection of matched variables
                    LocalVariables values = new LocalVariables();

                    for (int i = 1; i < m.Groups .Count; ++i)   // skip the first group, it's the whole match
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
        /// <param name="context">Data context.</param>
        /// <param name="exp">Regular expression with .NET group names (?&lt;GroupName&gt;\w*).</param>
        /// <returns>Enumerator of dictionary of matched groups (variables).</returns>
        public static RegExpEnumerator regexp(DataContext context, string exp)
        {
            // regular expression to be matched
            Regex regularExpression = new Regex(exp, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return regexp(context, regularExpression);
        }

        /// <summary>
        /// Matches given regular expression onto the current Content.
        /// </summary>
        /// <param name="context">Data context.</param>
        /// <param name="exp">Regular expression with .NET group names (?&lt;GroupName&gt;\w*). </param>
        /// <returns>Enumerator of dictionary of matched groups (variables).</returns>
        public static RegExpEnumerator regexp(DataContext context, Regex exp)
        {
            // TODO: cache the results
            return new RegExpEnumerator(context.Content, exp);
        }
    }

}