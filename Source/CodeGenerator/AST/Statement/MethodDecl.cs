using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class MethodDecl : Expression
    {
        /// <summary>
        /// Method name as it was declared in the original source.
        /// </summary>
        public string DeclMethodName { get; private set; }

        /// <summary>
        /// Method name as it is generated in the resulted code.
        /// </summary>
        public string GeneratedMethodName { get; private set; }

        /// <summary>
        /// This is the main method.
        /// </summary>
        public bool IsMainMethod
        {
            get
            {
                return (DeclMethodName == "main");
            }
        }

        public List<VariableDecl> MethodArguments { get; private set; }

        public Expression Body { get; protected set; }

        public string BodyCSharp { get; protected set; }
        public readonly ExpressionType ReturnType = ExpressionType.VoidType;

        public MethodDecl(ExprPosition position, string methodname, List<VariableDecl> arguments, Expression body)
            : base(position)
        {
            this.DeclMethodName = methodname;
            this.GeneratedMethodName = methodname + "_" + position.StartLine + "_" + position.StartColumn;
            this.MethodArguments = arguments;
            this.Body = body;
        }

        public MethodDecl(ExprPosition position, string methodname, List<VariableDecl> arguments, string bodycsharp, ExpressionType ReturnType)
            : base(position)
        {
            this.DeclMethodName = methodname;
            this.GeneratedMethodName = methodname + "_" + position.StartLine + "_" + position.StartColumn;
            this.MethodArguments = arguments;
            this.BodyCSharp = bodycsharp;
            this.ReturnType = ReturnType;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append(DeclMethodName + "(");

            for (int i = 0; i < MethodArguments.Count; ++i)
            {
                str.Append(MethodArguments[i].ToString());

                if (i < MethodArguments.Count - 1)
                    str.Append(", ");
            }

            str.Append(")");

            if (Body != null) str.Append(Body.ToString());
            if (BodyCSharp != null) str.Append(BodyCSharp.ToString());

            return str.ToString();
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            if (Body != null)
            {
                codecontext.WriteLine("// " + DeclMethodName);
                codecontext.WriteLine("private static void " + GeneratedMethodName + "(DataContext _datacontext, LocalVariables _parameters)");
                codecontext.WriteLine("{");

                EmitCodeContext bodycontext = codecontext.NewScope();

                // declare parameters as local variables
                foreach (var x in MethodArguments)
                {
                    // declare variable from _parameters argument
                    // checks for declaration duplicity
                    bodycontext.DeclareLocalVar(
                        x.VariableType,
                        x.VariableName,
                        new CustomExpression(x.Position, x.VariableType, "(" + x.VariableType.CsArgumentTypeName + ")_parameters[\"" + x.VariableName + "\"]")
                        );
                }

                // declare special scope variable
                bodycontext.WriteLine("ScopesStack " + scopeLocalVarName + " = new ScopesStack(_datacontext, null);");

                // emit method body
                Body.EmitCs(bodycontext);

                //
                codecontext.WriteLine("}" + codecontext.Output.NewLine);

                return ExpressionType.VoidType;
            }
            else if (BodyCSharp != null)
            {
                codecontext.WriteLine("// " + DeclMethodName);
                codecontext.WriteLine("private static " + ReturnType.CsArgumentTypeName + " " + GeneratedMethodName + "(" + CsArgList + ")");
                codecontext.WriteLine("{");
                string[] lines = BodyCSharp.Split(new string[] { codecontext.Output.NewLine }, StringSplitOptions.None);
                foreach (var line in lines)
                    codecontext.WriteLine(line);
                codecontext.WriteLine("}");

                return ReturnType;
            }


            return ExpressionType.VoidType;
        }

        internal string CsArgList
        {
            get
            {
                string args = null;

                if (MethodArguments != null)
                foreach (var x in MethodArguments)
                {
                    if (args != null) args += ", ";

                    args = args + x.VariableType.CsArgumentTypeName + " " + x.VariableName;
                }

                return args;
            }
        }
    }
}
