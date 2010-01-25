﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core.datacontext;

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


            foreach ( var x in datacontext.regexp(@"Porno\s+(?<Title>\w+)") )
            {
                LocalVariables l = new LocalVariables(parameters, x);

                ((ExtractionListBase<string>)l["sampleList"]).AddElement((string)l["Title"]);
            }
        }

        #endregion
    }
}
