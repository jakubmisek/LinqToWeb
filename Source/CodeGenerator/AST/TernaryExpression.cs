using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    /*public class IfExpression:Expression
    {

    }*/

    /// <summary>
    /// expr ? expr : expr
    /// </summary>
    public class TernaryCondExpression : Expression
    {
        public readonly Expression ConditionExpr, Expr1, Expr2;

        public TernaryCondExpression(ExprPosition position, Expression condition, Expression expr1, Expression expr2)
            :base(position)
        {
            this.ConditionExpr = condition;
            this.Expr1 = expr1;
            this.Expr2 = expr2;
        }
    }
}
