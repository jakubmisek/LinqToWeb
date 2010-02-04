using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    #region base binary expression

    public abstract class BinaryExpr : Expression
    {
        public Expression LValue { get; protected set; }
        public Expression RValue { get; protected set; }

        /// <summary>
        /// Constructs a binary expression.
        /// </summary>
        /// <param name="position">Expression position.</param>
        /// <param name="lValue">Left operand.</param>
        /// <param name="rValue">Right operand.</param>
        public BinaryExpr(ExprPosition position, Expression lValue, Expression rValue)
            :base(position)
        {
            this.LValue = lValue;
            this.RValue = rValue;
        }

        /// <summary>
        /// Emit the binary operation expression.
        /// </summary>
        /// <param name="codecontext">Code context.</param>
        /// <param name="binaryOperator">Binary operator.</param>
        /// <param name="expectedLType">Expected left type, or null.</param>
        /// <param name="expectedRType">Expected right type, or null.</param>
        internal ExpressionType EmitBinaryExpr(EmitCodeContext codecontext, string binaryOperator, ExpressionType expectedLType, ExpressionType expectedRType)
        {
            codecontext.Write("((");
            ExpressionType lType = LValue.EmitCs(codecontext);
            codecontext.Write(")" + binaryOperator + "(");
            ExpressionType rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            if (!lType.Equals(rType))
                throw new Exception("Type mishmash!");

            if ((expectedLType != null && !expectedLType.Equals(lType)) ||
                (expectedRType != null && !expectedRType.Equals(rType)))
                throw new Exception("Type mishmash!");

            return lType;
        }
    }

    #endregion

    #region arithmetic (+ - * /)
    public class ExpressionAdd : BinaryExpr
    {
        public ExpressionAdd( ExprPosition position, Expression lValue, Expression rValue )
            :base(position, lValue, rValue)
        {
            
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType opType = EmitBinaryExpr(codecontext, "+", null, null);

            // only int,double,string
            if (opType.TypeName != ExpressionType.KnownTypes.TDouble &&
                opType.TypeName != ExpressionType.KnownTypes.TInt &&
                opType.TypeName != ExpressionType.KnownTypes.TString)
                throw new Exception("Type mishmash.");

            return opType;
        }
    }

    public class ExpressionSub : BinaryExpr
    {
        public ExpressionSub(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType opType = EmitBinaryExpr(codecontext, "-", null, null);

            // only int,double,string
            if (opType.TypeName != ExpressionType.KnownTypes.TDouble &&
                opType.TypeName != ExpressionType.KnownTypes.TInt)
                throw new Exception("Type mishmash.");

            return opType;
        }
    }

    public class ExpressionMul : BinaryExpr
    {
        public ExpressionMul(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType opType = EmitBinaryExpr(codecontext, "*", null, null);

            // only int,double,string
            if (opType.TypeName != ExpressionType.KnownTypes.TDouble &&
                opType.TypeName != ExpressionType.KnownTypes.TInt)
                throw new Exception("Type mishmash.");

            return opType;
        }
    }

    public class ExpressionDiv : BinaryExpr
    {
        public ExpressionDiv(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType opType = EmitBinaryExpr(codecontext, "/", null, null);

            // only int,double,string
            if (opType.TypeName != ExpressionType.KnownTypes.TDouble &&
                opType.TypeName != ExpressionType.KnownTypes.TInt)
                throw new Exception("Type mishmash.");

            return opType;
        }
    }

    #endregion

    #region assign, addelement (=, []=)

    public class ExpressionAssign : BinaryExpr
    {
        public ExpressionAssign(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {
            if (!(lValue is VariableUse))
                throw new ArgumentException("L-Value must be a VariableUse.");
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            // l = r

            VariableUse lValueVariable = (VariableUse)LValue;
            ExpressionType lType = codecontext.GetLocalVarType(lValueVariable.VariableName);
            if (lType == null)
                throw new Exception(lValueVariable.VariableName + " is not declared.");

            if (lType.ListOf != null)
                throw new Exception("Unable to assign to a list.");

            codecontext.Write(lValueVariable.VariableName + " = ");

            ExpressionType rType = RValue.EmitCs(codecontext);

            if (!(lType.Equals(rType)))
                throw new Exception("Type mishmash.");

            return lType;
        }
    }

    public class ExpressionAddElement : Expression
    {
        public readonly VariableUse lvalue;
        public readonly Expression rvalue;

        public ExpressionAddElement(ExprPosition position, VariableUse lvalue, Expression rvalue)
            : base(position)
        {
            this.lvalue = lvalue;
            this.rvalue = rvalue;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lValueType = codecontext.GetLocalVarType(lvalue.VariableName);
            if (lValueType == null)
                throw new Exception(lvalue.VariableName + " not declared.");
            if(lValueType.ListOf == null)
                throw new Exception("Unable to add an element to a non-list variable.");

            ExpressionType rValueType;

            if (!lvalue.VariableName.Contains("."))
            {   // add an element directly
                // lvalue.AddElement( rvalue );
                codecontext.Write(lvalue.VariableName);

                codecontext.Write(".AddElement(");
                rValueType = rvalue.EmitCs(codecontext);
                codecontext.Write(")");
            }
            else
            {   // element cannot be added directly
                // must be created action that add the element when list is actually enumerated

                codecontext.Write("AddElementAction(" + scopeLocalVarName + ".context, " + lvalue.VariableName + ", ");
                rValueType = rvalue.EmitCs(codecontext);
                codecontext.Write(")");
            }

            if (!rValueType.Equals(lValueType.ListOf))
                throw new Exception("Type mishmash, adding an element of type " + rValueType.ToString() + " to the list of " + lValueType.ListOf.ToString());
            
            return lValueType.ListOf;
        }
    }

    #endregion

    #region logical operators ( and, or, xor )

    public class ExpressionLogicalAnd : BinaryExpr
    {
        public ExpressionLogicalAnd(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "&&", ExpressionType.BoolType, ExpressionType.BoolType);

            return ExpressionType.BoolType;
        }
    }

    public class ExpressionLogicalOr : BinaryExpr
    {
        public ExpressionLogicalOr(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "||", ExpressionType.BoolType, ExpressionType.BoolType);

            return ExpressionType.BoolType;
        }
    }

    public class ExpressionLogicalXor : BinaryExpr
    {
        public ExpressionLogicalXor(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "^", ExpressionType.BoolType, ExpressionType.BoolType);

            return ExpressionType.BoolType;
        }
    }

    #endregion

    #region compare (< > <= >= != ==)

    public class ExpressionEq : BinaryExpr
    {
        public ExpressionEq(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "==", null, null);

            return ExpressionType.BoolType;
        }
    }

    public class ExpressionNotEq : BinaryExpr
    {
        public ExpressionNotEq(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "!=", null, null);

            return ExpressionType.BoolType;
        }
    }

    public class ExpressionLess : BinaryExpr
    {
        public ExpressionLess(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "<", null, null);

            return ExpressionType.BoolType;
        }
    }
    public class ExpressionGreater : BinaryExpr
    {
        public ExpressionGreater(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, ">", null, null);

            return ExpressionType.BoolType;
        }
    }

    public class ExpressionLessEq : BinaryExpr
    {
        public ExpressionLessEq(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, "<=", null, null);

            return ExpressionType.BoolType;
        }
    }
    public class ExpressionGreaterEq : BinaryExpr
    {
        public ExpressionGreaterEq(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            EmitBinaryExpr(codecontext, ">=", null, null);

            return ExpressionType.BoolType;
        }
    }

    #endregion
}
