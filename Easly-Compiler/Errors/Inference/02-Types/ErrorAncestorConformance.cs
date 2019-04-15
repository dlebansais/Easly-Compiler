namespace EaslyCompiler
{
    /// <summary>
    /// A type doesn't inherit from an ancestor.
    /// </summary>
    public interface IErrorAncestorConformance : IError
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
    /// A type doesn't inherit from an ancestor.
    /// </summary>
    internal class ErrorAncestorConformance : Error, IErrorAncestorConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorAncestorConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="baseType">The base type.</param>
        public ErrorAncestorConformance(ISource source, ICompiledType derivedType, ICompiledType baseType)
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
        public override string Message { get { return $"Ancestor Conformance Error: '{DerivedType.TypeFriendlyName}' does not inherit from '{BaseType.TypeFriendlyName}'."; } }
        #endregion
    }
}
