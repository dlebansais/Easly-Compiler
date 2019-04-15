namespace EaslyCompiler
{
    /// <summary>
    /// A cloneable class can't inherit from a non-cloneable.
    /// </summary>
    public interface IErrorCloneableClass : IError
    {
    }

    /// <summary>
    /// A cloneable class can't inherit from a non-cloneable.
    /// </summary>
    internal class ErrorCloneableClass : Error, IErrorCloneableClass
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCloneableClass"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorCloneableClass(ISource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "A class inheriting from non-cloneable parents cannot be cloneable."; } }
        #endregion
    }
}
