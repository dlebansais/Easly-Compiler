namespace TestEaslyCompiler
{
    using BaseNode;
    using BaseNodeHelper;
    using EaslyCompiler;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    [TestFixture]
    public partial class CoverageSet
    {
        [Test]
        [Category("Coverage")]
        public static void TestCompilationCalls()
        {
            Compiler Compiler = new Compiler();

            Assert.That(Compiler != null, "Sanity Check #0");

            string TestFileName = $"{RootPath}/coverage/coverage.easly";

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

            Root NullRoot = null;
            ex = Assert.Throws<ArgumentNullException>(() => Compiler.Compile(NullRoot));
            Assert.That(ex.Message == $"Value cannot be null.{NL}Parameter name: root", ex.Message);

            using (FileStream fs = new FileStream(TestFileName, FileMode.Open, FileAccess.Read))
            {
                Compiler.Compile(fs);
                Assert.That(Compiler.ErrorList.IsEmpty, ErrorListToString(Compiler));
            }

            Root ClonedRoot = NodeHelper.DeepCloneNode(CoverageNode, cloneCommentGuid: true) as Root;
            NodeTreeHelper.SetGuidProperty(ClonedRoot.ClassBlocks.NodeBlockList[0].NodeList[0], nameof(Class.ClassGuid), Guid.Empty);
            Assert.That(!NodeTreeDiagnostic.IsValid(ClonedRoot, throwOnInvalid: false));

            Compiler.Compile(ClonedRoot);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInputRootInvalid, ErrorListToString(Compiler));

            Compiler.ActivateVerification = false;

            Compiler.InferenceRetries = -1;
            Compiler.Compile(CoverageNode as Root);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInternal, ErrorListToString(Compiler));

            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(CoverageNode as Root);
            Assert.That(Compiler.ErrorList.IsEmpty, ErrorListToString(Compiler));

            Assert.That(Compiler.ActivateVerification == false);

            TargetCSharp t = new TargetCSharp(Compiler, "Test");
            t.OutputRootFolder = "./bin/Output";
            t.Translate();

            Assert.That(t.ErrorList.IsEmpty, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestReplication()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}/coverage/coverage replication.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(Compiler.ErrorList.IsEmpty);
        }
    }
}
