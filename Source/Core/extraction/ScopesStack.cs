using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using linqtoweb.Core.datacontext;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Dynamic stack of variables.
    /// Must be used instead of native .NET variables, because some extraction methods are able to add variables in runtime.
    /// </summary>
    public class ScopesStack
    {
        private readonly LocalVariables variables = new LocalVariables();

        private readonly Stack<DataContext> datacontextstack = new Stack<DataContext>();
        private DataContext currentDataContext = null;
        
        /// <summary>
        /// Init new stack of variables.
        /// </summary>
        /// <param name="vars"></param>
        public ScopesStack(DataContext context, LocalVariables vars)
        {
            Push(context, vars);
        }

        /// <summary>
        /// Start new scope.
        /// </summary>
        /// <param name="vars">Variables. Can be null.</param>
        /// <param name="context">data context</param>
        public void Push(DataContext context, LocalVariables vars)
        {
            // local variables
            variables.AddVariables(vars);

            // data context
            if (context != null)
                currentDataContext = context;

            datacontextstack.Push(currentDataContext);
        }

        /// <summary>
        /// Close scope.
        /// </summary>
        public void Pop()
        {
            // data context
            datacontextstack.Pop();
            currentDataContext = datacontextstack.Peek();
        }

        /// <summary>
        /// Current data context.
        /// </summary>
        public DataContext context
        {
            get
            {
                return currentDataContext;
            }
        }

        /// <summary>
        /// Access the variable on the current scope.
        /// </summary>
        /// <param name="varName">Variable name.</param>
        /// <returns>Variable value.</returns>
        public object this[string varName]
        {
            get
            {
                return variables[varName];
            }
            set
            {
                variables[varName] = value;
            }
        }
        
    }
}
