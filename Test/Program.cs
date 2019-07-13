﻿namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using BaseNode;
    using EaslyCompiler;
    using PolySerializer;

    class Program
    {
        static int Main(string[] args)
        {
            ICompiler c = CreateCompiler();
            IErrorList ErrorList;

            c.Compile(@"C:\Users\DLB\AppData\Local\Temp\root.easly");

            ErrorList = c.ErrorList;
            if (ErrorList.IsEmpty)
            {
                ITargetLanguage t = CreateTarget(c);
                t.Translate();

                ErrorList = t.ErrorList;
            }

            Debug.WriteLine(ErrorList.ToString());
            Console.WriteLine(ErrorList.ToString());

            return ErrorList.IsEmpty ? 0 : -1 * ((List<IError>)ErrorList).Count;
        }

        static ICompiler CreateCompiler()
        {
            Compiler c = new Compiler();
            //c.InferenceRetries = 200;

            return c;
        }

        static ITargetLanguage CreateTarget(ICompiler c)
        {
            TargetCSharp t = new TargetCSharp(c, "Test");
            t.OutputRootFolder = "../../../Output";

            return t;
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
