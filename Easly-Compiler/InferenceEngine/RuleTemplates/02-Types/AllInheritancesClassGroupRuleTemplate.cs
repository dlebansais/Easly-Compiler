namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllInheritancesClassGroupRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllInheritancesClassGroupRuleTemplate : RuleTemplate<IClass, AllInheritancesClassGroupRuleTemplate>, IAllInheritancesClassGroupRuleTemplate
    {
        #region Init
        static AllInheritancesClassGroupRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClass>(nameof(IClass.InheritanceList), nameof(IInheritance.ClassGroup)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IClass, IClass>(nameof(IClass.ClassGroupList)),
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
            foreach (IInheritance Inheritance in node.InheritanceList)
            {
                Debug.Assert(Inheritance.ClassGroup.IsAssigned);
                IClass BaseClass = Inheritance.ClassGroup.Item;

                if (BaseClass != Class.ClassAny)
                    node.ClassGroupList.Add(BaseClass);
            }

            node.ClassGroupList.Seal();
        }
        #endregion
    }
}
