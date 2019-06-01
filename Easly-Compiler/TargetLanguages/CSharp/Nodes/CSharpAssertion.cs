namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# assertion node.
    /// </summary>
    public interface ICSharpAssertion : ICSharpSource<IAssertion>
    {
        /// <summary>
        /// The assertion tag. Can be null.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Writes down the C# assertion.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        void WriteCSharp(ICSharpWriter writer, string outputNamespace);
    }

    /// <summary>
    /// A C# assertion node.
    /// </summary>
    public class CSharpAssertion : CSharpSource<IAssertion>, ICSharpAssertion
    {
        #region Init
        /// <summary>
        /// Create a new C# assertion.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpAssertion Create(ICSharpContext context, IAssertion source)
        {
            return new CSharpAssertion(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssertion"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpAssertion(ICSharpContext context, IAssertion source)
            : base(source)
        {
            if (source.Tag.IsAssigned)
                Tag = ((IName)source.Tag.Item).ValidText.Item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The assertion tag. Can be null.
        /// </summary>
        public string Tag { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# assertion.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            //TODO
        }
        #endregion
    }
}
