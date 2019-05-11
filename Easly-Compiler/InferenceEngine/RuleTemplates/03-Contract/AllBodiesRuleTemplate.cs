namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllBodiesRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllBodiesRuleTemplate : RuleTemplate<IClass, AllBodiesRuleTemplate>, IAllBodiesRuleTemplate
    {
        #region Init
        static AllBodiesRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableCollectionSourceTemplate<IClass, IBody, string, IExpression>(nameof(IClass.BodyList), nameof(IBody.ResolvedTagTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, IBody, IList<IExpressionType>>(nameof(IClass.BodyList), nameof(IBody.ResolvedResult)),
                new OnceReferenceCollectionSourceTemplate<IClass, ICommandOverload, ICompiledBody>(nameof(IClass.CommandOverloadList), nameof(ICommandOverload.ResolvedBody)),
                new OnceReferenceCollectionSourceTemplate<IClass, IQueryOverload, ICompiledBody>(nameof(IClass.QueryOverloadList), nameof(IQueryOverload.ResolvedBody)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClass, IList<IBody>>(nameof(IClass.ResolvedBodyTagList)),
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
            data = null;
            bool Success = true;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            node.ResolvedBodyTagList.Item = node.BodyList;
        }
        #endregion
    }
}
