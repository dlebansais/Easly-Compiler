namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IConstantFeature"/>.
    /// </summary>
    public interface IConstantFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConstantFeature"/>.
    /// </summary>
    public class ConstantFeatureRuleTemplate : RuleTemplate<IConstantFeature, ConstantFeatureRuleTemplate>, IConstantFeatureRuleTemplate
    {
        #region Init
        static ConstantFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConstantFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IConstantFeature>.Default),
                new OnceReferenceSourceTemplate<IConstantFeature, ITypeName>(nameof(IConstantFeature.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IConstantFeature, ICompiledType>(nameof(IConstantFeature.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConstantFeature, ITypeName>(nameof(IConstantFeature.ResolvedEntityTypeName)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ICompiledType>(nameof(IConstantFeature.ResolvedEntityType)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ITypeName>(nameof(IConstantFeature.ResolvedAgentTypeName)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ICompiledType>(nameof(IConstantFeature.ResolvedAgentType)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ITypeName>(nameof(IConstantFeature.ResolvedEffectiveTypeName)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ICompiledType>(nameof(IConstantFeature.ResolvedEffectiveType)),
                new OnceReferenceDestinationTemplate<IConstantFeature, ICompiledFeature>(nameof(IConstantFeature.ResolvedFeature)),
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
        public override bool CheckConsistency(IConstantFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IName EntityName = (IName)((IFeatureWithName)node).EntityName;
            IObjectType TypeToResolve = (IObjectType)node.EntityType;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (TypeToResolve.ResolvedType.Item is IClassType AsClassType)
            {
                if (AsClassType.BaseClass.Cloneable == BaseNode.CloneableStatus.Single)
                {
                    AddSourceError(new ErrorSingleTypeNotAllowed(node, EntityName.ValidText.Item));
                    Success = false;
                }
            }

            if (Success)
            {
                IScopeAttributeFeature NewEntity = ScopeAttributeFeature.CreateResultFeature(TypeToResolve, EmbeddingClass, node);
                data = NewEntity;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConstantFeature node, object data)
        {
            IObjectType TypeToResolve = (IObjectType)node.EntityType;
            IClass EmbeddingClass = node.EmbeddingClass;
            IScopeAttributeFeature NewEntity = (IScopeAttributeFeature)data;

            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            ITypeName EntityTypeName = TypeToResolve.ResolvedTypeName.Item;
            ICompiledType EntityType = TypeToResolve.ResolvedType.Item;

            PropertyType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType.SourceType, BaseType, EntityTypeName, EntityType, BaseNode.UtilityType.ReadOnly, new List<IAssertion>(), new List<IIdentifier>(), new List<IAssertion>(), new List<IIdentifier>(), out ITypeName ResolvedAgentTypeName, out ICompiledType ResolvedAgentType);

            node.ResolvedEntityTypeName.Item = EntityTypeName;
            node.ResolvedEntityType.Item = EntityType;
            node.ResolvedAgentTypeName.Item = ResolvedAgentTypeName;
            node.ResolvedAgentType.Item = ResolvedAgentType;
            node.ResolvedEffectiveTypeName.Item = EntityTypeName;
            node.ResolvedEffectiveType.Item = EntityType;

            node.ResolvedFeature.Item = node;

            EmbeddingClass.NodeWithDefaultList.Add((IExpression)node.ConstantValue);
        }
        #endregion
    }
}
