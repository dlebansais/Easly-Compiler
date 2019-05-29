namespace EaslyCompiler
{
    using System.Diagnostics;

    /// <summary>
    /// A C# parameter node.
    /// </summary>
    public interface ICSharpParameter : ICSharpSource<IParameter>
    {
        /// <summary>
        /// The corresponding attribute.
        /// </summary>
        ICSharpScopeAttributeFeature Feature { get; }

        /// <summary>
        /// The parameter name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// A C# parameter node.
    /// </summary>
    public class CSharpParameter : CSharpSource<IParameter>, ICSharpParameter
    {
        #region Init
        /// <summary>
        /// Create a new C# parameter.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpParameter Create(IParameter source, ICSharpContext context)
        {
            return new CSharpParameter(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpParameter"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpParameter(IParameter source, ICSharpContext context)
            : base(source)
        {
            Feature = context.GetFeature(source.ResolvedParameter) as ICSharpScopeAttributeFeature;
            Debug.Assert(Feature != null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The corresponding attribute.
        /// </summary>
        public ICSharpScopeAttributeFeature Feature { get; }

        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name { get { return Feature.Name; } }
        #endregion
    }
}
