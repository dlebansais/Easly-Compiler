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
        /// <param name="owner">The class where the parameter is declared.</param>
        public static ICSharpParameter Create(IParameter source, ICSharpClass owner)
        {
            return new CSharpParameter(source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpParameter"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the parameter is declared.</param>
        protected CSharpParameter(IParameter source, ICSharpClass owner)
            : base(source)
        {
            Feature = CSharpScopeAttributeFeature.Create(owner, source.ResolvedParameter);
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
