using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
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

        #region elements

        // TODO: small cache of extracted elements
        
        /// <summary>
        /// Add an element into the enumeration. Called by extraction method that is called by an action by DoNextAction().
        /// </summary>
        /// <param name="element">An element to be added into the enumerated collection buffer.</param>
        public override void  AddElement(T element)
        {
            // TODO: add element into the cache
        }

        /// <summary>
        /// Extract and enumerate through the collection of objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            // TODO: collection enumeration
            // if element in cache, yield return it
            // if !DoNextAction(parametersTransform) break;

            throw new NotImplementedException();
        }

        #endregion

    }
}
