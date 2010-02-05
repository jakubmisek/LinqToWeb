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
        /// List of actions that was added into the local ActionsToDo list.
        /// </summary>
        private readonly ActionList AddedActionsToDo;

        /// <summary>
        /// Init the enumerator.
        /// </summary>
        /// <param name="listContainer"></param>
        public ExtractionListEnumerator(ExtractionList<T> listContainer)
            : base( listContainer.Parent, new ActionList(listContainer.ActionsToDo) )
        {
            this.listContainer = listContainer;
            
            // save the list of known initial actions to do, listContainer may extend its list of ActionsToDo, so this.ActionsToDo must be updated
            this.AddedActionsToDo = new ActionList(listContainer.ActionsToDo);  // shadow copy of used ActionList
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

                // list of the Initial actions to do of the listContainer could be changed
                CheckForNewActionsToDo();
            }
        }

        /// <summary>
        /// Checks the listContainer for new actions to do.
        /// They could be added during the Parent.DoNextAction() called by this.DoNextAction().
        /// </summary>
        private void CheckForNewActionsToDo()
        {
            if (listContainer.ActionsToDo.Items.Count > AddedActionsToDo.Items.Count)
                lock (listContainer.ActionsToDo)// always not null
                {
                    foreach (var action in listContainer.ActionsToDo.Items)
                    {
                        if (AddedActionsToDo.ItemsByAction.ContainsKey(action) /*&& exists*/)
                        {
                            // action already known by this enumerator
                            // following actions will be known too, so exit
                            return;
                        }
                        else
                        {
                            // action is new
                            AddedActionsToDo.AddAction(action);
                            ActionsToDo.AddAction(action);
                        }
                    }
                }
        }

        #endregion

    }
}
