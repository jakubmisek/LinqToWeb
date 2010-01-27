using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;

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

        #region context objects

        public readonly ExtractionList<string> sampleList = new ExtractionList<string>();

        #endregion

        #region context objects initialization

        /// <summary>
        /// Initialize ActionsToDo lists for context objects.
        /// </summary>
        protected virtual void InitActionsToDo()
        {
            ScopesStack l = new ScopesStack(InitialDataContext, null);

            // initialize the context objects here
            // OpenHtml("http://www.freesutra.cz/").Categories( sampleList );
            l.context
                .OpenContextDynamic(null, new object[] { "http://www.freesutra.cz/" })
                    .AddAction(
                        MethodsContainer.Categories,
                        new LocalVariables(
                            new Dictionary<string, object>() {
                                {"sampleList", sampleList}
                            }));
        }

        #endregion
    }
}
