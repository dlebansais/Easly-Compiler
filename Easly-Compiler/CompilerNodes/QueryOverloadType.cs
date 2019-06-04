namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IQueryOverloadType.
    /// </summary>
    public interface IQueryOverloadType : BaseNode.IQueryOverloadType, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ParameterList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ResultBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ResultList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.RequireBlocks"/>.
        /// </summary>
        IList<IAssertion> RequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.EnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> EnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> ExceptionIdentifierList { get; }

        /// <summary>
        /// Type name associated to this overload.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Table of parameters for this overload.
        /// </summary>
        ISealableList<IParameter> ParameterTable { get; }

        /// <summary>
        /// Table of results for this overload.
        /// </summary>
        ISealableList<IParameter> ResultTable { get; }

        /// <summary>
        /// Table of conformant results for this overload.
        /// </summary>
        ISealableList<ICompiledType> ConformantResultTable { get; }

        /// <summary>
        /// List of result types for each results.
        /// </summary>
        IList<IExpressionType> ResultTypeList { get; }
    }

    /// <summary>
    /// Compiler IQueryOverloadType.
    /// </summary>
    public class QueryOverloadType : BaseNode.QueryOverloadType, IQueryOverloadType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOverloadType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public QueryOverloadType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOverloadType"/> class.
        /// </summary>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The closed or open status.</param>
        /// <param name="resultList">The list of results.</param>
        /// <param name="requireList">The list of require assertions.</param>
        /// <param name="ensureList">The list of ensure assertions.</param>
        /// <param name="exceptionIdentifierList">The list of exceptions this overload can throw.</param>
        public QueryOverloadType(IList<IEntityDeclaration> parameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IEntityDeclaration> resultList, IList<IAssertion> requireList, IList<IAssertion> ensureList, IList<IIdentifier> exceptionIdentifierList)
        {
            ParameterList = parameterList;
            ParameterEnd = parameterEnd;
            ResultList = resultList;
            RequireList = requireList;
            EnsureList = ensureList;
            ExceptionIdentifierList = exceptionIdentifierList;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ResultBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ResultList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.RequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> RequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.EnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> EnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.QueryOverloadType.ExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ExceptionIdentifierList { get; } = new List<IIdentifier>();

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

                case nameof(RequireBlocks):
                    TargetList = (IList)RequireList;
                    break;

                case nameof(EnsureBlocks):
                    TargetList = (IList)EnsureList;
                    break;

                case nameof(ExceptionIdentifierBlocks):
                    TargetList = (IList)ExceptionIdentifierList;
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
                TypeName = null;
                ParameterTable = new SealableList<IParameter>();
                ResultTable = new SealableList<IParameter>();
                ConformantResultTable = new SealableList<ICompiledType>();
                ResultTypeList = new List<IExpressionType>();
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
                IsResolved = ConformantResultTable.IsSealed;
                Debug.Assert(ParameterTable.IsSealed || !IsResolved);
                Debug.Assert(ResultTable.IsSealed || !IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Type name associated to this overload.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Table of parameters for this overload.
        /// </summary>
        public ISealableList<IParameter> ParameterTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// Table of results for this overload.
        /// </summary>
        public ISealableList<IParameter> ResultTable { get; private set; } = new SealableList<IParameter>();

        /// <summary>
        /// Table of conformant results for this overload.
        /// </summary>
        public ISealableList<ICompiledType> ConformantResultTable { get; private set; } = new SealableList<ICompiledType>();

        /// <summary>
        /// List of result types for each results.
        /// </summary>
        public IList<IExpressionType> ResultTypeList { get; private set; } = new List<IExpressionType>();

        /// <summary>
        /// Finds or creates an overload type with the corresponding parameters.
        /// </summary>
        /// <param name="instancingClassType">The type attempting to find the overload type.</param>
        /// <param name="instancedOverload">The new overload type upon return if not found.</param>
        public static void InstanciateQueryOverloadType(IClassType instancingClassType, ref IQueryOverloadType instancedOverload)
        {
            bool IsNewInstance = false;

            IList<IEntityDeclaration> InstancedParameterList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Parameter in instancedOverload.ParameterList)
            {
                ITypeName InstancedParameterTypeName = Parameter.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                ICompiledType InstancedParameterType = Parameter.ValidEntity.Item.ResolvedFeatureType.Item;
                InstancedParameterType.InstanciateType(instancingClassType, ref InstancedParameterTypeName, ref InstancedParameterType);

                IEntityDeclaration InstancedParameter = new EntityDeclaration(Parameter, InstancedParameterTypeName, InstancedParameterType);
                IName ParameterName = (IName)Parameter.EntityName;

                IScopeAttributeFeature NewEntity;
                if (Parameter.DefaultValue.IsAssigned)
                {
                    // The default value has already been checked and validated.
                    bool IsCreated = ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, (IExpression)Parameter.DefaultValue.Item, ErrorList.Ignored, out NewEntity);
                    Debug.Assert(IsCreated);
                }
                else
                    NewEntity = ScopeAttributeFeature.Create(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType);

                InstancedParameter.ValidEntity.Item = NewEntity;

                InstancedParameterList.Add(InstancedParameter);

                if (InstancedParameterType != Parameter.ValidEntity.Item.ResolvedFeatureType.Item)
                    IsNewInstance = true;
            }

            IList<IEntityDeclaration> InstancedResultList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Result in instancedOverload.ResultList)
            {
                ITypeName InstancedResultTypeName = Result.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                ICompiledType InstancedResultType = Result.ValidEntity.Item.ResolvedFeatureType.Item;
                InstancedResultType.InstanciateType(instancingClassType, ref InstancedResultTypeName, ref InstancedResultType);

                IEntityDeclaration InstancedResult = new EntityDeclaration(Result, InstancedResultTypeName, InstancedResultType);
                IName ResultName = (IName)Result.EntityName;

                IScopeAttributeFeature NewEntity;
                if (Result.DefaultValue.IsAssigned)
                {
                    // The default value has already been checked and validated.
                    bool IsCreated = ScopeAttributeFeature.Create(Result, ResultName.ValidText.Item, InstancedResultTypeName, InstancedResultType, (IExpression)Result.DefaultValue.Item, ErrorList.Ignored, out NewEntity);
                    Debug.Assert(IsCreated);
                }
                else
                    NewEntity = ScopeAttributeFeature.Create(Result, ResultName.ValidText.Item, InstancedResultTypeName, InstancedResultType);

                InstancedResult.ValidEntity.Item = NewEntity;

                InstancedResultList.Add(InstancedResult);

                if (InstancedResultType != Result.ValidEntity.Item.ResolvedFeatureType.Item)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
            {
                IQueryOverloadType NewOverloadInstance = new QueryOverloadType(InstancedParameterList, instancedOverload.ParameterEnd, InstancedResultList, instancedOverload.RequireList, instancedOverload.EnsureList, instancedOverload.ExceptionIdentifierList);

                foreach (IEntityDeclaration Item in InstancedParameterList)
                {
                    string ValidName = Item.ValidEntity.Item.ValidFeatureName.Item.Name;
                    NewOverloadInstance.ParameterTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
                }

                foreach (IEntityDeclaration Item in InstancedResultList)
                {
                    string ValidName = Item.ValidEntity.Item.ValidFeatureName.Item.Name;
                    NewOverloadInstance.ResultTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
                }

                instancedOverload = NewOverloadInstance;
            }
        }

        /// <summary>
        /// Compares two overloads.
        /// </summary>
        /// <param name="overload1">The first overload.</param>
        /// <param name="overload2">The second overload.</param>
        public static bool QueryOverloadsHaveIdenticalSignature(IQueryOverloadType overload1, IQueryOverloadType overload2)
        {
            bool IsIdentical = true;

            IsIdentical &= overload1.ParameterList.Count == overload2.ParameterList.Count;
            IsIdentical &= overload1.ParameterEnd == overload2.ParameterEnd;
            IsIdentical &= overload1.ResultList.Count != overload2.ResultList.Count;

            for (int i = 0; i < overload1.ParameterList.Count && i < overload2.ParameterList.Count; i++)
            {
                Debug.Assert(overload1.ParameterList[i].ValidEntity.IsAssigned);
                Debug.Assert(overload1.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(overload2.ParameterList[i].ValidEntity.IsAssigned);
                Debug.Assert(overload2.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                IsIdentical &= ObjectType.TypesHaveIdenticalSignature(overload1.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item, overload2.ParameterList[i].ValidEntity.Item.ResolvedFeatureType.Item);
            }

            for (int i = 0; i < overload1.ResultList.Count && i < overload2.ResultList.Count; i++)
            {
                Debug.Assert(overload1.ResultList[i].ValidEntity.IsAssigned);
                Debug.Assert(overload1.ResultList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(overload2.ResultList[i].ValidEntity.IsAssigned);
                Debug.Assert(overload2.ResultList[i].ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                IsIdentical &= ObjectType.TypesHaveIdenticalSignature(overload1.ResultList[i].ValidEntity.Item.ResolvedFeatureType.Item, overload2.ResultList[i].ValidEntity.Item.ResolvedFeatureType.Item);
            }

            IsIdentical &= Assertion.IsAssertionListEqual(overload1.RequireList, overload2.RequireList);
            IsIdentical &= Assertion.IsAssertionListEqual(overload1.EnsureList, overload2.EnsureList);
            IsIdentical &= ExceptionHandler.IdenticalExceptionSignature(overload1.ExceptionIdentifierList, overload2.ExceptionIdentifierList);

            return IsIdentical;
        }
        #endregion
    }
}
