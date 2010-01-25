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
    /// Methods of data context.
    /// </summary>
    public partial class DataContext
    {
        #region regular expressions

        public class RegExpEnumerator : IEnumerable<LocalVariables>
        {
            private readonly Regex exp;
            private readonly DataContext datacontext;

            public RegExpEnumerator(DataContext datacontext, Regex exp)
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
    }

}