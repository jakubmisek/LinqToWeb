using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace linqtoweb.Core.datacontext
{
    internal class HtmlContext : DataContext
    {
        /// <summary>
        /// Initialize the HTML context by given URL (or relative URL) address.
        /// </summary>
        /// <param name="referrer"></param>
        /// <param name="url"></param>
        internal HtmlContext(DataContext referer, string relativeurl)
            : base((referer != null && referer.ContextUri != null) ? (new Uri(referer.ContextUri, relativeurl)) : (new Uri(relativeurl)),referer)
        {

        }

        private bool RequestProcessed = false;

        /// <summary>
        /// Process the request (once). Get the response (page content, cookies, ...).
        /// </summary>
        private void ProcessRequest()
        {
            if (RequestProcessed)
                return; // test, unlocked
                
            lock(this)
            {
                if (RequestProcessed) return; // test again
                RequestProcessed = true;

                // cache

                // get response
                HttpWebResponse resp = GetHttpResponse(
                    ContextUri,
                    (RefererContext != null && RefererContext.ContextUri != null) ? RefererContext.ContextUri.AbsoluteUri : null,
                    DefaultUserAgent,
                    15000,
                    false,
                    false,
                    (RefererContext != null) ? RefererContext.Cookies : null);

                // page content
                _Content = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                // cookies
                if (resp.Cookies != null && resp.Cookies.Count > 0)
                {
                    _Cookies = new CookieCollection();
                    _Cookies.Add(resp.Cookies);
                }

                // headers

                // content type, encoding

                // close response
                resp.Close();
            }
        }

        private CookieCollection _Cookies = null;

        /// <summary>
        /// Page cookies.
        /// </summary>
        public override CookieCollection Cookies
        {
            get
            {
                ProcessRequest();
           
                return _Cookies;
            }
        }

        private string _Content = null;

        /// <summary>
        /// HTML page content.
        /// </summary>
        public override string Content
        {
            get
            {
                ProcessRequest();

                return _Content;
            }
        }
    }
}
