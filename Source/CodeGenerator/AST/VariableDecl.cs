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
        private ExpressionType VariableType;

        /// <summary>
        /// Init new variable declaration.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="vartype">Type of the variable/property.</param>
        /// <param name="varname">Name of the variable/property.</param>
        public VariableDecl(ExprPosition position, ExpressionType vartype, string varname)
            : base(position)
        {
            this.VariableName = varname;
            this.VariableType = vartype;
        }

        /// <summary>
        /// Variable type + name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return VariableType.ToString() + " " + VariableName;
        }
    }
}
