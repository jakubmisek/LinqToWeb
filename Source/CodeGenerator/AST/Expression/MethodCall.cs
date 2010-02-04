using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class MethodCall : Expression
    {
        public string MethodName { get; protected set; }
        public List<Expression> CallArguments { get; private set; }

        public MethodCall(ExprPosition position, string methodName, List<Expression> callArguments)
            :base(position)
        {
            this.MethodName = methodName;
            this.CallArguments = callArguments;
        }

        public override string ToString()
        {

            StringBuilder str = new StringBuilder();

            str.Append(MethodName + "(");

            for (int i = 0; i < CallArguments.Count; ++i)
            {
                str.Append(CallArguments[i].ToString());

                if (i < CallArguments.Count - 1)
                    str.Append(", ");
            }

            str.Append(")");

            return str.ToString();
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            // is it an extraction method ?
            List<MethodDecl> matchingMethods = new List<MethodDecl>();

            foreach (var decl in codecontext.Declarations.Methods)
            {
                if (decl.DeclMethodName == MethodName)
                {
                    if (decl.MethodArguments.Count != CallArguments.Count)
                        throw new Exception("Invalid arguments count in method call " + MethodName);

                    matchingMethods.Add(decl);
                }
            }

            if (matchingMethods.Count > 0)
            {
                codecontext.Write("ActionItem.AddAction( new ActionItem.ExtractionMethod[]{");

                bool bFirstMethod = true;
                foreach (var decl in matchingMethods)
                {
                    if (bFirstMethod) bFirstMethod = false;
                    else codecontext.Write(", ");
                    codecontext.Write(decl.GeneratedMethodName);
                   
                }

                codecontext.Write("}, "+scopeLocalVarName+".context, new LocalVariables() {" + codecontext.Output.NewLine);

                codecontext.Level++;

                MethodDecl somedecl = matchingMethods[0];
                for (int arg = 0; arg < CallArguments.Count; ++arg )
                {
                    if (arg > 0) codecontext.Write("," + codecontext.Output.NewLine);
                    codecontext.Write("{\"" + somedecl.MethodArguments[arg].VariableName + "\", ", codecontext.Level);
                    ExpressionType t = CallArguments[arg].EmitCs(codecontext);
                    codecontext.Write("}");

                    if (!t.Equals(somedecl.MethodArguments[arg].VariableType))
                        throw new Exception("Type mishmash.");
                }

                codecontext.Write(" })");
                codecontext.Level--;

                return ExpressionType.VoidType;
            }

            // is it a class construction ?
            foreach (var decl in codecontext.Declarations.Classes.Values)
            {
                if (decl.ClassName == MethodName)
                {
                    // emit // (new XXX(){ a = val1, b = val2 });

                    codecontext.Write("(new " + MethodName + "(){ ");

                    for ( int arg = 0; arg < CallArguments.Count; ++arg )
                    {
                        ExpressionAssign ass = CallArguments[arg] as ExpressionAssign;
                        VariableUse lvalue;
                        if (ass == null || (lvalue = ass.LValue as VariableUse) == null)
                            throw new ArgumentException("Argument " + arg + ": class construct arguments must be in a form of 'PropertyName = Expression'");

                        if (arg > 0) codecontext.Write(", ");

                        ExpressionType propType = decl.ContainsProperty(lvalue.VariableName);

                        if (propType == null)
                            throw new Exception(lvalue.VariableName + " is not a property of " + decl.ClassName);

                        codecontext.Write(lvalue.VariableName + " = ");
                        ExpressionType propValueType =  ass.RValue.EmitCs(codecontext);

                        if (!propValueType.Equals(propType))
                            throw new ArgumentException("Type mishmash, " + propType.ToString() + " and " + propValueType.ToString());
                    }

                    codecontext.Write(" })");

                    return new ExpressionUserType(decl.ClassName);
                }
            }

            //
            throw new Exception("Undeclared method or class " + MethodName);
        }
    }
}
