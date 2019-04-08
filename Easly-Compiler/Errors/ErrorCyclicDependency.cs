namespace EaslyCompiler
{
    /// <summary>
    /// Cyclic dependency.
    /// </summary>
    public interface IErrorCyclicDependency : IError
    {
        /// <summary>
        /// The name with cyclic dependency.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Cyclic dependency.
    /// </summary>
    internal class ErrorCyclicDependency : Error, IErrorCyclicDependency
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCyclicDependency"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The name with cyclic dependency.</param>
        public ErrorCyclicDependency(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name with cyclic dependency.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Cyclic dependency detected in '{Name}'"; } }
        #endregion
    }
}
