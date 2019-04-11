namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
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

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}
