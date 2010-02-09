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
            WebContext context = new WebContext();

            Console.WriteLine(context.el.str);


            foreach (var x in context.sampleList)
            {
                Console.WriteLine(x.str + "    " + context.el.str);
            }

            //Scanner scanner = new Scanner();
            //scanner.SetSource(File.ReadAllText("code.txt"), 0);

            //Parser parser = new Parser(scanner);
            //if (parser.Parse())
            //{
            //    GlobalCode x = parser.Ast;

            //    x.EmitCs(new StreamWriter("code.cs", false, Encoding.Unicode), "Example1", "WebContext");
            //}

        }
    }
}
