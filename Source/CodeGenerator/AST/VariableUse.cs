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

    }
}
