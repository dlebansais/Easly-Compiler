namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllLocalTypedefsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllLocalTypedefsRuleTemplate : RuleTemplate<IClass, AllLocalTypedefsRuleTemplate>, IAllLocalTypedefsRuleTemplate
    {
        #region Init
        static AllLocalTypedefsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, ITypedef, IFeatureName>(nameof(IClass.TypedefList), nameof(ITypedef.ValidTypedefName)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable)),
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
            IHashtableEx<IFeatureName, ITypedefType> LocalTypedefTable = node.LocalTypedefTable;
            LocalTypedefTable.Seal();

            node.LocalNamespaceTable.Add("Typedef", LocalTypedefTable);
        }
        #endregion
    }
}
