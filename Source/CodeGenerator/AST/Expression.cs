﻿using System;
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
    public class Expression
    {
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
            public EmitCodeContext(DeclarationsList declarations, StreamWriter output)
            {
                Debug.Assert(declarations != null);
                Debug.Assert(output != null);

                this.Output = output;
                this.Declarations = declarations;
            }

            public void WriteLine( string str )
            {
                for(int i = 0; i < Level; ++i)
                {
                    Output.Write("    ");
                }

                Output.WriteLine(str);
            }

            public EmitCodeContext NewScope()
            {
                EmitCodeContext code = new EmitCodeContext(Declarations, Output);

                foreach (var x in DeclaredLocalVars)
                    code.DeclaredLocalVars[x.Key] = x.Value;

                code.Level = this.Level + 1;

                return code;
            }
        }

        #endregion
    }
   
}
