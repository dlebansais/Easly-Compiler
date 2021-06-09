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
        public static void TestInvalid0100_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSourceRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0101_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCyclicDependency, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0102_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-02.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0103_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-03.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0104_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-04.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0105_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-05.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0106_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-06.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0107_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-07.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0108_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-08.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0109_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-09.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNameChanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0110_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-10.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0111_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-11.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNameAlreadyUsed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0112_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-12.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0113_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-13.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorImportTypeConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0114_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-14.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassAlreadyImported, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0115_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-15.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0116_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-16.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0117_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-17.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0118_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-18.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0119_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-19.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateImport, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0120_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-20.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0121_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-21.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0122_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-22.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0123_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-23.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0124_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-24.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0125_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-25.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0126_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-26.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNameUnchanged, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0127_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-27.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0128_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-28.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDoubleRename, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0129_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-29.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0130_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-30.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0131_ClassAndLibrary()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 01-31.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringValidity, ErrorListToString(Compiler));
        }
    }
}
