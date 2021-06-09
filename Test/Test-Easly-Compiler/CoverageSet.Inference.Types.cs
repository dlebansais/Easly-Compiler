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
        public static void TestInvalid0300_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBooleanTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0301_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnavailableValue, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0302_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-02.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorResultUsedOutsideGetter, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0303_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-03.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnavailableResult, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0304_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-04.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorResultNotReturned, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0305_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-05.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorResultUsedOutsideGetter, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0306_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-06.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEqualParameters, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0307_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-07.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEqualParameters, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0308_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-08.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNonConformingType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0309_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-09.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleTypeNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0310_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-10.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleInstanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0311_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-11.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0312_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-12.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0313_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-13.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0314_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-14.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIndexerInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0315_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-15.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0316_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-16.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0317_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-17.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0318_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-18.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInsufficientConstraintConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0319_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-19.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0320_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-20.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0321_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-21.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0322_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-22.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0323_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-23.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0324_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-24.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0325_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-25.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0326_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-26.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeKindConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0327_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-27.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0328_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-28.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0329_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-29.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCloneableClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0330_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-30.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDiscreteNameConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0331_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-31.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateIndexer, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0332_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-32.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExceptionTypeMissing, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0333_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-33.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorExportNameConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0334_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-34.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorGenericClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0335_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-35.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIndexerBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0336_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-36.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIndexerInheritance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0337_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-37.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIndexerMissingBody, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0338_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-38.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && (Compiler.ErrorList.At(0) is IErrorInvalidAnchoredType || Compiler.ErrorList.At(0) is IErrorUnknownIdentifier), ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0339_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-39.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidIdentifierContext, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0340_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-40.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingAncestor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0341_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-41.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingSelectedPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0342_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-42.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingTypeArgument, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0343_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-43.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMultipleEffectiveFeature, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0344_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-44.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNameResultNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0345_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-45.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNameValueNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0346_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-46.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNotAnchor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0347_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-47.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorAncestorConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0348_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-48.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorRenameNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0349_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-49.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorReservedName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0350_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-50.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeAlreadyInherited, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0351_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-51.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeAlreadyUsedAsConstraint, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0352_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-52.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeArgumentCount, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0353_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-53.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTooManyTypeArguments, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0354_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-54.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0355_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-55.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypeArgumentMixed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0356_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-56.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorTypedefNameConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0357_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-57.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0358_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-58.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAnchoredType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0359_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-59.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0360_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-60.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0361_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-61.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0362_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-62.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0363_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-63.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0364_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-64.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0365_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-65.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0366_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-66.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0367_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-67.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0368_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-68.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0369_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-69.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0370_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-70.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0371_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-71.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0372_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-72.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleTypeNotAllowed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0373_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-73.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0374_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-74.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDoubleRename, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0375_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-75.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0376_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-76.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0377_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-77.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0378_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-78.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0379_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-79.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEqualParameters, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0380_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-80.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0381_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-81.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0382_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-82.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0383_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-83.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0384_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-84.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0385_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-85.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0386_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-86.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0387_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-87.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0388_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-88.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0389_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-89.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0390_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-90.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 50;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0391_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-91.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0392_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-92.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0393_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-93.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0394_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-94.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0395_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-95.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAnchoredType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0396_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-96.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInheritanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0397_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-97.easly";

            Compiler.ActivateVerification = false;
            Compiler.InferenceRetries = 20;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0398_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-98.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0399_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 03-99.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0400_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-00.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0401_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-01.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorNonConformingType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0402_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-02.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEqualParameters, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0403_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-03.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && (Compiler.ErrorList.At(0) is IErrorAncestorConformance || Compiler.ErrorList.At(0) is IErrorReferenceValueConstraintConformance), ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0404_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-04.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorReferenceValueConstraintConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0405_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-05.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorReferenceValueConstraintConformance, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0406_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-06.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0407_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-07.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0408_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-08.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidAnchoredType, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0409_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-09.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorGenericClass, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0410_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-10.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0411_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-11.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleInstanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0412_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-12.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0413_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-13.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0414_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-14.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorClassTypeRequired, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0415_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-15.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorIdentifierAlreadyListed, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0416_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-16.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0417_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-17.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0418_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-18.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0419_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-19.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0420_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-20.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0421_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-21.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorEqualParameters, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0422_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-22.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0423_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-23.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0424_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-24.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0425_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-25.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorBodyTypeMismatch, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0426_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-26.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0427_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-27.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0428_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-28.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0429_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-29.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0430_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-30.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0431_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-31.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0432_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-32.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0433_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-33.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorVariableAlreadyDefined, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0434_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-34.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorSingleInstanceConflict, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0435_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-35.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorDuplicateName, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0436_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-36.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0437_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-37.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0438_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-38.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorCyclicDependency, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0439_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-39.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0440_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-40.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0441_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-41.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorUnknownIdentifier, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0442_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-42.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorMissingSelectedPrecursor, ErrorListToString(Compiler));
        }

        [Test]
        [Category("Coverage")]
        public static void TestInvalid0443_Types()
        {
            Compiler Compiler = new Compiler();

            string TestFileName = $"{RootPath}coverage/coverage invalid 04-43.easly";

            Compiler.ActivateVerification = false;

            //Debug.Assert(false);
            Compiler.Compile(TestFileName);
            Assert.That(!Compiler.ErrorList.IsEmpty && Compiler.ErrorList.At(0) is IErrorInvalidExpressionContext, ErrorListToString(Compiler));
        }
    }
}
