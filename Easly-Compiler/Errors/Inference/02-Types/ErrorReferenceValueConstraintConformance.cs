namespace EaslyCompiler
{
    /// <summary>
    /// A type doesn't conform to a reference or value constraint.
    /// </summary>
    public interface IErrorReferenceValueConstraintConformance : IError
    {
        /// <summary>
        /// The derived type.
        /// </summary>
        ICompiledType DerivedType { get; }

        /// <summary>
        /// The constraint.
        /// </summary>
        BaseNode.CopySemantic Constraint { get; }
    }

    /// <summary>
    /// A type doesn't conform to a reference or value constraint.
    /// </summary>
    internal class ErrorReferenceValueConstraintConformance : Error, IErrorReferenceValueConstraintConformance
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReferenceValueConstraintConformance"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="derivedType">The derived type</param>
        /// <param name="constraint">The constraint.</param>
        public ErrorReferenceValueConstraintConformance(ISource source, ICompiledType derivedType, BaseNode.CopySemantic constraint)
            : base(source)
        {
            DerivedType = derivedType;
            Constraint = constraint;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The derived type.
        /// </summary>
        public ICompiledType DerivedType { get; }

        /// <summary>
        /// The constraint.
        /// </summary>
        public BaseNode.CopySemantic Constraint { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message { get { return $"Reference Value Conformance Error: '{DerivedType.TypeFriendlyName}' does not conform to the '{Constraint}' constraint."; } }
        #endregion
    }
}
