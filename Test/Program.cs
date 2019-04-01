using EaslyCompiler;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler c = new Compiler();
            c.Compile("../../../root.easly");
        }
    }
}
