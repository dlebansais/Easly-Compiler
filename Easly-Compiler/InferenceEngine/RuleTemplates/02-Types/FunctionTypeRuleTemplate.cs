namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public interface IFunctionTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public class FunctionTypeRuleTemplate : RuleTemplate<IFunctionType, FunctionTypeRuleTemplate>, IFunctionTypeRuleTemplate
    {
        #region Init
        static FunctionTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.BaseType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.BaseType) + Dot + nameof(IObjectType.ResolvedType)),
                new SealedListCollectionSourceTemplate<IFunctionType, IQueryOverloadType, IParameter>(nameof(IFunctionType.OverloadList), nameof(IQueryOverloadType.ParameterTable)),
                new SealedListCollectionSourceTemplate<IFunctionType, IQueryOverloadType, IParameter>(nameof(IFunctionType.OverloadList), nameof(IQueryOverloadType.ResultTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.ResolvedType)),
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

            IList<IExpressionType> CommonResults = new List<IExpressionType>();

            IList<IError> CheckErrorList = new List<IError>();
            if (!Feature.DisjoinedParameterCheck(node.OverloadList, node, CheckErrorList))
            {
                Debug.Assert(CheckErrorList.Count > 0);
                AddSourceErrorList(CheckErrorList);
                Success = false;
            }
            else
            {
                CommonResults = Feature.CommonResultType(node.OverloadList);
                bool JoinSuccess = true;

                for (int i = 0; i < CommonResults.Count; i++)
                    JoinSuccess &= Feature.JoinedResultCheck(node.OverloadList, i, CommonResults[i].ValueType, node, CheckErrorList);

                if (!JoinSuccess)
                {
                    Debug.Assert(CheckErrorList.Count > 0);
                    AddSourceErrorList(CheckErrorList);
                    Success = false;
                }
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
            IList<IExpressionType> CommonResults = (IList<IExpressionType>)data;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType BaseTypeItem = (IObjectType)node.BaseType;
            ITypeName BaseTypeName = BaseTypeItem.ResolvedTypeName.Item;
            ICompiledType BaseType = BaseTypeItem.ResolvedType.Item;

            FunctionType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, node.OverloadList, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;

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
