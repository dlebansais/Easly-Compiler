namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IAllInheritanceDiscreteComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class AllInheritanceDiscreteComputationRuleTemplate : RuleTemplate<IInheritance, AllInheritanceDiscreteComputationRuleTemplate>, IAllInheritanceDiscreteComputationRuleTemplate
    {
        #region Init
        static AllInheritanceDiscreteComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IInheritance, IDiscrete, string>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.BaseClass) + Dot + nameof(IClass.AssignedDiscreteTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IInheritance, IDiscrete, string>(nameof(IInheritance.AssignedDiscreteTable)),
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
        public override bool CheckConsistency(IInheritance node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IInheritance node, object data)
        {
            ISealableDictionary<IDiscrete, string> AssignedDiscreteTable = node.AssignedDiscreteTable;
            ISealableDictionary<IDiscrete, string> ClassAssignedDiscreteTable = node.ResolvedClassParentType.Item.BaseClass.AssignedDiscreteTable;

            AssignedDiscreteTable.Merge(ClassAssignedDiscreteTable);
            AssignedDiscreteTable.Seal();
        }
        #endregion
    }
}
