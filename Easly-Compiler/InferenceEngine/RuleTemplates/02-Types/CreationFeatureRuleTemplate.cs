namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ICreationFeature"/>.
    /// </summary>
    public interface ICreationFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICreationFeature"/>.
    /// </summary>
    public class CreationFeatureRuleTemplate : RuleTemplate<ICreationFeature, CreationFeatureRuleTemplate>, ICreationFeatureRuleTemplate
    {
        #region Init
        static CreationFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<ICreationFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<ICreationFeature>.Default),
                new SealedListCollectionSourceTemplate<ICreationFeature, ICommandOverload, IParameter>(nameof(ICreationFeature.OverloadList), nameof(ICommandOverload.ParameterTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICreationFeature, ITypeName>(nameof(ICreationFeature.ResolvedFeatureTypeName)),
                new OnceReferenceDestinationTemplate<ICreationFeature, ICompiledType>(nameof(ICreationFeature.ResolvedFeatureType)),
                new OnceReferenceDestinationTemplate<ICreationFeature, ICompiledFeature>(nameof(ICreationFeature.ResolvedFeature)),
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
        public override bool CheckConsistency(ICreationFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

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
                    ErrorList.Add(new ErrorBodyTypeMismatch(node, EntityName.ValidText.Item));
                    Success = false;
                    break;
                }
            }

            IList<ICommandOverloadType> OverloadTypeList = new List<ICommandOverloadType>();
            foreach (ICommandOverload Overload in node.OverloadList)
                OverloadTypeList.Add(Overload.ResolvedAssociatedType.Item);

            IList<IError> CheckErrorList = new List<IError>();
            if (!Feature.DisjoinedParameterCheck(OverloadTypeList, node, CheckErrorList))
            {
                Debug.Assert(CheckErrorList.Count > 0);

                AddSourceErrorList(CheckErrorList);
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICreationFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            ICompiledType BaseType = EmbeddingClass.ResolvedClassType.Item;

            IList<ICommandOverloadType> OverloadList = new List<ICommandOverloadType>();
            foreach (ICommandOverload Item in node.OverloadList)
                OverloadList.Add(Item.ResolvedAssociatedType.Item);

            ProcedureType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, OverloadList, out ITypeName ResolvedCreationTypeName, out ICompiledType ResolvedCreationType);

            node.ResolvedFeatureTypeName.Item = ResolvedCreationTypeName;
            node.ResolvedFeatureType.Item = ResolvedCreationType;

            node.ResolvedFeature.Item = node;
        }
        #endregion
    }
}
