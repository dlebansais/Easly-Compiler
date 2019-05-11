namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IConstraint.
    /// </summary>
    public interface IConstraint : BaseNode.IConstraint, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Constraint.RenameBlocks"/>.
        /// </summary>
        IList<IRename> RenameList { get; }

        /// <summary>
        /// The resolved parent type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedParentTypeName { get; }

        /// <summary>
        /// The resolved parent type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedParentType { get; }

        /// <summary>
        /// The resolved conforming type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedConformingTypeName { get; }

        /// <summary>
        /// The resolved conforming type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedConformingType { get; }

        /// <summary>
        /// Table of resolved renames.
        /// </summary>
        IHashtableEx<IIdentifier, IIdentifier> RenameTable { get; }

        /// <summary>
        /// The resolved type after rename.
        /// </summary>
        OnceReference<ICompiledType> ResolvedTypeWithRename { get; }
    }

    /// <summary>
    /// Compiler IConstraint.
    /// </summary>
    public class Constraint : BaseNode.Constraint, IConstraint
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Constraint.RenameBlocks"/>.
        /// </summary>
        public IList<IRename> RenameList { get; } = new List<IRename>();

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
                case nameof(RenameBlocks):
                    TargetList = (IList)RenameList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedParentTypeName = new OnceReference<ITypeName>();
                ResolvedParentType = new OnceReference<ICompiledType>();
                ResolvedConformingTypeName = new OnceReference<ITypeName>();
                ResolvedConformingType = new OnceReference<ICompiledType>();
                RenameTable = new HashtableEx<IIdentifier, IIdentifier>();
                ResolvedTypeWithRename = new OnceReference<ICompiledType>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
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
                IsResolved = ResolvedTypeWithRename.IsAssigned;
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

        #region Compiler
        /// <summary>
        /// The resolved parent type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedParentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved parent type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedParentType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The resolved conforming type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedConformingTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved conforming type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedConformingType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Table of resolved renames.
        /// </summary>
        public IHashtableEx<IIdentifier, IIdentifier> RenameTable { get; private set; } = new HashtableEx<IIdentifier, IIdentifier>();

        /// <summary>
        /// The resolved type after rename.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedTypeWithRename { get; private set; } = new OnceReference<ICompiledType>();
        #endregion
    }
}
