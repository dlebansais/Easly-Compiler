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
    public interface IClassConstantExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClassConstantExpression"/>.
    /// </summary>
    public class ClassConstantExpressionRuleTemplate : RuleTemplate<IClassConstantExpression, ClassConstantExpressionRuleTemplate>, IClassConstantExpressionRuleTemplate
    {
        #region Init
        static ClassConstantExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClassConstantExpression, IResultType>(nameof(IClassConstantExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IClassConstantExpression, IExpression>(nameof(IClassConstantExpression.ConstantSourceList)),
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

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            if (ConstantSourceList.Count == 0)
            {
                IDiscreteLanguageConstant DiscreteLanguageConstant = ExpressionConstant as IDiscreteLanguageConstant;
                Debug.Assert(DiscreteLanguageConstant != null);
                Debug.Assert(DiscreteLanguageConstant.IsValueKnown);
                node.ExpressionConstant.Item = ExpressionConstant;
            }
            else
                Debug.Assert(ConstantSourceList.Count == 1);

            node.ResolvedClassTypeName.Item = ResolvedClassTypeName;
            node.ResolvedClassType.Item = ResolvedClassType;

            Debug.Assert(ResolvedFinalFeature != null || ResolvedFinalDiscrete != null);

            if (ResolvedFinalFeature != null)
                node.ResolvedFinalFeature.Item = ResolvedFinalFeature;

            if (ResolvedFinalDiscrete != null)
                node.ResolvedFinalDiscrete.Item = ResolvedFinalDiscrete;
        }
        #endregion
    }
}
