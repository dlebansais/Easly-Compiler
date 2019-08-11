namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// All information that can be obtained from an expression.
    /// </summary>
    public class ResolvedExpression
    {
        /// <summary>
        /// The expression result types.
        /// </summary>
        public IResultType ResolvedResult { get; set; }

        /// <summary>
        /// Exceptions the expression can throw.
        /// </summary>
        public IResultException ResolvedException { get; set; }

        /// <summary>
        /// Sources of the constant expression, if any.
        /// </summary>
        public ISealableList<IExpression> ConstantSourceList { get; set; } = new SealableList<IExpression>();

        /// <summary>
        /// The expression constant.
        /// </summary>
        public ILanguageConstant ExpressionConstant { get; set; } = NeutralLanguageConstant.NotConstant;

        /// <summary>
        /// The feature if the end of the path is a feature.
        /// </summary>
        public ICompiledFeature ResolvedFinalFeature { get; set; }

        /// <summary>
        /// The discrete if the end of the path is a discrete.
        /// </summary>
        public IDiscrete ResolvedFinalDiscrete { get; set; }

        /// <summary>
        /// The selected results.
        /// </summary>
        public ISealableList<IParameter> SelectedResultList { get; set; }

        /// <summary>
        /// The selected overload, if available.
        /// </summary>
        public IQueryOverload SelectedOverload { get; set; }

        /// <summary>
        /// The selected overload type, if available.
        /// </summary>
        public IQueryOverloadType SelectedOverloadType { get; set; }

        /// <summary>
        /// The precursor feature.
        /// </summary>
        public IFeatureInstance SelectedPrecursor { get; set; }

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        public IFeatureCall FeatureCall { get; set; }

        /// <summary>
        /// Inherit the side-by-side attribute.
        /// </summary>
        public bool InheritBySideAttribute { get; set; }
    }
}
