using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class ClassDecl:Expression
    {
        public string ClassName { get; private set; }

        public List<VariableDecl> ClassProperties { get; private set; }

        public ClassDecl(ExprPosition position, string classname, List<VariableDecl> properties)
            :base(position)
        {
            this.ClassName = classname;
            this.ClassProperties = properties;
        }
    }
}
