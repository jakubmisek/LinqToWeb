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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            if (Value == null)
                throw new Exception("Value cannot be null.");

            codecontext.Write("(!(");

            ExpressionType valtype = Value.EmitCs(codecontext);

            codecontext.Write("))");

            if (!valtype.Equals(ExpressionType.BoolType))
                throw new Exception("Value must be of type bool.");

            return ExpressionType.BoolType;
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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write("(-(");

            ExpressionType valtype = Value.EmitCs(codecontext);

            codecontext.Write("))");

            if (!(valtype.Equals(ExpressionType.DoubleType) || valtype.Equals(ExpressionType.IntType)))
                throw new Exception("Value must be of type double or int.");

            return valtype;
        }
    }

    
}
