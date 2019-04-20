namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public interface IConstraintParentTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public class ConstraintParentTypeRuleTemplate : RuleTemplate<IConstraint, ConstraintParentTypeRuleTemplate>, IConstraintParentTypeRuleTemplate
    {
        #region Init
        static ConstraintParentTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConstraint, ITypeName>(nameof(IConstraint.ParentType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ParentType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConstraint, ITypeName>(nameof(IConstraint.ResolvedParentTypeName)),
                new OnceReferenceDestinationTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ResolvedParentType)),
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

            IObjectType ParentType = (IObjectType)node.ParentType;
            ITypeName ResolvedTypeName = ParentType.ResolvedTypeName.Item;
            ICompiledType ResolvedType = ParentType.ResolvedType.Item;

            if (ResolvedType is IAnchoredType AsAnchoredType)
            {
                AddSourceError(new ErrorInvalidAnchoredType(AsAnchoredType));
                Success = false;
            }
            else if (ResolvedType is IKeywordAnchoredType AsKeywordAnchoredType)
            {
                AddSourceError(new ErrorInvalidAnchoredType(AsKeywordAnchoredType));
                Success = false;
            }
            else
                data = new Tuple<ITypeName, ICompiledType>(ResolvedTypeName, ResolvedType);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConstraint node, object data)
        {
            node.ResolvedParentTypeName.Item = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            node.ResolvedParentType.Item = ((Tuple<ITypeName, ICompiledType>)data).Item2;
        }
        #endregion
    }
}
