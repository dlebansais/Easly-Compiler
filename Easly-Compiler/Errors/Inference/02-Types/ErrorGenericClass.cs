namespace EaslyCompiler
{
    /// <summary>
    /// Use of a generic class without type arguments.
    /// </summary>
    public interface IErrorGenericClass : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }
    }

    /// <summary>
    /// Use of a generic class without type arguments.
    /// </summary>
    internal class ErrorGenericClass : Error, IErrorGenericClass
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorGenericClass"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        public ErrorGenericClass(ISource source, string className)
            : base(source)
        {
            ClassName = className;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class name.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Using the generic class '{ClassName}' requires type arguments."; } }
        #endregion
    }
}
