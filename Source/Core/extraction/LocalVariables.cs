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
        /// Add variables from another collection.
        /// </summary>
        /// <param name="vars"></param>
        public void AddVariables( LocalVariables vars )
        {
            if (vars != null)
                foreach (var x in vars)
                    this[x.Key] = x.Value;
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
                    eObj.ActionsToDo.RemoveAction(action);
                }
            }
        }
    }

}
