using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using BaseNode;
using BaseNodeHelper;
using EaslyCompiler;
using NUnit.Framework;
using PolySerializer;

namespace TestEaslyCompiler
{
    [TestFixture]
    public class ATestSet
    {
        static bool TestOff = false;

        #region Setup
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            TestEnvironment.InitTestSession();
            FileNameTable = TestEnvironment.FileNameTable;
            CoverageNode = TestEnvironment.CoverageNode;
            RootPath = TestEnvironment.RootPath;
        }

        static List<string> FileNameTable;
        static INode CoverageNode;
        static string RootPath;
        static string NL = Environment.NewLine;
        #endregion

        #region Tools
        private static string ErrorListToString(Compiler compiler)
        {
            return compiler.ErrorList.ToString();
        }
        #endregion

        #region Compile as file
        [Test]
        public static void TestCompileFile()
        {
            if (TestOff)
                return;

            TestContext.Out.WriteLine($"RootPath: {RootPath}");
            string s = Environment.CurrentDirectory;

            Compiler Compiler = new Compiler();

            Assert.That(Compiler != null, "Sanity Check #0");

            string TestFileName = $"{RootPath}coverage/coverage.easly";

            Exception ex;
            string NullString = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullString));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: fileName", ex.Message);

            Compiler.Compile("notfound.easly");
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInputFileNotFound AsInputFileNotFound && AsInputFileNotFound.Message == "File not found: 'notfound.easly'.", ErrorListToString(Compiler));

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Compiler.Compile(TestFileName);
                Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInputFileInvalid, ErrorListToString(Compiler));
            }

            Stream NullStream = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullStream));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: stream", ex.Message);

            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.IsEmpty, ErrorListToString(Compiler));

            string InvalidFile = File.Exists($"{RootPath}Test-Easly-Compiler.dll") ? $"{RootPath}Test-Easly-Compiler.dll" : $"{RootPath}Test-Easly-Compiler.csproj";
            using (FileStream fs = new FileStream(InvalidFile, FileMode.Open, FileAccess.Read))
            {
                Compiler.Compile(fs);
                Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInputFileInvalid, ErrorListToString(Compiler));
            }

            IRoot NullRoot = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullRoot));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: root", ex.Message);

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.Read))
            {
                Compiler.Compile(fs);
                Assert.That(Compiler.ErrorList.IsEmpty, ErrorListToString(Compiler));
            }

            IRoot ClonedRoot = NodeHelper.DeepCloneNode(CoverageNode, cloneCommentGuid: true) as IRoot;
            NodeTreeHelper.SetGuidProperty(ClonedRoot.ClassBlocks.NodeBlockList[0].NodeList[0], nameof(IClass.ClassGuid), Guid.Empty);
            Assert.That(!NodeTreeDiagnostic.IsValid(ClonedRoot, assertValid: false));

            Compiler.Compile(ClonedRoot);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInputRootInvalid, ErrorListToString(Compiler));
        }
        #endregion

        #region Compile as object
        [Test]
        [TestCaseSource(nameof(TestEnvironment.FileIndexRange))]
        public static void TestCompileObject(int index)
        {
            if (TestOff)
                return;

            string Name = null;
            INode RootNode = null;
            int n = index;
            foreach (string FileName in FileNameTable)
            {
                if (n == 0)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        Name = FileName;
                        Serializer Serializer = new Serializer();
                        RootNode = Serializer.Deserialize(fs) as INode;
                    }
                    break;
                }

                n--;
            }

            if (n > 0)
                throw new ArgumentOutOfRangeException($"{n} / {FileNameTable.Count}");

            TestCompileObject(index, Name, RootNode);
        }

        public static void TestCompileObject(int index, string name, INode rootNode)
        {
            Compiler Compiler = new Compiler();

            Compiler.Compile(CoverageNode as IRoot);
            Assert.That(Compiler.ErrorList.IsEmpty, ErrorListToString(Compiler));
        }
        #endregion
    }
}
