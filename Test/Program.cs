using System.Diagnostics;
using EaslyCompiler;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler c = new Compiler();
            c.Compile("../../../test.easly");

            Debug.WriteLine($"{c.ErrorList.Count} error(s).");
        }
    }
}
