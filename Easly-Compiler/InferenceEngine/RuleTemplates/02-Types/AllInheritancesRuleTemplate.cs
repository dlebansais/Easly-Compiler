namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllInheritancesRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllInheritancesRuleTemplate : RuleTemplate<IClass, AllInheritancesRuleTemplate>, IAllInheritancesRuleTemplate
    {
        #region Init
        static AllInheritancesRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, ITypeName>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedClassParentTypeName)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedClassParentType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IClassType, IObjectType>(nameof(IClass.InheritedClassTypeTable)),
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
            IHashtableEx<IClassType, IObjectType> InheritedClassTypeTable = node.InheritedClassTypeTable;
            InheritedClassTypeTable.Seal();
        }
        #endregion
    }
}
