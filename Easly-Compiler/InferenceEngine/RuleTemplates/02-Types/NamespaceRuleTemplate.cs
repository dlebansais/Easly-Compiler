namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface INamespaceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class NamespaceRuleTemplate : RuleTemplate<IClass, NamespaceRuleTemplate>, INamespaceRuleTemplate
    {
        #region Init
        static NamespaceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IClass, string, ICompiledType>(nameof(IClass.GenericTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IHashtableEx<string, IClass>>(nameof(IClass.ExportTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, ITypedefType>(nameof(IClass.TypedefTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IDiscrete>(nameof(IClass.DiscreteTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IFeatureInstance>(nameof(IClass.FeatureTable)),
                new SealedTableSourceTemplate<IClass, string, IHashtableEx>(nameof(IClass.LocalNamespaceTable)),
                new SealedTableSourceTemplate<IClass, ITypeName, IClassType>(nameof(IClass.ResolvedImportedClassTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, string, IHashtableEx>(nameof(IClass.NamespaceTable)),
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
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IClass node, object data)
        {
            node.NamespaceTable.Add("Generic", node.GenericTable);
            node.NamespaceTable.Add("Export", node.ExportTable);
            node.NamespaceTable.Add("Typedef", node.TypedefTable);
            node.NamespaceTable.Add("Discrete", node.DiscreteTable);
            node.NamespaceTable.Add("Feature", node.FeatureTable);

            node.NamespaceTable.Seal();
        }
        #endregion
    }
}
