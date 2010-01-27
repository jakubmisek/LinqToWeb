﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core;
using linqtoweb.Core.extraction;
using linqtoweb.CodeGenerator;
using linqtoweb.CodeGenerator.AST;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*ExtractionContext context = new ExtractionContext();

            foreach ( var x in context.sampleList )
            {
                Console.WriteLine(x);
            }*/

            Scanner scanner = new Scanner();
            scanner.SetSource("class XXX{  } ",0);            

            Parser parser = new Parser(scanner);
            if (parser.Parse())
            {
                GlobalCode x = parser.Ast;
            }
        }
    }
}
