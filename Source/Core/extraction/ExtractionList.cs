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
    class ExtractionList<T> : ExtractionListBase<T>, IEnumerable<T>
    {    
        #region ExtractionListBase

        public override void AddElement(T element)
        {
            // do nothing, should not be called!
            throw new InvalidOperationException("Should not be called. AddElement() method should be called on the ExtractionListEnumerator<T> object instance only!");
        }

        internal override bool DoNextAction(Dictionary<object, object> parametersTransform)
        {
            throw new InvalidOperationException("Should not be called. DoNextAction() method should be called on the ExtractionListEnumerator<T> object instance only!");
        }

        /// <summary>
        /// List is not passed to extraction method. The collection is filled only when the collection is enumerated using GetEnumerator() method.
        /// Therefore this list container is ignored in extraction methods.
        /// </summary>
        /// <returns>Static EmptyList, without action list, without elements.</returns>
        internal override object TransformParameter()
        {
            return ExtractionListEmpty<T>.EmptyList;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return (new ExtractionListEnumerator<T>( this )).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // TODO: non-typed enumeration
            throw new NotImplementedException();
        }

        #endregion
    }
}
