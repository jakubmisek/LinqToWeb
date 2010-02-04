using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Container for the enumerator object. Represents the collection of data described by the list of actions to do.
    /// </summary>
    /// <typeparam name="T">The type of collection element.</typeparam>
    public class ExtractionList<T> : ExtractionListBase<T>, IEnumerable<T>
    {    
        #region ExtractionListBase

        /// <summary>
        /// ctor
        /// </summary>
        public ExtractionList()
            :base()
        {

        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="parent"></param>
        public ExtractionList(ExtractionObjectBase parent)
            :base(parent)
        {

        }

        public override T AddElement(T element)
        {
            // do nothing, should not be called!
            throw new InvalidOperationException("Should not be called. AddElement() method should be called on the ExtractionListEnumerator<T> object instance only!");
        }

        protected override bool DoNextAction(Dictionary<object, object> parametersTransform)
        {
            // do nothing, should not be called!
            throw new InvalidOperationException("Should not be called. DoNextAction() method should be called on the ExtractionListEnumerator<T> object instance only!");
        }

        /// <summary>
        /// List is not passed to extraction method. The collection is filled only when the collection is enumerated using GetEnumerator() method.
        /// Therefore this list container is ignored in extraction methods and it does not invoke other extraction methods (has empty ActionsToDo constantly).
        /// The returned object is also not reachable by the user.
        /// </summary>
        /// <returns>Static EmptyList, without action list, without elements. Unable to add elements into this.</returns>
        internal override object TransformParameter()
        {
            return ExtractionListEmpty<T>.EmptyList;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Generic collection enumeration.
        /// Starts the enumeration using new ExtractionListEnumerator object, with the initial ActionsToDo list.
        /// It causes extraction process of this collection from the beginning; enumerated collection is gathered every time the collection is being enumerated.
        /// </summary>
        /// <returns>New list enumerator object instance.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (new ExtractionListEnumerator<T>(this)).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Non-generic enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (T element in this)
            {
                yield return (object)element;
            }
        }

        #endregion
    }
}
