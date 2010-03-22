using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Encapsulates all the declarations and methods.
    /// </summary>
    public class GlobalCode : Expression
    {
        /// <summary>
        /// List of declared classes and methods.
        /// </summary>
        public DeclarationsList Declarations { get; private set; }

        /// <summary>
        /// Init the global code with the list of global declarations.
        /// </summary>
        /// <param name="decls">List of classes and methods in the global code.</param>
        public GlobalCode(DeclarationsList decls)
            : base(decls.Position)
        {
            Debug.Assert(decls != null);

            this.Declarations = decls;
            
        }

        public override string ToString()
        {
            return Declarations.ToString();
        }

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">OUtput stream.</param>
        public void EmitCs(StreamWriter output, string namespaceName, string contextName)
        {
            output.NewLine = "\r\n";

            EmitCs(new EmitCodeContext(Declarations, output, namespaceName, contextName));

            output.Flush();
        }

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="level">The level of code indent, default 0.</param>
        /// <param name="declaredVariables">List of variables declared in the current context and their type.</param>
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.WriteLine("/*");
            codecontext.WriteLine(" * Generated LinqToWeb context");
            codecontext.WriteLine(" * " + DateTime.Now.ToString());
            codecontext.WriteLine(" */");
            codecontext.WriteLine();
            codecontext.WriteLine("using System;");
            codecontext.WriteLine("using System.Collections.Generic;");
            codecontext.WriteLine("using System.Text;");
            codecontext.WriteLine("using System.Diagnostics;");
            codecontext.WriteLine("using linqtoweb.Core.datacontext;");
            codecontext.WriteLine("using linqtoweb.Core.extraction;");
            codecontext.WriteLine("using linqtoweb.Core.methods;");
            codecontext.WriteLine("using linqtoweb.Core.storage;");
            codecontext.WriteLine();
            codecontext.WriteLine("namespace " + codecontext.NamespaceName);
            codecontext.WriteLine("{");

            EmitCodeContext indentc = codecontext.NewScope();

            indentc.WriteLine("public partial class " + codecontext.ContextName + " : ExtractionContext");
            indentc.WriteLine("{");

            Declarations.EmitCs(indentc.NewScope());

            // emit initialize, emit vars (use arguments from main methods), constructors
            EmitCs_Init(indentc.NewScope());

            indentc.WriteLine("}");

            codecontext.WriteLine("}");


            return ExpressionType.VoidType;
        }
        internal void EmitCs_Init(EmitCodeContext codecontext )
        {
            Dictionary<string, ExpressionType> contextVars = new Dictionary<string, ExpressionType>();

            List<MethodDecl> mainMethods = new List<MethodDecl>();

            // collect global vars, extraction arguments
            foreach (var m in codecontext.Declarations.Methods)
            {
                if (m.IsMainMethod)
                {
                    mainMethods.Add(m);

                    foreach (var arg in m.MethodArguments)
                    {
                        ExpressionType vartype;
                        if (contextVars.TryGetValue(arg.VariableName, out vartype))
                        {
                            if (vartype != arg.VariableType)
                                throw new GeneratorException(Position, "Two context variables with different type defined.");
                        }
                        else
                        {
                            contextVars[arg.VariableName] = arg.VariableType;
                        }
                    }
                }
            }

            // emit properties
            codecontext.WriteLine("#region Public extracted data");
            foreach (var v in contextVars)
            {
                codecontext.WriteLine("// " + v.Value.ToString() + " " + v.Key);
                    
                if (v.Value.IsExtractionObject)
                {
                    // emit prop
                    codecontext.WriteLine("public readonly " + v.Value.CsPropertyTypeName + " " + v.Key + " = " + v.Value.CsPropertyRootInitValue + ";");
                }
                /*else
                {
                    // emit context parameter
                    codecontext.WriteLine("private readonly " + v.Value.CsArgumentTypeName + " " + v.Key + ";");
                }*/
            }
            codecontext.WriteLine("#endregion" + codecontext.Output.NewLine);

            //
            // context arguments, initialization
            //
            string ctorArgs = null;
            string argsPass = null;
            foreach (var x in contextVars)
            {
                if (!x.Value.IsExtractionObject)
                {
                    // argument
                    if (ctorArgs != null) ctorArgs += ", ";
                    if (argsPass != null) argsPass += ", ";

                    ctorArgs = ctorArgs + x.Value.CsArgumentTypeName + " " + x.Key;
                    argsPass = argsPass + x.Key;
                }
            }

            // emit initializing actions
            codecontext.WriteLine("#region Context construct");
            codecontext.WriteLine("private void InitActionsToDo(" + ctorArgs + ")");
            codecontext.WriteLine("{"); codecontext.Level++;
            
            foreach (var m in mainMethods)
            {
                codecontext.WriteLine("ActionItem.AddAction(" + m.GeneratedMethodName + ", InitialDataContext, new LocalVariables(){");
                codecontext.Level ++;

                bool bfirstarg = true;
                foreach (var arg in m.MethodArguments)
                {
                    if (bfirstarg) bfirstarg = false;
                    else codecontext.Write("," + codecontext.Output.NewLine);
                    codecontext.Write("{\"" + arg.VariableName + "\", " + arg.VariableName + "}", codecontext.Level);
                }

                codecontext.Write("});" + codecontext.Output.NewLine);
                codecontext.Level --;
                
            }
            
            codecontext.Level--;
            codecontext.WriteLine("}");

            // emit ctors
            codecontext.WriteLine("#region Constructors");
            codecontext.WriteLine("public " + codecontext.ContextName + "(" + ctorArgs + "):base(){InitActionsToDo(" + argsPass + ");}");
            codecontext.WriteLine("public " + codecontext.ContextName + "(" + ((ctorArgs == null) ? "" : (ctorArgs + ", ")) + "StorageBase cache):base(cache){InitActionsToDo(" + argsPass + ");}");
            codecontext.WriteLine("#endregion");

            codecontext.WriteLine("#endregion" + codecontext.Output.NewLine);
        }

    }

    /// <summary>
    /// List of declaration in global code.
    /// </summary>
    public class DeclarationsList : Expression
    {
        /// <summary>
        /// List of declared classes.
        /// </summary>
        public Dictionary<string, ClassDecl> Classes = new Dictionary<string, ClassDecl>();

        /// <summary>
        /// List of declared methods.
        /// </summary>
        public List<MethodDecl> Methods = new List<MethodDecl>();

        /// <summary>
        /// Init empty declarations list.
        /// </summary>
        /// <param name="position"></param>
        public DeclarationsList(ExprPosition position)
            :base(position)
        {

        }

        /// <summary>
        /// Init declarations list with one class declaration.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="classdecl"></param>
        public DeclarationsList(ExprPosition position, DeclarationsList decls, ClassDecl classdecl)
            : this(position)
        {
            if (decls != null)
            {
                Classes = decls.Classes;
                Methods.AddRange(decls.Methods);
            }

            if (classdecl != null)
            {
                Classes.Add(classdecl.ClassName, classdecl);
            }
        }

        /// <summary>
        /// Init declarations list with one method declaration.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="methoddecl"></param>
        public DeclarationsList(ExprPosition position, DeclarationsList decls, MethodDecl methoddecl)
            : this(position)
        {
            if (decls != null)
            {
                Classes = decls.Classes;
                Methods.AddRange(decls.Methods);
            }

            if (methoddecl != null)
            {
                if (!methoddecl.IsMainMethod)
                foreach ( var m in Methods )
                    if (m.DeclMethodName == methoddecl.DeclMethodName)
                    {   // arguments must match
                        if (m.MethodArguments.Count != methoddecl.MethodArguments.Count)
                            throw new GeneratorException(position, "Methods " + methoddecl.DeclMethodName + ": Arguments mishmash.");

                        for(int arg = 0; arg < m.MethodArguments.Count; ++arg)
                        {
                            if ( !m.MethodArguments[arg].VariableType.Equals(methoddecl.MethodArguments[arg].VariableType) )
                                throw new GeneratorException(position, "Methods " + methoddecl.DeclMethodName + ": Arguments mishmash.");
                            if (m.MethodArguments[arg].VariableName != methoddecl.MethodArguments[arg].VariableName)
                                throw new GeneratorException(position, "Methods " + methoddecl.DeclMethodName + ": Arguments mishmash.");
                        }
                    }

                Methods.Insert(0,methoddecl);
            }
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.WriteLine("#region Public classes declaration");
            foreach (var c in Classes)
                c.Value.EmitCs(codecontext);
            codecontext.WriteLine("#endregion" + codecontext.Output.NewLine);

            codecontext.WriteLine("#region Private extraction methods");
            foreach (var m in Methods)
                m.EmitCs(codecontext);
            codecontext.WriteLine("#endregion" + codecontext.Output.NewLine);

            return ExpressionType.VoidType;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            // emit classes
            foreach (var x in Classes)
            {
                str.AppendLine(x.Value.ToString());
            }

            // emit methods
            foreach (var x in Methods)
            {
                str.AppendLine(x.ToString());
            }

            //
            return str.ToString();
        }
    }

}
