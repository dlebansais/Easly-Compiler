namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Bad conversion feature.
    /// </summary>
    public interface IErrorInvalidConversionFeature : IError
    {
        /// <summary>
        /// The invalid feature.
        /// </summary>
        string Identifier { get; }
    }

    /// <summary>
    /// Bad conversion feature.
    /// </summary>
    internal class ErrorInvalidConversionFeature : Error, IErrorInvalidConversionFeature
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInvalidConversionFeature"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorInvalidConversionFeature(IIdentifier source)
            : base(source)
        {
            Debug.Assert(source.ValidText.IsAssigned);

            Identifier = source.ValidText.Item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The invalid feature.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"'{Identifier}' is an invalid conversion feature."; } }
        #endregion
    }
}
