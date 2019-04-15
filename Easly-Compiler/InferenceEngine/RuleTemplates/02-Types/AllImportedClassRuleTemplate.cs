namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllImportedClassRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllImportedClassRuleTemplate : RuleTemplate<IClass, AllImportedClassRuleTemplate>, IAllImportedClassRuleTemplate
    {
        #region Init
        static AllImportedClassRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IClass, string, IImportedClass>(nameof(IClass.ImportedClassTable), nameof(IImportedClass.ResolvedClassTypeName)),
                new OnceReferenceTableSourceTemplate<IClass, string, IImportedClass>(nameof(IClass.ImportedClassTable), nameof(IImportedClass.ResolvedClassType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, ITypeName, ICompiledType>(nameof(IClass.TypeTable)),
                new UnsealedTableDestinationTemplate<IClass, ITypeName, IClassType>(nameof(IClass.ResolvedImportedClassTable)),
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
            IHashtableEx<string, IImportedClass> ResolvedImportedClassTable = node.ImportedClassTable;

            foreach (KeyValuePair<string, IImportedClass> Entry in ResolvedImportedClassTable)
            {
                ITypeName ImportedClassTypeName = Entry.Value.ResolvedClassTypeName.Item;
                IClassType ImportedClassType = Entry.Value.ResolvedClassType.Item;

                node.ResolvedImportedClassTable.Add(ImportedClassTypeName, ImportedClassType);
                node.TypeTable.Add(ImportedClassTypeName, ImportedClassType);
            }

            node.ResolvedImportedClassTable.Seal();
        }
        #endregion
    }
}
