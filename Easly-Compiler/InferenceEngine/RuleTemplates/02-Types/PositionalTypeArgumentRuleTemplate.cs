namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IPositionalTypeArgument"/>.
    /// </summary>
    public interface IPositionalTypeArgumentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPositionalTypeArgument"/>.
    /// </summary>
    public class PositionalTypeArgumentRuleTemplate : RuleTemplate<IPositionalTypeArgument, PositionalTypeArgumentRuleTemplate>, IPositionalTypeArgumentRuleTemplate
    {
        #region Init
        static PositionalTypeArgumentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IPositionalTypeArgument, ITypeName>(nameof(IPositionalTypeArgument.Source) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IPositionalTypeArgument, ICompiledType>(nameof(IPositionalTypeArgument.Source) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPositionalTypeArgument, ITypeName>(nameof(IPositionalTypeArgument.ResolvedSourceTypeName)),
                new OnceReferenceDestinationTemplate<IPositionalTypeArgument, ICompiledType>(nameof(IPositionalTypeArgument.ResolvedSourceType)),
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
        public override bool CheckConsistency(IPositionalTypeArgument node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IPositionalTypeArgument node, object data)
        {
            IObjectType TypeToResolve = (IObjectType)node.Source;

            node.ResolvedSourceTypeName.Item = TypeToResolve.ResolvedTypeName.Item;
            node.ResolvedSourceType.Item = TypeToResolve.ResolvedType.Item;
        }
        #endregion
    }
}
