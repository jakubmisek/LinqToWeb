﻿using System;
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

        private readonly Dictionary<ActionItem, bool> AddedActionsToDo;

        /// <summary>
        /// Init the enumerator.
        /// </summary>
        /// <param name="listContainer"></param>
        public ExtractionListEnumerator(ExtractionList<T> listContainer)
            : base( listContainer.Parent, new ActionList(listContainer.ActionsToDo) )
        {
            this.listContainer = listContainer;
            this.parametersTransform = new Dictionary<object, object>(){ {listContainer, this} };

            // save the list of known initial actions to do, listContainer may extend its list of ActionsToDo, so this.ActionsToDo must be updated
            this.AddedActionsToDo = new Dictionary<ActionItem, bool>(ActionsToDo.Count);
            foreach (var x in this.ActionsToDo) this.AddedActionsToDo[x] = true;
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

                // list of the Initial actions to do of the listContainer could be changed
                lock (listContainer.ActionsToDo)
                {
                    foreach (var action in listContainer.ActionsToDo)
                    {
                        bool exists;
                        if ( AddedActionsToDo.TryGetValue(action, out exists) && exists )
                        {
                            // action already known by this enumerator
                        }
                        else
                        {
                            // action is new
                            AddedActionsToDo[action] = true;
                            ActionsToDo.AddAction(action);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
