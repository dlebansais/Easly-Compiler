﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BaseNodeHelper;
    using CompilerNode;
    using EaslyNumber;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllEntitiesWithDefaultRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllEntitiesWithDefaultRuleTemplate : RuleTemplate<IClass, AllEntitiesWithDefaultRuleTemplate>, IAllEntitiesWithDefaultRuleTemplate
    {
        #region Init
        static AllEntitiesWithDefaultRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IExpression, ILanguageConstant>(nameof(IClass.NodeWithDefaultList), nameof(IExpression.ExpressionConstant)),
                new OnceReferenceCollectionSourceTemplate<IClass, IExpression, ILanguageConstant>(nameof(IClass.NodeWithNumberConstantList), nameof(IExpression.ExpressionConstant)),
                new OnceReferenceTableSourceTemplate<IClass, IFeatureName, IExpression, ILanguageConstant>(nameof(IClass.DiscreteWithValueTable), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClass, IList<IExpression>>(nameof(IClass.ResolvedNodeWithDefaultList)),
                new OnceReferenceDestinationTemplate<IClass, IList<IExpression>>(nameof(IClass.ResolvedNodeWithNumberConstantList)),
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
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= AreAllConstantsValid(node);
            Success &= CheckNoCycle(node, out IDictionary<Number, IDiscrete> CombinedDiscreteNumericValueList);

            return Success;
        }

        private bool AreAllConstantsValid(IClass node)
        {
            bool Success = true;

            foreach (IExpression Item in node.NodeWithDefaultList)
            {
                IResultType ResolvedResult = Item.ResolvedResult.Item;
                if (ResolvedResult.Count != 1)
                {
                    AddSourceError(new ErrorInvalidExpression(Item));
                    Success = false;
                }
                else if (Expression.FinalConstant(Item) == NeutralLanguageConstant.NotConstant)
                {
                    AddSourceError(new ErrorConstantExpected(Item));
                    Success = false;
                }
            }

            return Success;
        }

        private bool CheckNoCycle(IClass node, out IDictionary<Number, IDiscrete> combinedDiscreteNumericValueList)
        {
            bool Success = true;
            combinedDiscreteNumericValueList = new Dictionary<Number, IDiscrete>();

            foreach (KeyValuePair<IFeatureName, IExpression> Entry in node.DiscreteWithValueTable)
            {
                Debug.Assert(node.DiscreteTable.ContainsKey(Entry.Key));
                IDiscrete DiscreteItem = node.DiscreteTable[Entry.Key];

                IExpression NumericValue = (IExpression)DiscreteItem.NumericValue.Item;
                ILanguageConstant ExpressionConstant = Expression.FinalConstant(NumericValue);

                if (ExpressionConstant is INumberLanguageConstant AsNumberLanguageConstant && AsNumberLanguageConstant.IsValueKnown)
                {
                    Number NumberConstant = AsNumberLanguageConstant.Value;

                    if (combinedDiscreteNumericValueList.ContainsKey(NumberConstant))
                    {
                        AddSourceError(new ErrorMultipleIdenticalDiscrete(DiscreteItem, NumberConstant));
                        Success = false;
                    }
                    else
                        combinedDiscreteNumericValueList.Add(AsNumberLanguageConstant.Value, DiscreteItem);
                }
                else
                {
                    AddSourceError(new ErrorInvalidExpression(NumericValue));
                    Success = false;
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            node.ResolvedNodeWithDefaultList.Item = node.NodeWithDefaultList;
            node.ResolvedNodeWithNumberConstantList.Item = node.NodeWithNumberConstantList;
        }
        #endregion
    }
}
