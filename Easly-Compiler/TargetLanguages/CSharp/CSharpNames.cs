namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Helper class to obtain C# names from various objects.
    /// </summary>
    public static class CSharpNames
    {
        /// <summary>
        /// Gets a C#-compliant name from any string.
        /// </summary>
        /// <param name="s">The string.</param>
        public static string ToCSharpIdentifier(string s)
        {
            string Result = string.Empty;
            bool NextCharUpperCase = true;

            if (s.Length == 1)
                Result = s;
            else
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s[i];

                    if (c >= 'a' && c <= 'z')
                        if (NextCharUpperCase)
                        {
                            Result += (char)(c + 'A' - 'a');
                            NextCharUpperCase = false;
                        }
                        else
                            Result += c;

                    else if (c >= 'A' && c <= 'Z')
                        if (NextCharUpperCase)
                        {
                            Result += c;
                            NextCharUpperCase = false;
                        }
                        else
                            Result += (char)(c + 'a' - 'A');

                    else if (i > 0 && (c >= '0' && c <= '9'))
                        Result += c;

                    else if (i > 0 && (c == '_' || c == '-' || c == ' '))
                        NextCharUpperCase = true;

                    else
                        Result += '_';
                }

            return Result;
        }

        /// <summary>
        /// Gets the name of a .NET component from any string.
        /// </summary>
        /// <param name="s">The string.</param>
        public static string ToDotNetIdentifier(string s)
        {
            return s;
        }

        /// <summary>
        /// Returns the text of an export.
        /// </summary>
        /// <param name="isOverride">True if the feature is an override of a parent virtual feature.</param>
        /// <param name="isAbstract">True if the feature is abstract.</param>
        /// <param name="isNonVirtual">True if the feature is NOT virtual.</param>
        /// <param name="exportStatus">The base export status.</param>
        public static string ComposedExportStatus(bool isOverride, bool isAbstract, bool isNonVirtual, CSharpExports exportStatus)
        {
            string Result = null;

            switch (exportStatus)
            {
                case CSharpExports.Private:
                    Result = "private";
                    break;

                case CSharpExports.Protected:
                    Result = "protected" + (isAbstract ? " " + "abstract" : (isOverride ? " " + "override" : (isNonVirtual ? string.Empty : " " + "virtual")));
                    break;

                case CSharpExports.Public:
                    Result = "public" + (isAbstract ? " " + "abstract" : (isOverride ? " " + "override" : (isNonVirtual ? string.Empty : " " + "virtual")));
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }
    }
}
