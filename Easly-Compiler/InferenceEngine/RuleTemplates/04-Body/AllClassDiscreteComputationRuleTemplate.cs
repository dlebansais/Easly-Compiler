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
    public interface IAllClassDiscreteComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllClassDiscreteComputationRuleTemplate : RuleTemplate<IClass, AllClassDiscreteComputationRuleTemplate>, IAllClassDiscreteComputationRuleTemplate
    {
        #region Init
        static AllClassDiscreteComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IClass, IInheritance, IDiscrete, string>(nameof(IClass.InheritanceList), nameof(IInheritance.AssignedDiscreteTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IDiscrete, string>(nameof(IClass.AssignedDiscreteTable)),
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
            ISealableDictionary<IDiscrete, string> AssignedDiscreteTable = node.AssignedDiscreteTable;
            Debug.Assert(AssignedDiscreteTable.Count == 0);

            foreach (IInheritance Inheritance in node.InheritanceList)
                AssignedDiscreteTable.Merge(Inheritance.AssignedDiscreteTable);

            foreach (IDiscrete Discrete in node.DiscreteList)
                if (!Discrete.NumericValue.IsAssigned)
                    AssignedDiscreteTable.Add(Discrete, Guid.NewGuid().ToString());

            AssignedDiscreteTable.Seal();

            // Also seal the filled list.
            node.InitializedObjectList.Seal();
        }
        #endregion
    }
}
