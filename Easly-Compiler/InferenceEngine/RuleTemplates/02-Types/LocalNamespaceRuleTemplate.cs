namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface ILocalNamespaceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class LocalNamespaceRuleTemplate : RuleTemplate<IClass, LocalNamespaceRuleTemplate>, ILocalNamespaceRuleTemplate
    {
        #region Init
        static LocalNamespaceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IClass, string, ICompiledType>(nameof(IClass.LocalGenericTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IHashtableEx<string, IClass>>(nameof(IClass.LocalExportTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IDiscrete>(nameof(IClass.LocalDiscreteTable)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IFeatureInstance>(nameof(IClass.LocalFeatureTable)),
                new SealedTableSourceTemplate<IClass, ITypeName, ICompiledType>(nameof(IClass.InheritanceTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, string, IHashtableEx>(nameof(IClass.LocalNamespaceTable)),
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
            node.LocalNamespaceTable.Seal();
        }
        #endregion
    }
}
