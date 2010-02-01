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

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType varType;
            string varuseString;

            // TODO: dotted identifier

            if (codecontext.DeclaredLocalVars.TryGetValue(VariableName, out varType))
            {
                // VariableName
                varuseString = VariableName;
            }
            else
            {
                // ((string)__l["VariableName"])    // dynamic var
                varType = ExpressionType.StringType;
                varuseString = "((string)" + scopeLocalVarName + "[\"" + VariableName + "\"])";
            }

            codecontext.Write(varuseString);

            return varType;
        }
    }
}
