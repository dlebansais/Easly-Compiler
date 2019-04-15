namespace EaslyCompiler
{
    /// <summary>
    /// A type isn't sufficiently constrained.
    /// </summary>
    public interface IErrorInsufficientConstraintConformance : IError
    {
        /// <summary>
        /// The derived type.
        /// </summary>
        ICompiledType DerivedType { get; }

        /// <summary>
        /// The base type.
        /// </summary>
        ICompiledType BaseType { get; }
    }

    /// <summary>
    /// A type isn't sufficiently constrained.
    /// </summary>
    internal class ErrorInsufficientConstraintConformance : Error, IErrorInsufficientConstraintConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInsufficientConstraintConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorInsufficientConstraintConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
            : base(source)
        {
            DerivedType = derivedType;
            BaseType = baseType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The derived type.
        /// </summary>
        public ICompiledType DerivedType { get; }

        /// <summary>
        /// The base type.
        /// </summary>
        public ICompiledType BaseType { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Insufficient Constraint Conformance Error: '{DerivedType.TypeFriendlyName}' is not sufficiently constrained to be compatible with '{BaseType.TypeFriendlyName}."; } }
        #endregion
    }
}
