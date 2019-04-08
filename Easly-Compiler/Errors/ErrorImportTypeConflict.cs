namespace EaslyCompiler
{
    /// <summary>
    /// A class name is imported with conflicting specifications.
    /// </summary>
    public interface IErrorImportTypeConflict : IError
    {
        /// <summary>
        /// The class name.
        /// </summary>
        string ClassName { get; }
    }

    /// <summary>
    /// A class name is imported with conflicting specifications.
    /// </summary>
    internal class ErrorImportTypeConflict : Error, IErrorImportTypeConflict
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorImportTypeConflict"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="className">The class name.</param>
        public ErrorImportTypeConflict(ISource source, string className)
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
        public override string Message { get { return $"Conflicting import type for class '{ClassName}'."; } }
        #endregion
    }
}
