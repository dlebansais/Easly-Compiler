namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public interface IQueryExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionComputationRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionComputationRuleTemplate>, IQueryExpressionComputationRuleTemplate
    {
        #region Init
        static QueryExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IArgument, IResultException>(nameof(IQueryExpression.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, IResultException>(nameof(IQueryExpression.ResolvedException)),
                new UnsealedListDestinationTemplate<IQueryExpression, IParameter>(nameof(IQueryExpression.SelectedParameterList)),
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
        public override bool CheckConsistency(IQueryExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= QueryExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ListTableEx<IParameter> SelectedParameterList, out ListTableEx<IParameter> SelectedResultList, out List<IExpressionType> ResolvedArgumentList, out TypeArgumentStyles ArgumentStyle, out bool InheritBySideAttribute);

            if (Success)
            {
                Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool> AdditionalData = new Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>(ResolvedFinalFeature, ResolvedFinalDiscrete, SelectedParameterList, SelectedResultList, ResolvedArgumentList, ArgumentStyle, InheritBySideAttribute);
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, AdditionalData);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>)data).Item4;
            Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool> AdditionalData = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ListTableEx<IParameter>, ListTableEx<IParameter>, List<IExpressionType>, TypeArgumentStyles, bool>>)data).Item5;
            ICompiledFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ListTableEx<IParameter> SelectedParameterList = AdditionalData.Item3;
            ListTableEx<IParameter> SelectedResultList = AdditionalData.Item4;
            List<IExpressionType> ResolvedArgumentList = AdditionalData.Item5;
            TypeArgumentStyles ArgumentStyle = AdditionalData.Item6;
            bool InheritBySideAttribute = AdditionalData.Item7;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
            node.SelectedResultList.AddRange(SelectedResultList);
            node.SelectedResultList.Seal();
            node.ResolvedArgumentList.Item = ResolvedArgumentList;
            node.ArgumentStyle = ArgumentStyle;
            node.InheritBySideAttribute = InheritBySideAttribute;
        }
        #endregion
    }
}
