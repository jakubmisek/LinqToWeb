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
            codecontext.WriteLine("private static void " + MethodName + "(DataContext datacontext, LocalVariables parameters)");
            codecontext.WriteLine("{");

            EmitCodeContext bodycontext = codecontext.NewScope();

            // declare parameters as local variables
            foreach (var x in MethodArguments)
            {
                bodycontext.DeclareLocalVar(
                    x.VariableType,
                    x.VariableName,
                    new CustomExpression(x.Position, x.VariableType, "(" + x.VariableType.CsName + ")parameters[\"" + x.VariableName + "\"]")
                    );
            }

            // declare special scope variable
            bodycontext.WriteLine("ScopesStack " + scopeLocalVarName + " = new ScopesStack(datacontext, null);");

            // emit method body
            Body.EmitCs(bodycontext);

            //
            codecontext.WriteLine("}");

            return ExpressionType.VoidType;
        }
    }
}
