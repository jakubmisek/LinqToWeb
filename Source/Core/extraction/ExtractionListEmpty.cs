using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// List used for no-action,
    /// always empty, with no actions list.
    /// </summary>
    /// <typeparam name="T">Type of elements in this empty collection.</typeparam>
    class ExtractionListEmpty<T> : ExtractionListBase<T>
    {
        /// <summary>
        /// static empty list with no elements and no action list.
        /// </summary>
        public static ExtractionListEmpty<T> EmptyList = new ExtractionListEmpty<T>();

        /// <summary>
        /// Default initialization.
        /// </summary>
        public ExtractionListEmpty()
            :base(null)
        {

        }

        /// <summary>
        /// add no element.
        /// </summary>
        /// <param name="element">ignored</param>
        public override T AddElement(T element)
        {
            // do nothing
            return element;
        }

        /// <summary>
        /// do no action.
        /// </summary>
        /// <param name="parametersTransform">ignored</param>
        /// <returns>Always false.</returns>
        protected override bool DoNextAction(Dictionary<object, object> parametersTransform)
        {
            // do nothing
            return false;
        }
    }
}
