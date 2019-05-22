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

            Success &= ClassConstantExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out ITypeName ResolvedClassTypeName, out ICompiledType ResolvedClassType);

            if (Success)
            {
                Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType> AdditionalData = new Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>(ResolvedFinalFeature, ResolvedFinalDiscrete, ResolvedClassTypeName, ResolvedClassType);
                data = new Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, AdditionalData);
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
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item4;
            Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType> AdditionalData = ((Tuple<IResultType, IResultException, ListTableEx<IExpression>, ILanguageConstant, Tuple<ICompiledFeature, IDiscrete, ITypeName, ICompiledType>>)data).Item5;
            ICompiledFeature ResolvedFinalFeature = AdditionalData.Item1;
            IDiscrete ResolvedFinalDiscrete = AdditionalData.Item2;
            ITypeName ResolvedClassTypeName = AdditionalData.Item3;
            ICompiledType ResolvedClassType = AdditionalData.Item4;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
