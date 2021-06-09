namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public interface IInspectInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInspectInstruction"/>.
    /// </summary>
    public class InspectInstructionComputationRuleTemplate : RuleTemplate<IInspectInstruction, InspectInstructionComputationRuleTemplate>, IInspectInstructionComputationRuleTemplate
    {
        #region Init
        static InspectInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IInspectInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IInspectInstruction>.Default),
                new OnceReferenceSourceTemplate<IInspectInstruction, IResultException>(nameof(IInspectInstruction.Source) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IInspectInstruction, IWith, IResultException>(nameof(IInspectInstruction.WithList), nameof(IWith.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IInspectInstruction, IScope, IResultException>(nameof(IInspectInstruction.ElseInstructions), nameof(IScope.ResolvedException))
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInspectInstruction, IResultException>(nameof(IInspectInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IInspectInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;

            IExpression Source = (IExpression)node.Source;
            IResultType ResolvedResult = Source.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (ResolvedResult.Count > 1)
            {
                AddSourceError(new ErrorInvalidExpression(Source));
                return false;
            }

            ICompiledType ValueType = ResolvedResult.At(0).ValueType;
            ISealableDictionary<IFeatureName, IDiscrete> EnforcedDiscreteTable = null;

            if (ValueType is IClassType AsClassType)
            {
                if (!CheckConsistencyClassType(node, AsClassType, ref EnforcedDiscreteTable))
                    return false;
            }
            else
            {
                AddSourceError(new ErrorInvalidExpression(Source));
                return false;
            }

            if (!CheckConsistencyRange(node, EnforcedDiscreteTable))
                return false;

            IResultException ResolvedException = new ResultException();

            ResultException.Merge(ResolvedException, Source.ResolvedException.Item);

            foreach (IWith Item in node.WithList)
                ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

            if (node.ElseInstructions.IsAssigned)
            {
                IScope ElseInstructions = (IScope)node.ElseInstructions.Item;
                ResultException.Merge(ResolvedException, ElseInstructions.ResolvedException.Item);
            }

            data = ResolvedException;

            return true;
        }

        private bool CheckConsistencyClassType(IInspectInstruction node, IClassType valueType, ref ISealableDictionary<IFeatureName, IDiscrete> enforcedDiscreteTable)
        {
            IExpression Source = (IExpression)node.Source;

            bool IsNumberAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);
            Debug.Assert(IsNumberAvailable);

            if (valueType != NumberType)
            {
                IClass BaseClass = valueType.BaseClass;
                if (BaseClass.IsEnumeration)
                    enforcedDiscreteTable = BaseClass.DiscreteTable;
                else
                {
                    AddSourceError(new ErrorInvalidExpression(Source));
                    return false;
                }
            }

            return true;
        }

        private bool CheckConsistencyRange(IInspectInstruction node, ISealableDictionary<IFeatureName, IDiscrete> enforcedDiscreteTable)
        {
            bool Success = true;

            IList<IConstantRange> CompleteRangeList = new List<IConstantRange>();
            foreach (IWith WithItem in node.WithList)
                foreach (IRange RangeItem in WithItem.RangeList)
                {
                    IConstantRange ResolvedRange = RangeItem.ResolvedRange.Item;
                    if (IsRangeCompatible(CompleteRangeList, ResolvedRange))
                    {
                        if (enforcedDiscreteTable != null && !IsRangeValidDiscrete(enforcedDiscreteTable, ResolvedRange))
                        {
                            AddSourceError(new ErrorInvalidRange(RangeItem));
                            Success = false;
                        }
                        else
                            CompleteRangeList.Add(ResolvedRange);
                    }
                    else
                    {
                        AddSourceError(new ErrorInvalidRange(RangeItem));
                        Success = false;
                    }
                }

            return Success;
        }

        private bool IsRangeCompatible(IList<IConstantRange> completeRangeList, IConstantRange range)
        {
            bool IsIntersecting = false;

            foreach (IConstantRange Item in completeRangeList)
                IsIntersecting |= Item.IsIntersecting(range);

            return !IsIntersecting;
        }

        private bool IsRangeValidDiscrete(ISealableDictionary<IFeatureName, IDiscrete> discreteTable, IConstantRange constantRange)
        {
            bool IsValidRange = false;

            if (constantRange.Minimum is IDiscreteLanguageConstant AsMinimum && AsMinimum.IsValueKnown && constantRange.Maximum is IDiscreteLanguageConstant AsMaximum && AsMaximum.IsValueKnown)
            {
                bool IsValidMinimum = false;
                bool IsValidMaximum = false;

                foreach (KeyValuePair<IFeatureName, IDiscrete> Entry in discreteTable)
                {
                    if (AsMinimum.Discrete == Entry.Value)
                        IsValidMinimum = true;
                    if (AsMaximum.Discrete == Entry.Value)
                        IsValidMaximum = true;
                }

                IsValidRange |= IsValidMinimum && IsValidMaximum;
            }

            return IsValidRange;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInspectInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
