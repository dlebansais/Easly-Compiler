namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInheritance.
    /// </summary>
    public interface IInheritance : BaseNode.IInheritance, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.RenameBlocks"/>.
        /// </summary>
        IList<IRename> RenameList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.ForgetBlocks"/>.
        /// </summary>
        IList<IIdentifier> ForgetList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.KeepBlocks"/>.
        /// </summary>
        IList<IIdentifier> KeepList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.DiscontinueBlocks"/>.
        /// </summary>
        IList<IIdentifier> DiscontinueList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.ExportChangeBlocks"/>.
        /// </summary>
        IList<IExportChange> ExportChangeList { get; }

        /// <summary>
        /// Type name of the resolved inherited type.
        /// </summary>
        OnceReference<ITypeName> ResolvedTypeName { get; }

        /// <summary>
        /// Resolved inherited type.
        /// </summary>
        OnceReference<IClassType> ResolvedType { get; }

        /// <summary>
        /// Name of the resolved parent type.
        /// </summary>
        OnceReference<ITypeName> ResolvedParentTypeName { get; }

        /// <summary>
        /// The resolved parent type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedParentType { get; }

        /// <summary>
        /// Name of the resolved parent class type.
        /// </summary>
        OnceReference<ITypeName> ResolvedClassParentTypeName { get; }

        /// <summary>
        /// The resolved parent class type.
        /// </summary>
        OnceReference<IClassType> ResolvedClassParentType { get; }

        /// <summary>
        /// Table of association of discrete names to their instance.
        /// </summary>
        OnceReference<IHashtableEx<IFeatureName, IDiscrete>> DiscreteTable { get; }

        /// <summary>
        /// Table of association of typedef names to their instance.
        /// </summary>
        OnceReference<IHashtableEx<IFeatureName, ITypedefType>> TypedefTable { get; }

        /// <summary>
        /// Table of association of feature names to their instance.
        /// </summary>
        OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>> FeatureTable { get; }

        /// <summary>
        /// Table of association of export names to their instance.
        /// </summary>
        OnceReference<IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>> ExportTable { get; }
    }

    /// <summary>
    /// Compiler IInheritance.
    /// </summary>
    public class Inheritance : BaseNode.Inheritance, IInheritance
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.RenameBlocks"/>.
        /// </summary>
        public IList<IRename> RenameList { get; } = new List<IRename>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.ForgetBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ForgetList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.KeepBlocks"/>.
        /// </summary>
        public IList<IIdentifier> KeepList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.DiscontinueBlocks"/>.
        /// </summary>
        public IList<IIdentifier> DiscontinueList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.Inheritance.ExportChangeBlocks"/>.
        /// </summary>
        public IList<IExportChange> ExportChangeList { get; } = new List<IExportChange>();

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

                case nameof(ForgetBlocks):
                    TargetList = (IList)ForgetList;
                    break;

                case nameof(KeepBlocks):
                    TargetList = (IList)KeepList;
                    break;

                case nameof(DiscontinueBlocks):
                    TargetList = (IList)DiscontinueList;
                    break;

                case nameof(ExportChangeBlocks):
                    TargetList = (IList)ExportChangeList;
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
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<IClassType>();
                ResolvedParentTypeName = new OnceReference<ITypeName>();
                ResolvedParentType = new OnceReference<ICompiledType>();
                ResolvedClassParentTypeName = new OnceReference<ITypeName>();
                ResolvedClassParentType = new OnceReference<IClassType>();
                DiscreteTable = new OnceReference<IHashtableEx<IFeatureName, IDiscrete>>();
                TypedefTable = new OnceReference<IHashtableEx<IFeatureName, ITypedefType>>();
                FeatureTable = new OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>>();
                ExportTable = new OnceReference<IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>>();
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
                Debug.Assert(ExportTable.IsAssigned == IsResolved);
                Debug.Assert(TypedefTable.IsAssigned == IsResolved);
                Debug.Assert(DiscreteTable.IsAssigned == IsResolved);
                Debug.Assert(FeatureTable.IsAssigned == IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Type name of the resolved inherited type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved inherited type.
        /// </summary>
        public OnceReference<IClassType> ResolvedType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// Name of the resolved parent type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedParentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved parent type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedParentType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Name of the resolved parent class type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassParentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved parent class type.
        /// </summary>
        public OnceReference<IClassType> ResolvedClassParentType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// Table of association of discrete names to their instance.
        /// </summary>
        public OnceReference<IHashtableEx<IFeatureName, IDiscrete>> DiscreteTable { get; private set; } = new OnceReference<IHashtableEx<IFeatureName, IDiscrete>>();

        /// <summary>
        /// Table of association of typedef names to their instance.
        /// </summary>
        public OnceReference<IHashtableEx<IFeatureName, ITypedefType>> TypedefTable { get; private set; } = new OnceReference<IHashtableEx<IFeatureName, ITypedefType>>();

        /// <summary>
        /// Table of association of feature names to their instance.
        /// </summary>
        public OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>> FeatureTable { get; private set; } = new OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>>();

        /// <summary>
        /// Table of association of export names to their instance.
        /// </summary>
        public OnceReference<IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>> ExportTable { get; private set; } = new OnceReference<IHashtableEx<IFeatureName, IHashtableEx<string, IClass>>>();
        #endregion
    }
}
