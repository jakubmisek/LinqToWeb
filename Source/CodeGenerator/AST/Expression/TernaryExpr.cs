using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
            if (condition == null || expr1 == null || expr2 == null)
                throw new ArgumentNullException();

            this.ConditionExpr = condition;
            this.Expr1 = expr1;
            this.Expr2 = expr2;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType condType, expr1Type, expr2Type;

            codecontext.Write("(");
            condType = ConditionExpr.EmitCs(codecontext);
            codecontext.Write(")?(");
            expr1Type = Expr1.EmitCs(codecontext);
            codecontext.Write("):(");
            expr2Type = Expr2.EmitCs(codecontext);
            codecontext.Write(")");

            if (!condType.Equals(ExpressionType.BoolType))
                throw new Exception("Condition must be of type bool.");

            if (!expr1Type.Equals(expr2Type))
                throw new Exception("Type mishmash, " + expr1Type.ToString() + " and " + expr2Type.ToString());

            return expr1Type;
        }
    }
}
