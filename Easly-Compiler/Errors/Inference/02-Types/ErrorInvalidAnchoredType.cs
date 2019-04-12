namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// Anchored type used in bad context.
    /// </summary>
    public interface IErrorInvalidAnchoredType : IError
    {
    }

    /// <summary>
    /// Anchored type used in bad context.
    /// </summary>
    internal class ErrorInvalidAnchoredType : Error, IErrorInvalidAnchoredType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidAnchoredType"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidAnchoredType(IAnchoredType source)
            : base(source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidAnchoredType"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidAnchoredType(IKeywordAnchoredType source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Anchored type not allowed in this context."; } }
        #endregion
    }
}
