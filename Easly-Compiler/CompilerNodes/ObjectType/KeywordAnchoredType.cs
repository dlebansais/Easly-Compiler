namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IKeywordAnchoredType.
    /// </summary>
    public interface IKeywordAnchoredType : BaseNode.IKeywordAnchoredType, IObjectType
    {
        /// <summary>
        /// The resolved type name if the anchor is Current.
        /// </summary>
        OnceReference<ITypeName> ResolvedCurrentTypeName { get; }

        /// <summary>
        /// The resolved type if the anchor is Current.
        /// </summary>
        OnceReference<ICompiledType> ResolvedCurrentType { get; }

        /// <summary>
        /// The resolved type name if the anchor is not Current.
        /// </summary>
        OnceReference<ITypeName> ResolvedOtherTypeName { get; }

        /// <summary>
        /// The resolved type if the anchor is not Current.
        /// </summary>
        OnceReference<ICompiledType> ResolvedOtherType { get; }
    }

    /// <summary>
    /// Compiler IKeywordAnchoredType.
    /// </summary>
    public class KeywordAnchoredType : BaseNode.KeywordAnchoredType, IKeywordAnchoredType
    {
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
        public IOverload EmbeddingOverload { get; private set; }

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
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                ResolvedCurrentTypeName = new OnceReference<ITypeName>();
                ResolvedCurrentType = new OnceReference<ICompiledType>();
                ResolvedOtherTypeName = new OnceReference<ITypeName>();
                ResolvedOtherType = new OnceReference<ICompiledType>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                Debug.Assert(ResolvedTypeName.IsAssigned == ResolvedType.IsAssigned);
                IsResolved = ResolvedType.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IObjectType
        /// <summary>
        /// The resolved type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved type name if the anchor is Current.
        /// </summary>
        public OnceReference<ITypeName> ResolvedCurrentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type if the anchor is Current.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedCurrentType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The resolved type name if the anchor is not Current.
        /// </summary>
        public OnceReference<ITypeName> ResolvedOtherTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type if the anchor is not Current.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedOtherType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"like {Anchor}†"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Keyword Anchored Type '{TypeToString}'";
        }
        #endregion
    }
}
