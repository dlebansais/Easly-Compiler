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

            Success &= ClassConstantExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ITypeName ResolvedClassTypeName, out ICompiledType ResolvedClassType, out ILanguageConstant ResultNumberConstant, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>(ResolvedClassTypeName, ResolvedClassType, ResultNumberConstant, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IClassConstantExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedClassTypeName">The class type name upon return.</param>
        /// <param name="resolvedClassType">The class name upon return.</param>
        /// <param name="resultNumberConstant">The expression constant upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IClassConstantExpression node, IErrorList errorList, out ITypeName resolvedClassTypeName, out ICompiledType resolvedClassType, out ILanguageConstant resultNumberConstant, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resolvedClassTypeName = null;
            resolvedClassType = null;
            resultNumberConstant = null;
            resolvedResult = null;
            resolvedExceptions = null;

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
                    if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, errorList, out ITypeName ResultTypeName, out ICompiledType ResultType))
                    {
                        errorList.AddError(new ErrorNumberTypeMissing(node));
                        return false;
                    }

                ConstantTypeName = BaseClass.ResolvedClassTypeName.Item;
                ConstantType = BaseClass.ResolvedClassType.Item;
                resultNumberConstant = new DiscreteLanguageConstant(Discrete);
            }
            else if (FeatureName.TableContain(FeatureTable, ValidConstantText, out Key, out IFeatureInstance FeatureInstance))
            {
                if (FeatureInstance.Feature.Item is IConstantFeature AsConstantFeature)
                {
                    ConstantTypeName = AsConstantFeature.ResolvedEntityTypeName.Item;
                    ConstantType = AsConstantFeature.ResolvedEntityType.Item;

                    IExpression ConstantValue = (IExpression)AsConstantFeature.ConstantValue;
                    if (ConstantValue.NumberConstant.IsAssigned)
                        resultNumberConstant = ConstantValue.NumberConstant.Item;
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
            ITypeName ResolvedClassTypeName = ((Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            ICompiledType ResolvedClassType = ((Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            ILanguageConstant ResultNumberConstant = ((Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IExpressionType> ResolvedResult = ((Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ITypeName, ICompiledType, ILanguageConstant, IList<IExpressionType>, IList<IIdentifier>>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedClassTypeName.Item = ResolvedClassTypeName;
            node.ResolvedClassType.Item = ResolvedClassType;
            node.SetIsConstant(ResultNumberConstant);
        }
        #endregion
    }
}
