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
                new OnceReferenceDestinationTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.MostCommonTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.MostCommonType)),
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

            IResultType CommonResults = Feature.CommonResultType(node.OverloadList);

            IErrorList CheckErrorList = new ErrorList();
            for (int i = 0; i < CommonResults.Count; i++)
                Success &= Feature.JoinedResultCheck(node.OverloadList, i, CommonResults.At(i).ValueType, node, CheckErrorList);

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

            ITypeName MostCommonTypeName = null;
            ICompiledType MostCommonType = null;
            foreach (IExpressionType Item in CommonResults)
                if (MostCommonType == null || Item.Name == nameof(BaseNode.Keyword.Result))
                {
                    MostCommonTypeName = Item.ValueTypeName;
                    MostCommonType = Item.ValueType;
                }

            node.MostCommonTypeName.Item = MostCommonTypeName;
            node.MostCommonType.Item = MostCommonType;
        }
        #endregion
    }
}
