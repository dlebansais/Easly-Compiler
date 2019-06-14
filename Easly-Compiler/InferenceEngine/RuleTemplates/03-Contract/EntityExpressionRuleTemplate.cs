namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public interface IEntityExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public class EntityExpressionRuleTemplate : RuleTemplate<IEntityExpression, EntityExpressionRuleTemplate>, IEntityExpressionRuleTemplate
    {
        #region Init
        static EntityExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IEntityExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateScopeStart<IEntityExpression>.Default),
                new OnceReferenceTableSourceTemplate<IEntityExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType2), TemplateScopeStart<IEntityExpression>.Default),
                new OnceReferenceSourceTemplate<IEntityExpression, IList<IExpressionType>>(nameof(IEntityExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEntityExpression, IResultType>(nameof(IEntityExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IEntityExpression, IExpression>(nameof(IEntityExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IEntityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= EntityExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete);

            if (Success)
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature, ResolvedFinalDiscrete);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEntityExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item5;
            IDiscrete ResolvedFinalDiscrete = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, ICompiledFeature, IDiscrete>)data).Item6;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
