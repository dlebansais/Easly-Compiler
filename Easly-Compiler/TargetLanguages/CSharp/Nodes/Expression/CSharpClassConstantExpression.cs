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
        /// The constant discrete.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpClassConstantExpression Create(ICSharpContext context, IClassConstantExpression source)
        {
            return new CSharpClassConstantExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassConstantExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpClassConstantExpression(ICSharpContext context, IClassConstantExpression source)
            : base(context, source)
        {
            if (source.ResolvedFinalFeature.IsAssigned)
                Feature = context.GetFeature(source.ResolvedFinalFeature.Item) as ICSharpConstantFeature;

            if (source.ResolvedFinalDiscrete.IsAssigned)
                Discrete = CSharpDiscrete.Create(context, source.ResolvedFinalDiscrete.Item);

            Debug.Assert((Feature != null && Discrete == null) || (Feature == null && Discrete != null));
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IClassConstantExpression Source { get { return (IClassConstantExpression)base.Source; } }

        /// <summary>
        /// The constant feature.
        /// </summary>
        public ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The constant discrete.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        public ICSharpClass Class { get { return Feature?.Owner; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            if (Feature != null)
                if (Class.ValidSourceName == "Microsoft .NET")
                    return CSharpNames.ToDotNetIdentifier(Class.ValidClassName) + "." + CSharpNames.ToDotNetIdentifier(Feature.Name);
                else
                    return CSharpNames.ToCSharpIdentifier(Class.ValidClassName) + "." + CSharpNames.ToCSharpIdentifier(Feature.Name);
            else
                return CSharpNames.ToCSharpIdentifier(Discrete.Name);
        }
        #endregion
    }
}
