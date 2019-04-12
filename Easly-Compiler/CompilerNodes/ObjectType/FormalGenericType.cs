namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public interface IFormalGenericType : ISource, ICompiledType
    {
        /// <summary>
        /// The generic from which this instance is issued.
        /// </summary>
        IGeneric FormalGeneric { get; }

        /// <summary>
        /// The associated unique type name.
        /// </summary>
        ITypeName ResolvedTypeName { get; }
    }

    /// <summary>
    /// Compiler-only IFormalGenericType.
    /// </summary>
    public class FormalGenericType : IFormalGenericType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormalGenericType"/> class.
        /// </summary>
        /// <param name="formalGeneric">The generic from which this instance is issued.</param>
        /// <param name="resolvedTypeName">The associated unique type name.</param>
        public FormalGenericType(IGeneric formalGeneric, ITypeName resolvedTypeName)
        {
            FormalGeneric = formalGeneric;
            ResolvedTypeName = resolvedTypeName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The generic from which this instance is issued.
        /// </summary>
        public IGeneric FormalGeneric { get; }

        /// <summary>
        /// The associated unique type name.
        /// </summary>
        public ITypeName ResolvedTypeName { get; }

        /// <summary>
        /// Discretes available in this type.
        /// </summary>
        public IHashtableEx<IFeatureName, IDiscrete> DiscreteTable { get; private set; } = new HashtableEx<IFeatureName, IDiscrete>();

        /// <summary>
        /// Features available in this type.
        /// </summary>
        public IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable { get; private set; } = new HashtableEx<IFeatureName, IFeatureInstance>();
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IQueryOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                DiscreteTable = new HashtableEx<IFeatureName, IDiscrete>();
                FeatureTable = new HashtableEx<IFeatureName, IFeatureInstance>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}
