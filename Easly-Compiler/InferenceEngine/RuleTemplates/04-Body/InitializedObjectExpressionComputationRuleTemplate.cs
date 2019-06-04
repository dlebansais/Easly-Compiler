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
    public interface IInitializedObjectExpressionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public class InitializedObjectExpressionComputationRuleTemplate : RuleTemplate<IInitializedObjectExpression, InitializedObjectExpressionComputationRuleTemplate>, IInitializedObjectExpressionComputationRuleTemplate
    {
        #region Init
        static InitializedObjectExpressionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IInitializedObjectExpression, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IInitializedObjectExpression>.Default),
                new OnceReferenceCollectionSourceTemplate<IInitializedObjectExpression, IAssignmentArgument, IResultException>(nameof(IInitializedObjectExpression.AssignmentList), nameof(IAssignmentArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInitializedObjectExpression, IResultException>(nameof(IInitializedObjectExpression.ResolvedException)),
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

            Success &= InitializedObjectExpression.ResolveCompilerReferences(node, ErrorList, out IResultType ResolvedResult, out IResultException ResolvedException, out SealableList<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ITypeName InitializedObjectTypeName, out IClassType InitializedObjectType, out ISealableDictionary<string, ICompiledFeature> AssignedFeatureTable);

            if (Success)
                data = new Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>(ResolvedResult, ResolvedException, ConstantSourceList, ExpressionConstant, InitializedObjectTypeName, InitializedObjectType, AssignedFeatureTable);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInitializedObjectExpression node, object data)
        {
            IResultType ResolvedResult = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item1;
            IResultException ResolvedException = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item2;
            SealableList<IExpression> ConstantSourceList = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item4;
            ITypeName InitializedObjectTypeName = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item5;
            IClassType InitializedObjectType = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item6;
            ISealableDictionary<string, ICompiledFeature> AssignedFeatureTable = ((Tuple<IResultType, IResultException, SealableList<IExpression>, ILanguageConstant, ITypeName, IClassType, ISealableDictionary<string, ICompiledFeature>>)data).Item7;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
