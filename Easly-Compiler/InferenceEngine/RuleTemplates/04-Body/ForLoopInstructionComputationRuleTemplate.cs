namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public interface IForLoopInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public class ForLoopInstructionComputationRuleTemplate : RuleTemplate<IForLoopInstruction, ForLoopInstructionComputationRuleTemplate>, IForLoopInstructionComputationRuleTemplate
    {
        #region Init
        static ForLoopInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IForLoopInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IForLoopInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IResultException>(nameof(IForLoopInstruction.InitInstructionList), nameof(IInstruction.ResolvedException)),
                new OnceReferenceSourceTemplate<IForLoopInstruction, IResultException>(nameof(IForLoopInstruction.WhileCondition) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IResultException>(nameof(IForLoopInstruction.LoopInstructionList), nameof(IInstruction.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IInstruction, IResultException>(nameof(IForLoopInstruction.IterationInstructionList), nameof(IInstruction.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IAssertion, IResultException>(nameof(IForLoopInstruction.InvariantList), nameof(IAssertion.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IForLoopInstruction, IExpression, IResultException>(nameof(IForLoopInstruction.Variant), nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IForLoopInstruction, IResultException>(nameof(IForLoopInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IForLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression WhileCondition = (IExpression)node.WhileCondition;
            IResultType ResolvedResult = WhileCondition.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (ResolvedResult.Count > 1)
            {
                AddSourceError(new ErrorInvalidExpression(WhileCondition));
                Success = false;
            }
            else if (!IsForLoopTypeAvailable(node))
                Success = false;
            else
            {
                IResultException ResolvedException = new ResultException();

                foreach (IInstruction Item in node.InitInstructionList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                ResultException.Merge(ResolvedException, WhileCondition.ResolvedException.Item);

                foreach (IInstruction Item in node.LoopInstructionList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                foreach (IInstruction Item in node.IterationInstructionList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                foreach (Assertion Item in node.InvariantList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                if (node.Variant.IsAssigned)
                {
                    IExpression Variant = (IExpression)node.Variant.Item;
                    ResultException.Merge(ResolvedException, Variant.ResolvedException.Item);
                }

                data = ResolvedException;
            }

            return Success;
        }

        private bool IsForLoopTypeAvailable(IForLoopInstruction node)
        {
            bool IsBooleanTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
            bool IsNumberTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, node, out ITypeName NumberTypeName, out ICompiledType NumberType);

            if (node.Variant.IsAssigned && !IsNumberTypeAvailable)
            {
                AddSourceError(new ErrorNumberTypeMissing(node));
                return false;
            }

            if (!IsBooleanTypeAvailable)
            {
                AddSourceError(new ErrorBooleanTypeMissing(node));
                return false;
            }

            IExpression WhileCondition = (IExpression)node.WhileCondition;
            IResultType ResolvedResult = WhileCondition.ResolvedResult.Item;
            IExpressionType ContractType = ResolvedResult.At(0);

            if (ContractType.ValueType != BooleanType)
            {
                AddSourceError(new ErrorInvalidExpression(WhileCondition));
                return false;
            }

            bool Success = true;

            if (node.Variant.IsAssigned)
            {
                IExpression VariantCondition = (IExpression)node.Variant.Item;

                foreach (IExpressionType Item in VariantCondition.ResolvedResult.Item)
                    if (Item.ValueType != NumberType)
                    {
                        AddSourceError(new ErrorInvalidExpression(VariantCondition));
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
        public override void Apply(IForLoopInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
