using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.Core.contexts
{
    /// <summary>
    /// Base container for extraction methods.
    /// </summary>
    class MethodsContainer
    {
        /// <summary>
        /// Extraction method delegate. Declared into the MethodsContainer object.
        /// </summary>
        public delegate void ExtractionMethod( /* DataContext, dictionary parameters */ );

        
    }
}
