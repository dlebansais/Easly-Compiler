namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IPrecursorExpression.
    /// </summary>
    public interface IPrecursorExpression : BaseNode.IPrecursorExpression, IExpression, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }
    }

    /// <summary>
    /// Compiler IPrecursorExpression.
    /// </summary>
    public class PrecursorExpression : BaseNode.PrecursorExpression, IPrecursorExpression
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorExpression.ArgumentBlocks"/>.
        /// </summary>
        public IList<IArgument> ArgumentList { get; } = new List<IArgument>();

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
                case nameof(ArgumentBlocks):
                    TargetList = (IList)ArgumentList;
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
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ResolvedException = new OnceReference<IResultException>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
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
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IExpression
        /// <summary>
        /// Types of expression results.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IPrecursorExpression expression1, IPrecursorExpression expression2)
        {
            bool Result = true;

            if (expression1.AncestorType.IsAssigned && expression2.AncestorType.IsAssigned)
            {
                IObjectType AncestorType1 = (IObjectType)expression1.AncestorType.Item;
                IObjectType AncestorType2 = (IObjectType)expression2.AncestorType.Item;

                Debug.Assert(AncestorType1.ResolvedType.IsAssigned);
                Debug.Assert(AncestorType2.ResolvedType.IsAssigned);

                Result &= AncestorType1.ResolvedType.Item == AncestorType2.ResolvedType.Item;
            }

            Result &= expression1.AncestorType.IsAssigned == expression2.AncestorType.IsAssigned;
            Result &= Argument.IsArgumentListEqual(expression1.ArgumentList, expression2.ArgumentList);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IPrecursorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        public static bool ResolveCompilerReferences(IPrecursorExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ListTableEx<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IOptionalReference<BaseNode.IObjectType> AncestorType = node.AncestorType;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            OnceReference<IFeatureInstance> SelectedPrecursor = new OnceReference<IFeatureInstance>();
            IFeature InnerFeature = (IFeature)node.EmbeddingFeature;

            if (InnerFeature is IndexerFeature)
            {
                errorList.AddError(new ErrorPrecursorNotAllowedInIndexer(node));
                return false;
            }

            IFeature AsNamedFeature = InnerFeature;
            IFeatureInstance Instance = FeatureTable[AsNamedFeature.ValidFeatureName.Item];

            if (AncestorType.IsAssigned)
            {
                IObjectType AssignedAncestorType = (IObjectType)AncestorType.Item;
                IClassType Ancestor = AssignedAncestorType.ResolvedType.Item as IClassType;
                Debug.Assert(Ancestor != null);

                foreach (IPrecursorInstance PrecursorItem in Instance.PrecursorList)
                    if (PrecursorItem.Ancestor.BaseClass == Ancestor.BaseClass)
                    {
                        SelectedPrecursor.Item = PrecursorItem.Precursor;
                        break;
                    }

                if (!SelectedPrecursor.IsAssigned)
                {
                    errorList.AddError(new ErrorInvalidPrecursor(AssignedAncestorType));
                    return false;
                }
            }
            else if (Instance.PrecursorList.Count == 0)
            {
                errorList.AddError(new ErrorNoPrecursor(node));
                return false;
            }
            else if (Instance.PrecursorList.Count > 1)
            {
                errorList.AddError(new ErrorInvalidPrecursor(node));
                return false;
            }
            else
                SelectedPrecursor.Item = Instance.PrecursorList[0].Precursor;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, errorList))
                return false;

            ICompiledFeature OperatorFeature = SelectedPrecursor.Item.Feature.Item;
            ITypeName OperatorTypeName = OperatorFeature.ResolvedFeatureTypeName.Item;
            ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;
            IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
            bool IsHandled = false;
            bool Success = false;

            switch (OperatorType)
            {
                case IFunctionType AsFunctionType:
                    foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                        ParameterTableList.Add(Overload.ParameterTable);

                    int SelectedIndex;
                    if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, errorList, node, out SelectedIndex))
                        return false;

                    IQueryOverloadType SelectedOverload = AsFunctionType.OverloadList[SelectedIndex];
                    resolvedResult = new ResultType(SelectedOverload.ResultTypeList);
                    resolvedException = new ResultException(SelectedOverload.ExceptionIdentifierList);
                    selectedParameterList = SelectedOverload.ParameterTable;
                    resolvedArgumentList = MergedArgumentList;
                    Success = true;
                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                case IIndexerType AsIndexerType:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    IsHandled = true;
                    break;

                case IClassType AsClassType:
                    if (ArgumentList.Count > 0)
                        errorList.AddError(new ErrorInvalidExpression(node));
                    else
                    {
                        resolvedResult = new ResultType(OperatorTypeName, OperatorType, string.Empty);

                        resolvedException = new ResultException();
                        selectedParameterList = new ListTableEx<IParameter>();
                        resolvedArgumentList = new List<IExpressionType>();

                        if (OperatorFeature is IConstantFeature AsConstantFeature)
                        {
                            IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                            constantSourceList.Add(ConstantValue);
                        }

                        Success = true;
                    }
                    IsHandled = true;
                    break;

                case IPropertyType AsPropertyType:
                    IPropertyFeature Property = (IPropertyFeature)OperatorFeature;
                    string PropertyName = ((IFeatureWithName)Property).EntityName.Text;

                    resolvedResult = new ResultType(AsPropertyType.ResolvedEntityTypeName.Item, AsPropertyType.ResolvedEntityType.Item, PropertyName);

                    resolvedException = new ResultException();

                    if (Property.GetterBody.IsAssigned)
                    {
                        IBody GetterBody = (IBody)Property.GetterBody.Item;
                        resolvedException = new ResultException(GetterBody.ExceptionIdentifierList);
                    }

                    selectedParameterList = new ListTableEx<IParameter>();
                    resolvedArgumentList = new List<IExpressionType>();
                    Success = true;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            // TODO: check if the precursor is a constant number
            return Success;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString
        {
            get
            {
                string AncestorString = AncestorType.IsAssigned ? $" {{{((IObjectType)AncestorType.Item).TypeToString}}} " : string.Empty;
                return $"precursor{AncestorString}({Argument.ArgumentListToString(ArgumentList)})";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Precursor Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
