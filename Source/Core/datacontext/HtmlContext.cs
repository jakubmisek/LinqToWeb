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

        private string StorageKey
        {
            get
            {
                return
                    ContextUri.AbsoluteUri +
                    ((RefererContext != null && RefererContext.ContextUri != null) ? RefererContext.ContextUri.AbsoluteUri.GetHashCode() : 0) +
                    ((RefererContext != null && RefererContext.Cookies != null)?("todo"):(string.Empty))
                    ;
            }
        }

        private class StorageValue
        {
            public string Content { get; set; }
            public CookieCollection Cookies { get; set; }
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

                StorageValue data = (StorageValue)DataCache.GetItem(StorageKey,
                    delegate(out DateTime expiration)
                    {
                        WebRequest webreq = WebRequest.Create(ContextUri);

                        string RespContent = null;
                        CookieCollection RespCookies = null;

                        if (webreq is HttpWebRequest)
                        {
                            HttpWebRequest req = (HttpWebRequest)webreq;

                            req.Referer = (RefererContext != null && RefererContext.ContextUri != null) ? RefererContext.ContextUri.AbsoluteUri : null;
                            req.UserAgent = DefaultUserAgent;
                            req.Timeout = 30000;
                            req.AllowAutoRedirect = false;
                            req.KeepAlive = false;
                            if (RefererContext != null && RefererContext.Cookies != null)
                            {   // TODO: filter cookies by domain and path
                                req.CookieContainer = new CookieContainer();
                                req.CookieContainer.Add(RefererContext.Cookies);
                            }
                            //req.Headers.Add("Accept-Language", "en,cs");

                            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                            // page content
                            Stream RespStream = resp.GetResponseStream();
                            RespContent = new StreamReader(RespStream/*, Encoding.GetEncoding(resp.ContentEncoding)*/).ReadToEnd();
                            RespStream.Close();

                            // cookies
                            if (resp.Cookies != null && resp.Cookies.Count > 0)
                            {
                                RespCookies = new CookieCollection();
                                RespCookies.Add(resp.Cookies);
                            }

                            // TODO: headers (language, cache expire, content type, encoding, Response URI, ...)

                            // close the response
                            resp.Close();
                        }
                        else if (webreq is FileWebRequest)
                        {
                            FileWebRequest req = (FileWebRequest)webreq;
                            FileWebResponse resp = (FileWebResponse)req.GetResponse();

                            // page content
                            Stream RespStream = resp.GetResponseStream();
                            RespContent = new StreamReader(RespStream/*, Encoding.GetEncoding(resp.ContentEncoding)*/).ReadToEnd();
                            RespStream.Close();

                            // close the response
                            resp.Close();
                        }                        

                        expiration = DateTime.Now.AddHours(1.0);    // TODO: time based on HTML header or HtmlContext parameters
                        
                        return new StorageValue() { Content = RespContent, Cookies = RespCookies };
                    }
                    );

                _Content = data.Content;
                _Cookies = data.Cookies;
            }
        }

        #region IDataContextCookies Members

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
        private CookieCollection _Cookies = null;

        #endregion

        #region IDataContextContent Members

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
        private string _Content = null;

        #endregion

    }
}
