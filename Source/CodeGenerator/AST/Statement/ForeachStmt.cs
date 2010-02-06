using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Reflection;

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

        /// <summary>
        /// Checks the foreach method arguments and return type.
        /// Throws an exception if something is wrong.
        /// </summary>
        /// <param name="foreachMethodName"></param>
        /// <param name="argTypes"></param>
        private void CheckMethodSignature( string foreachMethodName, ExpressionType[] argTypes )
        {
            // class containing foreach methods
            Type feType = typeof(linqtoweb.Core.methods.ForeachMethods);

            // arguments type
            Type[] args = new Type[argTypes.Length + 1];
            args[0] = typeof(linqtoweb.Core.datacontext.DataContext);   // the first argument is the DataContext
            for (int i = 0; i < argTypes.Length; ++i)
            {
                args[i + 1] = argTypes[i].CorrespondingSystemType;
            }

            // find the foreach method
            MethodInfo feMethod = feType.GetMethod(foreachMethodName, args);

            if (feMethod == null)
                throw new Exception("Method " + foreachMethodName + " could not be found within the class " + feType.FullName + ", or arguments type mishmash.");

            // check the return type, which must implement the IEnumerable<LocalVariables> interface
            Type enumType = typeof(IEnumerable<linqtoweb.Core.extraction.LocalVariables>);
            foreach (var x in feMethod.ReturnType.GetInterfaces())
                if (x == enumType)
                    return; // OK
            
            throw new Exception("Method " + foreachMethodName + " does not return object that implements IEnumerable<LocalVariables> interface.");
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            if (Body != null)
            {
                MethodCall foreachMethod = ForeachExpression as MethodCall;
                if (foreachMethod == null)
                    throw new Exception("argument of foreach must be a method call");

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

                // check signature
                CheckMethodSignature(foreachMethod.MethodName, methodargs.ToArray());

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
