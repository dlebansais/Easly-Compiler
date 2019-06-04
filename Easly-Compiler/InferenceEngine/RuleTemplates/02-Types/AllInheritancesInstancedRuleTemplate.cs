namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllInheritancesInstancedRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllInheritancesInstancedRuleTemplate : RuleTemplate<IClass, AllInheritancesInstancedRuleTemplate>, IAllInheritancesInstancedRuleTemplate
    {
        #region Init
        static AllInheritancesInstancedRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, ITypeName>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedTypeName)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, ITypeName, ICompiledType>(nameof(IClass.InheritanceTable)),
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
            ISealableDictionary<ITypeName, ICompiledType> InheritanceTable = node.InheritanceTable;

            InheritanceTable.Seal();
            node.LocalNamespaceTable.Add("Inheritance", InheritanceTable);
        }
        #endregion
    }
}
