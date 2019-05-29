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
    }
}
