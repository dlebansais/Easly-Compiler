namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IGeneric.
    /// </summary>
    public interface IGeneric : BaseNode.IGeneric, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Generic.ConstraintBlocks"/>.
        /// </summary>
        IList<IConstraint> ConstraintList { get; }

        /// <summary>
        /// The resolved, unique type name for this generic.
        /// </summary>
        OnceReference<ITypeName> ResolvedGenericTypeName { get; }

        /// <summary>
        /// The corresponding resolved type.
        /// </summary>
        OnceReference<IFormalGenericType> ResolvedGenericType { get; }

        /// <summary>
        /// Resolved type for the default value of the generic.
        /// </summary>
        OnceReference<ICompiledType> ResolvedDefaultType { get; }

        /// <summary>
        /// Table of resolved conforming types.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> ResolvedConformanceTable { get; }

        /// <summary>
        /// Table of all types used in this class.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> TypeTable { get; }
    }

    /// <summary>
    /// Compiler IGeneric.
    /// </summary>
    public class Generic : BaseNode.Generic, IGeneric
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Generic.ConstraintBlocks"/>.
        /// </summary>
        public IList<IConstraint> ConstraintList { get; } = new List<IConstraint>();

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
                case nameof(ConstraintBlocks):
                    TargetList = (IList)ConstraintList;
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
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedGenericTypeName = new OnceReference<ITypeName>();
                ResolvedGenericType = new OnceReference<IFormalGenericType>();
                ResolvedDefaultType = new OnceReference<ICompiledType>();
                ResolvedConformanceTable = new SealableDictionary<ITypeName, ICompiledType>();
                TypeTable = new SealableDictionary<ITypeName, ICompiledType>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = ResolvedConformanceTable.IsSealed;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved, unique type name for this generic.
        /// </summary>
        public OnceReference<ITypeName> ResolvedGenericTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The corresponding resolved type.
        /// </summary>
        public OnceReference<IFormalGenericType> ResolvedGenericType { get; private set; } = new OnceReference<IFormalGenericType>();

        /// <summary>
        /// Resolved type for the default value of the generic.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedDefaultType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Table of resolved conforming types.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> ResolvedConformanceTable { get; private set; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// Table of all types used in this class.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> TypeTable { get; private set; } = new SealableDictionary<ITypeName, ICompiledType>();
        #endregion
    }
}
