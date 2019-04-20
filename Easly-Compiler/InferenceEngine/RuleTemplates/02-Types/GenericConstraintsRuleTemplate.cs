namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public interface IGenericConstraintsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public class GenericConstraintsRuleTemplate : RuleTemplate<IGeneric, GenericConstraintsRuleTemplate>, IGenericConstraintsRuleTemplate
    {
        #region Init
        static GenericConstraintsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IGeneric, ITypeName>(nameof(IGeneric.ResolvedGenericTypeName)),
                new OnceReferenceSourceTemplate<IGeneric, IFormalGenericType>(nameof(IGeneric.ResolvedGenericType)),
                new OnceReferenceSourceTemplate<IGeneric, ICompiledType>(nameof(IGeneric.ResolvedDefaultType)),
                new OnceReferenceCollectionSourceTemplate<IGeneric, IConstraint, ICompiledType>(nameof(IGeneric.ConstraintList), nameof(IConstraint.ResolvedTypeWithRename)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IGeneric, ITypeName, ICompiledType>(nameof(IGeneric.ResolvedConformanceTable)),
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

            BaseNode.CopySemantic CopyConstraint = BaseNode.CopySemantic.Any;

            if (node.DefaultValue.IsAssigned)
            {
                if (node.ResolvedDefaultType.Item.IsReference && CopyConstraint == BaseNode.CopySemantic.Value)
                {
                    AddSourceError(new ErrorReferenceValueConstraintConformance(node, node.ResolvedDefaultType.Item, CopyConstraint));
                    Success = false;
                }

                else if (node.ResolvedDefaultType.Item.IsValue && CopyConstraint == BaseNode.CopySemantic.Reference)
                {
                    AddSourceError(new ErrorReferenceValueConstraintConformance(node, node.ResolvedDefaultType.Item, CopyConstraint));
                    Success = false;
                }
            }

            if (Success)
            {
                BaseNode.CopySemantic InducedConstraint = CopyConstraint;

                foreach (IConstraint ConstraintItem in node.ConstraintList)
                {
                    ITypeName BaseTypeName = ConstraintItem.ResolvedParentTypeName.Item;
                    ICompiledType BaseType = ConstraintItem.ResolvedParentType.Item;
                    IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
                    IList<IError> ConstraintErrorList = new List<IError>();

                    if (node.DefaultValue.IsAssigned && !ObjectType.TypeConformToBase(node.ResolvedDefaultType.Item, BaseType, SubstitutionTypeTable, ConstraintErrorList, (IObjectType)node.DefaultValue.Item, true))
                        AddSourceErrorList(ConstraintErrorList);

                    if (InducedConstraint == BaseNode.CopySemantic.Reference)
                    {
                        if (BaseType.IsValue)
                        {
                            AddSourceError(new ErrorReferenceValueConstraintConformance(node, node.ResolvedDefaultType.Item, InducedConstraint));
                            Success = false;
                            break;
                        }
                    }

                    else if (InducedConstraint == BaseNode.CopySemantic.Value)
                    {
                        if (BaseType.IsReference)
                        {
                            AddSourceError(new ErrorReferenceValueConstraintConformance(node, node.ResolvedDefaultType.Item, InducedConstraint));
                            Success = false;
                            break;
                        }
                    }

                    else
                    {
                        if (BaseType.IsReference)
                            InducedConstraint = BaseNode.CopySemantic.Reference;

                        else if (BaseType.IsValue)
                            InducedConstraint = BaseNode.CopySemantic.Value;
                    }
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IGeneric node, object data)
        {
            node.ResolvedConformanceTable.Seal();
        }
        #endregion
    }
}
