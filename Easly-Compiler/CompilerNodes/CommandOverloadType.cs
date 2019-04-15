namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ICommandOverloadType.
    /// </summary>
    public interface ICommandOverloadType : BaseNode.ICommandOverloadType, INode, INodeWithReplicatedBlocks, ISource
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.ParameterBlocks"/>.
        /// </summary>
        IList<IEntityDeclaration> ParameterList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.RequireBlocks"/>.
        /// </summary>
        IList<IAssertion> RequireList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.EnsureBlocks"/>.
        /// </summary>
        IList<IAssertion> EnsureList { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.ExceptionIdentifierBlocks"/>.
        /// </summary>
        IList<IIdentifier> ExceptionIdentifierList { get; }

        /// <summary>
        /// Type name associated to this overload.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Table of parameters for this overload.
        /// </summary>
        ListTableEx<IParameter> ParameterTable { get; }
    }

    /// <summary>
    /// Compiler ICommandOverloadType.
    /// </summary>
    public class CommandOverloadType : BaseNode.CommandOverloadType, ICommandOverloadType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOverloadType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public CommandOverloadType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOverloadType"/> class.
        /// </summary>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="parameterEnd">The closed or open status.</param>
        /// <param name="requireList">The list of require assertions.</param>
        /// <param name="ensureList">The list of ensure assertions.</param>
        /// <param name="exceptionIdentifierList">The list of exceptions this overload can throw.</param>
        public CommandOverloadType(IList<IEntityDeclaration> parameterList, BaseNode.ParameterEndStatus parameterEnd, IList<IAssertion> requireList, IList<IAssertion> ensureList, IList<IIdentifier> exceptionIdentifierList)
        {
            ParameterList = parameterList;
            ParameterEnd = parameterEnd;
            RequireList = requireList;
            EnsureList = ensureList;
            ExceptionIdentifierList = exceptionIdentifierList;
        }
        #endregion

        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.ParameterBlocks"/>.
        /// </summary>
        public IList<IEntityDeclaration> ParameterList { get; } = new List<IEntityDeclaration>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.RequireBlocks"/>.
        /// </summary>
        public IList<IAssertion> RequireList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.EnsureBlocks"/>.
        /// </summary>
        public IList<IAssertion> EnsureList { get; } = new List<IAssertion>();

        /// <summary>
        /// Replicated list from <see cref="BaseNode.CommandOverloadType.ExceptionIdentifierBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ExceptionIdentifierList { get; } = new List<IIdentifier>();

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
                TypeName = null;
                ParameterTable = new ListTableEx<IParameter>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
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
        public ListTableEx<IParameter> ParameterTable { get; private set; } = new ListTableEx<IParameter>();

        /// <summary>
        /// Finds or creates an overload type with the corresponding parameters.
        /// </summary>
        /// <param name="instancingClassType">The type attempting to find the overload type.</param>
        /// <param name="instancedOverload">The new overload type upon return if not found.</param>
        /// <param name="errorList">The list of errors.</param>
        public static void InstanciateCommandOverloadType(IClassType instancingClassType, ref ICommandOverloadType instancedOverload, IList<IError> errorList)
        {
            bool IsNewInstance = false;

            IList<IEntityDeclaration> InstancedParameterList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Parameter in instancedOverload.ParameterList)
            {
                ITypeName InstancedParameterTypeName = Parameter.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                ICompiledType InstancedParameterType = Parameter.ValidEntity.Item.ResolvedFeatureType.Item;
                InstancedParameterType.InstanciateType(instancingClassType, ref InstancedParameterTypeName, ref InstancedParameterType, errorList);

                IEntityDeclaration InstancedParameter = new EntityDeclaration(InstancedParameterTypeName, InstancedParameterType);
                IName ParameterName = (IName)Parameter.EntityName;

                IScopeAttributeFeature NewEntity;
                if (Parameter.DefaultValue.IsAssigned)
                    NewEntity = new ScopeAttributeFeature(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, (IExpression)Parameter.DefaultValue.Item, errorList);
                else
                    NewEntity = new ScopeAttributeFeature(Parameter, ParameterName.ValidText.Item, InstancedParameterTypeName, InstancedParameterType, errorList);
                InstancedParameter.ValidEntity.Item = NewEntity;

                InstancedParameterList.Add(InstancedParameter);

                if (InstancedParameterType != Parameter.ValidEntity.Item.ResolvedFeatureType.Item)
                    IsNewInstance = true;
            }

            if (IsNewInstance)
            {
                ICommandOverloadType NewOverloadInstance = new CommandOverloadType(InstancedParameterList, instancedOverload.ParameterEnd, instancedOverload.RequireList, instancedOverload.EnsureList, instancedOverload.ExceptionIdentifierList);

                foreach (IEntityDeclaration Item in InstancedParameterList)
                {
                    string ValidName = Item.ValidEntity.Item.ValidFeatureName.Item.Name;
                    NewOverloadInstance.ParameterTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
                }

                instancedOverload = NewOverloadInstance;
            }
        }

        /// <summary>
        /// Compares two overloads.
        /// </summary>
        /// <param name="overload1">The first overload.</param>
        /// <param name="overload2">The second overload.</param>
        public static bool CommandOverloadsHaveIdenticalSignature(ICommandOverloadType overload1, ICommandOverloadType overload2)
        {
            if (overload1.ParameterList.Count != overload2.ParameterList.Count || overload1.ParameterEnd != overload2.ParameterEnd)
                return false;

            for (int i = 0; i < overload1.ParameterList.Count && i < overload2.ParameterList.Count; i++)
                if (!ObjectType.TypesHaveIdenticalSignature(overload1.ParameterList[i].ResolvedEntityType.Item, overload2.ParameterList[i].ResolvedEntityType.Item))
                    return false;

            if (!Assertion.IsAssertionListEqual(overload1.RequireList, overload2.RequireList))
                return false;

            if (!Assertion.IsAssertionListEqual(overload1.EnsureList, overload2.EnsureList))
                return false;

            if (!ExceptionHandler.IdenticalExceptionSignature(overload1.ExceptionIdentifierList, overload2.ExceptionIdentifierList))
                return false;

            return true;
        }
        #endregion
    }
}
