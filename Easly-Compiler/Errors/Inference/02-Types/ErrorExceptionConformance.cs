namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// Two lists of exception identifiers don't match.
    /// </summary>
    public interface IErrorExceptionConformance : IError
    {
        /// <summary>
        /// The list of exception identifiers of the derived type.
        /// </summary>
        IList<IIdentifier> DerivedExceptionIdentifierList { get; }

        /// <summary>
        /// The list of exception identifiers of the base type.
        /// </summary>
        IList<IIdentifier> BaseExceptionIdentifierList { get; }
    }

    /// <summary>
    /// Two lists of exception identifiers don't match.
    /// </summary>
    internal class ErrorExceptionConformance : Error, IErrorExceptionConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorExceptionConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedExceptionIdentifierList">The list of exception identifiers of the derived type.</param>
        /// <param name="baseExceptionIdentifierList">The list of exception identifiers of the base type.</param>
        public ErrorExceptionConformance(ISource source, IList<IIdentifier> derivedExceptionIdentifierList, IList<IIdentifier> baseExceptionIdentifierList)
            : base(source)
        {
            DerivedExceptionIdentifierList = derivedExceptionIdentifierList;
            BaseExceptionIdentifierList = baseExceptionIdentifierList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of exception identifiers of the derived type.
        /// </summary>
        public IList<IIdentifier> DerivedExceptionIdentifierList { get; }

        /// <summary>
        /// The list of exception identifiers of the base type.
        /// </summary>
        public IList<IIdentifier> BaseExceptionIdentifierList { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Exception Conformance Error: '{ExceptionNameList(DerivedExceptionIdentifierList)}' not compatible with inherited exception list '{ExceptionNameList(BaseExceptionIdentifierList)}'"; } }

        private static string ExceptionNameList(IList<IIdentifier> exceptionIdentifierList)
        {
            string Result = string.Empty;

            foreach (IIdentifier Item in exceptionIdentifierList)
            {
                if (Result.Length > 0)
                    Result += ", ";

                Result += Item.ValidText.Item;
            }

            if (Result.Length == 0)
                Result = "(empty)";

            return Result;
        }
        #endregion
    }
}
