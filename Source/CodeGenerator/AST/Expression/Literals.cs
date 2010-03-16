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
    public abstract class Literal : Expression
    {
        public Literal(ExprPosition position)
            : base(position)
        {
            
        }
    }


    /// <summary>
    /// String literal or identifier.
    /// </summary>
    public class StringLiteral : Literal
    {
        /// <summary>
        /// The value of the string literal. C# string.
        /// </summary>
        public string CsValue
        {
            get
            {
                return
                    "\"" +
                    Value.
                    Replace("\\", "\\\\").
                    Replace("\"", "\\\"").
                    Replace("\t", "\\t").
                    Replace("\n", "\\n").
                    Replace("\r", "\\r").
                    Replace("\f", "\\f") +
                    "\"";
            }
        }
        public string Value { get; private set; }

        public StringLiteral( ExprPosition position, string value )
            :base(position)
        {
            Debug.Assert(value != null);

            this.Value = value;
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
    public class IntLiteral : Literal
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
    public class DoubleLiteral : Literal
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
    public class DateTimeLiteral : Literal
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
    public class BoolLiteral : Literal
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
