namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IClassConstantExpression.
    /// </summary>
    public interface IClassConstantExpression : BaseNode.IClassConstantExpression, IExpression
    {
        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFinalFeature { get; }

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
        OnceReference<ICompiledType> ResolvedClassType { get; }
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
                ResolvedFinalFeature = new OnceReference<ICompiledFeature>();
                ResolvedFinalDiscrete = new OnceReference<IDiscrete>();
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<ICompiledType>();
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
                Debug.Assert(ResolvedFinalFeature.IsAssigned || ResolvedFinalDiscrete.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassTypeName.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(ResolvedClassType.IsAssigned == ResolvedResult.IsAssigned);

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
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFinalFeature { get; private set; } = new OnceReference<ICompiledFeature>();

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
        public OnceReference<ICompiledType> ResolvedClassType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IClassConstantExpression expression1, IClassConstantExpression expression2)
        {
            bool Result = true;

            Result &= expression1.ClassIdentifier.Text == expression2.ClassIdentifier.Text;
            Result &= expression1.ConstantIdentifier.Text == expression2.ConstantIdentifier.Text;

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
        public static bool ResolveCompilerReferences(IClassConstantExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ITypeName resolvedClassTypeName, out ICompiledType resolvedClassType)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
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
            IHashtableEx<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;

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

            IHashtableEx<IFeatureName, IDiscrete> DiscreteTable = BaseClass.DiscreteTable;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = BaseClass.FeatureTable;

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
                if (FeatureInstance.Feature.Item is IConstantFeature AsConstantFeature)
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

            return true;
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
