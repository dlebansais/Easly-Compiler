namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public interface IGenericDefaultTypeRule : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public class GenericDefaultTypeRule : RuleTemplate<IGeneric, GenericDefaultTypeRule>, IGenericDefaultTypeRule
    {
        #region Init
        static GenericDefaultTypeRule()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ConditionallyAssignedReferenceSourceTemplate<IGeneric, IObjectType, ICompiledType>(nameof(IGeneric.DefaultValue), nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IGeneric, ICompiledType>(nameof(IGeneric.ResolvedDefaultType)),
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
        public override bool CheckConsistency(IGeneric node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(IGeneric node, object data)
        {
            if (node.DefaultValue.IsAssigned)
            {
                IObjectType TypeToResolve = (IObjectType)node.DefaultValue.Item;
                node.ResolvedDefaultType.Item = TypeToResolve.ResolvedType.Item;
            }
            else
                node.ResolvedDefaultType.Item = Class.ClassAny.ResolvedClassType.Item;
        }
        #endregion
    }
}
