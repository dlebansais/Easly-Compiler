﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPrecursorBody"/>.
    /// </summary>
    public interface IPrecursorBodyComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPrecursorBody"/>.
    /// </summary>
    /// <typeparam name="T">The effective body type.</typeparam>
    public class PrecursorBodyComputationRuleTemplate<T> : RuleTemplate<T, PrecursorBodyComputationRuleTemplate<T>>, IPrecursorBodyComputationRuleTemplate
        where T : IPrecursorBody
    {
        #region Init
        static PrecursorBodyComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPrecursorBody, IList<IInstruction>>(nameof(IPrecursorBody.ResolvedInstructionList)),
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
        public override bool CheckConsistency(T node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(T node, object data)
        {
            node.ResolvedInstructionList.Item = new List<IInstruction>();
        }
        #endregion
    }
}
