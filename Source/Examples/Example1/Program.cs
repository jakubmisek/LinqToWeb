using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core;
using linqtoweb.Core.extraction;
using linqtoweb.CodeGenerator;
using linqtoweb.CodeGenerator.AST;

using System.IO;

using linqtoweb.Example;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebContext context = new WebContext();

            //foreach (var x in context.sampleList)
            //{
            //    Console.WriteLine(x.str);
            //}


            Scanner scanner = new Scanner();
            scanner.SetSource("class XXX{ string str; XXX[] xxxs; }\n" +
                "_main( XXX[] sampleList )\n{ [open(\"http://www.freesutra.cz/\")]\nforeach(regexp(\"(?<x>ahoj)\")){addel(sampleList,x);} }" +
                "addel(XXX[] l,string val){l[]=XXX(str=val);}"
                , 0);

            Parser parser = new Parser(scanner);
            if (parser.Parse())
            {
                GlobalCode x = parser.Ast;

                x.EmitCs(new StreamWriter("code.cs", false, Encoding.Unicode));
            }
        }
    }
}
