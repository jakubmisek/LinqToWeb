using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.Declarations = decls;

            ContextName = "WebContext";
            NamespaceName = "linqtoweb.Example";
        }

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="level">The level of code indent, default 0.</param>
        public override void EmitCs(System.IO.StreamWriter output, int level)
        {
            
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
    }
}
