using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using HtmlAgilityPack;

namespace linqtoweb.Core.datacontext
{
    internal class XPathContext : DataContext
    {
        /// <summary>
        /// Initialize the HTML context by given URL (or relative URL) address.
        /// </summary>
        /// <param name="referrer"></param>
        /// <param name="url"></param>
        internal XPathContext(DataContext referer, string xpath)
            : base(referer.ContextUri, referer)
        {
            this.XPathQuery = xpath;
        }

        private readonly string XPathQuery;

        private string StorageKey
        {
            get
            {
                return
                    ContextUri.AbsoluteUri +
                    ((RefererContext != null && RefererContext.ContextUri != null) ? RefererContext.ContextUri.AbsoluteUri.GetHashCode() : 0) +
                    "xpath" + XPathQuery.GetHashCode()
                    ;
            }
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

                _Content = (string)DataCache.GetItem(StorageKey,
                    delegate(out DateTime expiration)
                    {
                        StringBuilder str = new StringBuilder();

                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(RefererContext.Content);
                        var navigator = doc.CreateNavigator();

                        foreach (var x in navigator.Select(XPathQuery))
                        {
                            HtmlNodeNavigator node = x as HtmlNodeNavigator;
                            if (node != null)
                            {
                                str.Append(node.InnerXml);
                            }

                        }

                        expiration = DateTime.Now.AddHours(1.0);
                        return str.ToString();
                    }
                    );
            }
        }

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
