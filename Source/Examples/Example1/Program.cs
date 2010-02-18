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
            if (true)
            {
                WebContext context = new WebContext("jakub misek");

                foreach (var x in context.GoogleResults/*.Where(x => x.url.Contains(".cz"))*/)
                {
                    Console.WriteLine(x.title);
                    Console.WriteLine(" - " + x.url);
                }

                Console.WriteLine("people.devsense.com position: " + context.GoogleResults.TakeWhile(x => !x.url.ToLower().Contains("people.devsense.com")).Count());

                //Console.WriteLine("results count: " + context.GoogleResults.Count());

            }
            else
            {
                Scanner scanner = new Scanner();
                scanner.SetSource(File.ReadAllText("..\\..\\code.txt"), 0);

                Parser parser = new Parser(scanner);
                if (parser.Parse())
                {
                    GlobalCode x = parser.Ast;

                    x.EmitCs(new StreamWriter("..\\..\\code.cs", false, Encoding.Unicode), "Example1", "WebContext");

                    Console.WriteLine("Code emitted");
                }
            }

        }
    }
}
