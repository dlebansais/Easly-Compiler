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
                new OnceReferenceDestinationTemplate<IClassConstantExpression, IList<IIdentifier>>(nameof(IClassConstantExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IClassConstantExpression, IExpression>(nameof(IClassConstantExpression.ConstantSourceList)),
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

            Success &= ClassConstantExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ITypeName ResolvedClassTypeName, out ICompiledType ResolvedClassType);

            if (Success)
            {
                Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType> AdditionalData = new Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>(ResolvedFinalFeature, ResolvedFinalDiscrete, ResolvedClassTypeName, ResolvedClassType);
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, AdditionalData);
            }

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IClassConstantExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        /// <param name="resolvedClassTypeName">The class type name upon return.</param>
        /// <param name="resolvedClassType">The class name upon return.</param>
        public static bool ResolveCompilerReferences(IClassConstantExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out ITypeName resolvedClassTypeName, out ICompiledType resolvedClassType)
        {
            resolvedResult = null;
            resolvedExceptions = null;
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
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item4;
            Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType> AdditionalData = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item5;
            ICompiledFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ITypeName ResolvedClassTypeName = AdditionalData.Item3;
            ICompiledType ResolvedClassType = AdditionalData.Item4;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            if (ConstantSourceList.Count == 0)
            {
                IDiscreteLanguageConstant DiscreteLanguageConstant = ExpressionConstant as IDiscreteLanguageConstant;
                Debug.Assert(DiscreteLanguageConstant != null);
                Debug.Assert(DiscreteLanguageConstant.IsValueKnown);
                node.ExpressionConstant.Item = ExpressionConstant;
            }
            else
                Debug.Assert(ConstantSourceList.Count == 1);

            node.ResolvedClassTypeName.Item = ResolvedClassTypeName;
            node.ResolvedClassType.Item = ResolvedClassType;

            Debug.Assert(ResolvedFinalFeature != null || ResolvedFinalDiscrete != null);

            if (ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedFinalFeature;

            if (ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedFinalDiscrete;
        }
        #endregion
    }
}
