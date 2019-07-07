namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public interface IPrecursorExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorExpression"/>.
    /// </summary>
    public class PrecursorExpressionRuleTemplate : RuleTemplate<IPrecursorExpression, PrecursorExpressionRuleTemplate>, IPrecursorExpressionRuleTemplate
    {
        #region Init
        static PrecursorExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ITypeName>(nameof(IPrecursorExpression.AncestorType), nameof(IObjectType.ResolvedTypeName)),
                new ConditionallyAssignedReferenceSourceTemplate<IPrecursorExpression, IObjectType, ICompiledType>(nameof(IPrecursorExpression.AncestorType), nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IPrecursorExpression, IArgument, IResultType>(nameof(IPrecursorExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorExpression, IResultType>(nameof(IPrecursorExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IPrecursorExpression, IExpression>(nameof(IPrecursorExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IPrecursorExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= PrecursorExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFeatureInstance SelectedPrecursor, out IQueryOverloadType SelectedOverloadType, out IFeatureCall FeatureCall);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedPrecursor, SelectedOverloadType, FeatureCall);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPrecursorExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item4;
            IFeatureInstance SelectedPrecursor = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item5;
            IQueryOverloadType SelectedOverloadType = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item6;
            IFeatureCall FeatureCall = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFeatureInstance, IQueryOverloadType, IFeatureCall>)data).Item7;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ResolvedPrecursor.Item = SelectedPrecursor;

            if (node.ConstantSourceList.Count == 0)
                node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
