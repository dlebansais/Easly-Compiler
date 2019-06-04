namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IExport"/>.
    /// </summary>
    public interface IExportRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IExport"/>.
    /// </summary>
    public class ExportRuleTemplate : RuleTemplate<IExport, ExportRuleTemplate>, IExportRuleTemplate
    {
        #region Init
        static ExportRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IExport, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IExport>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IExport, IFeatureName>(nameof(IExport.ValidExportName)),
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
        public override bool CheckConsistency(IExport node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName Key;

            if (EmbeddingClass.ImportedClassTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalExportTable, ValidText, out Key, out ISealableDictionary<string, IClass> ExportTable))
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
            else if (FeatureName.TableContain(EmbeddingClass.LocalFeatureTable, ValidText, out Key, out IFeatureInstance FeatureInstance))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }

            // Ensured since the root is valid.
            Debug.Assert(node.ClassIdentifierList.Count > 0);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IExport node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName ExportEntityName = new FeatureName(ValidText);
            ISealableDictionary<string, IClass> EmptyClassTable = new SealableDictionary<string, IClass>();

            Debug.Assert(!EmbeddingClass.LocalExportTable.IsSealed);
            EmbeddingClass.LocalExportTable.Add(ExportEntityName, EmptyClassTable);
            node.ValidExportName.Item = ExportEntityName;
        }
        #endregion
    }
}
