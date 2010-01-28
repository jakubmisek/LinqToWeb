using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    class TypeCastExpression : Expression
    {
        public readonly ExpressionType NewType;
        public readonly Expression Expr;

        public TypeCastExpression(ExprPosition position, ExpressionType newType, Expression expr)
            : base(position)
        {
            this.NewType = newType;
            this.Expr = expr;
        }
    }
}
