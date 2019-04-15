namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// The parameter types of a type and a base type don't match.
    /// </summary>
    public interface IErrorParameterMismatchConformance : IError
    {
        /// <summary>
        /// The list of parameters of the derived type.
        /// </summary>
        IList<IEntityDeclaration> DerivedParameterList { get; }

        /// <summary>
        /// The parameter end status of the derived type.
        /// </summary>
        BaseNode.ParameterEndStatus DerivedParameterEnd { get; }

        /// <summary>
        /// The list of parameters of the base type.
        /// </summary>
        IList<IEntityDeclaration> BaseParameterList { get; }

        /// <summary>
        /// The parameter end status of the base type.
        /// </summary>
        BaseNode.ParameterEndStatus BaseParameterEnd { get; }
    }

    /// <summary>
    /// The parameter types of a type and a base type don't match.
    /// </summary>
    internal class ErrorParameterMismatchConformance : Error, IErrorParameterMismatchConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorParameterMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorParameterMismatchConformance(ISource source, IQueryOverloadType derivedType, IQueryOverloadType baseType)
            : base(source)
        {
            DerivedParameterList = derivedType.ParameterList;
            DerivedParameterEnd = derivedType.ParameterEnd;
            BaseParameterList = baseType.ParameterList;
            BaseParameterEnd = baseType.ParameterEnd;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorParameterMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorParameterMismatchConformance(ISource source, ICommandOverloadType derivedType, ICommandOverloadType baseType)
            : base(source)
        {
            DerivedParameterList = derivedType.ParameterList;
            DerivedParameterEnd = derivedType.ParameterEnd;
            BaseParameterList = baseType.ParameterList;
            BaseParameterEnd = baseType.ParameterEnd;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorParameterMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorParameterMismatchConformance(ISource source, IIndexerType derivedType, IIndexerType baseType)
            : base(source)
        {
            DerivedParameterList = derivedType.IndexParameterList;
            DerivedParameterEnd = derivedType.ParameterEnd;
            BaseParameterList = baseType.IndexParameterList;
            BaseParameterEnd = baseType.ParameterEnd;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of parameters of the derived type.
        /// </summary>
        public IList<IEntityDeclaration> DerivedParameterList { get; }

        /// <summary>
        /// The parameter end status of the derived type.
        /// </summary>
        public BaseNode.ParameterEndStatus DerivedParameterEnd { get; }

        /// <summary>
        /// The list of parameters of the base type.
        /// </summary>
        public IList<IEntityDeclaration> BaseParameterList { get; }

        /// <summary>
        /// The parameter end status of the base type.
        /// </summary>
        public BaseNode.ParameterEndStatus BaseParameterEnd { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Parameter Mismatch Conformance Error."; } }
        #endregion
    }
}
