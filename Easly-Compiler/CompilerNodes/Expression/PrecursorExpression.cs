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
        /// The selected overload, if any.
        /// </summary>
        OnceReference<IQueryOverload> SelectedOverload { get; }

        /// <summary>
        /// The selected overload, if any.
        /// </summary>
        OnceReference<IQueryOverloadType> SelectedOverloadType { get; }

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
                SelectedOverload = new OnceReference<IQueryOverload>();
                SelectedOverloadType = new OnceReference<IQueryOverloadType>();
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
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public bool IsComplex { get { return false; } }

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
        /// The selected overload, if any.
        /// </summary>
        public OnceReference<IQueryOverload> SelectedOverload { get; private set; } = new OnceReference<IQueryOverload>();

        /// <summary>
        /// The selected overload, if any.
        /// </summary>
        public OnceReference<IQueryOverloadType> SelectedOverloadType { get; private set; } = new OnceReference<IQueryOverloadType>();

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
        /// <param name="resolvedExpression">The result of the search.</param>
        public static bool ResolveCompilerReferences(IPrecursorExpression node, IErrorList errorList, out ResolvedExpression resolvedExpression)
        {
            resolvedExpression = new ResolvedExpression();

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

            if (!Instance.FindPrecursor(node.AncestorType, errorList, node, out IFeatureInstance SelectedPrecursor))
                return false;

            resolvedExpression.SelectedPrecursor = SelectedPrecursor;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList))
                return false;

            if (!ResolveCall(node, SelectedPrecursor, MergedArgumentList, TypeArgumentStyle, errorList, ref resolvedExpression, out ISealableList<IParameter> SelectedParameterList, out ISealableList<IParameter> SelectedResultList, out List<IExpressionType> ResolvedArgumentList))
                return false;

            resolvedExpression.FeatureCall = new FeatureCall(SelectedParameterList, SelectedResultList, ArgumentList, ResolvedArgumentList, TypeArgumentStyle);

            bool IsHandled = false;

            switch (SelectedPrecursor.Feature)
            {
                case IConstantFeature AsConstantFeature:
                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    resolvedExpression.ConstantSourceList.Add(ConstantValue);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    Argument.AddConstantArguments(node, resolvedExpression.ResolvedResult, ArgumentList, resolvedExpression.ConstantSourceList, out ILanguageConstant ExpressionConstant);
                    resolvedExpression.ExpressionConstant = ExpressionConstant;
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    resolvedExpression.ExpressionConstant = Expression.GetDefaultConstant(node, resolvedExpression.ResolvedResult);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            return true;
        }

        private static bool ResolveCall(IPrecursorExpression node, IFeatureInstance selectedPrecursor, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, ref ResolvedExpression resolvedExpression, out ISealableList<IParameter> selectedParameterList, out ISealableList<IParameter> selectedResultList, out List<IExpressionType> resolvedArgumentList)
        {
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
            bool IsHandled = false;
            bool Success = false;

            switch (OperatorFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                case ICreationFeature AsCreationFeature:
                case IProcedureFeature AsProcedureFeature:
                case IIndexerFeature AsIndexerFeature:
                    errorList.AddError(new ErrorInvalidExpression(node));
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    Success = ResolveCallClass(node, selectedPrecursor, mergedArgumentList, argumentStyle, errorList, ref resolvedExpression, out selectedParameterList, out selectedResultList, out resolvedArgumentList);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    IFunctionType FunctionType = AsFunctionFeature.ResolvedAgentType.Item as IFunctionType;
                    Debug.Assert(FunctionType != null);

                    Success = ResolveCallFunction(node, selectedPrecursor, FunctionType, mergedArgumentList, argumentStyle, errorList, ref resolvedExpression, out selectedParameterList, out selectedResultList, out resolvedArgumentList);
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    IPropertyType PropertyType = AsPropertyFeature.ResolvedAgentType.Item as IPropertyType;
                    Debug.Assert(PropertyType != null);

                    Success = ResolveCallProperty(node, selectedPrecursor, PropertyType, mergedArgumentList, argumentStyle, errorList, ref resolvedExpression, out selectedParameterList, out selectedResultList, out resolvedArgumentList);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return Success;
        }

        private static bool ResolveCallClass(IPrecursorExpression node, IFeatureInstance selectedPrecursor, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, ref ResolvedExpression resolvedExpression, out ISealableList<IParameter> selectedParameterList, out ISealableList<IParameter> selectedResultList, out List<IExpressionType> resolvedArgumentList)
        {
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;

            if (ArgumentList.Count > 0)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }
            else
            {
                ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
                ITypeName OperatorTypeName = OperatorFeature.ResolvedEffectiveTypeName.Item;
                ICompiledType OperatorType = OperatorFeature.ResolvedEffectiveType.Item;
                IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

                resolvedExpression.ResolvedResult = new ResultType(OperatorTypeName, OperatorType, string.Empty);
                resolvedExpression.ResolvedException = new ResultException();
                selectedParameterList = new SealableList<IParameter>();
                selectedResultList = new SealableList<IParameter>();
                resolvedArgumentList = new List<IExpressionType>();
            }

            return true;
        }

        private static bool ResolveCallFunction(IPrecursorExpression node, IFeatureInstance selectedPrecursor, IFunctionType callType, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, ref ResolvedExpression resolvedExpression, out ISealableList<IParameter> selectedParameterList, out ISealableList<IParameter> selectedResultList, out List<IExpressionType> resolvedArgumentList)
        {
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;
            ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

            foreach (IQueryOverloadType Overload in callType.OverloadList)
                ParameterTableList.Add(Overload.ParameterTable);

            if (!Argument.ArgumentsConformToParameters(ParameterTableList, mergedArgumentList, argumentStyle, errorList, node, out int SelectedIndex))
                return false;

            IFunctionFeature AsFunctionFeature = OperatorFeature as IFunctionFeature;
            Debug.Assert(AsFunctionFeature != null);
            Debug.Assert(AsFunctionFeature.OverloadList.Count == callType.OverloadList.Count);

            resolvedExpression.SelectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];
            resolvedExpression.SelectedOverloadType = callType.OverloadList[SelectedIndex];
            resolvedExpression.ResolvedResult = new ResultType(resolvedExpression.SelectedOverloadType.ResultTypeList);
            resolvedExpression.ResolvedException = new ResultException(resolvedExpression.SelectedOverloadType.ExceptionIdentifierList);
            selectedParameterList = resolvedExpression.SelectedOverloadType.ParameterTable;
            selectedResultList = resolvedExpression.SelectedOverloadType.ResultTable;
            resolvedArgumentList = mergedArgumentList;

            return true;
        }

        private static bool ResolveCallProperty(IPrecursorExpression node, IFeatureInstance selectedPrecursor, IPropertyType callType, List<IExpressionType> mergedArgumentList, TypeArgumentStyles argumentStyle, IErrorList errorList, ref ResolvedExpression resolvedExpression, out ISealableList<IParameter> selectedParameterList, out ISealableList<IParameter> selectedResultList, out List<IExpressionType> resolvedArgumentList)
        {
            selectedParameterList = null;
            selectedResultList = null;
            resolvedArgumentList = null;

            IList<IArgument> ArgumentList = node.ArgumentList;

            if (ArgumentList.Count > 0)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }
            else
            {
                ICompiledFeature OperatorFeature = selectedPrecursor.Feature;
                IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
                IPropertyFeature Property = (IPropertyFeature)OperatorFeature;
                string PropertyName = ((IFeatureWithName)Property).EntityName.Text;

                resolvedExpression.ResolvedResult = new ResultType(callType.ResolvedEntityTypeName.Item, callType.ResolvedEntityType.Item, PropertyName);

                resolvedExpression.ResolvedException = new ResultException();

                if (Property.GetterBody.IsAssigned)
                {
                    IBody GetterBody = (IBody)Property.GetterBody.Item;
                    resolvedExpression.ResolvedException = new ResultException(GetterBody.ExceptionIdentifierList);
                }

                selectedParameterList = new SealableList<IParameter>();
                selectedResultList = new SealableList<IParameter>();
                resolvedArgumentList = new List<IExpressionType>();

                return true;
            }
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind { get { return ResolvedResult.Item.NumberKind; } }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.CheckNumberType(ref isChanged);

            if (SelectedOverload.IsAssigned)
            {
                ResolvedResult.Item.UpdateNumberKind(SelectedOverload.Item.NumberKind, ref isChanged);

                IDictionary<IParameter, IList<NumberKinds>> NumberArgumentTable = SelectedOverload.Item.NumberArgumentTable;

                for (int i = 0; i < FeatureCall.Item.ArgumentList.Count && i < FeatureCall.Item.ParameterList.Count; i++)
                {
                    IArgument Argument = FeatureCall.Item.ArgumentList[i];
                    IParameter Parameter = FeatureCall.Item.ParameterList[i];

                    Debug.Assert(NumberArgumentTable.ContainsKey(Parameter));
                    IList<NumberKinds> NumberKindList = NumberArgumentTable[Parameter];

                    IExpression SourceExpression = null;
                    switch (Argument)
                    {
                        case IPositionalArgument AsPositionalArgument:
                            SourceExpression = (IExpression)AsPositionalArgument.Source;
                            break;

                        case IAssignmentArgument AsAssignmentArgument:
                            SourceExpression = (IExpression)AsAssignmentArgument.Source;
                            break;
                    }

                    Debug.Assert(SourceExpression != null);

                    IExpressionType PreferredArgumentType = SourceExpression.ResolvedResult.Item.Preferred;
                    if (PreferredArgumentType != null && PreferredArgumentType.ValueType is ICompiledNumberType AsNumberArgumentType)
                    {
                        Debug.Assert(AsNumberArgumentType.NumberKind != NumberKinds.NotChecked);

                        NumberKindList.Add(AsNumberArgumentType.NumberKind);
                    }
                }
            }
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.ValidateNumberType(errorList);
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
