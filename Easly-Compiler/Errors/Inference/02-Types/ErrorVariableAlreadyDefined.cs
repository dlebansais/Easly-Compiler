namespace EaslyCompiler
{
    /// <summary>
    /// Use of a name already defined.
    /// </summary>
    public interface IErrorVariableAlreadyDefined : IError
    {
        /// <summary>
        /// The defined name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Use of a name already defined.
    /// </summary>
    internal class ErrorVariableAlreadyDefined : Error, IErrorVariableAlreadyDefined
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorVariableAlreadyDefined"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="name">The defined name.</param>
        public ErrorVariableAlreadyDefined(ISource source, string name)
            : base(source)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The defined name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"A local variable '{Name}' is already defined in this scope."; } }
        #endregion
    }
}
