namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// The result types of a type and a base type don't match.
    /// </summary>
    public interface IErrorResultMismatchConformance : IError
    {
        /// <summary>
        /// The list of results of the derived type.
        /// </summary>
        IList<IEntityDeclaration> DerivedResultList { get; }

        /// <summary>
        /// The list of results of the base type.
        /// </summary>
        IList<IEntityDeclaration> BaseResultList { get; }
    }

    /// <summary>
    /// The result types of a type and a base type don't match.
    /// </summary>
    internal class ErrorResultMismatchConformance : Error, IErrorResultMismatchConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResultMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorResultMismatchConformance(ISource source, IQueryOverloadType derivedType, IQueryOverloadType baseType)
            : base(source)
        {
            DerivedResultList = derivedType.ResultList;
            BaseResultList = baseType.ResultList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of results of the derived type.
        /// </summary>
        public IList<IEntityDeclaration> DerivedResultList { get; }

        /// <summary>
        /// The list of results of the base type.
        /// </summary>
        public IList<IEntityDeclaration> BaseResultList { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Result Mismatch Conformance Error."; } }
        #endregion
    }
}
