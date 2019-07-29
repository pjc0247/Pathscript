using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathscript
{
    class Foo
    {
        public static int bar = 1234;
        public int foo = 123;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var f = new Foo();

            Console.WriteLine(PScript.Eval(f, "Foo.bar"));
        }
    }
}
