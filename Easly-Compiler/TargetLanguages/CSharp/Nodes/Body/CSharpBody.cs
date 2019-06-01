namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        IBody Source { get; }

        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public abstract class CSharpBody : ICSharpBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpBody Create(ICSharpContext context, ICSharpFeature parentFeature, ICompiledBody source)
        {
            ICSharpBody Result = null;

            switch (source)
            {
                case IDeferredBody AsDeferredBody:
                    Result = CSharpDeferredBody.Create(context, parentFeature, AsDeferredBody);
                    break;

                case IEffectiveBody AsEffectiveBody:
                    Result = CSharpEffectiveBody.Create(context, parentFeature, AsEffectiveBody);
                    break;

                case IExternBody AsExternBody:
                    Result = CSharpExternBody.Create(context, parentFeature, AsExternBody);
                    break;

                case IPrecursorBody AsPrecursorBody:
                    Result = CSharpPrecursorBody.Create(context, parentFeature, AsPrecursorBody);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpBody(ICSharpContext context, ICSharpFeature parentFeature, IBody source)
        {
            Debug.Assert(source != null);

            ParentFeature = parentFeature;
            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public IBody Source { get; }

        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }
        #endregion
    }
}
