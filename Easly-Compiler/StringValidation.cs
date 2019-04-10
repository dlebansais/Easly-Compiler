﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Validation of source code strings
    /// </summary>
    public class StringValidation
    {
        static StringValidation()
        {
            InitAllowedCharactersTable();
            FakeIdentifier = new CompilerNode.Identifier();
        }

        // Used to avoid an assert that hides the real error.
        private static CompilerNode.IIdentifier FakeIdentifier;

        /// <summary>
        /// Checks if an identifier or name is valid.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if the identifier or name is valid.</returns>
        public static bool IsValidIdentifier(string text)
        {
            return IsValidIdentifier(FakeIdentifier, text, out string validText, out IErrorStringValidity error);
        }

        /// <summary>
        /// Checks if an identifier or name is valid.
        /// </summary>
        /// <param name="source">Location to use when reporting errors.</param>
        /// <param name="text">The text to check.</param>
        /// <param name="validText">If valid, the normalized string to use instead of <paramref name="text"/>.</param>
        /// <param name="error">If not valid, the error to report.</param>
        /// <returns>True if the identifier or name is valid.</returns>
        public static bool IsValidIdentifier(ISource source, string text, out string validText, out IErrorStringValidity error)
        {
            validText = string.Empty;
            error = null;
            bool IsNormalized;

            try
            {
                IsNormalized = text.IsNormalized(NormalizationForm.FormD);
                if (!IsNormalized)
                    throw new ArgumentException();
            }
            catch
            {
                IsNormalized = false;
            }

            if (!IsNormalized)
            {
                error = new ErrorIllFormedString(source);
                return false;
            }

            bool WhiteSpaceFound = false;
            byte[] Bytes = Encoding.UTF32.GetBytes(text);
            int[] Codes = new int[Bytes.Length / 4];
            for (int i = 0; i < Codes.Length; i++)
                Codes[i] = BitConverter.ToInt32(Bytes, i * 4);

            for (int i = 0; i < Codes.Length; i++)
            {
                int c = Codes[i];

                if (!AllowedCharactersTypes.ContainsKey(c))
                {
                    error = new ErrorInvalidCharacter(source, c);
                    return false;
                }

                CharacterType t = AllowedCharactersTypes[c];
                if (t == CharacterType.WhiteSpace)
                {
                    if (i == 0 || i + 1 >= text.Length)
                    {
                        error = new ErrorWhiteSpaceNotAllowed(source);
                        return false;
                    }

                    if (!WhiteSpaceFound)
                    {
                        validText += ' ';
                        WhiteSpaceFound = true;
                    }
                }
                else
                {
                    validText += text[i];
                    WhiteSpaceFound = false;
                }
            }

            if (validText.Length == 0)
            {
                error = new ErrorEmptyString(source);
                return false;
            }

            validText = validText.Normalize(NormalizationForm.FormD);
            return true;
        }

        /// <summary>
        /// Checks if a manifest string is valid.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if the manifest string is valid.</returns>
        public static bool IsValidManifestString(string text)
        {
            return IsValidManifestString(FakeIdentifier, text, out string validText, out IErrorStringValidity error);
        }

        /// <summary>
        /// Checks if a manifest string is valid.
        /// </summary>
        /// <param name="source">Location to use when reporting errors.</param>
        /// <param name="text">The text to check.</param>
        /// <param name="validText">If valid, the normalized string to use instead of <paramref name="text"/>.</param>
        /// <param name="error">If not valid, the error to report.</param>
        /// <returns>True if the manifest string is valid.</returns>
        public static bool IsValidManifestString(ISource source, string text, out string validText, out IErrorStringValidity error)
        {
            validText = string.Empty;
            error = null;
            bool IsNormalized;

            try
            {
                IsNormalized = text.IsNormalized(NormalizationForm.FormD);
                if (!IsNormalized)
                    throw new ArgumentException();
            }
            catch
            {
                IsNormalized = false;
            }

            if (!IsNormalized)
            {
                error = new ErrorIllFormedString(source);
                return false;
            }

            Dictionary<char, string> CSharpEscapeTable = new Dictionary<char, string>();
            CSharpEscapeTable.Add('\"', "\\\"");
            CSharpEscapeTable.Add('\a', "\\a");
            CSharpEscapeTable.Add('\b', "\\b");
            CSharpEscapeTable.Add('\f', "\\f");
            CSharpEscapeTable.Add('\n', "\\n");
            CSharpEscapeTable.Add('\r', "\\r");
            CSharpEscapeTable.Add('\t', "\\t");

            bool InUnicodeSyntax = false;
            string UnicodeCharacter = string.Empty;
            byte[] Bytes = Encoding.UTF32.GetBytes(text);
            int[] Codes = new int[Bytes.Length / 4];
            for (int i = 0; i < Codes.Length; i++)
                Codes[i] = BitConverter.ToInt32(Bytes, i * 4);

            for (int i = 0; i < Codes.Length; i++)
            {
                int c = Codes[i];

                if (c == '\\')
                {
                    if (InUnicodeSyntax)
                    {
                        InUnicodeSyntax = false;

                        if (UnicodeCharacter.Length == 0)
                            validText += "\\\\";
                        else
                        {
                            bool ParsedAsEscape = false;
                            int CodePoint;
                            if (int.TryParse(UnicodeCharacter, NumberStyles.HexNumber, null, out CodePoint))
                            {
                                if (CodePoint < 32)
                                {
                                    char Character = (char)CodePoint;
                                    if (CSharpEscapeTable.ContainsKey(Character))
                                    {
                                        validText += CSharpEscapeTable[Character];
                                        ParsedAsEscape = true;
                                    }
                                }
                            }

                            if (!ParsedAsEscape)
                            {
                                if (UnicodeCharacter.Length <= 4)
                                {
                                    while (UnicodeCharacter.Length < 4)
                                        UnicodeCharacter = "0" + UnicodeCharacter;

                                    validText += "\\u" + UnicodeCharacter;
                                }
                                else
                                {
                                    if (UnicodeCharacter.Length > 8)
                                    {
                                        error = new ErrorIllFormedString(source);
                                        return false;
                                    }

                                    while (UnicodeCharacter.Length < 8)
                                        UnicodeCharacter = "0" + UnicodeCharacter;

                                    validText += "\\U" + UnicodeCharacter;
                                }
                            }
                        }
                    }
                    else
                    {
                        InUnicodeSyntax = true;
                        UnicodeCharacter = string.Empty;
                    }
                }
                else
                {
                    if (InUnicodeSyntax)
                    {
                        if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                            UnicodeCharacter += c;
                        else
                        {
                            error = new ErrorIllFormedString(source);
                            return false;
                        }
                    }
                    else if (CSharpEscapeTable.ContainsKey(text[i]))
                        validText += CSharpEscapeTable[text[i]];

                    else
                        validText += text[i];
                }
            }

            validText = validText.Normalize(NormalizationForm.FormD);
            return true;
        }

        private static void InitAllowedCharactersTable()
        {
            AllowedCharactersTypes = new Dictionary<int, CharacterType>();

            // From http://www.unicode.org/Public/UCD/latest/ucd/PropList.txt
            // # ================================================
            //
            // 0009..000D    ; White_Space # Cc   [5] <control-0009>..<control-000D>
            // 0020          ; White_Space # Zs       SPACE
            // 0085          ; White_Space # Cc       <control-0085>
            // 00A0          ; White_Space # Zs       NO-BREAK SPACE
            // 1680          ; White_Space # Zs       OGHAM SPACE MARK
            // 2000..200A    ; White_Space # Zs  [11] EN QUAD..HAIR SPACE
            // 2028          ; White_Space # Zl       LINE SEPARATOR
            // 2029          ; White_Space # Zp       PARAGRAPH SEPARATOR
            // 202F          ; White_Space # Zs       NARROW NO-BREAK SPACE
            // 205F          ; White_Space # Zs       MEDIUM MATHEMATICAL SPACE
            // 3000          ; White_Space # Zs       IDEOGRAPHIC SPACE
            //
            // # Total code points: 25
            AllowedCharactersTypes.Add(0x0009, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x0020, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x00A0, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x1680, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2000, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2001, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2002, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2003, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2004, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2005, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2006, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2007, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2008, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x2009, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x200A, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x202F, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x205F, CharacterType.WhiteSpace);
            AllowedCharactersTypes.Add(0x3000, CharacterType.WhiteSpace);

            for (int i = 0x21; i < 0x7F; i++)
                AllowedCharactersTypes.Add(i, CharacterType.Printable);

            for (int i = 0xA1; i < 0xAD; i++)
                AllowedCharactersTypes.Add(i, CharacterType.Printable);

            for (int i = 0xAE; i < 0x34F; i++)
                AllowedCharactersTypes.Add(i, CharacterType.Printable);

            for (int i = 0x350; i < 0x378; i++)
                AllowedCharactersTypes.Add(i, CharacterType.Printable);

            for (int i = 0x37A; i < 0x37F; i++)
                AllowedCharactersTypes.Add(i, CharacterType.Printable);

            for (int i = 0x384; i < 0xE01EF; i++)
                if (!AllowedCharactersTypes.ContainsKey(i))
                    AllowedCharactersTypes.Add(i, CharacterType.Printable);
        }

        private enum CharacterType { Printable, WhiteSpace }
        private static Dictionary<int, CharacterType> AllowedCharactersTypes;
    }
}
