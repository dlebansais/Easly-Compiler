namespace TestEaslyCompiler
{
    using BaseNode;
    using NUnit.Framework;
    using PolySerializer;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    [TestFixture]
    public class TestEnvironment
    {
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly EaslyCompilerAssembly;

            try
            {
                EaslyCompilerAssembly = Assembly.Load("Easly-Compiler");
            }
            catch
            {
                EaslyCompilerAssembly = null;
            }
            Assume.That(EaslyCompilerAssembly != null);

            string Folder = Path.GetDirectoryName(EaslyCompilerAssembly.Location);
            string TestFilePath;

            while (Folder != null && Folder.Length > 0 && RootPath == null)
            {
                TestFilePath = Path.Combine(Folder, "test.easly");
                if (File.Exists(TestFilePath))
                    RootPath = $"{Folder}/";
                else
                {
                    TestFilePath = Path.Combine(Folder, "Test-Easly-Compiler", "test.easly");
                    if (File.Exists(TestFilePath))
                    {
                        RootPath = Path.Combine(Folder, "Test-Easly-Compiler");
                        RootPath += "/";
                    }
                    else
                        Folder = Path.GetDirectoryName(Folder);
                }
            }

            if (RootPath == null)
                RootPath = "./";

            TestContext.Progress.WriteLine($"RootPath: {RootPath}");

            FileNameTable = new List<string>();
            CoverageNode = null;
            AddEaslyFiles(RootPath);
        }

        private static void AddEaslyFiles(string path)
        {
            foreach (string FileName in Directory.GetFiles(path, "*.easly"))
            {
                FileNameTable.Add(FileName.Replace("\\", "/"));

                if (FileName.EndsWith("coverage.easly"))
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Serializer Serializer = new Serializer();
                        Node RootNode = Serializer.Deserialize(fs) as Node;

                        CoverageNode = RootNode;
                    }
                }
            }

            foreach (string Folder in Directory.GetDirectories(path))
                AddEaslyFiles(Folder);
        }

        private static void SeedRand(int seed)
        {
            RandValue = seed;
        }

        private static int RandNext(int maxValue)
        {
            RandValue = (int)(5478541UL + ((ulong)RandValue * 872143693217UL));
            if (RandValue < 0)
                RandValue = -RandValue;

            return RandValue % maxValue;
        }

        public static string RootPath;
        public static List<string> FileNameTable;
        public static Node CoverageNode;
        private static int RandValue = 0;
    }
}
