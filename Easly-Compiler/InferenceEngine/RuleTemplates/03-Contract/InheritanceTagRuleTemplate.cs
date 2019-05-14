namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IInheritanceTagRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class InheritanceTagRuleTemplate : RuleTemplate<IInheritance, InheritanceTagRuleTemplate>, IInheritanceTagRuleTemplate
    {
        #region Init
        static InheritanceTagRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IInheritance, IList<IBody>>(nameof(IInheritance.ResolvedClassParentType) + Dot + nameof(IClassType.BaseClass) + Dot + nameof(IClass.ResolvedBodyTagList)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInheritance, IList<IBody>>(nameof(IInheritance.ResolvedBodyTagList)),
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
            data = null;
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInheritance node, object data)
        {
            IClassType ResolvedClassParentType = node.ResolvedClassParentType.Item;
            IClass BaseClass = ResolvedClassParentType.BaseClass;
            IList<IBody> ResolvedBodyTagList = BaseClass.ResolvedBodyTagList.Item;

            node.ResolvedBodyTagList.Item = ResolvedBodyTagList;
        }
        #endregion
    }
}
