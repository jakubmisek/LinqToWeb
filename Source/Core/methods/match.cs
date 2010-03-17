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
        private static string PatternCharToRegexp(char c)
        {
            switch (c)
            {
                case ' ':
                    return "\\s";
                case '\"':
                case '\'':
                case '<':
                case '>':
                case '(':
                case ')':
                case '{':
                case '}':
                case '\\':
                case '?':
                case '.':
                case '[':
                case ']':
                case '^':
                case '+':
                case '*':
                case '$':
                case ',':
                case '|':
                case '#':
                    return "\\" + c;
                default:
                    return c.ToString();
            }
        }

        private static bool TryMatchPatternIdentifier(string pattern, ref int i, out string identifier, out string exp)
        {
            identifier = null;
            exp = null;

            if (i + 1 < pattern.Length && pattern.Substring(i, 2) == "~@")  // ~@ 
            {
                int start = i + 2;
                int end = pattern.IndexOf("@~", start);
                if (end < 0)
                    return false;

                i = end + 2;

                string content = pattern.Substring(start, end - start);
                string[] args = content.Split(new char[] { ',' });
                if (args.Length > 2)
                    return false;

                if (args.Length >= 1) identifier = args[0];
                if (args.Length >= 2) exp = args[1];

                return true;
            }

            return false;
        }

        /// <summary>
        /// Matches given pattern onto the current Content.
        /// </summary>
        /// <param name="context">Data context.</param>
        /// <param name="exp">Pattern string. ~@IDENTIFIER@~ indicates variable name.</returns>
        public static RegExpEnumerator match(DataContext context, string pattern)
        {
            StringBuilder resultregexp = new StringBuilder(pattern.Length);

            string identifier, exp;

            for (int i = 0; i < pattern.Length; ++i )
            {
                if (TryMatchPatternIdentifier(pattern, ref i, out identifier, out exp))
                {   // ~@identifier,regexp@~
                    --i;

                    if (exp == null)
                        exp = ".*?";

                    resultregexp.Append("(?<" + identifier + ">" + exp + ")");
                }
                else
                {
                    resultregexp.Append(PatternCharToRegexp(pattern[i]));
                }
            }

            return regexp(context, resultregexp.ToString());
        }
        
    }

}