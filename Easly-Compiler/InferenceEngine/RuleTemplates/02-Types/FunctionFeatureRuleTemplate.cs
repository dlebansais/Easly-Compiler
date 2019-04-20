namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IFunctionFeature"/>.
    /// </summary>
    public interface IFunctionFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFunctionFeature"/>.
    /// </summary>
    public class FunctionFeatureRuleTemplate : RuleTemplate<IFunctionFeature, FunctionFeatureRuleTemplate>, IFunctionFeatureRuleTemplate
    {
        #region Init
        static FunctionFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IFunctionFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IFunctionFeature>.Default),
                new SealedListCollectionSourceTemplate<IFunctionFeature, IQueryOverload, IParameter>(nameof(IFunctionFeature.OverloadList), nameof(IQueryOverload.ParameterTable)),
                new SealedListCollectionSourceTemplate<IFunctionFeature, IQueryOverload, IParameter>(nameof(IFunctionFeature.OverloadList), nameof(IQueryOverload.ResultTable)),
                new OnceReferenceCollectionSourceTemplate<IFunctionFeature, IQueryOverload, IQueryOverloadType>(nameof(IFunctionFeature.OverloadList), nameof(IQueryOverload.ResolvedAssociatedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionFeature, ITypeName>(nameof(IFunctionFeature.ResolvedFeatureTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionFeature, ICompiledType>(nameof(IFunctionFeature.ResolvedFeatureType)),
                new OnceReferenceDestinationTemplate<IFunctionFeature, ICompiledFeature>(nameof(IFunctionFeature.ResolvedFeature)),
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

            IList<IExpressionType> CommonResults = new List<IExpressionType>();
            IName EntityName = (IName)((IFeatureWithName)node).EntityName;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            IQueryOverload FirstOverload = node.OverloadList[0];
            ICompiledBody FirstOverloadBody = (ICompiledBody)FirstOverload.QueryBody;

            for (int i = 1; i < node.OverloadList.Count; i++)
            {
                IQueryOverload ThisOverload = node.OverloadList[i];
                ICompiledBody ThisOverloadBody = (ICompiledBody)ThisOverload.QueryBody;

                if (ThisOverloadBody.IsDeferredBody != FirstOverloadBody.IsDeferredBody)
                {
                    ErrorList.Add(new ErrorBodyTypeMismatch(ThisOverload, EntityName.ValidText.Item));
                    Success = false;
                    break;
                }
            }

            IList<IError> CheckErrorList = new List<IError>();
            IList<IQueryOverloadType> OverloadTypeList = new List<IQueryOverloadType>();
            foreach (IQueryOverload Overload in node.OverloadList)
            {
                Debug.Assert(Overload.ResolvedAssociatedType.IsAssigned);
                OverloadTypeList.Add(Overload.ResolvedAssociatedType.Item);
            }

            if (!Feature.DisjoinedParameterCheck(OverloadTypeList, node, CheckErrorList))
            {
                Debug.Assert(CheckErrorList.Count > 0);
                AddSourceErrorList(CheckErrorList);
                Success = false;
            }
            else
            {
                CommonResults = Feature.CommonResultType(OverloadTypeList);

                bool JoinSuccess = true;
                for (int i = 0; i < CommonResults.Count; i++)
                    JoinSuccess &= Feature.JoinedResultCheck(OverloadTypeList, i, CommonResults[i].ValueType, node, CheckErrorList);

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
        public override void Apply(IFunctionFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IList<IExpressionType> CommonResults = (IList<IExpressionType>)data;

            IList<IQueryOverloadType> OverloadList = new List<IQueryOverloadType>();
            foreach (IQueryOverload Overload in node.OverloadList)
            {
                Debug.Assert(Overload.ResolvedAssociatedType.IsAssigned);
                OverloadList.Add(Overload.ResolvedAssociatedType.Item);
            }

            ITypeName ResolvedFunctionTypeName;
            ICompiledType ResolvedFunctionType;
            FunctionType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, OverloadList, out ResolvedFunctionTypeName, out ResolvedFunctionType);

            node.ResolvedFeatureTypeName.Item = ResolvedFunctionTypeName;
            node.ResolvedFeatureType.Item = ResolvedFunctionType;

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

            node.ResolvedFeature.Item = node;
        }
        #endregion
    }
}
