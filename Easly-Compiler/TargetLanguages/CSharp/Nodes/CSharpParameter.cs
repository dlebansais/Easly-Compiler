namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

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

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);
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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the parameter is declared.</param>
        public static ICSharpParameter Create(ICSharpContext context, IParameter source, ICSharpClass owner)
        {
            return new CSharpParameter(context, source, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpParameter"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="owner">The class where the parameter is declared.</param>
        protected CSharpParameter(ICSharpContext context, IParameter source, ICSharpClass owner)
            : base(source)
        {
            Feature = CSharpScopeAttributeFeature.Create(context, owner, source.ResolvedParameter);
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

        #region Client Interface
        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            ((ICSharpFeature)Feature).CheckNumberType(ref isChanged);
        }
        #endregion
    }
}
