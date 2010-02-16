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
        /// List of variables, which ActionsToDo can be extended in the current method call.
        /// </summary>
        private Dictionary<string, bool> CannotAddAction = null;

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
            : base(parameters)
        {

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
            foreach (var x in this)
            {
                ExtractionObjectBase eObj;
                
                if (CannotAddAction != null && CannotAddAction.ContainsKey(x.Key))
                    continue;

                // object is the ExtractionObject
                if ((eObj = x.Value as ExtractionObjectBase) != null)
                {
                    eObj.AddActionToDo(action);
                }
            }
        }

        /// <summary>
        /// Set the variables to CannotAddAction list.
        /// </summary>
        /// <param name="varsName">Array of variable names.</param>
        /// <returns>this</returns>
        public LocalVariables SetCannotAddAction( Dictionary<string,bool> vars )
        {
            foreach (var x in vars)
                if (x.Value == true)
                    SetCannotAddActionForVariable(x.Key);

            return this;
        }

        /// <summary>
        /// Determines if the AddAction method can add an action into the given variable.
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool CannotAddActionForVariable(string varName)
        {
            if (string.IsNullOrEmpty(varName))
                throw new ArgumentNullException("varName");

            string varRootName = varName.Split(new char[] { '.' })[0];

            return (CannotAddAction != null && CannotAddAction.ContainsKey(varRootName));
        }

        public void SetCannotAddActionForVariable(string varName)
        {
            if (CannotAddAction == null) CannotAddAction = new Dictionary<string, bool>();

            if (string.IsNullOrEmpty(varName))
                throw new ArgumentNullException("varName");

            if (varName.Contains('.'))
                throw new ArgumentException("Simple variable name expected.", "varName");

            CannotAddAction[varName] = true;
        }
        
    }

}
