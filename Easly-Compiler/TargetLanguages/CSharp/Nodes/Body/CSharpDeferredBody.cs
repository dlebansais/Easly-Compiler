namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpDeferredBody : ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        new IDeferredBody Source { get; }
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public class CSharpDeferredBody : CSharpBody, ICSharpDeferredBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpDeferredBody Create(ICSharpContext context, ICSharpFeature parentFeature, IDeferredBody source)
        {
            return new CSharpDeferredBody(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpDeferredBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpDeferredBody(ICSharpContext context, ICSharpFeature parentFeature, IDeferredBody source)
            : base(context, parentFeature, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public new IDeferredBody Source { get { return (IDeferredBody)base.Source; } }
        #endregion
    }
}
