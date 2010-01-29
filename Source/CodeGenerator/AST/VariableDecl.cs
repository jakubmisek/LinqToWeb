using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Variable declaration.
    /// </summary>
    public class VariableDecl : Expression
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// Variable type.
        /// </summary>
        public readonly ExpressionType VariableType;

        /// <summary>
        /// Variable initial value.
        /// </summary>
        public readonly Expression InitialValue;

        /// <summary>
        /// Init new variable declaration.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="vartype">Type of the variable/property.</param>
        /// <param name="varname">Name of the variable/property.</param>
        public VariableDecl(ExprPosition position, ExpressionType vartype, string varname)
            : this(position,vartype,varname,null)
        {
            
        }

        /// <summary>
        /// Init new variable declaration.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="vartype">Variable type.</param>
        /// <param name="varname">Variable name.</param>
        /// <param name="value">Variable initial value.</param>
        public VariableDecl(ExprPosition position, ExpressionType vartype, string varname, Expression value)
            :base(position)
        {
            this.VariableName = varname;
            this.VariableType = vartype;
            this.InitialValue = value;
        }

        /// <summary>
        /// Variable type + name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string decl = VariableType.ToString() + " " + VariableName;

            if (InitialValue != null)
                decl += " = " + InitialValue.ToString();

            return decl;
        }
    }
}
