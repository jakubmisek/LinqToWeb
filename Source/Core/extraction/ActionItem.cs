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
        // method, context, named parameters

        /// <summary>
        /// Method to be executed.
        /// </summary>
        private readonly MethodsContainer.ExtractionMethod method;

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
        public ActionItem(MethodsContainer.ExtractionMethod method, DataContext datacontext, LocalVariables parameters)
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
        internal void CallAction(Dictionary<object, object> parametersTransform)
        {
            // transform parameters to use with the extraction method (remove lists)
            LocalVariables transformedParameters = new LocalVariables(parameters, parametersTransform);

            // remove this action from all the objects passed by parameter
            transformedParameters.RemoveActionFromParameters(this);

            // call the method synchronously
            method(datacontext, transformedParameters);
        }
    }
}
