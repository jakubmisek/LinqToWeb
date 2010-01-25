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
    /// Methods for creating new DataContexts
    /// </summary>
    public partial class DataContext
    {
        #region new data contexts

        /// <summary>
        /// New HTML page context.
        /// </summary>
        /// <param name="relativeurl"></param>
        /// <returns></returns>
        public DataContext OpenHtml(string relativeurl)
        {
            return new HtmlContext(this, relativeurl);
        }

        #endregion
    }

}