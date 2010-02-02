using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class VariableUse : Expression
    {
        public readonly string VariableName;

        public VariableUse(ExprPosition position, string name)
            :base(position)
        {
            VariableName = name;
        }

        /// <summary>
        /// Emit variable read.
        /// </summary>
        /// <param name="codecontext"></param>
        /// <returns></returns>
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType varType = codecontext.GetLocalVarType(VariableName);

            if (varType != null)
            {
                // VariableName
                codecontext.Write(VariableName);
                return varType;
            }
            else
            {
                if (VariableName.Contains("."))
                    throw new Exception("Undeclared variable " + VariableName);

                // ((string)__l["VariableName"])    // dynamic var
                codecontext.Write("((string)" + scopeLocalVarName + "[\"" + VariableName + "\"])");
                return ExpressionType.StringType;
            }

            
        }
    }
}
