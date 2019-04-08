namespace EaslyCompiler
{
    /// <summary>
    /// Name already used.
    /// </summary>
    public interface IErrorNameAlreadyUsed : IError
    {
        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Name already used.
    /// </summary>
    internal class ErrorNameAlreadyUsed : Error, IErrorNameAlreadyUsed
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNameAlreadyUsed"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The name.</param>
        public ErrorNameAlreadyUsed(ISource source, string name)
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
        public override string Message { get { return $"More than one class is imported using the name '{Name}'."; } }
        #endregion
    }
}
