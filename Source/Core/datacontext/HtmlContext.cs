using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.datacontext
{
    internal class HtmlContext : DataContext
    {
        /// <summary>
        /// Initialize the HTML context by given URL (or relative URL) address.
        /// </summary>
        /// <param name="referrer"></param>
        /// <param name="url"></param>
        internal HtmlContext(DataContext referrer, string relativeurl)
        {
            _URI = (referrer.URI == null) ? new Uri(relativeurl) : new Uri(referrer.URI, relativeurl);
        }

        /// <summary>
        /// HTML page URL.
        /// </summary>
        protected readonly Uri _URI;
        
        /// <summary>
        /// HTML page URI.
        /// </summary>
        public override Uri URI
        {
            get
            {
                return _URI;
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
                if (_Content == null)
                    _Content = ReadWebRequest(URI);

                return _Content;
            }
        }
    }
}
