using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Block of code.
    /// </summary>
    public class CodeBlock : Expression
    {
        /// <summary>
        /// Expressions in the code block.
        /// </summary>
        public List<Expression> Statements { get; protected set; }

        public readonly LinkedList<MethodCall> DataContexts = new LinkedList<MethodCall>();

        // TODO: Chain of DataContexts

        public CodeBlock(ExprPosition position, List<Expression> statements)
            :base(position)
        {
            this.Statements = (statements != null) ? statements : new List<Expression>();
        }
    }
    
}
