namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IDiscrete"/>.
    /// </summary>
    public interface IDiscreteRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IDiscrete"/>.
    /// </summary>
    public class DiscreteRuleTemplate : RuleTemplate<IDiscrete, DiscreteRuleTemplate>, IDiscreteRuleTemplate
    {
        #region Init
        static DiscreteRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IDiscrete, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IDiscrete>.Default),
                new SealedTableSourceTemplate<IDiscrete, string, ICompiledType>(nameof(IClass.LocalGenericTable), TemplateClassStart<IDiscrete>.Default),
                new SealedTableSourceTemplate<IDiscrete, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable), TemplateClassStart<IDiscrete>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IDiscrete, IFeatureName, IDiscrete>(nameof(IClass.LocalDiscreteTable), TemplateClassStart<IDiscrete>.Default),
                new OnceReferenceDestinationTemplate<IDiscrete, IFeatureName>(nameof(IDiscrete.ValidDiscreteName)),
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
        public override bool CheckConsistency(IDiscrete node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;
            string ValidText = EntityName.ValidText.Item;
            IFeatureName Key;

            if (FeatureName.TableContain(EmbeddingClass.LocalExportTable, ValidText, out Key, out IHashtableEx<string, IClass> ExportTable))
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IDiscrete node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;
            string ValidText = EntityName.ValidText.Item;

            IFeatureName DiscreteEntityName = new FeatureName(ValidText);

            EmbeddingClass.LocalDiscreteTable.Add(DiscreteEntityName, node);
            node.ValidDiscreteName.Item = DiscreteEntityName;

            if (node.NumericValue.IsAssigned)
            {
                EmbeddingClass.NodeWithDefaultList.Add((IExpression)node.NumericValue.Item);
                EmbeddingClass.NodeWithNumberConstantList.Add((IExpression)node.NumericValue.Item);
            }
        }
        #endregion
    }
}
