using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Base AST node representing any expression.
    /// </summary>
    public abstract class Expression
    {
        public const string scopeLocalVarName = "__l";

        /// <summary>
        /// The position in the source code.
        /// </summary>
        public ExprPosition Position { get; private set; }

        /// <summary>
        /// Constructs the expression node.
        /// </summary>
        /// <param name="position"></param>
        public Expression(ExprPosition position)
        {
            this.Position = position;
        }

        #region Emit

        /// <summary>
        /// Emit the C# source code.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="level">Code indent level.</param>
        internal virtual ExpressionType EmitCs( EmitCodeContext codecontext )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Context of the currently emitted Expression.
        /// </summary>
        internal class EmitCodeContext
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
            /// Map of declared variables and their types.
            /// </summary>
            public readonly Dictionary<string, ExpressionType> DeclaredLocalVars = new Dictionary<string, ExpressionType>();

            /// <summary>
            /// Globally declared methods and classes.
            /// </summary>
            public readonly DeclarationsList Declarations;

            /// <summary>
            /// Output stream.
            /// </summary>
            public readonly StreamWriter Output;

            /// <summary>
            /// Current indent level.
            /// </summary>
            public int Level = 0;

            /// <summary>
            /// Init.
            /// </summary>
            /// <param name="declarations"></param>
            public EmitCodeContext(DeclarationsList declarations, StreamWriter output, string namespaceName, string contextName)
            {
                Debug.Assert(declarations != null);
                Debug.Assert(output != null);

                this.Output = output;
                this.Declarations = declarations;

                this.NamespaceName = namespaceName;
                this.ContextName = contextName;
            }

            /// <summary>
            /// Copz Code context with different output.
            /// </summary>
            /// <param name="codecontext"></param>
            /// <param name="output"></param>
            public EmitCodeContext(EmitCodeContext codecontext, StreamWriter output)
            {
                this.Output = output;
                this.Declarations = codecontext.Declarations;
                this.Level = codecontext.Level;

                this.NamespaceName = codecontext.NamespaceName;
                this.ContextName = codecontext.ContextName;

                foreach (var x in codecontext.DeclaredLocalVars)
                    DeclaredLocalVars[x.Key] = x.Value;
            }

            public void WriteLine( string str )
            {
                for(int i = 0; i < Level; ++i)
                {
                    Output.Write("    ");
                }

                Output.WriteLine(str);
            }

            public void WriteLine()
            {
                Output.WriteLine();
            }

            public void Write(string str, int spaces)
            {
                for(int i = 0; i < spaces; ++i)
                {
                    Output.Write("    ");
                }

                Output.Write(str);
            }

            public void Write(string str)
            {
                Output.Write(str);
            }

            public void DeclareLocalVar( ExpressionType vartype, string varname, Expression varvalue )
            {
                if (DeclaredLocalVars.ContainsKey(varname))
                    throw new InvalidOperationException("Symbol " + varname + " already defined.");

                Debug.Assert(varvalue != null, "Local variable must be initialized immediately.");

                Debug.Assert(varname != scopeLocalVarName, "Reserved variable name used.");

                DeclaredLocalVars[varname] = vartype;

                this.Write(vartype.CsArgumentTypeName + " " + varname + " = ", Level);
                ExpressionType rettype = varvalue.EmitCs(this);
                this.Write(";" + Output.NewLine);

                Debug.Assert(rettype.Equals(vartype), "Assigning different types: '" + vartype.ToString() + "' and '" + rettype.ToString() + "'");
            }

            public ExpressionType GetLocalVarType( ExprPosition position, string varname )
            {
                if (string.IsNullOrEmpty(varname))
                    return null;

                string[] chainName = varname.Split(new char[]{'.'});

                ExpressionType retType;
                if (!DeclaredLocalVars.TryGetValue(chainName[0], out retType))
                    return null;

                if (chainName.Length > 1)
                {
                    for (int i = 1; i < chainName.Length; ++i)
                    {
                        if (retType.UserTypeName == null)
                            throw new GeneratorException(position, "Member chain of non-user class not supported.");

                        ClassDecl classdecl;
                        if (!Declarations.Classes.TryGetValue(retType.UserTypeName, out classdecl))
                            throw new GeneratorException(position, "Undeclared type " + retType.UserTypeName);

                        retType = classdecl.ContainsProperty(chainName[i]);

                        if (retType == null)
                            throw new GeneratorException(position, "Undeclared property " + chainName[i]);
                    }
                }

                return retType;
            }

            public EmitCodeContext NewScope()
            {
                EmitCodeContext code = new EmitCodeContext(this, this.Output);

                code.Level = this.Level + 1;

                return code;
            }
        }

        #endregion
    }

    public class CustomExpression:Expression
    {
        private string CsCode;
        private ExpressionType ExprType;

        public CustomExpression(ExprPosition position, ExpressionType exprType, string csCode)
            :base(position)
        {
            this.CsCode = csCode;
            this.ExprType = exprType;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.Write(CsCode);
            return ExprType;
        }
    }
}
