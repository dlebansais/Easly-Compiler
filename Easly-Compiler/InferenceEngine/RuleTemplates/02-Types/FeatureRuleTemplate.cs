namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IFeature"/>.
    /// </summary>
    /// <typeparam name="TFeature">One of the <see cref="IFeature"/> descendants.</typeparam>
    public interface IFeatureRuleTemplate<TFeature> : IRuleTemplate
        where TFeature : IFeatureWithName, ICompiledFeature
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFeature"/>.
    /// </summary>
    /// <typeparam name="TFeature">One of the <see cref="IFeature"/> descendants.</typeparam>
    public class FeatureRuleTemplate<TFeature> : RuleTemplate<TFeature, FeatureRuleTemplate<TFeature>>, IFeatureRuleTemplate<TFeature>
        where TFeature : IFeature, IFeatureWithName, ICompiledFeature
    {
        #region Init
        static FeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IFeature, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IFeature>.Default),
                new SealedTableSourceTemplate<IFeature, string, ICompiledType>(nameof(IClass.LocalGenericTable), TemplateClassStart<IFeature>.Default),
                new SealedTableSourceTemplate<IFeature, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable), TemplateClassStart<IFeature>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFeature, IFeatureName>(nameof(IFeature.ValidFeatureName)),
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
        public override bool CheckConsistency(TFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName Key;

            if (FeatureName.TableContain(EmbeddingClass.LocalExportTable, ValidText, out Key, out IHashtableEx<string, IClass> ExportTable))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalTypedefTable, ValidText, out Key, out ITypedefType TypedefType))
            {
                Debug.Assert(false);
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(TFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName FeatureEntityName = new FeatureName(ValidText);
            IFeatureInstance NewInstance = new FeatureInstance(EmbeddingClass, node);

            node.ValidFeatureName.Item = FeatureEntityName;
            EmbeddingClass.LocalFeatureTable.Add(FeatureEntityName, NewInstance);
        }
        #endregion
    }
}
