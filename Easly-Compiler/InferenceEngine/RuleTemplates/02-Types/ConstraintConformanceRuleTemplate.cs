namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public interface IConstraintConformanceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public class ConstraintConformanceRuleTemplate : RuleTemplate<IConstraint, ConstraintConformanceRuleTemplate>, IConstraintConformanceRuleTemplate
    {
        #region Init
        static ConstraintConformanceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConstraint, ITypeName>(nameof(IGeneric.ResolvedGenericTypeName), TemplateGenericStart<IConstraint>.Default),
                new OnceReferenceSourceTemplate<IConstraint, IFormalGenericType>(nameof(IGeneric.ResolvedGenericType), TemplateGenericStart<IConstraint>.Default),
                new OnceReferenceSourceTemplate<IConstraint, ITypeName>(nameof(IConstraint.ResolvedParentTypeName)),
                new OnceReferenceSourceTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ResolvedParentType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConstraint, ITypeName>(nameof(IConstraint.ResolvedConformingTypeName)),
                new OnceReferenceDestinationTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ResolvedConformingType)),
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
        public override bool CheckConsistency(IConstraint node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IGeneric InnerGeneric = (IGeneric)node.ParentSource;

            Debug.Assert(node.ResolvedParentType.IsAssigned);
            ICompiledType ResolvedType = node.ResolvedParentType.Item;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in InnerGeneric.ResolvedConformanceTable)
            {
                ICompiledType OtherConformingType = Entry.Value;
                if (ObjectType.TypesHaveIdenticalSignature(ResolvedType, OtherConformingType))
                {
                    AddSourceError(new ErrorTypeAlreadyUsedAsConstraint((IObjectType)node.ParentType));
                    Success = false;
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConstraint node, object data)
        {
            IGeneric InnerGeneric = (IGeneric)node.ParentSource;

            Debug.Assert(node.ResolvedParentTypeName.IsAssigned);
            ITypeName ResolvedTypeName = node.ResolvedParentTypeName.Item;
            Debug.Assert(node.ResolvedParentType.IsAssigned);
            ICompiledType ResolvedType = node.ResolvedParentType.Item;

            InnerGeneric.ResolvedConformanceTable.Add(ResolvedTypeName, ResolvedType);
            InnerGeneric.ResolvedGenericType.Item.ConformanceTable.Add(ResolvedTypeName, ResolvedType);
            node.ResolvedConformingTypeName.Item = ResolvedTypeName;
            node.ResolvedConformingType.Item = ResolvedType;
        }
        #endregion
    }
}
