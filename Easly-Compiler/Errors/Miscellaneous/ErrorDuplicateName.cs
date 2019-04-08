namespace EaslyCompiler
{
    /// <summary>
    /// Duplicate name.
    /// </summary>
    public interface IErrorDuplicateName : IError
    {
        /// <summary>
        /// The duplicate name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Duplicate name.
    /// </summary>
    internal class ErrorDuplicateName : Error, IErrorDuplicateName
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDuplicateName"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The duplicate name.</param>
        public ErrorDuplicateName(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The duplicate name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Duplicate Name '{Name}'."; } }
        #endregion
    }
}
