namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IIndexerFeature"/>.
    /// </summary>
    public interface IIndexerRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexerFeature"/>.
    /// </summary>
    public class IndexerRuleTemplate : RuleTemplate<IIndexerFeature, IndexerRuleTemplate>, IIndexerRuleTemplate
    {
        #region Init
        static IndexerRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IIndexerFeature, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IIndexerFeature>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexerFeature, IFeatureName>(nameof(IIndexerFeature.ValidFeatureName)),
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
        public override bool CheckConsistency(IIndexerFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;

            if (EmbeddingClass.LocalFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
            {
                AddSourceError(new ErrorDuplicateIndexer(node));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexerFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;

            IFeatureName FeatureEntityName = FeatureName.IndexerFeatureName;
            IFeatureInstance NewInstance = new FeatureInstance(EmbeddingClass, node);

            node.ValidFeatureName.Item = FeatureEntityName;
            EmbeddingClass.LocalFeatureTable.Add(FeatureEntityName, NewInstance);
        }
        #endregion
    }
}
