using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core;
using linqtoweb.Core.extraction;
using linqtoweb.CodeGenerator;
using linqtoweb.CodeGenerator.AST;

using System.IO;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebContext context = new WebContext();

            //foreach (var x in context.sampleList)
            //{
            //    Console.WriteLine(x);
            //}


            Scanner scanner = new Scanner();
            scanner.SetSource("class XXX{ string str; string[] strs; }\n_main( string[] sampleList )\n{ [open(\"http://www.freesutra.cz/\")]\nforeach(regexp(\"(?<x>ahoj)\")){sampleList[]=x;} }", 0);

            Parser parser = new Parser(scanner);
            if (parser.Parse())
            {
                GlobalCode x = parser.Ast;

                x.EmitCs(new StreamWriter("code.cs", false, Encoding.Unicode));
            }
        }
    }
}
