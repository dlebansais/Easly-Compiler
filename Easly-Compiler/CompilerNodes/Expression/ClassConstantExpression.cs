namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IClassConstantExpression.
    /// </summary>
    public interface IClassConstantExpression : BaseNode.IClassConstantExpression, IExpression, IComparableExpression
    {
        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<IConstantFeature> ResolvedFinalFeature { get; }

        /// <summary>
        /// The resolved discrete.
        /// </summary>
        OnceReference<IDiscrete> ResolvedFinalDiscrete { get; }

        /// <summary>
        /// The class type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedClassTypeName { get; }

        /// <summary>
        /// The class type.
        /// </summary>
        OnceReference<IClassType> ResolvedClassType { get; }
    }

    /// <summary>
    /// Compiler IClassConstantExpression.
    /// </summary>
    public class ClassConstantExpression : BaseNode.ClassConstantExpression, IClassConstantExpression
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
                ResolvedFinalFeature = new OnceReference<IConstantFeature>();
                ResolvedFinalDiscrete = new OnceReference<IDiscrete>();
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<IClassType>();
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
                Debug.Assert(ResolvedFinalFeature.IsAssigned || ResolvedFinalDiscrete.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassTypeName.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(ResolvedClassType.IsAssigned == ResolvedResult.IsAssigned);

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
        /// The resolved feature.
        /// </summary>
        public OnceReference<IConstantFeature> ResolvedFinalFeature { get; private set; } = new OnceReference<IConstantFeature>();

        /// <summary>
        /// The resolved discrete.
        /// </summary>
        public OnceReference<IDiscrete> ResolvedFinalDiscrete { get; private set; } = new OnceReference<IDiscrete>();

        /// <summary>
        /// The class type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The class type.
        /// </summary>
        public OnceReference<IClassType> ResolvedClassType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IClassConstantExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IClassConstantExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= ClassIdentifier.Text == other.ClassIdentifier.Text;
            Result &= ConstantIdentifier.Text == other.ConstantIdentifier.Text;

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IClassConstantExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        /// <param name="resolvedClassTypeName">The class type name upon return.</param>
        /// <param name="resolvedClassType">The class name upon return.</param>
        public static bool ResolveCompilerReferences(IClassConstantExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out IConstantFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ITypeName resolvedClassTypeName, out IClassType resolvedClassType)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;
            resolvedFinalDiscrete = null;
            resolvedClassTypeName = null;
            resolvedClassType = null;

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            IIdentifier ConstantIdentifier = (IIdentifier)node.ConstantIdentifier;
            IClass EmbeddingClass = node.EmbeddingClass;
            string ValidClassText = ClassIdentifier.ValidText.Item;
            string ValidConstantText = ConstantIdentifier.ValidText.Item;
            ISealableDictionary<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;

            if (!ClassTable.ContainsKey(ValidClassText))
            {
                errorList.AddError(new ErrorUnknownIdentifier(ClassIdentifier, ValidClassText));
                return false;
            }

            IClass BaseClass = ClassTable[ValidClassText].Item;
            resolvedClassTypeName = BaseClass.ResolvedClassTypeName.Item;
            resolvedClassType = BaseClass.ResolvedClassType.Item;

            ITypeName ConstantTypeName;
            ICompiledType ConstantType;

            ISealableDictionary<IFeatureName, IDiscrete> DiscreteTable = BaseClass.DiscreteTable;
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = BaseClass.FeatureTable;

            if (FeatureName.TableContain(DiscreteTable, ValidConstantText, out IFeatureName Key, out IDiscrete Discrete))
            {
                if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType))
                {
                    errorList.AddError(new ErrorNumberTypeMissing(node));
                    return false;
                }

                if (Discrete.NumericValue.IsAssigned)
                    constantSourceList.Add((IExpression)Discrete.NumericValue.Item);
                else
                    expressionConstant = new DiscreteLanguageConstant(Discrete);

                resolvedFinalDiscrete = Discrete;
                ConstantTypeName = NumberTypeName;
                ConstantType = NumberType;
            }
            else if (FeatureName.TableContain(FeatureTable, ValidConstantText, out Key, out IFeatureInstance FeatureInstance))
            {
                if (FeatureInstance.Feature is IConstantFeature AsConstantFeature)
                {
                    resolvedFinalFeature = AsConstantFeature;
                    ConstantTypeName = AsConstantFeature.ResolvedEntityTypeName.Item;
                    ConstantType = AsConstantFeature.ResolvedEntityType.Item;

                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    constantSourceList.Add(ConstantValue);
                }
                else
                {
                    errorList.AddError(new ErrorConstantRequired(ConstantIdentifier, ValidConstantText));
                    return false;
                }
            }
            else
            {
                errorList.AddError(new ErrorUnknownIdentifier(ConstantIdentifier, ValidConstantText));
                return false;
            }

            resolvedResult = new ResultType(ConstantTypeName, ConstantType, ValidConstantText);
            resolvedException = new ResultException();

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            IExpressionType Preferred = ResolvedResult.Item.Preferred;
            if (Preferred != null && Preferred.ValueType is ICompiledNumberType AsNumberType)
            {
                if (AsNumberType.NumberKind == NumberKinds.NotChecked)
                {
                    IConstantFeature Feature = ResolvedFinalFeature.Item;
                    if (Feature.ResolvedEntityType.Item is ICompiledNumberType AsNumberTypeEntity)
                    {
                        if (AsNumberTypeEntity.NumberKind == NumberKinds.NotChecked)
                            Feature.CheckNumberType(ref isChanged);

                        Debug.Assert(AsNumberTypeEntity.NumberKind != NumberKinds.NotChecked);

                        AsNumberType.UpdateNumberKind(AsNumberTypeEntity, ref isChanged);
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
            if (ResolvedFinalFeature.IsAssigned)
            {
                IConstantFeature Feature = ResolvedFinalFeature.Item;
                if (Feature.ResolvedEntityType.Item is ICompiledNumberType AsNumberTypeEntity)
                {
                    Feature.ValidateNumberType(errorList);
                }
            }
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"{{{ClassIdentifier.Text}}}.{ConstantIdentifier.Text}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Class Constant Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
