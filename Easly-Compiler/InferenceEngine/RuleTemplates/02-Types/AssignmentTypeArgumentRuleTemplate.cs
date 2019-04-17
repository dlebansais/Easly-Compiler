namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAssignmentTypeArgument"/>.
    /// </summary>
    public interface IAssignmentTypeArgumentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentTypeArgument"/>.
    /// </summary>
    public class AssignmentTypeArgumentRuleTemplate : RuleTemplate<IAssignmentTypeArgument, AssignmentTypeArgumentRuleTemplate>, IAssignmentTypeArgumentRuleTemplate
    {
        #region Init
        static AssignmentTypeArgumentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAssignmentTypeArgument, ITypeName>(nameof(IAssignmentTypeArgument.Source) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IAssignmentTypeArgument, ICompiledType>(nameof(IAssignmentTypeArgument.Source) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentTypeArgument, ITypeName>(nameof(IAssignmentTypeArgument.ResolvedSourceTypeName)),
                new OnceReferenceDestinationTemplate<IAssignmentTypeArgument, ICompiledType>(nameof(IAssignmentTypeArgument.ResolvedSourceType)),
                new UnsealedTableDestinationTemplate<IAssignmentTypeArgument, string, IIdentifier>(nameof(IGenericType.ArgumentIdentifierTable), TemplateGenericTypeStart<IAssignmentTypeArgument>.Default),
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
        public override bool CheckConsistency(IAssignmentTypeArgument node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IIdentifier ParameterIdentifier = (IIdentifier)node.ParameterIdentifier;

            Debug.Assert(ParameterIdentifier.ValidText.IsAssigned);
            string ValidText = ParameterIdentifier.ValidText.Item;

            IGenericType ParentGenericType = (IGenericType)node.ParentSource;

            if (ParentGenericType.ArgumentIdentifierTable.ContainsKey(ValidText))
                AddSourceError(new ErrorIdentifierAlreadyListed(ParameterIdentifier, ValidText));

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssignmentTypeArgument node, object data)
        {
            IIdentifier ParameterIdentifier = (IIdentifier)node.ParameterIdentifier;

            Debug.Assert(ParameterIdentifier.ValidText.IsAssigned);
            string ValidText = ParameterIdentifier.ValidText.Item;

            IGenericType ParentGenericType = (IGenericType)node.ParentSource;
            IObjectType TypeToResolve = (IObjectType)node.Source;

            node.ResolvedSourceTypeName.Item = TypeToResolve.ResolvedTypeName.Item;
            node.ResolvedSourceType.Item = TypeToResolve.ResolvedType.Item;
            ParentGenericType.ArgumentIdentifierTable.Add(ValidText, ParameterIdentifier);
        }
        #endregion
    }
}
