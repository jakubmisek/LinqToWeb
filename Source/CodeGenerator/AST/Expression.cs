using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Base AST node representing any expression.
    /// </summary>
    public class Expression
    {
        /// <summary>
        /// The position in the source code.
        /// </summary>
        public ExprPosition Position { get; private set; }

        /// <summary>
        /// The type of resulting expression.
        /// </summary>
        public virtual ExpressionType ResultType
        {
            get
            {
                return ExpressionType.VoidType;
            }
        }

        /// <summary>
        /// Constructs the expression node.
        /// </summary>
        /// <param name="position"></param>
        public Expression(ExprPosition position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="level">Code indent level.</param>
        public virtual void EmitCs( StreamWriter output, int level )
        {
            throw new NotImplementedException();
        }
    }
}
