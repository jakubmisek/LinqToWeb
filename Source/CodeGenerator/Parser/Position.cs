using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QUT.Gppg;

namespace linqtoweb.CodeGenerator
{
    /// <summary>
    /// This is the class that carries location
    /// information from the scanner to the parser.
    /// </summary>
    public class ExprPosition : IMerge<ExprPosition>
    {
        /// <summary>
        /// The line at which the text span starts.
        /// </summary>
        public int StartLine { get; private set; }

        /// <summary>
        /// The column at which the text span starts.
        /// </summary>
        public int StartColumn { get; private set; }

        /// <summary>
        /// The line on which the text span ends.
        /// </summary>
        public int EndLine { get; private set; }

        /// <summary>
        /// The column of the first character
        /// beyond the end of the text span.
        /// </summary>
        public int EndColumn { get; private set; }

        /// <summary>
        /// Default no-arg constructor.
        /// </summary>
        public ExprPosition()
        { }

        /// <summary>
        /// Constructor for text-span with given start and end.
        /// </summary>
        /// <param name="sl">start line</param>
        /// <param name="sc">start column</param>
        /// <param name="el">end line </param>
        /// <param name="ec">end column</param>
        public ExprPosition(int sl, int sc, int el, int ec)
        {
            StartLine = sl;
            StartColumn = sc;
            EndLine = el;
            EndColumn = ec;
        }

        /// <summary>
        /// Create a text location which spans from the 
        /// start of "this" to the end of the argument "last"
        /// </summary>
        /// <param name="last">The last location in the result span</param>
        /// <returns>The merged span</returns>
        public ExprPosition Merge(ExprPosition last)
        {
            return new ExprPosition(this.StartLine, this.StartColumn, last.EndLine, last.EndColumn);
        }
    }
}
