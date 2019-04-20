namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public interface ISimpleTypeSourceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public class SimpleTypeSourceRuleTemplate : RuleTemplate<ISimpleType, SimpleTypeSourceRuleTemplate>, ISimpleTypeSourceRuleTemplate
    {
        #region Init
        static SimpleTypeSourceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<ISimpleType, ITypeName>(nameof(ISimpleType.TypeNameSource)),
                new OnceReferenceSourceTemplate<ISimpleType, ICompiledType>(nameof(ISimpleType.TypeSource)),
                new OnceReferenceSourceTemplate<ISimpleType, string>(nameof(ISimpleType.ValidTypeSource)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ISimpleType, ITypeName>(nameof(ISimpleType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<ISimpleType, ICompiledType>(nameof(ISimpleType.ResolvedType)),
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
        public override bool CheckConsistency(ISimpleType node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(ISimpleType node, object data)
        {
            node.ResolvedTypeName.Item = node.TypeNameSource.Item;
            node.ResolvedType.Item = node.TypeSource.Item;
        }
        #endregion
    }
}
