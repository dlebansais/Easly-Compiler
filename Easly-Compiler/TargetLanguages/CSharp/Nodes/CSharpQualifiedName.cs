namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# qualified name.
    /// </summary>
    public interface ICSharpQualifiedName : ICSharpSource<IQualifiedName>
    {
        /// <summary>
        /// The associated feature.
        /// </summary>
        ICSharpFeature Feature { get; }
    }

    /// <summary>
    /// A C# qualified name.
    /// </summary>
    public class CSharpQualifiedName : CSharpSource<IQualifiedName>, ICSharpQualifiedName
    {
        #region Init
        /// <summary>
        /// Create a new C# qualified name.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpQualifiedName Create(IQualifiedName source, ICSharpContext context)
        {
            return new CSharpQualifiedName(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQualifiedName"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpQualifiedName(IQualifiedName source, ICSharpContext context)
            : base(source)
        {
            ICompiledFeature SourceFeature = null;
            Feature = context.GetFeature(SourceFeature);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The associated feature.
        /// </summary>
        public ICSharpFeature Feature { get; }
        #endregion
    }
}
