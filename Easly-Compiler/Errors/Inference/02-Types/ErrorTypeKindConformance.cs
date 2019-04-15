namespace EaslyCompiler
{
    /// <summary>
    /// Incompatible types.
    /// </summary>
    public interface IErrorTypeKindConformance : IError
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
    /// Incompatible types.
    /// </summary>
    internal class ErrorTypeKindConformance : Error, IErrorTypeKindConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTypeKindConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorTypeKindConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Type Kind Conformance Error: '{DerivedType.TypeFriendlyName}' and '{BaseType.TypeFriendlyName}' are incompatible types."; } }
        #endregion
    }
}
