namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPropertyType"/>.
    /// </summary>
    public interface IPropertyTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPropertyType"/>.
    /// </summary>
    public class PropertyTypeRuleTemplate : RuleTemplate<IPropertyType, PropertyTypeRuleTemplate>, IPropertyTypeRuleTemplate
    {
        #region Init
        static PropertyTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IPropertyType, ITypeName>(nameof(IPropertyType.BaseType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IPropertyType, ICompiledType>(nameof(IPropertyType.BaseType) + Dot + nameof(IObjectType.ResolvedType)),
                new OnceReferenceSourceTemplate<IPropertyType, ITypeName>(nameof(IPropertyType.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IPropertyType, ICompiledType>(nameof(IPropertyType.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPropertyType, ITypeName>(nameof(IPropertyType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IPropertyType, ICompiledType>(nameof(IPropertyType.ResolvedType)),
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
        public override bool CheckConsistency(IPropertyType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPropertyType node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType BaseTypeItem = (IObjectType)node.BaseType;
            IObjectType EntityTypeItem = (IObjectType)node.EntityType;

            ITypeName BaseTypeName = BaseTypeItem.ResolvedTypeName.Item;
            ICompiledType BaseType = BaseTypeItem.ResolvedType.Item;

            ITypeName EntityTypeName = EntityTypeItem.ResolvedTypeName.Item;
            ICompiledType EntityType = EntityTypeItem.ResolvedType.Item;

            PropertyType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, EntityTypeName, EntityType, node.PropertyKind, node.GetEnsureList, node.GetExceptionIdentifierList, node.SetRequireList, node.SetExceptionIdentifierList, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
