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

        private ExpressionType TryEmitExtractionMethodCall(EmitCodeContext codecontext)
        {
            List<MethodDecl> matchingMethods = new List<MethodDecl>();

            foreach (var decl in codecontext.Declarations.Methods)
            {
                if (decl.DeclMethodName == MethodName && decl.Body != null)
                {
                    if (decl.IsMainMethod)
                        throw new Exception("main method cannot be called!");

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

                codecontext.Write("}, " + scopeLocalVarName + ".context, new LocalVariables() {" + codecontext.Output.NewLine);

                codecontext.Level++;

                MethodDecl somedecl = matchingMethods[0];
                string cannotAddActionDict = null;
                for (int arg = 0; arg < CallArguments.Count; ++arg)
                {
                    if (arg > 0) codecontext.Write("," + codecontext.Output.NewLine);
                    codecontext.Write("{\"" + somedecl.MethodArguments[arg].VariableName + "\", ", codecontext.Level);
                    ExpressionType t = CallArguments[arg].EmitCs(codecontext);
                    codecontext.Write("}");

                    if (!t.Equals(somedecl.MethodArguments[arg].VariableType))
                        throw new Exception("Type mishmash.");

                    // check if the argument is able to add new action
                    VariableUse varuse;
                    if ((varuse = CallArguments[arg] as VariableUse) != null)
                    {
                        if (cannotAddActionDict != null) cannotAddActionDict += ",";
                        cannotAddActionDict += "{\"" + somedecl.MethodArguments[arg].VariableName + "\",_parameters.CannotAddActionForVariable(\"" + varuse.VariableName + "\")}";
                    }
                }

                codecontext.Write(" }.SetCannotAddAction(new Dictionary<string,bool>(){" + cannotAddActionDict + "}))");
                codecontext.Level--;

                return ExpressionType.VoidType;
            }
            else
            {
                return null;
            }
        }
        private ExpressionType TryEmitClassConstruct(EmitCodeContext codecontext)
        {
            foreach (var decl in codecontext.Declarations.Classes.Values)
            {
                if (decl.ClassName == MethodName)
                {
                    // emit // (new XXX(){ a = val1, b = val2 });

                    codecontext.Write("(new " + MethodName + "(){ ");

                    for (int arg = 0; arg < CallArguments.Count; ++arg)
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
                        ExpressionType propValueType = ass.RValue.EmitCs(codecontext);

                        if (!propValueType.Equals(propType))
                            throw new ArgumentException("Type mishmash, " + propType.ToString() + " and " + propValueType.ToString());
                    }

                    codecontext.Write(" })");

                    return new ExpressionUserType(decl.ClassName);
                }
            }

            return null;
        }

        /// <summary>
        /// Emit the C# method call, defined in the context class (codecontext.ContextName).
        /// </summary>
        /// <param name="codecontext"></param>
        /// <returns></returns>
        private ExpressionType TryEmitCsMethodCall(EmitCodeContext codecontext)
        {
            List<MethodDecl> matchingMethods = new List<MethodDecl>();

            foreach (var decl in codecontext.Declarations.Methods)
            {
                if (decl.DeclMethodName == MethodName && decl.BodyCSharp != null)
                {
                    if (decl.MethodArguments != null && decl.MethodArguments.Count != CallArguments.Count)
                        throw new Exception("Invalid arguments count in method call " + MethodName);

                    matchingMethods.Add(decl);
                }
            }

            if (matchingMethods.Count > 1)
            {
                throw new Exception("Ambiguous C# method call.");
            }

            if (matchingMethods.Count == 1)
            {
                MethodDecl decl = matchingMethods[0];

                codecontext.Write(decl.GeneratedMethodName + "(");

                bool bFirst = true;
                if (CallArguments != null)
                foreach (var x in CallArguments)
                {
                    if (!bFirst) codecontext.Write(", ");
                    else bFirst = false;

                    x.EmitCs(codecontext);
                }
                codecontext.Write(")");

                return decl.ReturnType;
            }

            return null;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            ExpressionType retType;

            // is it an extraction method ?
            if ((retType = TryEmitExtractionMethodCall(codecontext)) != null)
                return retType;

            // is it a class construction ?
            if ((retType = TryEmitClassConstruct(codecontext)) != null)
                return retType;

            //  emit .NET method call (in the same context class)
            if ((retType = TryEmitCsMethodCall(codecontext)) != null)
                return retType;

            // 
            throw new Exception("Undeclared method or class " + MethodName);
        }
    }
}
