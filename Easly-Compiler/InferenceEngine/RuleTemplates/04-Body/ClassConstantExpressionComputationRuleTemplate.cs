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
    public interface IClassConstantExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public class ClassConstantExpressionComputationRuleTemplate : RuleTemplate<IClassConstantExpression, ClassConstantExpressionComputationRuleTemplate>, IClassConstantExpressionComputationRuleTemplate
    {
        #region Init
        static ClassConstantExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IClassConstantExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IClassConstantExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClassConstantExpression, IResultException>(nameof(IClassConstantExpression.ResolvedException)),
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

            Success &= ClassConstantExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ISealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out IConstantFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ITypeName ResolvedClassTypeName, out IClassType ResolvedClassType);

            if (Success)
            {
                Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType> AdditionalData = new Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>(ResolvedFinalFeature, ResolvedFinalDiscrete, ResolvedClassTypeName, ResolvedClassType);
                data = new Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, AdditionalData);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClassConstantExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>)data).Item2;
            ISealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>)data).Item4;
            Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType> AdditionalData = ((Tuple<IResultType, IResultException, ISealableList<IExpression>, ILanguageConstant, Tuple<IConstantFeature, IDiscrete, ITypeName, IClassType>>)data).Item5;
            IConstantFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ITypeName ResolvedClassTypeName = AdditionalData.Item3;
            IClassType ResolvedClassType = AdditionalData.Item4;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
