namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IGenericType"/>.
    /// </summary>
    public interface IGenericTypeArgumentsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IGenericType"/>.
    /// </summary>
    public class GenericTypeArgumentsRuleTemplate : RuleTemplate<IGenericType, GenericTypeArgumentsRuleTemplate>, IGenericTypeArgumentsRuleTemplate
    {
        #region Init
        static GenericTypeArgumentsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IGenericType, ITypeArgument, ITypeName>(nameof(IGenericType.TypeArgumentList), nameof(ITypeArgument.ResolvedSourceTypeName)),
                new OnceReferenceCollectionSourceTemplate<IGenericType, ITypeArgument, ICompiledType>(nameof(IGenericType.TypeArgumentList), nameof(ITypeArgument.ResolvedSourceType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IGenericType, string, IIdentifier>(nameof(IGenericType.ArgumentIdentifierTable)),
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
        public override bool CheckConsistency(IGenericType node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IGenericType node, object data)
        {
            node.ArgumentIdentifierTable.Seal();
        }
        #endregion
    }
}
