using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class BinaryExpression : Expression
    {
        public Expression LValue { get; protected set; }
        public Expression RValue { get; protected set; }

        /// <summary>
        /// Constructs a binary expression.
        /// </summary>
        /// <param name="position">Expression position.</param>
        /// <param name="lValue">Left operand.</param>
        /// <param name="rValue">Right operand.</param>
        public BinaryExpression(ExprPosition position, Expression lValue, Expression rValue)
            :base(position)
        {
            this.LValue = lValue;
            this.RValue = rValue;
        }
    }


    public class BinaryAddExpression : BinaryExpression
    {
        public BinaryAddExpression( ExprPosition position, Expression lValue, Expression rValue )
            :base(position, lValue, rValue)
        {

        }
    }

    public class BinarySubExpression : BinaryExpression
    {
        public BinarySubExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class BinaryMulExpression : BinaryExpression
    {
        public BinaryMulExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class BinaryDivExpression : BinaryExpression
    {
        public BinaryDivExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class BinaryAssignExpression : BinaryExpression
    {
        public BinaryAssignExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }
}
