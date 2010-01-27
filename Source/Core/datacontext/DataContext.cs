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
        internal DataContext( Uri uri, DataContext referer /*, cache*/)
        {
            this.ContextUri = uri;
            this.RefererContext = referer;
        }

        #endregion

        #region calling methods on context

        /// <summary>
        /// Create new action and adds it into the parameter's ActionsToDo list.
        /// </summary>
        /// <param name="method"></param>
        public void AddAction( MethodsContainer.ExtractionMethod method, LocalVariables parameters )
        {
            // create the action
            ActionItem newAction = new ActionItem(method, this, parameters);

            // add the action to all objects specified within the action parameters
            parameters.AddActionToParameters(newAction);
        }

        #endregion

        #region data source common information

        /// <summary>
        /// Referer data context. Can be null.
        /// </summary>
        public DataContext RefererContext { get; private set; }

        /// <summary>
        /// The data content URI identifier. Can be null.
        /// </summary>
        public Uri ContextUri { get; private set; }

        /// <summary>
        /// Cookies in the current context. Can be null (no cookies).
        /// </summary>
        public virtual CookieCollection Cookies
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
