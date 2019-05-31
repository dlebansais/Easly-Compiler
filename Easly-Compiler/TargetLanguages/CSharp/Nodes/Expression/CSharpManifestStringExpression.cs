﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpManifestStringExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IManifestStringExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpManifestStringExpression : CSharpExpression, ICSharpManifestStringExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpManifestStringExpression Create(ICSharpContext context, IManifestStringExpression source)
        {
            return new CSharpManifestStringExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpManifestStringExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpManifestStringExpression(ICSharpContext context, IManifestStringExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IManifestStringExpression Source { get { return (IManifestStringExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpText(cSharpNamespace, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public virtual string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result = string.Empty;

            foreach (char c in Source.ValidText.Item)
                switch (c)
                {
                    case '\"':
                        Result += "\\\"";
                        break;
                    case '\\':
                        Result += "\\\\";
                        break;
                    case '\a':
                        Result += "\\a";
                        break;
                    case '\b':
                        Result += "\\b";
                        break;
                    case '\f':
                        Result += "\\f";
                        break;
                    case '\n':
                        Result += "\\n";
                        break;
                    case '\r':
                        Result += "\\r";
                        break;
                    case '\t':
                        Result += "\\t";
                        break;
                    default:
                        Result += c;
                        break;
                }

            return $"\"{Result}\"";
        }
        #endregion
    }
}
