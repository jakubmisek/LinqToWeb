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
        /// As a result of clicking on a link on the current page.
        /// </summary>
        /// <param name="relativeurl">Relative or absolute URL.</param>
        /// <returns></returns>
        public DataContext OpenHtml(string relativeurl)
        {
            return new HtmlContext(this, relativeurl);
        }

        /// <summary>
        /// Page context as a result of HTML form submission.
        /// As a result of clicking on a form button on the current page.
        /// </summary>
        /// <returns></returns>
        public DataContext SubmitForm( /* input values, submit button (in DOM tree) */ )
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}