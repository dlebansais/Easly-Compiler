namespace EaslyCompiler
{
    /// <summary>
    /// Two fields don't have conforming types.
    /// </summary>
    public interface IErrorFieldMismatchConformance : IError
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
    /// Two fields don't have conforming types.
    /// </summary>
    internal class ErrorFieldMismatchConformance : Error, IErrorFieldMismatchConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFieldMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorFieldMismatchConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Field Mismatch Conformance Error: '{DerivedType.TypeFriendlyName}' and '{BaseType.TypeFriendlyName}' don't declare compatible fields."; } }
        #endregion
    }
}
