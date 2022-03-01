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
        static Node CoverageNode;
        static string RootPath;
        static string NL = Environment.NewLine;
        #endregion

        #region Tools
        private static string ErrorListToString(Compiler compiler)
        {
            return compiler.ErrorList.ToString();
        }
        #endregion
    }
}
