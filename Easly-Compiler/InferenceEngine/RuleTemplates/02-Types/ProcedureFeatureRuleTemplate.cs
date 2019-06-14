namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IProcedureFeature"/>.
    /// </summary>
    public interface IProcedureFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IProcedureFeature"/>.
    /// </summary>
    public class ProcedureFeatureRuleTemplate : RuleTemplate<IProcedureFeature, ProcedureFeatureRuleTemplate>, IProcedureFeatureRuleTemplate
    {
        #region Init
        static ProcedureFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IProcedureFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IProcedureFeature>.Default),
                new SealedListCollectionSourceTemplate<IProcedureFeature, ICommandOverload, IParameter>(nameof(IProcedureFeature.OverloadList), nameof(ICommandOverload.ParameterTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IProcedureFeature, ITypeName>(nameof(IProcedureFeature.ResolvedFeatureTypeName)),
                new OnceReferenceDestinationTemplate<IProcedureFeature, ICompiledType>(nameof(IProcedureFeature.ResolvedFeatureType)),
                new OnceReferenceDestinationTemplate<IProcedureFeature, ICompiledFeature>(nameof(IProcedureFeature.ResolvedFeature)),
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
        public override bool CheckConsistency(IProcedureFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IName EntityName = (IName)((IFeatureWithName)node).EntityName;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            ICommandOverload FirstOverload = node.OverloadList[0];
            ICompiledBody FirstOverloadBody = (ICompiledBody)FirstOverload.CommandBody;

            for (int i = 1; i < node.OverloadList.Count; i++)
            {
                ICommandOverload ThisOverload = node.OverloadList[i];
                ICompiledBody ThisOverloadBody = (ICompiledBody)ThisOverload.CommandBody;

                if (ThisOverloadBody.IsDeferredBody != FirstOverloadBody.IsDeferredBody)
                {
                    ErrorList.AddError(new ErrorBodyTypeMismatch(ThisOverload, EntityName.ValidText.Item));
                    Success = false;
                    break;
                }
            }

            IList<ICommandOverloadType> OverloadTypeList = new List<ICommandOverloadType>();
            foreach (ICommandOverload Overload in node.OverloadList)
            {
                Debug.Assert(Overload.ResolvedAssociatedType.IsAssigned);
                OverloadTypeList.Add(Overload.ResolvedAssociatedType.Item);
            }

            if (!Feature.DisjoinedParameterCheck(OverloadTypeList, ErrorList))
            {
                Debug.Assert(!ErrorList.IsEmpty);
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IProcedureFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            IList<ICommandOverloadType> OverloadList = new List<ICommandOverloadType>();
            foreach (ICommandOverload Item in node.OverloadList)
                OverloadList.Add(Item.ResolvedAssociatedType.Item);

            ProcedureType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType.SourceType, BaseType, OverloadList, out ITypeName ResolvedProcedureTypeName, out ICompiledType ResolvedProcedureType);
            node.ResolvedFeatureTypeName.Item = ResolvedProcedureTypeName;
            node.ResolvedFeatureType.Item = ResolvedProcedureType;
            node.TypeAsDestinationOrSource.Item = ResolvedProcedureType;

            node.ResolvedFeature.Item = node;

#if COVERAGE
            string TypeString = ResolvedProcedureType.ToString();
#endif
        }
        #endregion
    }
}
