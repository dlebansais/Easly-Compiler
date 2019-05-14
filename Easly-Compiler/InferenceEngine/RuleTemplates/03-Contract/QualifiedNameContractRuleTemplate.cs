namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IQualifiedName"/>.
    /// </summary>
    public interface IQualifiedNameContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQualifiedName"/>.
    /// </summary>
    public class QualifiedNameContractRuleTemplate : RuleTemplate<IQualifiedName, QualifiedNameContractRuleTemplate>, IQualifiedNameContractRuleTemplate
    {
        #region Init
        static QualifiedNameContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IQualifiedName, IList<IIdentifier>>(nameof(IQualifiedName.ValidPath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IQualifiedName, IList<IExpressionType>>(nameof(IQualifiedName.ValidResultTypePath)),
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
        public override bool CheckConsistency(IQualifiedName node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IQualifiedName node, object data)
        {
            node.ValidResultTypePath.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
