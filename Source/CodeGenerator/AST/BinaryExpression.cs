using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    #region binary expression
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

    #endregion

    #region arithmetic (+ - * /)
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

    #endregion

    #region assign, addelement (=, []=)

    public class BinaryAssignExpression : BinaryExpression
    {
        public BinaryAssignExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {
            if (!(lValue is VariableUse))
                throw new ArgumentException("L-Value must be a VariableUse.");
        }
    }

    public class AddElementExpression : Expression
    {
        public readonly VariableUse lvalue;
        public readonly Expression rvalue;

        public AddElementExpression(ExprPosition position, VariableUse lvalue, Expression rvalue)
            : base(position)
        {
            this.lvalue = lvalue;
            this.rvalue = rvalue;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            // lvalue.AddElement( rvalue );

            ExpressionType lValueType = lvalue.EmitCs(codecontext);

            if(lValueType.ListOf == null)
                throw new Exception("Unable to add an element to a non-list variable.");

            codecontext.Write(".AddElement(");

            ExpressionType rValueType = rvalue.EmitCs(codecontext);

            codecontext.Write(")");

            if(!rValueType.Equals(lValueType.ListOf))
                throw new Exception("Type mishmash, adding an element of type " + rValueType.CsName + " to the list of " + lValueType.ListOf.CsName);

            return lValueType.ListOf;
        }
    }

    #endregion

    #region logical operators ( and, or, xor )

    public class LogicalAndExpression : BinaryExpression
    {
        public LogicalAndExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class LogicalOrExpression : BinaryExpression
    {
        public LogicalOrExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class LogicalXorExpression : BinaryExpression
    {
        public LogicalXorExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    #endregion

    #region compare (< > <= >= != ==)

    public class EqExpression : BinaryExpression
    {
        public EqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class NotEqExpression : BinaryExpression
    {
        public NotEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class BinaryLessExpression : BinaryExpression
    {
        public BinaryLessExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }
    public class BinaryGreaterExpression : BinaryExpression
    {
        public BinaryGreaterExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    public class BinaryLessEqExpression : BinaryExpression
    {
        public BinaryLessEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }
    public class BinaryGreaterEqExpression : BinaryExpression
    {
        public BinaryGreaterEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
    }

    #endregion
}
