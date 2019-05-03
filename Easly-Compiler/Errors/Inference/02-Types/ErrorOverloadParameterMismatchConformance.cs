namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// The parameter types of a type and a base type don't match.
    /// </summary>
    public interface IErrorOverloadParameterMismatchConformance : IError
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
        /// The base type.
        /// </summary>
        ICompiledType BaseType { get; }
    }

    /// <summary>
    /// The parameter types of a type and a base type don't match.
    /// </summary>
    internal class ErrorOverloadParameterMismatchConformance : Error, IErrorOverloadParameterMismatchConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOverloadParameterMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorOverloadParameterMismatchConformance(ISource source, IQueryOverloadType derivedType, ICompiledType baseType)
            : base(source)
        {
            DerivedParameterList = derivedType.ParameterList;
            DerivedParameterEnd = derivedType.ParameterEnd;
            BaseType = baseType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOverloadParameterMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorOverloadParameterMismatchConformance(ISource source, ICommandOverloadType derivedType, ICompiledType baseType)
            : base(source)
        {
            DerivedParameterList = derivedType.ParameterList;
            DerivedParameterEnd = derivedType.ParameterEnd;
            BaseType = baseType;
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
        /// The base type.
        /// </summary>
        public ICompiledType BaseType { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return "Overload Parameter Mismatch Conformance Error."; } }
        #endregion
    }
}
