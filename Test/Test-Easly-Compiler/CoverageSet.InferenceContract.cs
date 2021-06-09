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
        public static void TestInvalid0500_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0501_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0502_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-02.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0503_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-03.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorConstantExpected, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0504_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-04.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMultipleIdenticalDiscrete, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0505_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-05.easly";

            Compiler.ActivateVerification = false;

            Compiler.Compile(TestFileName);

            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0506_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-06.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMultipleIdenticalDiscrete, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0507_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-07.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0508_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-08.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0509_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-09.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0510_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-10.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpressionContext, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0511_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-11.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0512_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-12.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0513_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-13.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleInstanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0514_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-14.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0515_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-15.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0516_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-16.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOperator, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0517_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-17.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0518_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-18.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0519_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-19.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0520_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-20.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0521_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-21.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0522_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-22.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0523_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-23.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorConstantRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0524_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-24.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0525_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-25.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEntityTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0526_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-26.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0527_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-27.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExpressionResultMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0528_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-28.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0529_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-29.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExpressionResultMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0530_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-30.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExpressionResultMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0531_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-31.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0532_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-32.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0533_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-33.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0534_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-34.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0535_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-35.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0536_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-36.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorConstantExpected, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0537_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-37.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0538_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-38.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0539_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-39.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0540_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-40.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0541_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-41.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAttributeOrPropertyRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0542_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-42.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0543_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-43.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0544_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-44.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0545_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-45.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0546_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-46.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0547_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-47.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnavailableValue, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0548_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-48.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCharacterTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0549_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-49.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0550_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-50.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorStringTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0551_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-51.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0552_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-52.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0553_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-53.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorConstantNewExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0554_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-54.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0555_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-55.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOldExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0556_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-56.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0557_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-57.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOldExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0558_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-58.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0559_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-59.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingOverSourceAndIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0560_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-60.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOverSourceType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0561_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-61.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0562_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-62.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0563_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-63.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorPrecursorNotAllowedInIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0564_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-64.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0565_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-65.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNoPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0566_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-66.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0567_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-67.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTooManyArguments, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0568_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-68.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0569_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-69.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0570_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-70.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0571_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-71.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0572_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-72.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNoPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0573_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-73.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0574_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-74.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0575_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-75.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0576_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-76.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIndexPrecursorNotAllowedOutsideIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0577_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-77.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0578_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-78.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0579_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-79.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0580_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-80.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0581_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-81.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberConstantExpected, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0582_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-82.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberConstantExpected, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0583_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-83.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIncompatibleRangeBounds, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0584_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-84.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0585_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-85.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0586_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-86.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0587_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-87.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0588_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-88.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0589_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-89.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0590_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-90.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOperator, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0591_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-91.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidOperator, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0592_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-92.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0593_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-93.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0594_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-94.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentNameMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0595_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-95.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0596_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-96.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0597_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-97.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEventTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0598_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-98.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCyclicDependency, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0599_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 05-99.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0600_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 06-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0601_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 06-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberTypeMissing, ErrorListToString(Compiler));
        }
    }
}
