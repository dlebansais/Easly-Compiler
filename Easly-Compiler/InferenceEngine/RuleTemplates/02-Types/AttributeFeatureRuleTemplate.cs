namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAttributeFeature"/>.
    /// </summary>
    public interface IAttributeFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAttributeFeature"/>.
    /// </summary>
    public class AttributeFeatureRuleTemplate : RuleTemplate<IAttributeFeature, AttributeFeatureRuleTemplate>, IAttributeFeatureRuleTemplate
    {
        #region Init
        static AttributeFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAttributeFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IAttributeFeature>.Default),
                new OnceReferenceSourceTemplate<IAttributeFeature, ITypeName>(nameof(IAttributeFeature.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IAttributeFeature, ICompiledType>(nameof(IAttributeFeature.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAttributeFeature, ITypeName>(nameof(IAttributeFeature.ResolvedEntityTypeName)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ICompiledType>(nameof(IAttributeFeature.ResolvedEntityType)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ITypeName>(nameof(IAttributeFeature.ResolvedFeatureTypeName)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ICompiledType>(nameof(IAttributeFeature.ResolvedFeatureType)),
                new UnsealedTableDestinationTemplate<IAttributeFeature, ITypeName, ICompiledType>(nameof(IClass.TypeTable), TemplateClassStart<IAttributeFeature>.Default),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ICompiledFeature>(nameof(IAttributeFeature.ResolvedFeature)),
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
        public override bool CheckConsistency(IAttributeFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IObjectType TypeToResolve = (IObjectType)node.EntityType;
            IClass EmbeddingClass = node.EmbeddingClass;

            Success &= ScopeAttributeFeature.CreateResultFeature(TypeToResolve, EmbeddingClass, node, ErrorList, out IScopeAttributeFeature resultFeature);
            if (Success)
                data = resultFeature;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAttributeFeature node, object data)
        {
            IScopeAttributeFeature NewEntity = (IScopeAttributeFeature)data;

            IObjectType TypeToResolve = (IObjectType)node.EntityType;

            node.ResolvedEntityTypeName.Item = TypeToResolve.ResolvedTypeName.Item;
            node.ResolvedEntityType.Item = TypeToResolve.ResolvedType.Item;
            node.ResolvedFeatureTypeName.Item = NewEntity.ResolvedFeatureTypeName.Item;
            node.ResolvedFeatureType.Item = NewEntity.ResolvedFeatureType.Item;

            node.ResolvedFeature.Item = node;
        }
        #endregion
    }
}
