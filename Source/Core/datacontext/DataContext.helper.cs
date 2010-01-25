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

        protected string ReadWebRequest(Uri uri)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            req.Timeout = 15000;
            req.AllowAutoRedirect = true;
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; YPC 3.0.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            req.Referer = "";
            req.Headers.Add("Accept-Language", "en,cs");
            
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd();
        }

        #endregion
    }

}