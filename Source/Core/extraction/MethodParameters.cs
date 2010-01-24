using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    class MethodParameters : Dictionary<string, object>
    {
        /// <summary>
        /// Creates an empty parameters collection.
        /// </summary>
        public MethodParameters()
        {

        }

        /// <summary>
        /// Create collection of parameters transformed from another collection of parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parametersTransform"></param>
        public MethodParameters( MethodParameters parameters, Dictionary<object,object> parametersTransform )
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
}
