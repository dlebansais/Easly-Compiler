﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpClassConstantExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IClassConstantExpression Source { get; }

        /// <summary>
        /// The constant feature.
        /// </summary>
        ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        ICSharpClass Class { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpClassConstantExpression : CSharpExpression, ICSharpClassConstantExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpClassConstantExpression Create(IClassConstantExpression source, ICSharpContext context)
        {
            return new CSharpClassConstantExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassConstantExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpClassConstantExpression(IClassConstantExpression source, ICSharpContext context)
            : base(source, context)
        {
            Feature = context.GetFeature(source.ResolvedFinalFeature.Item) as ICSharpConstantFeature;
            Debug.Assert(Feature != null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IClassConstantExpression Source { get { return (IClassConstantExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The constant feature.
        /// </summary>
        public ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        public ICSharpClass Class { get { return Feature.Owner; } }
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
            if (Class.ValidSourceName == "Microsoft .NET")
                return CSharpNames.ToDotNetIdentifier(Class.ValidClassName) + "." + CSharpNames.ToDotNetIdentifier(Feature.Name);
            else
                return CSharpNames.ToCSharpIdentifier(Class.ValidClassName) + "." + CSharpNames.ToCSharpIdentifier(Feature.Name);
        }
        #endregion
    }
}