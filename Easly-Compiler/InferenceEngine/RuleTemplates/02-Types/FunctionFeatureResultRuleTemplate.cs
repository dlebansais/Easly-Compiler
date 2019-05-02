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
                new OnceReferenceSourceTemplate<IFunctionFeature, ITypeName>(nameof(IFunctionFeature.ResolvedFeatureTypeName)),
                new OnceReferenceSourceTemplate<IFunctionFeature, ICompiledType>(nameof(IFunctionFeature.ResolvedFeatureType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionFeature, ITypeName>(nameof(IFunctionFeature.MostCommonTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionFeature, ICompiledType>(nameof(IFunctionFeature.MostCommonType)),
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

            IList<IQueryOverloadType> OverloadTypeList = new List<IQueryOverloadType>();
            foreach (IQueryOverload Overload in node.OverloadList)
            {
                Debug.Assert(Overload.ResolvedAssociatedType.IsAssigned);
                OverloadTypeList.Add(Overload.ResolvedAssociatedType.Item);
            }

            IList<IExpressionType> CommonResults = Feature.CommonResultType(OverloadTypeList);

            IList<IError> CheckErrorList = new List<IError>();
            for (int i = 0; i < CommonResults.Count; i++)
                Success &= Feature.JoinedResultCheck(OverloadTypeList, i, CommonResults[i].ValueType, node, CheckErrorList);

            if (!Success)
            {
                Debug.Assert(CheckErrorList.Count > 0);
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
            IList<IExpressionType> CommonResults = (IList<IExpressionType>)data;

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
