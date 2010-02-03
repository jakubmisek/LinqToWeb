using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Statement, encapsulates an expression.
    /// </summary>
    public class Statement : Expression
    {
        public readonly Expression ExpressionInside;

        public Statement(ExprPosition position, Expression expr)
            : base(position)
        {
            ExpressionInside = expr;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            if (ExpressionInside != null)
            {
                codecontext.Write(string.Empty, codecontext.Level);
                ExpressionInside.EmitCs(codecontext);
                codecontext.Write(";");
            }

            return ExpressionType.VoidType;
        }
    }
}
