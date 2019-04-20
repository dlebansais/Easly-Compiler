namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ITupleType"/>.
    /// </summary>
    public interface ITupleTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ITupleType"/>.
    /// </summary>
    public class TupleTypeRuleTemplate : RuleTemplate<ITupleType, TupleTypeRuleTemplate>, ITupleTypeRuleTemplate
    {
        #region Init
        static TupleTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<ITupleType, IEntityDeclaration, IScopeAttributeFeature>(nameof(ITupleType.EntityDeclarationList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<ITupleType, string, IScopeAttributeFeature>(nameof(ITupleType.FieldTable)),
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
        public override bool CheckConsistency(ITupleType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<string, IScopeAttributeFeature> FieldTable = new HashtableEx<string, IScopeAttributeFeature>();

            foreach (IEntityDeclaration Item in node.EntityDeclarationList)
            {
                IName FieldName = (IName)Item.EntityName;

                Debug.Assert(FieldName.ValidText.IsAssigned);
                string ValidText = FieldName.ValidText.Item;

                IScopeAttributeFeature FieldAttribute = Item.ValidEntity.Item;

                if (FieldTable.ContainsKey(ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(FieldName, ValidText));
                    Success = false;
                }
                else
                    FieldTable.Add(ValidText, FieldAttribute);
            }

            if (Success)
                data = FieldTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ITupleType node, object data)
        {
            IHashtableEx<string, IScopeAttributeFeature> FieldTable = (IHashtableEx<string, IScopeAttributeFeature>)data;
            IClass EmbeddingClass = node.EmbeddingClass;

            Debug.Assert(node.FieldTable.Count == 0);
            node.FieldTable.Merge(FieldTable);
            node.FieldTable.Seal();

            TupleType.ResolveType(EmbeddingClass.TypeTable, node.EntityDeclarationList, node.Sharing, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);
            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
