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
    public interface IQueryExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryExpression"/>.
    /// </summary>
    public class QueryExpressionRuleTemplate : RuleTemplate<IQueryExpression, QueryExpressionRuleTemplate>, IQueryExpressionRuleTemplate
    {
        #region Init
        static QueryExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature>(nameof(IScope.FullScope), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveTypeName), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceTableSourceTemplate<IQueryExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveType), TemplateScopeStart<IQueryExpression>.Default),
                new OnceReferenceSourceTemplate<IQueryExpression, IList<IExpressionType>>(nameof(IQueryExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
                new OnceReferenceCollectionSourceTemplate<IQueryExpression, IArgument, IResultType>(nameof(IQueryExpression.ArgumentList), nameof(IArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQueryExpression, IResultType>(nameof(IQueryExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IQueryExpression, IExpression>(nameof(IQueryExpression.ConstantSourceList)),
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

            Success &= QueryExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ISealableList<IParameter> SelectedResultList, out IQueryOverloadType SelectedOverloadType, out IFeatureCall FeatureCall, out bool InheritBySideAttribute);

            if (Success)
            {
                Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool> AdditionalData = new Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>(ResolvedFinalFeature, ResolvedFinalDiscrete, SelectedResultList, SelectedOverloadType, FeatureCall, InheritBySideAttribute);
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, AdditionalData);
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
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>)data).Item4;
            Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool> AdditionalData = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ISealableList<IParameter>, IQueryOverloadType, IFeatureCall, bool>>)data).Item5;
            ICompiledFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ISealableList<IParameter> SelectedResultList = AdditionalData.Item3;
            IQueryOverloadType SelectedOverloadType = AdditionalData.Item4;
            IFeatureCall FeatureCall = AdditionalData.Item5;
            bool InheritBySideAttribute = AdditionalData.Item6;

            node.ResolvedResult.Item = ResolvedResult;

            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            if (node.ConstantSourceList.Count == 0)
                node.ExpressionConstant.Item = ExpressionConstant;

            Debug.Assert(ResolvedFinalFeature != null || ResolvedFinalDiscrete != null);

            if (ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedFinalFeature;

            if (ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedFinalDiscrete;
        }
        #endregion
    }
}
