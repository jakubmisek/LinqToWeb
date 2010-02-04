using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// Block of code.
    /// </summary>
    public class CodeBlock : Expression
    {
        /// <summary>
        /// Expressions in the code block.
        /// </summary>
        public List<Expression> Statements { get; protected set; }

        /// <summary>
        /// Chain of data contexts, or empty.
        /// </summary>
        public readonly LinkedList<MethodCall> DataContexts = new LinkedList<MethodCall>();

        /// <summary>
        /// Create statements block.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="statements"></param>
        public CodeBlock(ExprPosition position, List<Expression> statements)
            :base(position)
        {
            this.Statements = (statements != null) ? statements : new List<Expression>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            foreach (var c in DataContexts)
                str.Append("[" + c.ToString() + "]");

            if (Statements.Count > 1)
                str.Append("{");

            foreach (var s in Statements)
                str.Append(s.ToString());

            if (Statements.Count > 1)
                str.Append("}");

            return str.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codecontext"></param>
        /// <returns></returns>
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.WriteLine("{");
            codecontext.Level++;
            // push scope
            bool contextchanged = false;
            foreach (var x in DataContexts)
            {
                if (!contextchanged)
                {
                    contextchanged = true;
                    codecontext.Write(scopeLocalVarName + ".Push(" + scopeLocalVarName + ".context", codecontext.Level);
                }
                //.OpenContextDynamic(MethodName, new object[] { arg1, arg2, ... })
                codecontext.Write(".OpenContextDynamic(\"" + x.MethodName + "\", new object[] {");
                bool firstArg = true;
                foreach(var a in x.CallArguments)
                {
                    if (!firstArg) codecontext.Write(", ");
                    else firstArg = false;
                    
                    a.EmitCs(codecontext);
                }
                codecontext.Write("})");
            }
            if(contextchanged)
            {
                codecontext.Write(", null);" + codecontext.Output.NewLine);
            }

            // emit statements
            foreach (var s in Statements)
            {
                s.EmitCs(codecontext);
                codecontext.Write(codecontext.Output.NewLine);
            }

            // pop scope
            if (contextchanged)
            {
                codecontext.WriteLine(scopeLocalVarName + ".Pop();");
            }

            codecontext.Level--;
            codecontext.WriteLine("}");

            return ExpressionType.VoidType;
        }
    }
    
}
