namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IBinaryOperatorExpression.
    /// </summary>
    public interface IBinaryOperatorExpression : BaseNode.IBinaryOperatorExpression, IExpression, IComparableExpression
    {
        /// <summary>
        /// The resolved operator feature.
        /// </summary>
        OnceReference<IFunctionFeature> SelectedFeature { get; }

        /// <summary>
        /// The resolved operator feature overload.
        /// </summary>
        OnceReference<IQueryOverload> SelectedOverload { get; }

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        OnceReference<IFeatureCall> FeatureCall { get; }
    }

    /// <summary>
    /// Compiler IBinaryOperatorExpression.
    /// </summary>
    public class BinaryOperatorExpression : BaseNode.BinaryOperatorExpression, IBinaryOperatorExpression
    {
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
                SelectedFeature = new OnceReference<IFunctionFeature>();
                SelectedOverload = new OnceReference<IQueryOverload>();
                FeatureCall = new OnceReference<IFeatureCall>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
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
                Debug.Assert(SelectedFeature.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(SelectedOverload.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(FeatureCall.IsAssigned == ResolvedResult.IsAssigned);

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;
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
        public bool IsComplex { get { return true; } }

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
        /// The resolved operator feature.
        /// </summary>
        public OnceReference<IFunctionFeature> SelectedFeature { get; private set; } = new OnceReference<IFunctionFeature>();

        /// <summary>
        /// The resolved operator feature overload.
        /// </summary>
        public OnceReference<IQueryOverload> SelectedOverload { get; private set; } = new OnceReference<IQueryOverload>();

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
            return IsExpressionEqual(other as IBinaryOperatorExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IBinaryOperatorExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)LeftExpression, (IExpression)other.LeftExpression);
            Result &= Operator.Text == other.Operator.Text;
            Result &= Expression.IsExpressionEqual((IExpression)RightExpression, (IExpression)other.RightExpression);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IBinaryOperatorExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The constant value upon return, if any.</param>
        /// <param name="selectedFeature">The matching feature upon return.</param>
        /// <param name="selectedOverload">The matching overload in <paramref name="selectedFeature"/> upon return.</param>
        /// <param name="featureCall">Details of the feature call.</param>
        public static bool ResolveCompilerReferences(IBinaryOperatorExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IFunctionFeature selectedFeature, out IQueryOverload selectedOverload, out IFeatureCall featureCall)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedFeature = null;
            selectedOverload = null;
            featureCall = null;

            IExpression LeftExpression = (IExpression)node.LeftExpression;
            IIdentifier Operator = (IIdentifier)node.Operator;
            IExpression RightExpression = (IExpression)node.RightExpression;
            IResultType LeftResult = LeftExpression.ResolvedResult.Item;

            if (LeftResult.TryGetResult(out ICompiledType LeftExpressionType))
            {
                if (LeftExpressionType is IClassType AsClassType)
                {
                    string OperatorName = Operator.ValidText.Item;

                    ISealableDictionary<IFeatureName, IFeatureInstance> LeftFeatureTable = AsClassType.FeatureTable;

                    if (!FeatureName.TableContain(LeftFeatureTable, OperatorName, out IFeatureName Key, out IFeatureInstance Value))
                    {
                        errorList.AddError(new ErrorUnknownIdentifier(Operator, OperatorName));
                        return false;
                    }

                    Debug.Assert(Value.Feature != null);

                    ICompiledFeature OperatorFeature = Value.Feature;
                    ICompiledType OperatorType = OperatorFeature.ResolvedAgentType.Item;

                    if (OperatorFeature is IFunctionFeature AsFunctionFeature && OperatorType is FunctionType AsFunctionType)
                    {
                        IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
                        foreach (IQueryOverloadType Overload in AsFunctionType.OverloadList)
                            ParameterTableList.Add(Overload.ParameterTable);

                        IResultType RightResult = RightExpression.ResolvedResult.Item;
                        if (!Argument.ArgumentsConformToParameters(ParameterTableList, RightResult.ToList(), TypeArgumentStyles.Positional, errorList, Operator, out int SelectedIndex))
                            return false;

                        IQueryOverloadType SelectedOverloadType = AsFunctionType.OverloadList[SelectedIndex];
                        resolvedResult = new ResultType(SelectedOverloadType.ResultTypeList);
                        selectedFeature = AsFunctionFeature;
                        selectedOverload = AsFunctionFeature.OverloadList[SelectedIndex];

                        IArgument FirstArgument = new PositionalArgument(RightExpression);
                        IList<IArgument> ArgumentList = new List<IArgument>() { FirstArgument };

                        List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                        bool IsArgumentValid = Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList);
                        Debug.Assert(IsArgumentValid);

                        featureCall = new FeatureCall(SelectedOverloadType.ParameterTable, SelectedOverloadType.ResultTable, ArgumentList, MergedArgumentList, TypeArgumentStyle);
                        resolvedException = new ResultException(SelectedOverloadType.ExceptionIdentifierList);

                        constantSourceList.Add(LeftExpression);
                        constantSourceList.Add(RightExpression);
                    }
                    else
                    {
                        errorList.AddError(new ErrorInvalidOperator(Operator, OperatorName));
                        return false;
                    }
                }
                else
                {
                    errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                    return false;
                }
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(LeftExpression));
                return false;
            }

