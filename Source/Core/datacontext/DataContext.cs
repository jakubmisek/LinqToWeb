using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using System.Diagnostics;
using System.Net;
using System.IO;

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
    public partial class DataContext
    {
        #region internal

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
        internal void AddAction( MethodsContainer.ExtractionMethod method, LocalVariables parameters )
        {
            // create the action
            ActionItem newAction = new ActionItem(method, this, parameters);

            // add the action to all objects specified within the action parameters
            parameters.AddActionToParameters(newAction);
        }

        #endregion

        #region data source common information

        /// <summary>
        /// The data content URI identifier.
        /// </summary>
        public virtual Uri URI
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Data content.
        /// </summary>
        public virtual string Content
        {
            get
            {
                throw new NotExtractedDataException("Content is not available in the current context.");
            }
        }

        #endregion
        
    }
}
