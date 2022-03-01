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
    public interface IQueryOverload : INode, INodeWithReplicatedBlocks, IOverload, IScopeHolder, INodeWithResult
    {
        /// <summary>
        /// Gets or sets the list of parameters.
        /// </summary>
        BaseNode.IBlockList<BaseNode.EntityDeclaration> ParameterBlocks { get; }

        /// <summary>
        /// Gets or sets whether the query accepts extra parameters.
        /// </summary>
        BaseNode.ParameterEndStatus ParameterEnd { get; }

        /// <summary>
        /// Gets or sets the list of results.
        /// </summary>
        BaseNode.IBlockList<BaseNode.EntityDeclaration> ResultBlocks { get; }

        /// <summary>
        /// Gets or sets the list of other features this query modifies.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Identifier> ModifiedQueryBlocks { get; }

        /// <summary>
        /// Gets or sets the query variant.
        /// </summary>
        IOptionalReference<BaseNode.Expression> Variant { get; }

        /// <summary>
        /// Gets or sets the query body.
        /// </summary>
        BaseNode.Body QueryBody { get; }

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
        ISealableList<IParameter> ParameterTable { get; }

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        ISealableList<IParameter> ResultTable { get; }

        /// <summary>
        /// List of resolved conformant parameter types.
        /// </summary>
        ISealableList<ICompiledType> ConformantResultTable { get; }

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        OnceReference<IQueryOverloadType> ResolvedAssociatedType { get; }

        /// <summary>
        /// List of resolved conformant parameter types, both this overload and the associated type.
        /// </summary>
        ISealableList<ICompiledType> CompleteConformantResultTable { get; }

        /// <summary>
        /// The resolved body.
        /// </summary>
        OnceReference<ICompiledBody> ResolvedBody { get; }

        /// <summary>
        /// The know arguments to the overload for each number parameter.
        /// </summary>
        IDictionary<IParameter, IList<NumberKinds>> NumberArgumentTable { get; }

        /// <summary>
        /// The number kind if the overload result type is a number.
        /// </summary>
        NumberKinds NumberKind { get; }

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
        public void FillReplicatedList(string propertyEntityDeclaration, List<BaseNode.Node> nodeList)
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

            foreach (BaseNode.Node Node in nodeList)
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
                ResultTable = new SealableList<IParameter>();
                ConformantResultTable = new SealableList<ICompiledType>();
                ResolvedAssociatedType = new OnceReference<IQueryOverloadType>();
                CompleteConformantResultTable = new SealableList<ICompiledType>();
                ResolvedResultTypeName = new OnceReference<ITypeName>();
                ResolvedResultType = new OnceReference<ICompiledType>();
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
                IsResolved = LocalScope.IsSealed && CompleteConformantResultTable.IsSealed;
                Debug.Assert(ParameterTable.IsSealed || !IsResolved);
                Debug.Assert(ResultTable.IsSealed || !IsResolved);
                Debug.Assert(ConformantResultTable.IsSealed || !IsResolved);
                Debug.Assert(ResolvedResultTypeName.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedResultType.IsAssigned || !IsResolved);
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
        public ISealableList<IParameter> ParameterTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// List of resolved parameters.
        /// </summary>
        public ISealableList<IParameter> ResultTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// List of resolved conformant parameter types.
        /// </summary>
        public ISealableList<ICompiledType> ConformantResultTable { get; private set; } = new SealableList<ICompiledType>();

        /// <summary>
        /// The resolved associated type.
        /// </summary>
        public OnceReference<IQueryOverloadType> ResolvedAssociatedType { get; private set; } = new OnceReference<IQueryOverloadType>();

        /// <summary>
        /// List of resolved conformant parameter types, both this overload and the associated type.
        /// </summary>
        public ISealableList<ICompiledType> CompleteConformantResultTable { get; private set; } = new SealableList<ICompiledType>();

        /// <summary>
        /// The name of the resolved result type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedResultTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved result type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedResultType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The resolved body.
        /// </summary>
        public OnceReference<ICompiledBody> ResolvedBody { get; private set; } = new OnceReference<ICompiledBody>();

        /// <summary>
        /// The know arguments to the overload for each number parameter.
        /// </summary>
        public IDictionary<IParameter, IList<NumberKinds>> NumberArgumentTable { get; } = new Dictionary<IParameter, IList<NumberKinds>>();
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind
        {
            get
            {
                foreach (IParameter Result in ResultTable)
                {
                    if (Result.Name == nameof(BaseNode.Keyword.Result) || ResultTable.Count == 1)
                        return Result.ResolvedParameter.NumberKind;
                }

                return NumberKinds.NotApplicable;
            }
        }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.RestartNumberType(ref isChanged);

            foreach (IEntityDeclaration EntityDeclaration in ResultList)
                EntityDeclaration.RestartNumberType(ref isChanged);

            if (Variant.IsAssigned)
                ((IExpression)Variant).RestartNumberType(ref isChanged);

            ((IBody)QueryBody).RestartNumberType(ref isChanged);

            if (ParameterTable.Count > 0)
            {
                if (NumberArgumentTable.Count == 0)
                {
                    for (int i = 0; i < ParameterTable.Count; i++)
                    {
                        IParameter Parameter = ParameterTable[i];
                        NumberArgumentTable.Add(Parameter, new List<NumberKinds>());
                    }
                }
                else
                {
                    // Result of the previous pass.
                    for (int i = 0; i < ParameterTable.Count; i++)
                    {
                        IParameter Parameter = ParameterTable[i];
                        Debug.Assert(NumberArgumentTable.ContainsKey(Parameter));

                        Debug.Assert(Parameter.ResolvedParameter.ResolvedEffectiveType.IsAssigned);

                        if (Parameter.ResolvedParameter.ResolvedEffectiveType.Item is ICompiledNumberType AsNumberType)
                        {
                            NumberKinds BestGuess = AsNumberType.GetDefaultNumberKind();
                            UpdateParameterKind(NumberArgumentTable[Parameter], ref BestGuess);
                            AsNumberType.UpdateNumberKind(BestGuess, ref isChanged);
                        }

                        NumberArgumentTable[Parameter].Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.CheckNumberType(ref isChanged);

            foreach (IEntityDeclaration EntityDeclaration in ResultList)
                EntityDeclaration.CheckNumberType(ref isChanged);

            if (Variant.IsAssigned)
                ((IExpression)Variant).CheckNumberType(ref isChanged);

            ((IBody)QueryBody).CheckNumberType(ref isChanged);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IEntityDeclaration EntityDeclaration in ParameterList)
                EntityDeclaration.ValidateNumberType(errorList);

            foreach (IEntityDeclaration EntityDeclaration in ResultList)
                EntityDeclaration.ValidateNumberType(errorList);

            if (Variant.IsAssigned)
                ((IExpression)Variant).ValidateNumberType(errorList);

            ((IBody)QueryBody).ValidateNumberType(errorList);
        }

        /// <summary>
        /// Gets the best guess for the kind of a number parameter.
        /// </summary>
        public static void UpdateParameterKind(IList<NumberKinds> usageList, ref NumberKinds bestGuess)
        {
            foreach (NumberKinds Item in usageList)
            {
                switch (Item)
                {
                    case NumberKinds.NotApplicable:
                        bestGuess = NumberKinds.NotApplicable;
                        return;

                    case NumberKinds.Unknown:
                        bestGuess = NumberKinds.Unknown;
                        break;

                    case NumberKinds.Real:
                        if (bestGuess == NumberKinds.Integer)
                            bestGuess = NumberKinds.Real;
                        break;
                }
            }
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the overload.
        /// </summary>
        public string QueryOverloadToString
        {
            get
            {
                string ParameterString = $"{EntityDeclaration.EntityDeclarationListToString(ParameterList)}";
                if (ParameterEnd == BaseNode.ParameterEndStatus.Open)
                    ParameterString += ", ...";

                string ResultString = $"{EntityDeclaration.EntityDeclarationListToString(ResultList)}";
                string BodyString = QueryBody.GetType().Name;

                return $"({ResultString}) ← ({ParameterString}) {{{BodyString}}}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Query Overload '{QueryOverloadToString}'";
        }
        #endregion
    }
}
