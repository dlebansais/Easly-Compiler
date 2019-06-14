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
    public interface IPrecursorExpression : BaseNode.IPrecursorExpression, INodeWithReplicatedBlocks, IExpression, IComparableExpression
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// The resolved precursor.
        /// </summary>
        OnceReference<IFeatureInstance> ResolvedPrecursor { get; }

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        OnceReference<IFeatureCall> FeatureCall { get; }
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
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new SealableList<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedPrecursor = new OnceReference<IFeatureInstance>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
                FeatureCall = new OnceReference<IFeatureCall>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedPrecursor.IsAssigned || !IsResolved);

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;

                Debug.Assert(FeatureCall.IsAssigned || !IsResolved);

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
        public ISealableList<IExpression> ConstantSourceList { get; private set; } = new SealableList<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved precursor.
        /// </summary>
        public OnceReference<IFeatureInstance> ResolvedPrecursor { get; private set; } = new OnceReference<IFeatureInstance>();

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        public OnceReference<IFeatureCall> FeatureCall { get; private set; } = new OnceReference<IFeatureCall>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IPrecursorExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IPrecursorExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            if (AncestorType.IsAssigned && other.AncestorType.IsAssigned)
            {
                IObjectType AncestorType1 = (IObjectType)AncestorType.Item;
                IObjectType AncestorType2 = (IObjectType)other.AncestorType.Item;

                Debug.Assert(AncestorType1.ResolvedType.IsAssigned);
                Debug.Assert(AncestorType2.ResolvedType.IsAssigned);

                Result &= AncestorType1.ResolvedType.Item == AncestorType2.ResolvedType.Item;
            }

            Result &= AncestorType.IsAssigned == other.AncestorType.IsAssigned;
            Result &= Argument.IsArgumentListEqual(ArgumentList, other.ArgumentList);

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
        /// <param name="selectedPrecursor">The precursor feature.</param>
        /// <param name="featureCall">Details of the feature call.</param>
        public static bool ResolveCompilerReferences(IPrecursorExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFeatureInstance selectedPrecursor, out IFeatureCall featureCall)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedPrecursor = null;
            featureCall = null;

            IOptionalReference<BaseNode.IObjectType> AncestorType = node.AncestorType;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            ISealableDictionary<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IndexerFeature)
            {
                errorList.AddError(new ErrorPrecursorNotAllowedInIndexer(node));
                return false;
            }

            IFeature AsNamedFeature = InnerFeature;
            IFeatureInstance Instance = FeatureTable[AsNamedFeature.ValidFeatureName.Item];

            if (!Instance.FindPrecursor(node.AncestorType, errorList, node, out selectedPrecursor))
                return false;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList))
                return false;

            if (!ResolveCall(node, selectedPrecursor, MergedArgumentList, TypeArgumentStyle, errorList, out resolvedResult, out resolvedException, out ISealableList<IParameter> SelectedParameterList, out List<IExpressionType> ResolvedArgumentList))
                return false;

            featureCall = new FeatureCall(SelectedParameterList, ArgumentList, ResolvedArgumentList, TypeArgumentStyle);

            bool IsHandled = false;

            switch (selectedPrecursor.Feature)
            {
                case IConstantFeature AsConstantFeature:
                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    constantSourceList.Add(ConstantValue);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    Argument.AddConstantArguments(node, resolvedResult, ArgumentList, constantSourceList, out expressionConstant);
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    expressionConstant = Expression.GetDefaultConstant(node, resolvedResult);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return true;
        }

        private static bool ResolveCall(IPrecursorExpression node, IFeatureInstance selectedPrecursor, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            ITypeName OperatorTypeName = OperatorFeature.ResolvedFeatureTypeName.Item;
            ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
            bool IsHandled = false;
            bool Success = false;

            switch (OperatorType)
            {
                case IFunctionType AsFunctionType:
                    Success = ResolveCallFunction(node, selectedPrecursor, AsFunctionType, mergedArgumentList, argumentStyle, errorList, out resolvedResult, out resolvedException, out selectedParameterList, out resolvedArgumentList);
                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                case IIndexerType AsIndexerType:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    IsHandled = true;
                    break;

                case IClassType AsClassType:
                    Success = ResolveCallClass(node, selectedPrecursor, mergedArgumentList, argumentStyle, errorList, out resolvedResult, out resolvedException, out selectedParameterList, out resolvedArgumentList);
                    IsHandled = true;
                    break;

                case IPropertyType AsPropertyType:
                    Success = ResolveCallProperty(node, selectedPrecursor, AsPropertyType, mergedArgumentList, argumentStyle, errorList, out resolvedResult, out resolvedException, out selectedParameterList, out resolvedArgumentList);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return Success;
        }

        private static bool ResolveCallFunction(IPrecursorExpression node, IFeatureInstance selectedPrecursor, IFunctionType callType, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            ITypeName OperatorTypeName = OperatorFeature.ResolvedFeatureTypeName.Item;
            ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

            foreach (IQueryOverloadType Overload in callType.OverloadList)
                ParameterTableList.Add(Overload.ParameterTable);

            if (!Argument.ArgumentsConformToParameters(ParameterTableList, mergedArgumentList, argumentStyle, errorList, node, out int SelectedIndex))
                return false;

            IQueryOverloadType SelectedOverload = callType.OverloadList[SelectedIndex];
            resolvedResult = new ResultType(SelectedOverload.ResultTypeList);
            resolvedException = new ResultException(SelectedOverload.ExceptionIdentifierList);
            selectedParameterList = SelectedOverload.ParameterTable;
            resolvedArgumentList = mergedArgumentList;

            return true;
        }

        private static bool ResolveCallClass(IPrecursorExpression node, IFeatureInstance selectedPrecursor, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            ITypeName OperatorTypeName = OperatorFeature.ResolvedFeatureTypeName.Item;
            ICompiledType OperatorType = OperatorFeature.ResolvedFeatureType.Item;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

            if (ArgumentList.Count > 0)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }
            else
            {
                resolvedResult = new ResultType(OperatorTypeName, OperatorType, string.Empty);

                resolvedException = new ResultException();
                selectedParameterList = new SealableList<IParameter>();
                resolvedArgumentList = new List<IExpressionType>();
            }

            return true;
        }

        private static bool ResolveCallProperty(IPrecursorExpression node, IFeatureInstance selectedPrecursor, IPropertyType callType, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

            IPropertyFeature Property = (IPropertyFeature)OperatorFeature;
            string PropertyName = ((IFeatureWithName)Property).EntityName.Text;

            resolvedResult = new ResultType(callType.ResolvedEntityTypeName.Item, callType.ResolvedEntityType.Item, PropertyName);

            resolvedException = new ResultException();

            if (Property.GetterBody.IsAssigned)
            {
                IBody GetterBody = (IBody)Property.GetterBody.Item;
                resolvedException = new ResultException(GetterBody.ExceptionIdentifierList);
            }

            selectedParameterList = new SealableList<IParameter>();
            resolvedArgumentList = new List<IExpressionType>();

            return true;
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
