namespace EaslyCompiler
{
    /// <summary>
    /// Inheritance with conflicting settings.
    /// </summary>
    public interface IErrorIndexerInheritanceConflict : IError
    {
    }

    /// <summary>
    /// Inheritance with conflicting settings.
    /// </summary>
    internal class ErrorIndexerInheritanceConflict : Error, IErrorIndexerInheritanceConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorIndexerInheritanceConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorIndexerInheritanceConflict(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Indexer is inherited with conflicting settings."; } }
        #endregion
    }
}
