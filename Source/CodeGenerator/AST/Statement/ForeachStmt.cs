using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.methods;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    public class ForeachStmt : Expression
    {
        public readonly Expression ForeachExpression;
        public readonly Expression Body;    // if null, ignore this foreach

        public ForeachStmt(ExprPosition position, Expression foreachexpr, Expression body)
            : base(position)
        {
            this.Body = body;
            this.ForeachExpression = foreachexpr;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            if (Body != null)
            {
                MethodCall foreachMethod = ForeachExpression as MethodCall;
                if (foreachMethod == null)
                    throw new Exception("argument of foreach must be a method call");

                // TODO check foreachMethod arguments and return value

                /*  // emit this
                    foreach (var x in ForeachMethods.regexp(l.context, @"Porno\s+(?<Title>\w+)"))
                    {
                        l.Push(null, x);

                        {Body}

                        l.Pop();
                    }
                */

                string foreachVarName = "__fe" + Position.StartLine + "_" + Position.StartColumn;

                // write foreach header
                codecontext.Write("foreach(var " + foreachVarName + " in ForeachMethods." + foreachMethod.MethodName + "(" + scopeLocalVarName + ".context", codecontext.Level);
                // method arguments
                List<ExpressionType> methodargs = new List<ExpressionType>();
                foreach (var arg in foreachMethod.CallArguments)
                {
                    codecontext.Write(", ");
                    methodargs.Add(arg.EmitCs(codecontext));
                }
                codecontext.Write("))" + codecontext.Output.NewLine);

                // foreach block
                codecontext.WriteLine("{");

                codecontext.Level++;

                codecontext.WriteLine(scopeLocalVarName + ".Push(null," + foreachVarName + ");");

                // Body
                Body.EmitCs(codecontext);

                //
                codecontext.WriteLine(scopeLocalVarName + ".Pop();");


                //
                codecontext.Level--;
                codecontext.WriteLine("}");

            }

            return ExpressionType.VoidType;
        }
    }
}
