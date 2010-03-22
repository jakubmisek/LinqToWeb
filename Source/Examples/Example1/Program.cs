using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using linqtoweb.Core;
using linqtoweb.Core.extraction;

using System.IO;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new GoogleSearcher("jakub misek");

            /*foreach (var x in context.GoogleResults.Take(12).Where(x => x.url.Contains(".cz")))
            {
                Console.WriteLine(x.title);
                Console.WriteLine(" - " + x.url);
            }

            Console.WriteLine("people.devsense.com position: " + context.GoogleResults.TakeWhile(x => !x.url.ToLower().Contains("people.devsense.com")).Count());
            */

            //Console.WriteLine("results count: " + context.GoogleResults.Count());

            Console.WriteLine(context.state.info + "\n");

            int i = 0;

            foreach (var x in context.GoogleResults)
            {
                Console.WriteLine((i++) + ". " + x.title + "\n[" + x.url + "]\n");
            }
            

        }
    }
}
