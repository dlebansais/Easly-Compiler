namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IQueryOverload.
    /// </summary>
    public interface IQueryOverload : BaseNode.IQueryOverload, INode, INodeWithReplicatedBlocks, ISource, IScopeHolder, INodeWithResult
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ParameterList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ResultBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ResultList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ModifiedQueryBlocks"/>.
        /// </summary>
        IList<IIdentifier> ModifiedQueryList { get; }

        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        bool IsDeferredOverload { get; }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        bool HasExternBody { get; }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        bool HasPrecursorBody { get; }

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        ListTableEx<IParameter> ParameterTable { get; }

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        ListTableEx<IParameter> ResultTable { get; }

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        OnceReference<IQueryOverloadType> ResolvedAssociatedType { get; }
    }

    /// <summary>
    /// Compiler IQueryOverload.
    /// </summary>
    public class QueryOverload : BaseNode.QueryOverload, IQueryOverload
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ResultBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ResultList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverload.ModifiedQueryBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ModifiedQueryList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyEntityDeclaration">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyEntityDeclaration)
            {
                case nameof(ParameterBlocks):
                    TargetList = (IList)ParameterList;
                    break;

                case nameof(ResultBlocks):
                    TargetList = (IList)ResultList;
                    break;

                case nameof(ModifiedQueryBlocks):
                    TargetList = (IList)ModifiedQueryList;
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
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new HashtableEx<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new HashtableEx<string, IScopeAttributeFeature>();
                ParameterTable = new ListTableEx<IParameter>();
                ResultTable = new ListTableEx<IParameter>();
                ResolvedAssociatedType = new OnceReference<IQueryOverloadType>();
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
                IsResolved = LocalScope.IsSealed;
                Debug.Assert(ParameterTable.IsSealed || !IsResolved);
                Debug.Assert(ResultTable.IsSealed || !IsResolved);
                Debug.Assert(ResolvedResultTypeName.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedResultType.IsAssigned || !IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IScopeHolder
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public IHashtableEx<string, IScopeAttributeFeature> FullScope { get; private set; } = new HashtableEx<string, IScopeAttributeFeature>();
        #endregion

        #region Compiler
        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        public bool IsDeferredOverload { get { return ((ICompiledBody)QueryBody).IsDeferredBody; } }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        public bool HasExternBody { get { return QueryBody is IExternBody; } }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        public bool HasPrecursorBody { get { return QueryBody is IPrecursorBody; } }

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        public ListTableEx<IParameter> ParameterTable { get; private set; } = new ListTableEx<IParameter>();

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        public ListTableEx<IParameter> ResultTable { get; private set; } = new ListTableEx<IParameter>();

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        public OnceReference<IQueryOverloadType> ResolvedAssociatedType { get; private set; } = new OnceReference<IQueryOverloadType>();

        /// <summary>
        /// The name of the resolved result type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedResultTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved result type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedResultType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion
    }
}
