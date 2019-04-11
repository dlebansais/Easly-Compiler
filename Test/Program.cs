using System.Diagnostics;
using System.IO;
using BaseNode;
using EaslyCompiler;
using PolySerializer;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler c = new Compiler();
            //c.Compile("../../../coverage/coverage.easly");
            c.Compile("../../../coverage/coverage invalid 41.easly");
            //c.Compile("../../../test.easly");
            //c.Compile("../../../root.easly");
            //c.Compile("../../../coverage/coverage replication.easly");

            Debug.WriteLine($"{c.ErrorList.Count} error(s).");
            foreach (IError Error in c.ErrorList)
                Debug.WriteLine($"  {Error.Message} ({Error}) [{Error.Location}].");
        }

        static void DebugOutputLanguage()
        {
            IRoot Root;

            using (FileStream fs = new FileStream("../../../../Easly-Compiler/Resources/language.easly", FileMode.Open, FileAccess.Read))
            {
                ISerializer s = new Serializer();
                Root = s.Deserialize(fs) as IRoot;
            }

            foreach (IBlock<IClass, Class> Block in Root.ClassBlocks.NodeBlockList)
                foreach (IClass Item in Block.NodeList)
                    using (FileStream fs = new FileStream($"../../../Language/{Item.EntityName.Text}.easly", FileMode.Create, FileAccess.Write))
                    {
                        ISerializer s = new Serializer();
                        s.Serialize(fs, Item);
                    }

            foreach (IBlock<ILibrary, Library> Block in Root.LibraryBlocks.NodeBlockList)
                foreach (ILibrary Item in Block.NodeList)
                    using (FileStream fs = new FileStream($"../../../Language/{Item.EntityName.Text}.easly", FileMode.Create, FileAccess.Write))
                    {
                        ISerializer s = new Serializer();
                        s.Serialize(fs, Item);
                    }
        }
    }
}
