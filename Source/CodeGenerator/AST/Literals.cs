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
        public override ExpressionType ResultType
        {
            get
            {
                throw new InvalidOperationException("Depends on specified literal type.");
            }
        }

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
        /// The value of the string literal.
        /// </summary>
        public string Value { get; private set; }

        public override ExpressionType ResultType
        {
            get
            {
                return ExpressionType.StringType;
            }
        }

        public StringLiteral( ExprPosition position, string value )
            :base(position)
        {
            Debug.Assert(value != null);

            this.Value = value;
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

        public override ExpressionType ResultType
        {
            get
            {
                return ExpressionType.IntType;
            }
        }

        public IntLiteral(ExprPosition position, int value)
            :base(position)
        {
            this.Value = value;
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

        public override ExpressionType ResultType
        {
            get
            {
                return ExpressionType.DoubleType;
            }
        }

        public DoubleLiteral(ExprPosition position, double value)
            : base(position)
        {
            this.Value = value;
        }
    }
}
