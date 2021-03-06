﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAssignmentInstruction"/>.
    /// </summary>
    public interface IAssignmentInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssignmentInstruction"/>.
    /// </summary>
    public class AssignmentInstructionComputationRuleTemplate : RuleTemplate<IAssignmentInstruction, AssignmentInstructionComputationRuleTemplate>, IAssignmentInstructionComputationRuleTemplate
    {
        #region Init
        static AssignmentInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IAssignmentInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveTypeName), TemplateNodeStart<IAssignmentInstruction>.Default),
                new OnceReferenceTableSourceTemplate<IAssignmentInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedEffectiveType), TemplateNodeStart<IAssignmentInstruction>.Default),
                new SealedTableSourceTemplate<IAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IAssignmentInstruction>.Default),
                new OnceReferenceSourceTemplate<IAssignmentInstruction, IResultException>(nameof(IAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssignmentInstruction, IResultException>(nameof(IAssignmentInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            // This list has been verified during the node tree check.
            Debug.Assert(node.DestinationList.Count > 0);

            IExpression SourceExpression = (IExpression)node.Source;
            IResultType SourceResult = SourceExpression.ResolvedResult.Item;

            if (node.DestinationList.Count > SourceResult.Count)
            {
                AddSourceError(new ErrorAssignmentMismatch(node));
                Success = false;
            }
            else
            {
                IClass EmbeddingClass = node.EmbeddingClass;
                IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
                ISealableDictionary<IQualifiedName, ICompiledFeature> FinalFeatureTable = new SealableDictionary<IQualifiedName, ICompiledFeature>();

                for (int i = 0; i < node.DestinationList.Count; i++)
                {
                    IQualifiedName Destination = (QualifiedName)node.DestinationList[i];
                    IList<IIdentifier> ValidPath = Destination.ValidPath.Item;
                    ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

                    if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                        Success = false;
                    else
                    {
                        Debug.Assert(FinalFeature != null);
                        FinalType = FinalFeature.ResolvedEffectiveType.Item;

                        ICompiledType SourceType = SourceResult.At(i).ValueType;

                        if (!ObjectType.TypeConformToBase(SourceType, FinalType, isConversionAllowed: true))
                        {
                            AddSourceError(new ErrorAssignmentMismatch(Destination));
                            Success = false;
                        }
                        else
                            ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Destination.ValidResultTypePath.Item);
                    }

                    FinalFeatureTable.Add(Destination, FinalFeature);
                }

                if (Success)
                    data = new Tuple<IResultException, ISealableDictionary<IQualifiedName, ICompiledFeature>>(SourceExpression.ResolvedException.Item, FinalFeatureTable);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssignmentInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ISealableDictionary<IQualifiedName, ICompiledFeature>>)data).Item1;
            ISealableDictionary<IQualifiedName, ICompiledFeature> FinalFeatureTable = ((Tuple<IResultException, ISealableDictionary<IQualifiedName, ICompiledFeature>>)data).Item2;

            node.ResolvedException.Item = ResolvedException;
            node.FinalFeatureTable.Merge(FinalFeatureTable);
            node.FinalFeatureTable.Seal();
        }
        #endregion
    }
}
