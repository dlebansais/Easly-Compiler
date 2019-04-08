namespace EaslyCompiler
{
    /// <summary>
    /// A rename doesn't change a name.
    /// </summary>
    public interface IErrorNameUnchanged : IError
    {
        /// <summary>
        /// The name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// A rename doesn't change a name.
    /// </summary>
    internal class ErrorNameUnchanged : Error, IErrorNameUnchanged
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNameUnchanged"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The name.</param>
        public ErrorNameUnchanged(ISource source, string name)
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
        public override string Message { get { return $"Item renamed to the same name '{Name}'."; } }
        #endregion
    }
}
