namespace CompilerNode
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFunctionFeature.
    /// </summary>
    public interface IFunctionFeature : BaseNode.IFunctionFeature, IFeature, IFeatureWithName, INodeWithReplicatedBlocks, ICompiledFeature
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.FunctionFeature.OverloadBlocks"/>.
        /// </summary>
        IList<IQueryOverload> OverloadList { get; }

        /// <summary>
        /// The name of the resolved function type processing all overloads.
        /// </summary>
        OnceReference<ITypeName> MostCommonTypeName { get; }

        /// <summary>
        /// The resolved function type processing all overloads.
        /// </summary>
        OnceReference<ICompiledType> MostCommonType { get; }
    }

    /// <summary>
    /// Compiler IFunctionFeature.
    /// </summary>
    public class FunctionFeature : BaseNode.FunctionFeature, IFunctionFeature
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.FunctionFeature.OverloadBlocks"/>.
        /// </summary>
        public IList<IQueryOverload> OverloadList { get; } = new List<IQueryOverload>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(OverloadBlocks):
                    TargetList = (IList)OverloadList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
        }
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
                ResolvedFeatureTypeName = new OnceReference<ITypeName>();
                ResolvedFeatureType = new OnceReference<ICompiledType>();
                ValidFeatureName = new OnceReference<IFeatureName>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
                MostCommonTypeName = new OnceReference<ITypeName>();
                MostCommonType = new OnceReference<ICompiledType>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
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
                Debug.Assert(MostCommonTypeName.IsAssigned == MostCommonType.IsAssigned);
                Debug.Assert(ResolvedFeatureTypeName.IsAssigned == ResolvedFeatureType.IsAssigned);
                IsResolved = MostCommonType.IsAssigned;
                Debug.Assert(ResolvedFeature.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedFeatureType.IsAssigned || !IsResolved);
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IFeature
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        public OnceReference<IFeatureName> ValidFeatureName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFeature { get; private set; } = new OnceReference<ICompiledFeature>();
        #endregion

        #region Implementation of ICompiledFeature
        /// <summary>
        /// Indicates if the feature is deferred in another class.
        /// </summary>
        public bool IsDeferredFeature { get { return ((List<IQueryOverload>)OverloadList).Exists((IQueryOverload overload) => overload.IsDeferredOverload); } }

        /// <summary>
        /// True if the feature contains extern bodies in its overloads.
        /// </summary>
        public bool HasExternBody { get { return ((List<IQueryOverload>)OverloadList).Exists((IQueryOverload overload) => overload.HasExternBody); } }

        /// <summary>
        /// True if the feature contains precursor bodies in its overloads.
        /// </summary>
        public bool HasPrecursorBody { get { return ((List<IQueryOverload>)OverloadList).Exists((IQueryOverload overload) => overload.HasPrecursorBody); } }

        /// <summary>
        /// Name of the associated type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedFeatureTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Associated type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedFeatureType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Guid of the language type corresponding to the entity object for an instance of this class.
        /// </summary>
        public Guid EntityGuid { get { return LanguageClasses.FunctionEntity.Guid; } }

        /// <summary>
        /// The source node associated to this instance.
        /// </summary>
        public ISource Location { get { return this; } }
        #endregion

        #region Compiler
        /// <summary>
        /// The name of the resolved function type after processing all overloads.
        /// </summary>
        public OnceReference<ITypeName> MostCommonTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved function type processing all overloads.
        /// </summary>
        public OnceReference<ICompiledType> MostCommonType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Function '{EntityName.Text}'";
        }
        #endregion
    }
}
