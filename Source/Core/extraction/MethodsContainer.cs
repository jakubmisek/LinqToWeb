using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;
using linqtoweb.Core.methods;
using System.Diagnostics;

namespace linqtoweb.Core.extraction
{
    /// <summary>
    /// Base container for extraction methods.
    /// </summary>
    public class MethodsContainer
    {
        /// <summary>
        /// Extraction method delegate. Declared into the MethodsContainer object.
        /// </summary>
        public delegate void ExtractionMethod( DataContext datacontext, LocalVariables parameters );


        #region predefined methods

        public static void Categories( DataContext datacontext, LocalVariables parameters )
        {
            ScopesStack l = new ScopesStack(datacontext, parameters);

            foreach (var x in ExtractionMethods.regexp(l.context, @"Porno\s+(?<Title>\w+)"))
            {
                l.Push(null, x);

                ((ExtractionListBase<string>)l["sampleList"]).AddElement((string)l["Title"]);

                l.Pop();
            }
        }

        #endregion
    }
}
