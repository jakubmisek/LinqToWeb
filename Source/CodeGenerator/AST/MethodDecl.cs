using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class MethodDecl:Expression
    {
        public string MethodName { get; private set; }

        public List<VariableDecl> MethodArguments { get; private set; }

        public MethodDecl(ExprPosition position, string methodname, List<VariableDecl> arguments)
            :base(position)
        {
            this.MethodName = methodname;
            this.MethodArguments = arguments;
        }
    }
}
