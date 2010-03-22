using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class GeneratorException : Exception
    {
        /// <summary>
        /// The exception position.
        /// </summary>
        public readonly ExprPosition Position;

        /// <summary>
        /// Init exception.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        public GeneratorException(ExprPosition position, string message)
            : base(message)
        {
            this.Position = position;
        }
    }
}
