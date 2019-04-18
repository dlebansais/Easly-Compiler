namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEntityDeclaration"/>.
    /// </summary>
    public interface IEntityDeclarationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEntityDeclaration"/>.
    /// </summary>
    public class EntityDeclarationRuleTemplate : RuleTemplate<IEntityDeclaration, EntityDeclarationRuleTemplate>, IEntityDeclarationRuleTemplate
    {
        #region Init
        static EntityDeclarationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IEntityDeclaration, ITypeName>(nameof(IEntityDeclaration.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IEntityDeclaration, ICompiledType>(nameof(IEntityDeclaration.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEntityDeclaration, IScopeAttributeFeature>(nameof(IEntityDeclaration.ValidEntity)),
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
        public override bool CheckConsistency(IEntityDeclaration node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IName EntityName = (IName)node.EntityName;
            string ValidText = EntityName.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            IObjectType FieldType = (IObjectType)node.EntityType;
            ITypeName ValidTypeName = FieldType.ResolvedTypeName.Item;
            ICompiledType ValidType = FieldType.ResolvedType.Item;

            IScopeAttributeFeature NewEntity;

            if (node.DefaultValue.IsAssigned)
                Success = ScopeAttributeFeature.Create(node, ValidText, FieldType.ResolvedTypeName.Item, FieldType.ResolvedType.Item, (IExpression)node.DefaultValue.Item, ErrorList, out NewEntity);
            else
                Success = ScopeAttributeFeature.Create(node, ValidText, FieldType.ResolvedTypeName.Item, FieldType.ResolvedType.Item, ErrorList, out NewEntity);

            if (Success)
                data = NewEntity;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEntityDeclaration node, object data)
        {
            IScopeAttributeFeature NewEntity = (IScopeAttributeFeature)data;
            IClass EmbeddingClass = node.EmbeddingClass;

            node.ValidEntity.Item = NewEntity;

            if (node.DefaultValue.IsAssigned)
                EmbeddingClass.NodeWithDefaultList.Add(NewEntity.DefaultValue.Item);
        }
        #endregion
    }
}
