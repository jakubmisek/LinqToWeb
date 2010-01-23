using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.extraction
{
    class ExtractionListEnumerator<T> : ExtractionObjectBase
    {
        // small cache of extracted elements
        

        public ExtractionListEnumerator(ExtractionList<T> listContainer)
        {
            // ve sve kopii akci zmenit parametr rovny listContainer na this
            
        }

        public void Add(T element)
        {
            // add element into the cache
        }

        public IEnumerator<T> GetEnumerator()
        {
            // if element in cache, yield return it
            // if !DoNextAction break;

            throw new NotImplementedException();
        }

    }
}
