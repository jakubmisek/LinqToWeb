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

        public Expression Body { get; protected set; }

        public MethodDecl(ExprPosition position, string methodname, List<VariableDecl> arguments, Expression body)
            :base(position)
        {
            this.MethodName = methodname;
            this.MethodArguments = arguments;
            this.Body = body;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append( MethodName + "(" );

            for(int i = 0;i<MethodArguments.Count;++i)
            {
                str.Append(MethodArguments[i].ToString());

                if (i < MethodArguments.Count - 1)
                    str.Append(", ");
            }

            str.Append(")");

            if (Body != null) str.Append(Body.ToString());
            
            return str.ToString();
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {

            return ExpressionType.VoidType;
        }
    }
}
