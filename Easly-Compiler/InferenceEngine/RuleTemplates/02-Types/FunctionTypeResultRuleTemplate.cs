namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public interface IFunctionTypeResultRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public class FunctionTypeResultRuleTemplate : RuleTemplate<IFunctionType, FunctionTypeResultRuleTemplate>, IFunctionTypeResultRuleTemplate
    {
        #region Init
        static FunctionTypeResultRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.ResolvedType)),
                new SealedListCollectionSourceTemplate<IFunctionType, IQueryOverloadType, ICompiledType>(nameof(IFunctionType.OverloadList), nameof(IQueryOverloadType.ConformantResultTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionType, IExpressionType>(nameof(IFunctionType.MostCommonResult)),
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
        public override bool CheckConsistency(IFunctionType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            IFunctionType ResolvedType = node.ResolvedType.Item as IFunctionType;
            Debug.Assert(node.OverloadList.Count == ResolvedType.OverloadList.Count);
            IList<IQueryOverloadType> OverloadTypeList = ResolvedType.OverloadList;

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
        public override void Apply(IFunctionType node, object data)
        {
            IResultType CommonResults = (IResultType)data;

            int Index = CommonResults.ResultNameIndex >= 0 ? CommonResults.ResultNameIndex : 0;
            IExpressionType MostCommonResult = CommonResults.At(Index);

            node.MostCommonResult.Item = MostCommonResult;
        }
        #endregion
    }
}
