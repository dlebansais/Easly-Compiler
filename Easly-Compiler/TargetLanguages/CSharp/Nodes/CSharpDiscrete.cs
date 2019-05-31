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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpDiscrete Create(ICSharpContext context, IDiscrete source)
        {
            return new CSharpDiscrete(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpDiscrete"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpDiscrete(ICSharpContext context, IDiscrete source)
            : base(source)
        {
            Name = source.ValidDiscreteName.Item.Name;

            if (source.NumericValue.IsAssigned)
                ExplicitValue = CSharpExpression.Create(context, (IExpression)source.NumericValue.Item);
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
