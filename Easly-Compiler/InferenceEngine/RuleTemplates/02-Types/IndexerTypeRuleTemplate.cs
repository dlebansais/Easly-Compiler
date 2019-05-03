namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexerType"/>.
    /// </summary>
    public interface IIndexerTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexerType"/>.
    /// </summary>
    public class IndexerTypeRuleTemplate : RuleTemplate<IIndexerType, IndexerTypeRuleTemplate>, IIndexerTypeRuleTemplate
    {
        #region Init
        static IndexerTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IIndexerType, ITypeName>(nameof(IIndexerType.BaseType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IIndexerType, ICompiledType>(nameof(IIndexerType.BaseType) + Dot + nameof(IObjectType.ResolvedType)),
                new OnceReferenceSourceTemplate<IIndexerType, ITypeName>(nameof(IIndexerType.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IIndexerType, ICompiledType>(nameof(IIndexerType.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IIndexerType, IEntityDeclaration, IScopeAttributeFeature>(nameof(IIndexerType.IndexParameterList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexerType, ITypeName>(nameof(IIndexerType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IIndexerType, ICompiledType>(nameof(IIndexerType.ResolvedType)),
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
        public override bool CheckConsistency(IIndexerType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IObjectType BaseTypeItem = (IObjectType)node.BaseType;
            Debug.Assert(BaseTypeItem.ResolvedType.IsAssigned);

            IClassType BaseType = BaseTypeItem.ResolvedType.Item as IClassType;
            if (BaseType == null)
            {
                AddSourceError(new ErrorClassTypeRequired(node));
                Success = false;
            }

            ListTableEx<IParameter> ParameterTable = new ListTableEx<IParameter>();

            foreach (IEntityDeclaration Item in node.IndexParameterList)
            {
                IName ParameterName = (IName)Item.EntityName;
                string ValidText = ParameterName.ValidText.Item;
                IScopeAttributeFeature ParameterFeature = Item.ValidEntity.Item;

                if (Parameter.TableContainsName(ParameterTable, ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(ParameterName, ValidText));
                    Success = false;
                }
                else
                    ParameterTable.Add(new Parameter(ValidText, ParameterFeature));
            }

            if (Success)
                data = new Tuple<IClassType, ListTableEx<IParameter>>(BaseType, ParameterTable);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexerType node, object data)
        {
            IClassType BaseType = ((Tuple<IClassType, ListTableEx<IParameter>>)data).Item1;
            ListTableEx<IParameter> ParameterTable = ((Tuple<IClassType, ListTableEx<IParameter>>)data).Item2;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType BaseTypeItem = (IObjectType)node.BaseType;
            IObjectType EntityTypeItem = (IObjectType)node.EntityType;

            Debug.Assert(node.ParameterTable.Count == 0);
            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();

            ITypeName BaseTypeName = BaseTypeItem.ResolvedTypeName.Item;

            ITypeName EntityTypeName = EntityTypeItem.ResolvedTypeName.Item;
            ICompiledType EntityType = EntityTypeItem.ResolvedType.Item;

#if DEBUG
            // TODO: remove this code, for code coverage purpose only.
            string TypeString = node.ToString();
            Debug.Assert(!node.IsReference);
            Debug.Assert(node.IsValue);
#endif

            IndexerType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, EntityTypeName, EntityType, node.IndexerKind, node.IndexParameterList, node.ParameterEnd, node.GetRequireList, node.GetEnsureList, node.GetExceptionIdentifierList, node.SetRequireList, node.SetEnsureList, node.SetExceptionIdentifierList, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
