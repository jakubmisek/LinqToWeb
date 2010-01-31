using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;
using linqtoweb.Core.methods;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Context of the whole extraction task.
    /// Contains
    /// - Root ExtractionObjects with initial ActionsToDo list
    /// - Initial DataContext (containing used (shared) cache object)
    /// - etc.
    /// </summary>
    public class ExtractionContext
    {
        /// <summary>
        /// Initial data context.
        /// </summary>
        protected readonly DataContext InitialDataContext;

        /// <summary>
        /// Initialize the context.
        /// </summary>
        public ExtractionContext( /*cache*/ )
        {
            InitialDataContext = new DataContext( null, null );

            InitActionsToDo();
        }

        #region context objects initialization

        /// <summary>
        /// Initialize ActionsToDo lists for context objects.
        /// </summary>
        protected virtual void InitActionsToDo()
        {
           
        }

        #endregion
    }
}
