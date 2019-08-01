using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathscript.Testbed
{
    class Foo
    {
        public int[] a = new int[] { 1, 2, 3, 4 };

        public Dictionary<string, string> b = new Dictionary<string, string>()
        {
            ["asdf"] = "ASDF"
        };
    }
    class Program
    {
        static void Main(string[] args)
        {
            var f = new Foo();

            Console.WriteLine(PScript.Eval(f, "b[\"asdf\"]"));
        }
    }
}
