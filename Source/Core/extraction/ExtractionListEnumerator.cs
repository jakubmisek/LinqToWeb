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
    class ExtractionListEnumerator<T> : ExtractionListBase<T>
    {
        #region initialization

        /// <summary>
        /// Container of this enumeration. Used for action.call parameters transformation.
        /// </summary>
        private readonly ExtractionList<T> /*!*/listContainer;

        /// <summary>
        /// Arguments of methods call that will be transformed in specified way. (listContainer -> this)
        /// </summary>
        private readonly Dictionary<object, object> parametersTransform;

        /// <summary>
        /// Init the enumerator.
        /// </summary>
        /// <param name="listContainer"></param>
        public ExtractionListEnumerator(ExtractionList<T> listContainer)
            : base( new ActionList(listContainer.ActionsToDo) )
        {
            this.listContainer = listContainer;

            this.parametersTransform = new Dictionary<object, object>(){ {listContainer, this} };
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
                if (!DoNextAction(parametersTransform))
                    break;
            }
        }

        #endregion

    }
}
