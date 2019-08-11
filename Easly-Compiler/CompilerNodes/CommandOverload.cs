namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ICommandOverload.
    /// </summary>
    public interface ICommandOverload : BaseNode.ICommandOverload, INode, INodeWithReplicatedBlocks, IOverload, IScopeHolder
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverload.ParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ParameterList { get; }

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
        /// List of overload parameters.
        /// </summary>
        ISealableList<IParameter> ParameterTable { get; }

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        OnceReference<ICommandOverloadType> ResolvedAssociatedType { get; }

        /// <summary>
        /// The resolved body.
        /// </summary>
        OnceReference<ICompiledBody> ResolvedBody { get; }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        void RestartNumberType(ref bool isChanged);

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        void ValidateNumberType(IErrorList errorList);
    }

    /// <summary>
    /// Compiler ICommandOverload.
    /// </summary>
    public class CommandOverload : BaseNode.CommandOverload, ICommandOverload
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverload.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

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
                case nameof(ParameterBlocks):
                    TargetList = (IList)ParameterList;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                LocalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                InnerScopes = new List<IScopeHolder>();
                FullScope = new SealableDictionary<string, IScopeAttributeFeature>();
                ParameterTable = new SealableList<IParameter>();
                ResolvedAssociatedType = new OnceReference<ICommandOverloadType>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                AdditionalScope = new SealableDictionary<string, IScopeAttributeFeature>();
                AdditionalScope.Seal();
                ResolvedBody = new OnceReference<ICompiledBody>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = LocalScope.IsSealed;
                Debug.Assert(ParameterTable.IsSealed == IsResolved);
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedBody.IsAssigned;
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
        public ISealableDictionary<string, IScopeAttributeFeature> LocalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// Additional entities such as loop indexer.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> AdditionalScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        public IList<IScopeHolder> InnerScopes { get; private set; } = new List<IScopeHolder>();

        /// <summary>
        /// All reachable entities.
        /// </summary>
        public ISealableDictionary<string, IScopeAttributeFeature> FullScope { get; private set; } = new SealableDictionary<string, IScopeAttributeFeature>();
        #endregion

        #region Compiler
        /// <summary>
        /// Indicates if the overload is deferred in another class.
        /// </summary>
        public bool IsDeferredOverload { get { return ((ICompiledBody)CommandBody).IsDeferredBody; } }

        /// <summary>
        /// True if the overload contains an extern body.
        /// </summary>
        public bool HasExternBody { get { return CommandBody is IExternBody; } }

        /// <summary>
        /// True if the overload contains a precursor body.
        /// </summary>
        public bool HasPrecursorBody { get { return CommandBody is IPrecursorBody; } }

        /// <summary>
        /// List of overload parameters.
        /// </summary>
        public ISealableList<IParameter> ParameterTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        public OnceReference<ICommandOverloadType> ResolvedAssociatedType { get; private set; } = new OnceReference<ICommandOverloadType>();

        /// <summary>
        /// The resolved body.
        /// </summary>
        public OnceReference<ICompiledBody> ResolvedBody { get; private set; } = new OnceReference<ICompiledBody>();
        #endregion

        #region Numbers
        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.RestartNumberType(ref isChanged);

            ((IBody)CommandBody).RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.CheckNumberType(ref isChanged);

            ((IBody)CommandBody).CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.ValidateNumberType(errorList);

            ((IBody)CommandBody).ValidateNumberType(errorList);
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the overload.
        /// </summary>
        public string CommandOverloadToString
        {
            get
            {
                string ParameterString = $"{EntityDeclaration.EntityDeclarationListToString(ParameterList)}";
                if (ParameterEnd == BaseNode.ParameterEndStatus.Open)
                    ParameterString += ", ...";

                string BodyString = CommandBody.GetType().Name;

                return $"({ParameterString}) {{{BodyString}}}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Command Overload '{CommandOverloadToString}'";
        }
        #endregion
    }
}
