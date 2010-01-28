using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    public class Foreach:Expression
    {
        public readonly Expression ForeachExpression;
        public readonly Expression Body;    // if null, ignore this foreach

        public Foreach(ExprPosition position, Expression foreachexpr, Expression body)
            :base(position)
        {
            this.Body = body;
            this.ForeachExpression = foreachexpr;
        }
    }
}
