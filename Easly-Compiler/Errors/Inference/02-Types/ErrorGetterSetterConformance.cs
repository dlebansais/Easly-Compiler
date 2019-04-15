namespace EaslyCompiler
{
    /// <summary>
    /// Incompatible getter or setter.
    /// </summary>
    public interface IErrorGetterSetterConformance : IError
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
    /// Incompatible getter or setter.
    /// </summary>
    internal class ErrorGetterSetterConformance : Error, IErrorGetterSetterConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorGetterSetterConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorGetterSetterConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Getter Setter Mismatch Conformance Error: '{DerivedType.TypeFriendlyName}' and '{BaseType.TypeFriendlyName}' don't have compatible getter and setter."; } }
        #endregion
    }
}
