using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Base object containing extracted elements in collection (list).
    /// </summary>
    class ExtractionListBase<T> : ExtractionObjectBase
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public ExtractionListBase()
        {
        }

        /// <summary>
        /// Initialization with initial action list.
        /// </summary>
        /// <param name="initialActionList">Initial ActionList.</param>
        public ExtractionListBase(ActionList initialActionList)
            :base(initialActionList)
        {

        }

        /// <summary>
        /// Add new element into the list buffer.
        /// </summary>
        public virtual void AddElement(T element)
        {
            throw new NotImplementedException();
        }
    }
}
