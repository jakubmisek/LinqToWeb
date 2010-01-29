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
        /// The name of the emitted class that will contain types and methods.
        /// </summary>
        public string ContextName { get; private set; }

        /// <summary>
        /// The name of the emitted namespace that will contain the context class.
        /// </summary>
        public string NamespaceName { get; private set; }

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

            ContextName = "WebContext";
            NamespaceName = "linqtoweb.Example";
        }

        public override string ToString()
        {
            return 
                "namespace " + NamespaceName + "\n{" +
                Declarations.ToString() +
                "}";
        }

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">OUtput stream.</param>
        public void EmitCs(System.IO.StreamWriter output)
        {
            output.NewLine = "\r\n";

            EmitCs(new EmitCodeContext(Declarations, output));

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
            codecontext.WriteLine("using System;");
            codecontext.WriteLine("using System.Collections.Generic;");
            codecontext.WriteLine("using System.Text;");
            codecontext.WriteLine("using System.Diagnostics;");
            codecontext.WriteLine("using linqtoweb.Core.datacontext;");
            codecontext.WriteLine("using linqtoweb.Core.extraction;");

            codecontext.WriteLine("namespace " + NamespaceName);
            codecontext.WriteLine("{");

            EmitCodeContext indentc = codecontext.NewScope();

            indentc.WriteLine("class " + ContextName + " : ExtractionContext");
            indentc.WriteLine("{");

            Declarations.EmitCs(indentc.NewScope());

            // TODO: emit init, emit vars (use arguments from main methods)

            indentc.WriteLine("}");


            codecontext.WriteLine("}");


            return ExpressionType.VoidType;
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
                Methods.Add(methoddecl);
            }
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            foreach (var c in Classes)
                c.Value.EmitCs(codecontext);

            foreach (var m in Methods)
                m.EmitCs(codecontext);

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
