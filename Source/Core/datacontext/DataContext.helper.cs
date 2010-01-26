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
    /// Helper methods.
    /// </summary>
    public partial class DataContext
    {
        #region helper methods (downloading, processing)

        /// <summary>
        /// Default user agent for web requests.
        /// </summary>
        protected const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; YPC 3.0.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// Get the response string from the specified HTTP web request.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="referer"></param>
        /// <param name="userAgent"></param>
        /// <param name="timeOut"></param>
        /// <param name="allowRedirect"></param>
        /// <param name="keepAlive"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        protected HttpWebResponse GetHttpResponse(Uri uri, string referer, string userAgent, int timeOut, bool allowRedirect, bool keepAlive, CookieCollection cookies)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            req.Referer = referer;
            req.UserAgent = userAgent;
            req.Timeout = timeOut;
            req.AllowAutoRedirect = allowRedirect;
            req.KeepAlive = keepAlive;
            //req.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);
            if (cookies != null)
            {   // TODO: filter cookies by domain and path
                req.CookieContainer.Add(cookies);
            }
            //req.Headers.Add("Accept-Language", "en,cs");
            
            return (HttpWebResponse)req.GetResponse();
        }

        #endregion
    }

}