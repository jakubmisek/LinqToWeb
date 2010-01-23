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
    class ExtractionList<T> : IEnumerable<T>
    {
        // initial todo list // must be passed in ctor

        // variable name
        
        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return (new ExtractionListEnumerator<T>( this /*, initial todo list*/ )).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
