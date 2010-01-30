using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Literal expression.
    /// </summary>
    class Literal : Expression
    {
        public Literal(ExprPosition position)
            : base(position)
        {
            
        }
    }


    /// <summary>
    /// String literal or identifier.
    /// </summary>
    class StringLiteral : Literal
    {
        /// <summary>
        /// The value of the string literal. C# string.
        /// </summary>
        public string CsValue { get; private set; }

        public StringLiteral( ExprPosition position, string value )
            :base(position)
        {
            Debug.Assert(value != null);

            this.CsValue = value;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write(CsValue);

            return ExpressionType.StringType;
        }
    }

    /// <summary>
    /// Int literal.
    /// </summary>
    class IntLiteral:Literal
    {
        /// <summary>
        /// The value of the int literal.
        /// </summary>
        public int Value { get; private set; }

        public IntLiteral(ExprPosition position, int value)
            :base(position)
        {
            this.Value = value;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write(Value.ToString());

            return ExpressionType.IntType;
        }
    }

    /// <summary>
    /// Double literal.
    /// </summary>
    class DoubleLiteral : Literal
    {
        /// <summary>
        /// The value of the double literal.
        /// </summary>
        public double Value { get; private set; }

        public DoubleLiteral(ExprPosition position, double value)
            : base(position)
        {
            this.Value = value;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write(Value.ToString());

            return ExpressionType.DoubleType;
        }
    }

    /// <summary>
    /// DateTime literal.
    /// </summary>
    class DateTimeLiteral : Literal
    {
        /// <summary>
        /// The value of the double literal.
        /// </summary>
        public DateTime Value { get; private set; }

        public DateTimeLiteral(ExprPosition position, DateTime value)
            : base(position)
        {
            this.Value = value;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write("(new DateTime(\"" + Value.ToString() + "\"))");

            return ExpressionType.DateTimeType;
        }
    }

    /// <summary>
    /// Bool literal.
    /// </summary>
    class BoolLiteral : Literal
    {
        /// <summary>
        /// The value of the double literal.
        /// </summary>
        public bool Value { get; private set; }

        public BoolLiteral(ExprPosition position, bool value)
            : base(position)
        {
            this.Value = value;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write( Value ? "true" : "false" );

            return ExpressionType.BoolType;
        }
    }
}
