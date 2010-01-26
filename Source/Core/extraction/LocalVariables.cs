using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.Core.extraction
{
    public class LocalVariables : Dictionary<string, object>
    {
        /// <summary>
        /// Creates an empty parameters collection.
        /// </summary>
        public LocalVariables()
        {

        }

        /// <summary>
        /// Creates parameters collection from a dictionary object.
        /// </summary>
        /// <param name="parameters"></param>
        public LocalVariables( Dictionary<string,object> parameters )
            :base(parameters)
        {

        }

        /// <summary>
        /// Creates parameters collection from two dictionary objects.
        /// </summary>
        /// <param name="parameters"></param>
        public LocalVariables(Dictionary<string, object> parameters, Dictionary<string, object> parameters2)
            : base(parameters)
        {
            if (parameters2 != null)
                foreach (var x in parameters2)
                    this[x.Key] = x.Value;
        }

        /// <summary>
        /// Creates collection of parameters transformed from another collection of parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parametersTransform"></param>
        public LocalVariables( LocalVariables parameters, Dictionary<object,object> parametersTransform )
        {
            foreach ( KeyValuePair<string, object> pair in parameters )
            {
                ExtractionObjectBase eObj;
                object newValue;

                if (parametersTransform != null && parametersTransform.TryGetValue(pair.Value, out newValue))
                {   // transform the parameter by specified transformation map
                    this[pair.Key] = newValue;
                }
                else if ((eObj = pair.Value as ExtractionObjectBase) != null)
                {   // transform the parameter by default transformation method
                    this[pair.Key] = eObj.TransformParameter();
                }
                else
                {   // parameter is a value, do not transform
                    this[pair.Key] = pair.Value;
                }
            }
        }

        /// <summary>
        /// Add given action to the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="action"></param>
        public void AddActionToParameters( ActionItem action )
        {
            foreach (object obj in this.Values)
            {
                ExtractionObjectBase eObj;

                // object is the ExtractionObject
                if ((eObj = obj as ExtractionObjectBase) != null)
                {
                    eObj.ActionsToDo.AddAction(action);
                }
            }
        }

        /// <summary>
        /// Remove all the occurrences of specified action from the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveActionFromParameters( ActionItem action )
        {
            foreach (object obj in this.Values)
            {
                ExtractionObjectBase eObj;

                // object is the ExtractionObject
                if ((eObj = obj as ExtractionObjectBase) != null)
                {
                    eObj.ActionsToDo.Remove(action);
                }
            }
        }
    }

    /// <summary>
    /// Dynamic stack of variables.
    /// Must be used instead of native .NET variables, because some extraction methods are able to add variables in runtime.
    /// </summary>
    public class VariablesStack
    {
        private Stack<LocalVariables> stack = new Stack<LocalVariables>();
        private LocalVariables currentScope = null;

        /// <summary>
        /// Init new stack of variables.
        /// </summary>
        /// <param name="vars"></param>
        public VariablesStack( LocalVariables vars )
        {
            Push(vars);
        }

        /// <summary>
        /// Start new scope.
        /// </summary>
        /// <param name="vars"></param>
        public void Push( LocalVariables vars )
        {
            if (currentScope == null)
            {
                currentScope = new LocalVariables(vars);
            }
            else
            {
                currentScope = new LocalVariables(currentScope, vars);
            }

            stack.Push(currentScope);
        }

        /// <summary>
        /// Close scope.
        /// </summary>
        public void Pop()
        {
            stack.Pop();

            currentScope = (stack.Count > 0) ? stack.Peek() : null;
        }

        /// <summary>
        /// Access the variable on the current scope.
        /// </summary>
        /// <param name="varName">Variable name.</param>
        /// <returns>Variable value.</returns>
        public object this[ string varName ] 
        {
            get
            {
                Debug.Assert(currentScope != null);

                return currentScope[varName];
            }
            set
            {
                Debug.Assert(currentScope != null);

                currentScope[varName] = value;
            }
        }

        /// <summary>
        /// Get scopes count.
        /// </summary>
        public int Count
        {
            get
            {
                return stack.Count;
            }
        }
    }
}
