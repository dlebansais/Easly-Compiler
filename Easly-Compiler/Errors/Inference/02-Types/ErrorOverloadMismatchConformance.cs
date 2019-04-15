namespace EaslyCompiler
{
    /// <summary>
    /// The base of a type doesn't conform.
    /// </summary>
    public interface IErrorOverloadMismatchConformance : IError
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
    /// The base of a type doesn't conform.
    /// </summary>
    internal class ErrorOverloadMismatchConformance : Error, IErrorOverloadMismatchConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOverloadMismatchConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorOverloadMismatchConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Overload Mismatch Conformance Error: '{DerivedType.TypeFriendlyName}' overloads not compatible with all overloads from '{BaseType.TypeFriendlyName}."; } }
        #endregion
    }
}
