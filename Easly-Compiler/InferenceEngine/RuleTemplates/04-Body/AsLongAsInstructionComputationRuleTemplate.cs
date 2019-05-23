namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public interface IAsLongAsInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAsLongAsInstruction"/>.
    /// </summary>
    public class AsLongAsInstructionComputationRuleTemplate : RuleTemplate<IAsLongAsInstruction, AsLongAsInstructionComputationRuleTemplate>, IAsLongAsInstructionComputationRuleTemplate
    {
        #region Init
        static AsLongAsInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAsLongAsInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IAsLongAsInstruction>.Default),
                new OnceReferenceSourceTemplate<IAsLongAsInstruction, IResultException>(nameof(IAsLongAsInstruction.ContinueCondition) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceCollectionSourceTemplate<IAsLongAsInstruction, IContinuation, IResultException>(nameof(IAsLongAsInstruction.ContinuationList), nameof(IContinuation.ResolvedException)),
                new ConditionallyAssignedReferenceSourceTemplate<IAsLongAsInstruction, IScope, IResultException>(nameof(IAsLongAsInstruction.ElseInstructions), nameof(IScope.ResolvedException))
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAsLongAsInstruction, IResultException>(nameof(IAsLongAsInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IAsLongAsInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression ContinueCondition = (IExpression)node.ContinueCondition;
            IResultType ResolvedResult = ContinueCondition.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (ResolvedResult.Count > 1)
            {
                AddSourceError(new ErrorInvalidExpression(ContinueCondition));
                Success = false;
            }
            else if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType))
            {
                AddSourceError(new ErrorBooleanTypeMissing(node));
                Success = false;
            }
            else
            {
                IExpressionType ContractType = ResolvedResult.At(0);

                if (ContractType.ValueType != BooleanType)
                {
                    AddSourceError(new ErrorInvalidExpression(ContinueCondition));
                    Success = false;
                }
                else
                {
                    IResultException ResolvedException = new ResultException();

                    ResultException.Merge(ResolvedException, ContinueCondition.ResolvedException);

                    foreach (IContinuation Item in node.ContinuationList)
                        ResultException.Merge(ResolvedException, Item.ResolvedException);

                    if (node.ElseInstructions.IsAssigned)
                    {
                        IScope ElseInstructions = (IScope)node.ElseInstructions.Item;
                        ResultException.Merge(ResolvedException, ElseInstructions.ResolvedException);
                    }

                    data = ResolvedException;
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAsLongAsInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
