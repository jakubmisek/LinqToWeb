using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using linqtoweb.Core.extraction;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Reflection;

namespace linqtoweb.Core.datacontext
{
    /// <summary>
    /// Methods for creating new DataContexts
    /// </summary>
    public partial class DataContext
    {
        /// <summary>
        /// The name of the method that is able to create new context, if the name of the method is not specified.
        /// Method must return DataContext object.
        /// </summary>
        private static string DefaultOpenContextMethodName = "open";

        /// <summary>
        /// Dynamically find a method that creates new context using specified arguments.
        /// </summary>
        /// <param name="methodName">The method name. The parameter can be null, then the default method name is used.</param>
        /// <param name="arguments">Arguments that will be passed to the method.</param>
        /// <returns>New DataContext.</returns>
        /// <remarks>An exception ArgumentOutOfRangeException can be thrown if no matching method is found.</remarks>
        public DataContext OpenContextDynamic( string methodName, object[] arguments )
        {
            // method name
            if (methodName == null)
                methodName = DefaultOpenContextMethodName;

            //
            Type t = this.GetType();

            // get arguments type
            List<Type> typeArgs = new List<Type>(arguments.Length);

            if (arguments != null)
                foreach (object obj in arguments)
                    typeArgs.Add(obj.GetType());

            // find a method
            MethodInfo m = t.GetMethod(methodName, typeArgs.ToArray());
            
            if (m == null || m.ReturnType != typeof(DataContext))
                throw new ArgumentOutOfRangeException("Specified arguments does not match any method .");

            // call method
            return (DataContext)m.Invoke(this, arguments);
        }

        #region default context builders

        /// <summary>
        /// New HTML page context.
        /// As a result of clicking on a link on the current page.
        /// </summary>
        /// <param name="relativeurl">Relative or absolute URL.</param>
        /// <returns></returns>
        public virtual DataContext open(string relativeurl)
        {
            return new HtmlContext(this, relativeurl);
        }

        /// <summary>
        /// Page context as a result of HTML form submission.
        /// As a result of clicking on a form button on the current page.
        /// </summary>
        /// <returns></returns>
        public virtual DataContext open( /* input values, submit button (in DOM tree) */ )
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}