namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public interface IPrecursorIndexExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorIndexExpression"/>.
    /// </summary>
    public class PrecursorIndexExpressionRuleTemplate : RuleTemplate<IPrecursorIndexExpression, PrecursorIndexExpressionRuleTemplate>, IPrecursorIndexExpressionRuleTemplate
    {
        #region Init
        static PrecursorIndexExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorIndexExpression, IObjectType, ITypeName>(nameof(IPrecursorIndexExpression.AncestorType), nameof(IObjectType.ResolvedTypeName)),
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorIndexExpression, IObjectType, ICompiledType>(nameof(IPrecursorIndexExpression.AncestorType), nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorIndexExpression, IArgument, IResultType>(nameof(IPrecursorIndexExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorIndexExpression, IResultType>(nameof(IPrecursorIndexExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IPrecursorIndexExpression, IExpression>(nameof(IPrecursorIndexExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IPrecursorIndexExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorIndexExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFeatureCall FeatureCall);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, FeatureCall);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorIndexExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>)data).Item4;
            IFeatureCall FeatureCall = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureCall>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
