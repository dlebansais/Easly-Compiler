namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public interface IClassConstantExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public class ClassConstantExpressionRuleTemplate : RuleTemplate<IClassConstantExpression, ClassConstantExpressionRuleTemplate>, IClassConstantExpressionRuleTemplate
    {
        #region Init
        static ClassConstantExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IClassConstantExpression, ILanguageConstant>(nameof(IExpression.ExpressionConstant), TemplateConstantStart.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClassConstantExpression, IList<IExpressionType>>(nameof(IClassConstantExpression.ResolvedResult)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IClassConstantExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= ClassConstantExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ILanguageConstant ExpressionConstant, out ITypeName ResolvedClassTypeName, out ICompiledType ResolvedClassType);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>(ResolvedResult, ResolvedExceptions, ExpressionConstant, ResolvedClassTypeName, ResolvedClassType);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IClassConstantExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedClassTypeName">The class type name upon return.</param>
        /// <param name="resolvedClassType">The class name upon return.</param>
        public static bool ResolveCompilerReferences(IClassConstantExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ILanguageConstant expressionConstant, out ITypeName resolvedClassTypeName, out ICompiledType resolvedClassType)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            expressionConstant = null;
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
                if (Discrete.NumericValue.IsAssigned)
                    if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName ResultTypeName, out ICompiledType ResultType))
                    {
                        errorList.AddError(new ErrorNumberTypeMissing(node));
                        return false;
                    }

                ConstantTypeName = BaseClass.ResolvedClassTypeName.Item;
                ConstantType = BaseClass.ResolvedClassType.Item;
                expressionConstant = new DiscreteLanguageConstant(Discrete);
            }
            else if (FeatureName.TableContain(FeatureTable, ValidConstantText, out Key, out IFeatureInstance FeatureInstance))
            {
                if (FeatureInstance.Feature.Item is IConstantFeature AsConstantFeature)
                {
                    ConstantTypeName = AsConstantFeature.ResolvedEntityTypeName.Item;
                    ConstantType = AsConstantFeature.ResolvedEntityType.Item;

                    // This constant is assigned because the corresponding source template is ready.
                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    Debug.Assert(ConstantValue.ExpressionConstant.IsAssigned);

                    if (ConstantValue.ExpressionConstant.IsAssigned)
                        expressionConstant = ConstantValue.ExpressionConstant.Item;
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

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(ConstantTypeName, ConstantType, ValidConstantText)
            };

            resolvedExceptions = new List<IIdentifier>();

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClassConstantExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>)data).Item2;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>)data).Item3;
            ITypeName ResolvedClassTypeName = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>)data).Item4;
            ICompiledType ResolvedClassType = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ILanguageConstant, ITypeName, ICompiledType>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.SetExpressionConstant(ExpressionConstant);
            node.ResolvedClassTypeName.Item = ResolvedClassTypeName;
            node.ResolvedClassType.Item = ResolvedClassType;
        }
        #endregion
    }
}
