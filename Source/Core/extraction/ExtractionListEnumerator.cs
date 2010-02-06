using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Enumerator of extracted collection.
    /// </summary>
    /// <typeparam name="T">Type of elements in enumerated collection.</typeparam>
    public class ExtractionListEnumerator<T> : ExtractionListBase<T>
    {
        #region initialization

        /// <summary>
        /// Container of this enumeration. Used for action.call parameters transformation.
        /// </summary>
        internal readonly ExtractionList<T> /*!*/listContainer;

        /// <summary>
        /// Actions already processed.
        /// </summary>
        private readonly HashSet<ActionItem> ProcessedActions = new HashSet<ActionItem>();

        /// <summary>
        /// Init the enumerator.
        /// </summary>
        /// <param name="listContainer"></param>
        public ExtractionListEnumerator(ExtractionList<T> listContainer)
            : base( listContainer.Parent )
        {
            this.listContainer = listContainer;
        }

        #endregion

        #region elements enumeration / buffering

        /// <summary>
        /// Small buffer of extracted elements waiting for the enumeration.
        /// Expecting up to 100 of elements.
        /// </summary>
        private Queue<T> ElementsBuffer = new Queue<T>();
        
        /// <summary>
        /// Add an element into the enumeration. Called by extraction method that is called by an action by DoNextAction().
        /// </summary>
        /// <param name="element">An element to be added into the enumerated collection buffer.</param>
        public override T  AddElement(T element)
        {
            lock (ElementsBuffer)
            {
                ElementsBuffer.Enqueue(element);
            }

            return element;
        }

        /// <summary>
        /// Extract and enumerate through the collection of objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (; ; )
            {
                // return elements prepared in the queue (buffer) first
                while (ElementsBuffer.Count > 0)
                {
                    T element;

                    lock (ElementsBuffer)
                    {
                        element = ElementsBuffer.Dequeue();
                    }

                    yield return element;
                }

                // extract next elements into the queue
                if (!DoNextAction(this))
                    break;
            }
        }

        /// <summary>
        /// Do next action to do, if there are no more actions to do, check the listContainer for some new action.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="callerEnumerator">Always this.</param>
        /// <returns>True if there was processed some action.</returns>
        protected override bool DoNextAction<S>(ExtractionListEnumerator<S> callerEnumerator)
        {
            if (ActionsToDo.Count == 0 && ProcessedActions.Count < listContainer.ActionsToDo.Count )
            {   // no more actions to do, and listCOntainer contains some new actions
                foreach (var x in listContainer.ActionsToDo)
                {
                    if (!ProcessedActions.Contains(x))
                    {
                        ProcessedActions.Add(x);
                        ActionsToDo.AddAction(x);
                    }
                }
            }

            // process the actions to do (mine or parent's)
            return base.DoNextAction<S>(callerEnumerator);
        }

        #endregion

    }
}
