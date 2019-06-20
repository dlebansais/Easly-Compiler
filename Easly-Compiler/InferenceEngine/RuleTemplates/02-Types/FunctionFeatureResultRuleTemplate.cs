namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IFunctionFeature"/>.
    /// </summary>
    public interface IFunctionFeatureResultRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFunctionFeature"/>.
    /// </summary>
    public class FunctionFeatureResultRuleTemplate : RuleTemplate<IFunctionFeature, FunctionFeatureResultRuleTemplate>, IFunctionFeatureResultRuleTemplate
    {
        #region Init
        static FunctionFeatureResultRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IFunctionFeature, ITypeName>(nameof(IFunctionFeature.ResolvedAgentTypeName)),
                new OnceReferenceSourceTemplate<IFunctionFeature, ICompiledType>(nameof(IFunctionFeature.ResolvedAgentType)),
                new SealedListCollectionSourceTemplate<IFunctionFeature, IQueryOverload, ICompiledType>(nameof(IFunctionFeature.OverloadList), nameof(IQueryOverload.CompleteConformantResultTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionFeature, IExpressionType>(nameof(IFunctionFeature.MostCommonResult)),
                new OnceReferenceDestinationTemplate<IFunctionFeature, ITypeName>(nameof(IFunctionFeature.ResolvedEffectiveTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionFeature, ICompiledType>(nameof(IFunctionFeature.ResolvedEffectiveType)),
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
        public override bool CheckConsistency(IFunctionFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            IFunctionType ResolvedFeatureType = node.ResolvedAgentType.Item as IFunctionType;
            Debug.Assert(ResolvedFeatureType != null);

            foreach (IQueryOverloadType OverloadType in ResolvedFeatureType.OverloadList)
            {
                // TODO find why not
                // Debug.Assert(OverloadType.ConformantResultTable.IsSealed);
                Debug.Assert(OverloadType.ResultTypeList.Count == OverloadType.ResultTable.Count);
            }

            IList<IQueryOverloadType> OverloadTypeList = ResolvedFeatureType.OverloadList;

            IResultType CommonResults = Feature.CommonResultType(OverloadTypeList);

            IErrorList CheckErrorList = new ErrorList();
            for (int i = 0; i < CommonResults.Count; i++)
                Success &= Feature.JoinedResultCheck(OverloadTypeList, i, CommonResults.At(i).ValueType, node, CheckErrorList);

            if (!Success)
            {
                Debug.Assert(!CheckErrorList.IsEmpty);
                AddSourceErrorList(CheckErrorList);
            }

            if (Success)
                data = CommonResults;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IFunctionFeature node, object data)
        {
            IResultType CommonResults = (IResultType)data;

            int Index = CommonResults.ResultNameIndex >= 0 ? CommonResults.ResultNameIndex : 0;
            IExpressionType MostCommonResult = CommonResults.At(Index);

            node.MostCommonResult.Item = MostCommonResult;
            node.ResolvedEffectiveTypeName.Item = MostCommonResult.ValueTypeName;
            node.ResolvedEffectiveType.Item = MostCommonResult.ValueType;
        }
        #endregion
    }
}
