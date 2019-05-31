namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# constraint node.
    /// </summary>
    public interface ICSharpConstraint : ICSharpSource<IConstraint>
    {
        /// <summary>
        /// The corresponding type.
        /// </summary>
        ICSharpType Type { get; }
    }

    /// <summary>
    /// A C# constraint node.
    /// </summary>
    public class CSharpConstraint : CSharpSource<IConstraint>, ICSharpConstraint
    {
        #region Init
        /// <summary>
        /// Create a new C# constraint.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpConstraint Create(ICSharpContext context, IConstraint source)
        {
            return new CSharpConstraint(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConstraint"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpConstraint(ICSharpContext context, IConstraint source)
            : base(source)
        {
            Type = CSharpType.Create(context, source.ResolvedConformingType.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The corresponding type.
        /// </summary>
        public ICSharpType Type { get; }
        #endregion
    }
}
