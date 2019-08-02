namespace Compiler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using EaslyCompiler;

    public class Program
    {
        public static int Main(string[] args)
        {
            string Namespace;
            string SourceFileName;
            string ErrorFileName;
            string OutputRootFolder;
            bool ActivateVerification;
            Guid SingledGuid;
            string SingledName;

            try
            {
                if (args.Length < 4)
                    return -1;

                Namespace = args[0];
                SourceFileName = args[1];
                ErrorFileName = args[2];
                OutputRootFolder = args[3];
                ActivateVerification = args.Length > 4 && args[4] == "V";

                if (args.Length > 5 && Guid.TryParse(args[5], out Guid Singled))
                    SingledGuid = Singled;
                else
                    SingledGuid = Guid.Empty;

                SingledName = args.Length > 6 ? args[6] : null;

                if (string.IsNullOrEmpty(Namespace) || string.IsNullOrEmpty(SourceFileName) || string.IsNullOrEmpty(ErrorFileName) || string.IsNullOrEmpty(OutputRootFolder))
                    return -1;

                if (File.Exists(ErrorFileName))
                    try { File.Delete(ErrorFileName); } catch { }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return -3;
            }

            try
            {
                return Compile(Namespace, SourceFileName, ErrorFileName, OutputRootFolder, ActivateVerification, SingledGuid, SingledName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return -4;
            }
        }

        private static int Compile(string Namespace, string SourceFileName, string ErrorFileName, string OutputRootFolder, bool ActivateVerification, Guid singledGuid, string singledName)
        {
            Compiler c = new Compiler();
            c.Compile(SourceFileName);
            IErrorList ErrorList = c.ErrorList;

            if (ErrorList.IsEmpty)
            {
                TargetCSharp t = new TargetCSharp(c, Namespace);
                t.OutputRootFolder = OutputRootFolder;
                t.SingledGuid = singledGuid;
                t.SingledName = singledName;
                t.SourceFileName = SourceFileName;
                t.Translate();

                ErrorList = t.ErrorList;
            }

            Debug.WriteLine(ErrorList.ToString());

            if (!ErrorList.IsEmpty)
            {
                ReportErrors(ErrorFileName, ErrorList);
                return -2;
            }

            return 0;
        }

        private static void ReportErrors(string errorFileName, IErrorList errorList)
        {
            try
            {
                using (FileStream fs = new FileStream(errorFileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        ReportErrors(sw, errorList);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void ReportErrors(StreamWriter sw, IErrorList errorList)
        {
            sw.WriteLine("Error\tLocation\tClass\tFeature");

            foreach (IError Item in errorList)
            {
                Guid LocationIndex = Guid.Empty;
                Guid ClassIndex = Guid.Empty;
                Guid FeatureIndex = Guid.Empty;

                if (Item.Location != ErrorLocation.NoLocation)
                {
                    LocationIndex = Item.Location.Node.Documentation.Uuid;

                    if (Item.Location.Node is ISource AsSource)
                    {
                        ClassIndex = AsSource.EmbeddingClass != null ? AsSource.EmbeddingClass.Documentation.Uuid : Guid.Empty;
                        FeatureIndex = AsSource.EmbeddingFeature != null ? AsSource.EmbeddingFeature.Documentation.Uuid : Guid.Empty;
                    }
                }

                sw.WriteLine(Item.Message + "\t" + LocationIndex + "\t" + ClassIndex + "\t" + FeatureIndex);
            }
        }
    }
}
