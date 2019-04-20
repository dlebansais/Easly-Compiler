namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public interface ITypedefRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public class TypedefRuleTemplate : RuleTemplate<ITypedef, TypedefRuleTemplate>, ITypedefRuleTemplate
    {
        #region Init
        static TypedefRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<ITypedef, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<ITypedef>.Default),
                new SealedTableSourceTemplate<ITypedef, string, ICompiledType>(nameof(IClass.LocalGenericTable), TemplateClassStart<ITypedef>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ITypedef, IFeatureName>(nameof(ITypedef.ValidTypedefName)),
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
        public override bool CheckConsistency(ITypedef node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;
            IFeatureName Key;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            if (EmbeddingClass.ImportedClassTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (EmbeddingClass.LocalGenericTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalExportTable, ValidText, out Key, out IHashtableEx<string, IClass> Export))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalTypedefTable, ValidText, out Key, out ITypedefType TypedefType))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalDiscreteTable, ValidText, out Key, out IDiscrete Discrete))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalFeatureTable, ValidText, out Key, out IFeatureInstance Feature))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }

            if (node.DefinedType is IAnchoredType AsAnchoredType)
            {
                AddSourceError(new ErrorInvalidAnchoredType(AsAnchoredType));
                Success = false;
            }
            else if (node.DefinedType is IKeywordAnchoredType AsKeywordAnchoredType)
            {
                AddSourceError(new ErrorInvalidAnchoredType(AsKeywordAnchoredType));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ITypedef node, object data)
        {
            IName EntityName = (IName)node.EntityName;
            IClass EmbeddingClass = node.EmbeddingClass;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName TypedefEntityName = new FeatureName(ValidText);

            ITypedefType ResolvedTypedefType;
            if (node.ResolvedDefinedTypeName.IsAssigned && node.ResolvedDefinedType.IsAssigned)
                ResolvedTypedefType = new TypedefType(node.ResolvedDefinedTypeName.Item, node.ResolvedDefinedType.Item);
            else
                ResolvedTypedefType = new TypedefType();

            EmbeddingClass.LocalTypedefTable.Add(TypedefEntityName, ResolvedTypedefType);
            node.ValidTypedefName.Item = TypedefEntityName;
        }
        #endregion
    }
}
