namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public interface IBinaryOperatorExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBinaryOperatorExpression"/>.
    /// </summary>
    public class BinaryOperatorExpressionConstantRuleTemplate : RuleTemplate<IBinaryOperatorExpression, BinaryOperatorExpressionConstantRuleTemplate>, IBinaryOperatorExpressionConstantRuleTemplate
    {
        #region Init
        static BinaryOperatorExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IBinaryOperatorExpression, IExpression>(nameof(IBinaryOperatorExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IBinaryOperatorExpression, IExpression, ILanguageConstant>(nameof(IBinaryOperatorExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IBinaryOperatorExpression, ILanguageConstant>(nameof(IBinaryOperatorExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IBinaryOperatorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IBinaryOperatorExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 2);

            IExpression LeftConstantSource = node.ConstantSourceList[0];
            Debug.Assert(LeftConstantSource == node.LeftExpression);

            IExpression RightConstantSource = node.ConstantSourceList[1];
            Debug.Assert(RightConstantSource == node.RightExpression);

            Debug.Assert(LeftConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant LeftExpressionConstant = LeftConstantSource.ExpressionConstant.Item;

            Debug.Assert(RightConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant RightExpressionConstant = RightConstantSource.ExpressionConstant.Item;

            if (LeftExpressionConstant != NeutralLanguageConstant.NotConstant && RightExpressionConstant != NeutralLanguageConstant.NotConstant)
            {
                Debug.Assert(node.ResolvedResult.IsAssigned);
                IResultType ResolvedResult = node.ResolvedResult.Item;

                if (ResolvedResult.Count == 1)
                {
                    IExpressionType ConstantType = ResolvedResult.At(0);

                    bool IsBooleanTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
                    bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);

                    if (IsBooleanTypeAvailable && ConstantType.ValueType == BooleanType)
                        ExpressionConstant = new BooleanLanguageConstant();
                    else if (IsNumberTypeAvailable && ConstantType.ValueType == NumberType)
                        ExpressionConstant = new NumberLanguageConstant();
                }
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
