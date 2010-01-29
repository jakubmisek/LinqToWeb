using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class UnaryExpression : Expression
    {
        /// <summary>
        /// the value of the unary expression.
        /// </summary>
        public readonly Expression Value;

        /// <summary>
        /// Unary expression initialization.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        public UnaryExpression(ExprPosition position, Expression value)
            :base(position)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// !value
    /// </summary>
    public class LogicalNotExpression : UnaryExpression
    {
        public LogicalNotExpression(ExprPosition position, Expression value)
            :base(position, value)
        {

        }
    }

    /// <summary>
    /// -value
    /// </summary>
    public class UnaryMinusExpression : UnaryExpression
    {
        public UnaryMinusExpression(ExprPosition position, Expression value)
            : base(position, value)
        {

        }
    }

    /// <summary>
    /// +value
    /// </summary>
    public class UnaryPlusExpression : UnaryExpression
    {
        public UnaryPlusExpression(ExprPosition position, Expression value)
            : base(position, value)
        {

        }
    }
}
