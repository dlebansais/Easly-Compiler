namespace EaslyCompiler
{
    using System.Collections.Generic;
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
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpBody Create(ICSharpContext context, ICompiledBody source)
        {
            ICSharpBody Result = null;

            switch (source)
            {
                case IDeferredBody AsDeferredBody:
                    Result = CSharpDeferredBody.Create(context, AsDeferredBody);
                    break;

                case IEffectiveBody AsEffectiveBody:
                    Result = CSharpEffectiveBody.Create(context, AsEffectiveBody);
                    break;

                case IExternBody AsExternBody:
                    Result = CSharpExternBody.Create(context, AsExternBody);
                    break;

                case IPrecursorBody AsPrecursorBody:
                    Result = CSharpPrecursorBody.Create(context, AsPrecursorBody);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpBody(ICSharpContext context, IBody source)
        {
            Debug.Assert(source != null);

            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public IBody Source { get; }
        #endregion
    }
}
