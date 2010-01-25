using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core;
using linqtoweb.Core.extraction;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            ExtractionContext context = new ExtractionContext();

            foreach ( string x in context.sampleList )
            {
                Console.WriteLine(x);
            }
        }
    }
}
