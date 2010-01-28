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
    }
}
