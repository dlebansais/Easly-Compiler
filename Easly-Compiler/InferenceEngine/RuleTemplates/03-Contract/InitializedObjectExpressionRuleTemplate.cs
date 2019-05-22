namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public interface IInitializedObjectExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public class InitializedObjectExpressionRuleTemplate : RuleTemplate<IInitializedObjectExpression, InitializedObjectExpressionRuleTemplate>, IInitializedObjectExpressionRuleTemplate
    {
        #region Init
        static InitializedObjectExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IInitializedObjectExpression, IAssignmentArgument, IResultType>(nameof(IInitializedObjectExpression.AssignmentList), nameof(IAssignmentArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInitializedObjectExpression, IResultType>(nameof(IInitializedObjectExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<IInitializedObjectExpression, IExpression>(nameof(IInitializedObjectExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(IInitializedObjectExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= InitializedObjectExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ITypeName InitializedObjectTypeName, out ICompiledType InitializedObjectType, out IHashtableEx<string, ICompiledFeature> AssignedFeatureTable);

            if (Success)
                data = new Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, InitializedObjectTypeName, InitializedObjectType, AssignedFeatureTable);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInitializedObjectExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item4;
            ITypeName InitializedObjectTypeName = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item5;
            ICompiledType InitializedObjectType = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item6;
            IHashtableEx<string, ICompiledFeature> AssignedFeatureTable = ((Tuple<IResultType, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item7;

            node.ResolvedResult.Item = ResolvedResult;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            node.ResolvedClassTypeName.Item = InitializedObjectTypeName;
            node.ResolvedClassType.Item = InitializedObjectType;

            Debug.Assert(node.AssignedFeatureTable.Count == 0);
            node.AssignedFeatureTable.Merge(AssignedFeatureTable);
            node.AssignedFeatureTable.Seal();
        }
        #endregion
    }
}
