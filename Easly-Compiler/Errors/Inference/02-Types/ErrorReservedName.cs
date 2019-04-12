namespace EaslyCompiler
{
    /// <summary>
    /// Use of a reserved name.
    /// </summary>
    public interface IErrorReservedName : IError
    {
        /// <summary>
        /// The reserved name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Use of a reserved name.
    /// </summary>
    internal class ErrorReservedName : Error, IErrorReservedName
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReservedName"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The reserved name.</param>
        public ErrorReservedName(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The reserved name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Reserved Name: '{Name}'."; } }
        #endregion
    }
}
