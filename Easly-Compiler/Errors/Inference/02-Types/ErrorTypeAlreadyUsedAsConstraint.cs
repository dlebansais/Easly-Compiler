namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A type is already used in another constraint.
    /// </summary>
    public interface IErrorTypeAlreadyUsedAsConstraint : IError
    {
    }

    /// <summary>
    /// A type is already used in another constraint.
    /// </summary>
    internal class ErrorTypeAlreadyUsedAsConstraint : Error, IErrorTypeAlreadyUsedAsConstraint
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypeAlreadyUsedAsConstraint"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        public ErrorTypeAlreadyUsedAsConstraint(IObjectType source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Type already used in a constraint."; } }
        #endregion
    }
}
