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
        public static void TestInvalid0700_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0701_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0702_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-02.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0703_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-03.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAssignmentMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0704_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-04.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0705_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-05.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAssignmentMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0706_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-06.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0707_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-07.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            //Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0708_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-08.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            //Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0709_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-09.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0710_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-10.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            //Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0711_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-11.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0712_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-12.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0713_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-13.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAttachment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0714_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-14.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0715_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-15.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0716_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-16.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0717_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-17.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0718_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-18.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTooManyArguments, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0719_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-19.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0720_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-20.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0721_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-21.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0722_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-22.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0723_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-23.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCreatedFeatureNotAttributeOrProperty, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0724_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-24.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorReferenceTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0725_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-25.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0726_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-26.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0727_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-27.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0728_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-28.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0729_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-29.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0730_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-30.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCreationFeatureRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0731_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-31.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0732_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-32.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0733_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-33.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNumberTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0734_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-34.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0735_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-35.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0736_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-36.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0737_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-37.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0738_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-38.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0739_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-39.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0740_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-40.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0741_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-41.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0742_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-42.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0743_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-43.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0744_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-44.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0745_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-45.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidRange, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0746_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-46.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0747_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-47.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0748_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-48.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAssignmentMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0749_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-49.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0750_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-50.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAssignment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0751_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-51.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0752_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-52.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0753_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-53.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0754_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-54.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0755_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-55.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNoPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0756_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-56.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0757_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-57.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0758_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-58.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0759_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-59.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0760_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-60.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorPrecursorNotAllowedInIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0761_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-61.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0762_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-62.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0763_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-63.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNoPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0764_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-64.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0765_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-65.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0766_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-66.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExceptionTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0767_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-67.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExceptionTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0768_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-68.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0769_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-69.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCreationFeatureRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0770_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-70.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0771_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-71.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0772_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-72.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0773_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-73.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidRange, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0774_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-74.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAssignment, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0775_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-75.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0776_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-76.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAttributeOrPropertyRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0777_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-77.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEventTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0778_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-78.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0779_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-79.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0780_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-80.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0781_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-81.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0782_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-82.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidConversionFeature, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0783_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-83.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0784_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-84.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0785_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-85.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0786_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-86.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0787_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-87.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0788_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-88.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0789_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-89.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEntityTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0790_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-90.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0791_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-91.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0792_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-92.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0793_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-93.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpression, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0794_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-94.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0795_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 07-95.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidInstruction, ErrorListToString(Compiler));
        }
    }
}
