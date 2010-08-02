using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;
using linqtoweb.Core.methods;
using linqtoweb.Core.storage;

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
        public ExtractionContext(StorageBase cache)
        {
            // the initial location of the extraction process
            System.Reflection.Assembly entryPoint = System.Reflection.Assembly.GetEntryAssembly();
            if (entryPoint == null) entryPoint = System.Reflection.Assembly.GetExecutingAssembly();

            InitialDataContext = new DataContext(new Uri(entryPoint.Location), null, cache);
        }

        /// <summary>
        /// Initialize the context with the default storage object.
        /// </summary>
        public ExtractionContext()
            :this(new StorageMemory())
        {

        }

        #region Helper

        /// <summary>
        /// Method that can be called by ActionList mechanism, that adds an element into the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datacontext"></param>
        /// <param name="parameters"></param>
        private static void AddElementMethod<T>( DataContext datacontext, LocalVariables parameters )
        {
            ((ExtractionListBase<T>)parameters["list"]).AddElement((T)parameters["element"]);
        }

        /// <summary>
        /// Creates an action that adds an element into given list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="list"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        protected static T AddElementAction<T>( DataContext context, LocalVariables parameters, string listVarName, ExtractionListBase<T> list, T element )
        {
            ActionItem.AddAction(
                AddElementMethod<T>,
                context,
                new LocalVariables(new Dictionary<string, object>() { { "list", list }, { "element", element } })
                    .SetCannotAddAction(new Dictionary<string, bool>() { { "list", parameters.CannotAddActionForVariable(listVarName) } })
                    );

            return element;
        }

        #endregion
    }
}
