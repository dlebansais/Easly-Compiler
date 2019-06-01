namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpExternBody : ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        new IExternBody Source { get; }
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public class CSharpExternBody : CSharpBody, ICSharpExternBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpExternBody Create(ICSharpContext context, ICSharpFeature parentFeature, IExternBody source)
        {
            return new CSharpExternBody(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExternBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpExternBody(ICSharpContext context, ICSharpFeature parentFeature, IExternBody source)
            : base(context, parentFeature, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public new IExternBody Source { get { return (IExternBody)base.Source; } }
        #endregion
    }
}
