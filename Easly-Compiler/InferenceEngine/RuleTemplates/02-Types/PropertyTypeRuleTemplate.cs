namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
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

            IObjectType BaseType = (IObjectType)node.BaseType;
            Debug.Assert(BaseType.ResolvedType.IsAssigned);

            ICompiledTypeWithFeature ResolvedBaseType = BaseType.ResolvedType.Item as ICompiledTypeWithFeature;
            if (ResolvedBaseType == null)
            {
                AddSourceError(new ErrorClassTypeRequired(node));
                Success = false;
            }

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
            IObjectType BaseType = (IObjectType)node.BaseType;
            IObjectType EntityType = (IObjectType)node.EntityType;

            ITypeName ResolvedBaseTypeName = BaseType.ResolvedTypeName.Item;
            ICompiledTypeWithFeature ResolvedBaseType = BaseType.ResolvedType.Item as ICompiledTypeWithFeature;
            Debug.Assert(ResolvedBaseType != null);

            ITypeName ResolvedEntityTypeName = EntityType.ResolvedTypeName.Item;
            ICompiledType ResolvedEntityType = EntityType.ResolvedType.Item;

            PropertyType.ResolveType(EmbeddingClass.TypeTable, ResolvedBaseTypeName, BaseType, ResolvedBaseType, ResolvedEntityTypeName, ResolvedEntityType, node.PropertyKind, node.GetEnsureList, node.GetExceptionIdentifierList, node.SetRequireList, node.SetExceptionIdentifierList, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
