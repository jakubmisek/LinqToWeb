using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using linqtoweb.Core.storage;
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
    /// - TODO: cache object
    /// - methods for creating new DataContexts
    /// - downloading and processing methods
    /// </summary>
    public partial class DataContext
    {
        #region internal

        /// <summary>
        /// Storage for already known data. Can be null.
        /// </summary>
        private readonly StorageBase DataCache;

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext( Uri uri, DataContext referer, StorageBase cache)
        {
            this.ContextUri = uri;
            this.RefererContext = referer;
            this.DataCache = cache;
        }

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext(Uri uri, DataContext referer)
            : this(uri, referer, (referer != null) ? referer.DataCache : null)
        {

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
