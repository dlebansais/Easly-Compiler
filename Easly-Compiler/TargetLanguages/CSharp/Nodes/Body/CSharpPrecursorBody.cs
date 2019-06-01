namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpPrecursorBody : ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        new IPrecursorBody Source { get; }
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public class CSharpPrecursorBody : CSharpBody, ICSharpPrecursorBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpPrecursorBody Create(ICSharpContext context, IPrecursorBody source)
        {
            return new CSharpPrecursorBody(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpPrecursorBody(ICSharpContext context, IPrecursorBody source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public new IPrecursorBody Source { get { return (IPrecursorBody)base.Source; } }
        #endregion
    }
}
