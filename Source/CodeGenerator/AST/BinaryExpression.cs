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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")+(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            // only int,double,string
            if (!lType.Equals(rType) ||
                (lType.TypeName != ExpressionType.KnownTypes.TDouble && lType.TypeName != ExpressionType.KnownTypes.TInt && lType.TypeName != ExpressionType.KnownTypes.TString))
                throw new Exception("Type mishmash.");

            return lType;
        }
    }

    public class BinarySubExpression : BinaryExpression
    {
        public BinarySubExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")-(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            // only int,double
            if (!lType.Equals(rType) ||
                (lType.TypeName != ExpressionType.KnownTypes.TDouble && lType.TypeName != ExpressionType.KnownTypes.TInt))
                throw new Exception("Type mishmash.");

            return lType;
        }
    }

    public class BinaryMulExpression : BinaryExpression
    {
        public BinaryMulExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")*(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            // only int,double
            if (!lType.Equals(rType) ||
                (lType.TypeName != ExpressionType.KnownTypes.TDouble && lType.TypeName != ExpressionType.KnownTypes.TInt))
                throw new Exception("Type mishmash.");

            return lType;
        }
    }

    public class BinaryDivExpression : BinaryExpression
    {
        public BinaryDivExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")/(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            // only int,double
            if (!lType.Equals(rType) ||
                (lType.TypeName != ExpressionType.KnownTypes.TDouble && lType.TypeName != ExpressionType.KnownTypes.TInt))
                throw new Exception("Type mishmash.");

            return lType;
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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            // ((l) && (r))

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")&&(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");

            return ExpressionType.BoolType;
        }
    }

    public class LogicalOrExpression : BinaryExpression
    {
        public LogicalOrExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            // ((l) || (r))

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")||(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");

            return ExpressionType.BoolType;
        }
    }

    public class LogicalXorExpression : BinaryExpression
    {
        public LogicalXorExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            // ((l) xor (r))

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")^(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");

            return ExpressionType.BoolType;
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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")==(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }

    public class NotEqExpression : BinaryExpression
    {
        public NotEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")!=(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }

    public class BinaryLessExpression : BinaryExpression
    {
        public BinaryLessExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")<(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }
    public class BinaryGreaterExpression : BinaryExpression
    {
        public BinaryGreaterExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")>(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }

    public class BinaryLessEqExpression : BinaryExpression
    {
        public BinaryLessEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")<=(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }
    public class BinaryGreaterEqExpression : BinaryExpression
    {
        public BinaryGreaterEqExpression(ExprPosition position, Expression lValue, Expression rValue)
            : base(position, lValue, rValue)
        {

        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType lType, rType;

            codecontext.Write("((");
            lType = LValue.EmitCs(codecontext);
            codecontext.Write(")>=(");
            rType = RValue.EmitCs(codecontext);
            codecontext.Write("))");

            /*if (lType.TypeName != ExpressionType.KnownTypes.TBool ||
                rType.TypeName != ExpressionType.KnownTypes.TBool)
                throw new Exception("Values must be of type bool.");*/

            return ExpressionType.BoolType;
        }
    }

    #endregion
}
