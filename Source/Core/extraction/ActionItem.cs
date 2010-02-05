using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;
using System.Diagnostics;

namespace linqtoweb.Core.extraction
{
    public class ActionItem
    {
        /// <summary>
        /// Extraction method delegate. Declared into the MethodsContainer object.
        /// </summary>
        public delegate void ExtractionMethod(DataContext _datacontext, LocalVariables _parameters);

        /// <summary>
        /// Method to be executed.
        /// </summary>
        private readonly ExtractionMethod method;

        /// <summary>
        /// Data context.
        /// </summary>
        private readonly DataContext datacontext;

        /// <summary>
        /// Method parameters.
        /// </summary>
        private readonly LocalVariables parameters;


        /// <summary>
        /// Initialization of new extraction action.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="datacontext"></param>
        /// <param name="parameters"></param>
        public ActionItem(ExtractionMethod method, DataContext datacontext, LocalVariables parameters)
        {
            Debug.Assert(method != null);
            Debug.Assert(datacontext != null);
            Debug.Assert(parameters != null);

            this.method = method;
            this.datacontext = datacontext;
            this.parameters = parameters;
        }

        /// <summary>
        /// Execute the action.
        /// Call the method on the specified data context with specified parameters.
        /// 
        /// TODO: method will not be executed if some parameter does not satisfy conditions.
        /// </summary>
        /// <param name="parametersTransform">List of transformations that will be performed onto the parameters list.</param>
        internal void CallAction<T>(ExtractionListEnumerator<T> callerEnumerator)
        {
            // parameters that will be passed to the extraction method.
            LocalVariables transformedParameters = new LocalVariables();

            // transform the parameters to use with the extraction method
            foreach (var pair in parameters)
            {
                ExtractionObjectBase eObj;
                
                if ((eObj = pair.Value as ExtractionObjectBase) != null)
                {   // transform the parameter by transformation method

                    bool containsAction = (eObj.ActionsToDo != null && eObj.ActionsToDo.ContainsAction(this));
                    ExtractionObjectBase newValue = eObj.TransformArgument(containsAction, callerEnumerator);
                    newValue.RemoveActionToDo(this);
                    
                    transformedParameters[pair.Key] = newValue;

                    if (!containsAction)// add variable that can add an ActionItem within the following method call
                        transformedParameters.SetCannotAddActionForVariable(pair.Key);
                }
                else
                {   // parameter is a value, do not transform
                    transformedParameters[pair.Key] = pair.Value;
                }
            }
            
            // call the method synchronously
            method(datacontext, transformedParameters);
        }

        /// <summary>
        /// Create new action and adds it into the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="method"></param>
        public static void AddAction(ExtractionMethod method, DataContext context, LocalVariables parameters)
        {
            // create the action
            ActionItem newAction = new ActionItem(method, context, parameters);

            // add the action to all objects specified within the action parameters
            parameters.AddActionToParameters(newAction);
        }

        /// <summary>
        /// Create new actions and add them into the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="methods">List of methods to do.</param>
        public static void AddAction(ExtractionMethod[] methods, DataContext context, LocalVariables parameters)
        {
            foreach (var m in methods)
            {
                // create the action
                ActionItem newAction = new ActionItem(m, context, parameters);

                // add the action to all objects specified within the action parameters
                parameters.AddActionToParameters(newAction);
            }
        }
    }
}
