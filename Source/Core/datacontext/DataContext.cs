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
using System.Xml;

namespace linqtoweb.Core.datacontext
{
    #region DataContext interfaces, some DataContexts can implement it

    /// <summary>
    /// DataContext defining content.
    /// </summary>
    public interface IDataContextContent
    {
        /// <summary>
        /// The text content.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// The content binary stream.
        /// </summary>
        Stream ContentStream { get; }
    }

    /// <summary>
    /// DataContext with cookies.
    /// </summary>
    public interface IDataContextCookies
    {
        /// <summary>
        /// Collection of cookies known by the DataContext.
        /// </summary>
        CookieCollection Cookies { get; }
    }

    #endregion

    /// <summary>
    /// Page source data context, where the extraction method is called.
    /// Used by extraction method.
    /// Contains
    /// - information about the page (data source).
    /// - cache object
    /// - methods for creating new DataContexts
    /// - downloading and processing methods
    /// - implements IDataContextContent and IDataContextCookies by default
    /// </summary>
    public partial class DataContext : IDataContextContent, IDataContextCookies
    {
        #region internal

        /// <summary>
        /// Storage for already known data. Cannot be null.
        /// </summary>
        public StorageBase DataCache
        {
            get
            {
                return _DataCache;
            }
        }
        private StorageBase _DataCache = null;

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext(Uri uri, DataContext referer, StorageBase cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            _ContextUri = uri;
            _RefererContext = referer;
            _DataCache = cache;
        }

        /// <summary>
        /// Empty (initial) data context. Does not contain any data.
        /// </summary>
        internal DataContext(Uri uri, DataContext referer)
            : this(uri, referer, (referer != null) ? referer.DataCache : null)
        {

        }

        #endregion

        #region DataContext URI

        /// <summary>
        /// Referer data context. Can be null.
        /// </summary>
        public virtual DataContext RefererContext
        {
            get
            {
                return _RefererContext;
            }
        }
        protected DataContext _RefererContext = null;

        /// <summary>
        /// The data content URI identifier. Can be null.
        /// </summary>
        public virtual Uri ContextUri
        {
            get
            {
                return _ContextUri;
            }
        }
        protected Uri _ContextUri = null;

        #endregion

        #region IDataContextCookies Members

        /// <summary>
        /// All the known cookies in the current context. Can be null (no cookies).
        /// </summary>
        public virtual CookieCollection Cookies
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IDataContextContent Members

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

        /// <summary>
        /// Content stream. Can be null (stream not available).
        /// </summary>
        public virtual Stream ContentStream
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}