namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ICommandOverload"/>.
    /// </summary>
    public interface ICommandOverloadContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICommandOverload"/>.
    /// </summary>
    public class CommandOverloadContractRuleTemplate : RuleTemplate<ICommandOverload, CommandOverloadContractRuleTemplate>, ICommandOverloadContractRuleTemplate
    {
        #region Init
        static CommandOverloadContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<ICommandOverload, IList<IAssertion>>(nameof(ICommandOverload.CommandBody) + Dot + nameof(IBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<ICommandOverload, IList<IAssertion>>(nameof(ICommandOverload.CommandBody) + Dot + nameof(IBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<ICommandOverload, IList<IIdentifier>>(nameof(ICommandOverload.CommandBody) + Dot + nameof(IBody.ResolvedExceptionIdentifierList)),
                new OnceReferenceSourceTemplate<ICommandOverload, IList<IExpressionType>>(nameof(ICommandOverload.CommandBody) + Dot + nameof(IBody.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICommandOverload, ICompiledType>(nameof(ICommandOverload.ResolvedBody)),
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
        public override bool CheckConsistency(ICommandOverload node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(ICommandOverload node, object data)
        {
            node.ResolvedBody.Item = (ICompiledBody)node.CommandBody;
        }
        #endregion
    }
}
