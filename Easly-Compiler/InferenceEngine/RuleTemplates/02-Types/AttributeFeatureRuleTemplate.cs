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
                new OnceReferenceDestinationTemplate<IAttributeFeature, ITypeName>(nameof(IAttributeFeature.ResolvedAgentTypeName)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ICompiledType>(nameof(IAttributeFeature.ResolvedAgentType)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ITypeName>(nameof(IAttributeFeature.ResolvedEffectiveTypeName)),
                new OnceReferenceDestinationTemplate<IAttributeFeature, ICompiledType>(nameof(IAttributeFeature.ResolvedEffectiveType)),
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
            IObjectType TypeToResolve = (IObjectType)node.EntityType;
            IClass EmbeddingClass = node.EmbeddingClass;

            IScopeAttributeFeature ResultFeature = ScopeAttributeFeature.CreateResultFeature(TypeToResolve, EmbeddingClass, node);
            data = ResultFeature;

            return true;
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
            IClass EmbeddingClass = node.EmbeddingClass;

            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            ITypeName EntityTypeName = TypeToResolve.ResolvedTypeName.Item;
            ICompiledType EntityType = TypeToResolve.ResolvedType.Item;

            PropertyType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, EntityTypeName, EntityType, BaseNode.UtilityType.ReadOnly, new List<IAssertion>(), new List<IIdentifier>(), new List<IAssertion>(), new List<IIdentifier>(), out ITypeName ResolvedAgentTypeName, out ICompiledType ResolvedAgentType);

            node.ResolvedEntityTypeName.Item = EntityTypeName;
            node.ResolvedEntityType.Item = EntityType;
            node.ResolvedAgentTypeName.Item = ResolvedAgentTypeName;
            node.ResolvedAgentType.Item = ResolvedAgentType;
            node.ResolvedEffectiveTypeName.Item = EntityTypeName;
            node.ResolvedEffectiveType.Item = EntityType;

            node.ResolvedFeature.Item = node;
        }
        #endregion
    }
}
