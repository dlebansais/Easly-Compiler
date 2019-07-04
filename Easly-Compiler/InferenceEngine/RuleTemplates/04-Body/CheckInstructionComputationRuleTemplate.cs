namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ICheckInstruction"/>.
    /// </summary>
    public interface ICheckInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICheckInstruction"/>.
    /// </summary>
    public class CheckInstructionComputationRuleTemplate : RuleTemplate<ICheckInstruction, CheckInstructionComputationRuleTemplate>, ICheckInstructionComputationRuleTemplate
    {
        #region Init
        static CheckInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<ICheckInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<ICheckInstruction>.Default),
                new OnceReferenceSourceTemplate<ICheckInstruction, IResultException>(nameof(ICheckInstruction.BooleanExpression) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICheckInstruction, IResultException>(nameof(ICheckInstruction.ResolvedException)),
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
        public override bool CheckConsistency(ICheckInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression BooleanExpression = (IExpression)node.BooleanExpression;
            IResultType ResolvedResult = BooleanExpression.ResolvedResult.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (ResolvedResult.Count > 1)
            {
                AddSourceError(new ErrorInvalidExpression(BooleanExpression));
                Success = false;
            }
            else
            {
                bool IsBooleanTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
                bool IsEventTypeAvailable = Expression.IsLanguageTypeAvailable(LanguageClasses.Event.Guid, node, out ITypeName EventTypeName, out ICompiledType EventType);

                if (!IsBooleanTypeAvailable && !IsEventTypeAvailable)
                {
                    AddSourceError(new ErrorBooleanTypeMissing(node));
                    Success = false;
                }
                else
                {
                    IExpressionType ContractType = ResolvedResult.At(0);
                    if (ContractType.ValueType != BooleanType && ContractType.ValueType != EventType)
                    {
                        AddSourceError(new ErrorInvalidExpression(BooleanExpression));
                        Success = false;
                    }
                    else
                        data = BooleanExpression.ResolvedException.Item;
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICheckInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
