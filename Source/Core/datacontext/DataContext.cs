using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.extraction;

namespace linqtoweb.Core.datacontext
{
    /// <summary>
    /// Page source data context, where the extraction method is called.
    /// Used by extraction method.
    /// Contains
    /// - information about the page (data source).
    /// - cache object
    /// - extraction techniques (methods like RegExp, DOM, ...)
    /// - methods for creating new DataContexts
    /// - downloading and processing methods
    /// </summary>
    class DataContext
    {
        // TODO: internal cache object

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext(/*cache*/)
        {
            
        }

        /// <summary>
        /// Create new action and adds it into the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="method"></param>
        internal virtual void AddAction( MethodsContainer.ExtractionMethod method, MethodParameters parameters )
        {
            // create the action
            ActionItem newAction = new ActionItem(method, this, parameters);

            // add the action to all objects specified within the action parameters
            parameters.AddActionToParameters(newAction);
        }

        #region new data contexts

        #endregion

        #region data source information

        #endregion

        #region extraction methods (RegExp, DOM, ...)

        #endregion

        #region helper methods (downloading, processing)

        #endregion
    }
}
