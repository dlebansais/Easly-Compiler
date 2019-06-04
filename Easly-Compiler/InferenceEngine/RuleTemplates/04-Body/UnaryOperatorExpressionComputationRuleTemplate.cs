namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public interface IUnaryOperatorExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IUnaryOperatorExpression"/>.
    /// </summary>
    public class UnaryOperatorExpressionComputationRuleTemplate : RuleTemplate<IUnaryOperatorExpression, UnaryOperatorExpressionComputationRuleTemplate>, IUnaryOperatorExpressionComputationRuleTemplate
    {
        #region Init
        static UnaryOperatorExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IUnaryOperatorExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IUnaryOperatorExpression>.Default),
                new OnceReferenceSourceTemplate<IUnaryOperatorExpression, IResultException>(nameof(IUnaryOperatorExpression.RightExpression) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IUnaryOperatorExpression, IResultException>(nameof(IUnaryOperatorExpression.ResolvedException)),
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

            Success &= UnaryOperatorExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IFunctionFeature SelectedFeature, out IQueryOverload SelectedOverload, out IQueryOverloadType SelectedOverloadType);
            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, SelectedFeature, SelectedOverload, SelectedOverloadType);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IUnaryOperatorExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item4;
            IFunctionFeature SelectedFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item5;
            IQueryOverload SelectedOverload = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item6;
            IQueryOverloadType SelectedOverloadType = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, IFunctionFeature, IQueryOverload, IQueryOverloadType>)data).Item7;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
