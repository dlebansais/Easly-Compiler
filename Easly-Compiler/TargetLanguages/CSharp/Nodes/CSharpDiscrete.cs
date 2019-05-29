namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# discrete node.
    /// </summary>
    public interface ICSharpDiscrete : ICSharpSource<IDiscrete>
    {
        /// <summary>
        /// The discrete name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The explicit value. Can be null.
        /// </summary>
        ICSharpExpression ExplicitValue { get; }
    }

    /// <summary>
    /// A C# generic node.
    /// </summary>
    public class CSharpDiscrete : CSharpSource<IDiscrete>, ICSharpDiscrete
    {
        #region Init
        /// <summary>
        /// Create a new C# discrete.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpDiscrete Create(IDiscrete source, ICSharpContext context)
        {
            return new CSharpDiscrete(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpDiscrete"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpDiscrete(IDiscrete source, ICSharpContext context)
            : base(source)
        {
            Name = source.ValidDiscreteName.Item.Name;

            if (source.NumericValue.IsAssigned)
                ExplicitValue = CSharpExpression.Create((IExpression)source.NumericValue.Item, context);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The discrete name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The explicit value. Can be null.
        /// </summary>
        public ICSharpExpression ExplicitValue { get; }
        #endregion
    }
}
