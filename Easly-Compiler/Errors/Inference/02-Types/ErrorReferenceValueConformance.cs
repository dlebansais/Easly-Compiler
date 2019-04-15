namespace EaslyCompiler
{
    /// <summary>
    /// A type doesn't conform to another.
    /// </summary>
    public interface IErrorReferenceValueConformance : IError
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
    /// A type doesn't conform to another.
    /// </summary>
    internal class ErrorReferenceValueConformance : Error, IErrorReferenceValueConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReferenceValueConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorReferenceValueConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Reference Value Conformance Error: '{DerivedType.TypeFriendlyName}' and '{BaseType.TypeFriendlyName}' are not both reference or value types."; } }
        #endregion
    }
}
