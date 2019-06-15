namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public interface IUnaryOperatorExpressionConstantRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public class UnaryOperatorExpressionConstantRuleTemplate : RuleTemplate<IUnaryOperatorExpression, UnaryOperatorExpressionConstantRuleTemplate>, IUnaryOperatorExpressionConstantRuleTemplate
    {
        #region Init
        static UnaryOperatorExpressionConstantRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IUnaryOperatorExpression, IExpression>(nameof(IUnaryOperatorExpression.ConstantSourceList)),
                new OnceReferenceCollectionSourceTemplate<IUnaryOperatorExpression, IExpression, ILanguageConstant>(nameof(IUnaryOperatorExpression.ConstantSourceList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryOperatorExpression, ILanguageConstant>(nameof(IUnaryOperatorExpression.ExpressionConstant)),
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
        public override bool CheckConsistency(IUnaryOperatorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IUnaryOperatorExpression node, object data)
        {
            ILanguageConstant ExpressionConstant = NeutralLanguageConstant.NotConstant;

            Debug.Assert(node.ConstantSourceList.Count == 1);

            IExpression RightConstantSource = node.ConstantSourceList[0];
            Debug.Assert(RightConstantSource == node.RightExpression);

            Debug.Assert(RightConstantSource.ExpressionConstant.IsAssigned);
            ILanguageConstant RightExpressionConstant = RightConstantSource.ExpressionConstant.Item;

            if (RightExpressionConstant != NeutralLanguageConstant.NotConstant)
            {
                Debug.Assert(node.SelectedOverloadType.IsAssigned);
                IQueryOverloadType SelectedOverloadType = node.SelectedOverloadType.Item;

                if (SelectedOverloadType.ResultTable.Count == 1)
                {
                    IParameter OverloadResult = SelectedOverloadType.ResultTable[0];
                    Debug.Assert(OverloadResult.ResolvedParameter.ResolvedEffectiveType.IsAssigned);

                    ICompiledType ResultType = OverloadResult.ResolvedParameter.ResolvedEffectiveType.Item;

                    if (Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType) && ResultType == BooleanType)
                        ExpressionConstant = new BooleanLanguageConstant();
                    else if (Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType) && ResultType == NumberType)
                        ExpressionConstant = new NumberLanguageConstant();
                    else if (Expression.IsLanguageTypeAvailable(LanguageClasses.Character.Guid, node, out ITypeName CharacterTypeName, out ICompiledType CharacterType) && ResultType == CharacterType)
                        ExpressionConstant = new CharacterLanguageConstant();
                    else if (Expression.IsLanguageTypeAvailable(LanguageClasses.String.Guid, node, out ITypeName StringTypeName, out ICompiledType StringType) && ResultType == StringType)
                        ExpressionConstant = new StringLanguageConstant();
                    else if (Expression.IsLanguageTypeAvailable(LanguageClasses.DateAndTime.Guid, node, out ITypeName DateAndTimeTypeName, out ICompiledType DateAndTimeType) && ResultType == DateAndTimeType)
                        ExpressionConstant = new ObjectLanguageConstant();
                    else if (Expression.IsLanguageTypeAvailable(LanguageClasses.UniversallyUniqueIdentifier.Guid, node, out ITypeName UuidTypeName, out ICompiledType UuidType) && ResultType == UuidType)
                        ExpressionConstant = new ObjectLanguageConstant();
                }
            }

            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
