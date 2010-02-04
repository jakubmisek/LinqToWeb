using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace linqtoweb.CodeGenerator.AST
{
    public abstract class UnaryExpr : Expression
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
        public UnaryExpr(ExprPosition position, Expression value)
            :base(position)
        {
            this.Value = value;
        }

        /// <summary>
        /// Emit formatted value expression.
        /// </summary>
        /// <param name="codecontext"></param>
        /// <param name="format"></param>
        /// <returns>Value type.</returns>
        internal ExpressionType EmitValue( EmitCodeContext codecontext, string format )
        {
            if (Value == null)
                throw new Exception("Value cannot be null.");

            // process the value on separated output
            MemoryStream valstr = new MemoryStream();
            StreamWriter valoutput = new StreamWriter(valstr);

            EmitCodeContext valcontext = new EmitCodeContext(codecontext, valoutput);

            ExpressionType valType = Value.EmitCs(valcontext);

            valoutput.Flush();

            // get value emitted expression
            valstr.Position = 0;
            codecontext.Write(string.Format(format,new StreamReader(valstr).ReadToEnd()));

            return valType;
        }
    }

    /// <summary>
    /// !value
    /// </summary>
    public class ExpressionLogicalNot : UnaryExpr
    {
        public ExpressionLogicalNot(ExprPosition position, Expression value)
            :base(position, value)
        {
            
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "(!({0}))");

            if (!valType.Equals(ExpressionType.BoolType))
                throw new Exception("Value must be of type bool.");

            return ExpressionType.BoolType;
        }
    }

    /// <summary>
    /// -value
    /// </summary>
    public class ExpressionUnaryMinus : UnaryExpr
    {
        public ExpressionUnaryMinus(ExprPosition position, Expression value)
            : base(position, value)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "(-({0}))");

            if (!valType.Equals(ExpressionType.IntType) && !valType.Equals(ExpressionType.DoubleType))
                throw new Exception("Value must be of type int or double.");

            return ExpressionType.BoolType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpressionAddOneBefore : UnaryExpr
    {
        public ExpressionAddOneBefore(ExprPosition position, Expression value)
            :base(position, value)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "(++{0})");

            if (!valType.Equals(ExpressionType.IntType) && !valType.Equals(ExpressionType.DoubleType))
                throw new Exception("Value must be of type int or double.");

            return ExpressionType.BoolType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpressionSubOneBefore : UnaryExpr
    {
        public ExpressionSubOneBefore(ExprPosition position, Expression value)
            : base(position, value)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "(--{0})");

            if (!valType.Equals(ExpressionType.IntType) && !valType.Equals(ExpressionType.DoubleType))
                throw new Exception("Value must be of type int or double.");

            return ExpressionType.BoolType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpressionAddOneAfter : UnaryExpr
    {
        public ExpressionAddOneAfter(ExprPosition position, Expression value)
            : base(position, value)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "({0}++)");

            if (!valType.Equals(ExpressionType.IntType) && !valType.Equals(ExpressionType.DoubleType))
                throw new Exception("Value must be of type int or double.");

            return ExpressionType.BoolType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpressionSubOneAfter : UnaryExpr
    {
        public ExpressionSubOneAfter(ExprPosition position, Expression value)
            : base(position, value)
        {

        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType valType = EmitValue(codecontext, "({0}--)");

            if (!valType.Equals(ExpressionType.IntType) && !valType.Equals(ExpressionType.DoubleType))
                throw new Exception("Value must be of type int or double.");

            return ExpressionType.BoolType;
        }
    }

    
}
