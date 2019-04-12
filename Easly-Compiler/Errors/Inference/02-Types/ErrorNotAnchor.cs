namespace EaslyCompiler
{
    /// <summary>
    /// Name cannot be used as anchor.
    /// </summary>
    public interface IErrorNotAnchor : IError
    {
        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Name cannot be used as anchor.
    /// </summary>
    internal class ErrorNotAnchor : Error, IErrorNotAnchor
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNotAnchor"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The name.</param>
        public ErrorNotAnchor(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Feature '{Name}' cannot be used as anchor."; } }
        #endregion
    }
}
