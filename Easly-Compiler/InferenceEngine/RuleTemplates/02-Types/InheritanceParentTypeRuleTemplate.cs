namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IInheritanceParentTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class InheritanceParentTypeRuleTemplate : RuleTemplate<IInheritance, InheritanceParentTypeRuleTemplate>, IInheritanceParentTypeRuleTemplate
    {
        #region Init
        static InheritanceParentTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IInheritance, ITypeName>(nameof(IInheritance.ParentType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IInheritance, ICompiledType>(nameof(IInheritance.ParentType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInheritance, ITypeName>(nameof(IInheritance.ResolvedParentTypeName)),
                new OnceReferenceDestinationTemplate<IInheritance, ICompiledType>(nameof(IInheritance.ResolvedParentType)),
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
            IObjectType ParentType = (IObjectType)node.ParentType;

            node.ResolvedParentTypeName.Item = ParentType.ResolvedTypeName.Item;
            node.ResolvedParentType.Item = ParentType.ResolvedType.Item;
        }
        #endregion
    }
}