#if COVERAGE
            Debug.Assert(node.IsComplex);
#endif

            return true;
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
            ((IExpression)LeftExpression).RestartNumberType(ref isChanged);
            ((IExpression)RightExpression).RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            Debug.Assert(ResolvedResult.IsAssigned);

            ((IExpression)LeftExpression).CheckNumberType(ref isChanged);
            ((IExpression)RightExpression).CheckNumberType(ref isChanged);

            Debug.Assert(SelectedOverload.IsAssigned);
            IQueryOverload Overload = SelectedOverload.Item;
            Overload.CheckNumberType(ref isChanged);

            ResolvedResult.Item.UpdateNumberKind(Overload.NumberKind, ref isChanged);

            IDictionary<IParameter, IList<NumberKinds>> NumberArgumentTable = Overload.NumberArgumentTable;
            Debug.Assert(NumberArgumentTable.Count > 0);
            Debug.Assert(FeatureCall.Item.ArgumentList.Count <= NumberArgumentTable.Count);
            Debug.Assert(FeatureCall.Item.ParameterList.Count == NumberArgumentTable.Count);

            for (int i = 0; i < FeatureCall.Item.ArgumentList.Count; i++)
            {
                IArgument Argument = FeatureCall.Item.ArgumentList[i];
                IParameter Parameter = Overload.ParameterTable[i];

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
                Debug.Assert(SourceExpression.ResolvedResult.IsAssigned);

                NumberKindList.Add(SourceExpression.ResolvedResult.Item.NumberKind);
            }
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            ((IExpression)LeftExpression).ValidateNumberType(errorList);
            ((IExpression)RightExpression).ValidateNumberType(errorList);

            Debug.Assert(SelectedOverload.IsAssigned);
            IQueryOverload Overload = SelectedOverload.Item;

            Debug.Assert(Overload.EmbeddingClass != null);
            Debug.Assert(SelectedFeature.IsAssigned);
            Debug.Assert(SelectedFeature.Item.ValidFeatureName.IsAssigned);

            if (Overload.EmbeddingClass.ClassGuid == LanguageClasses.Number.Guid)
            {
                string FeatureName = SelectedFeature.Item.ValidFeatureName.Item.Name;

                switch (FeatureName)
                {
                    case "<<":
                    case ">>":
                    case "bitwise and":
                    case "bitwise or":
                    case "bitwise xor":
                        NumberKinds LeftKind = ((IExpression)LeftExpression).ResolvedResult.Item.NumberKind;
                        NumberKinds RightKind = ((IExpression)RightExpression).ResolvedResult.Item.NumberKind;

                        if (LeftKind != NumberKinds.Integer)
                            errorList.AddError(new ErrorInvalidOperatorOnNumber((IExpression)LeftExpression, FeatureName));
                        else if (RightKind != NumberKinds.Integer)
                            errorList.AddError(new ErrorInvalidOperatorOnNumber((IExpression)RightExpression, FeatureName));
                        break;
                }
            }
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
                IExpression Expression;

                Expression = (IExpression)LeftExpression;
                string LeftExpressionString = Expression.IsComplex ? $"({Expression.ExpressionToString})" : Expression.ExpressionToString;

                Expression = (IExpression)RightExpression;
                string RightExpressionString = Expression.IsComplex ? $"({Expression.ExpressionToString})" : Expression.ExpressionToString;

                string OperatorText = Operator.Text;

                return $"{LeftExpressionString} {OperatorText} {RightExpressionString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Binary Operator Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
