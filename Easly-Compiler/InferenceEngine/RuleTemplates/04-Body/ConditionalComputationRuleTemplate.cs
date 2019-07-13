namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public interface IConditionalComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConditional"/>.
    /// </summary>
    public class ConditionalComputationRuleTemplate : RuleTemplate<IConditional, ConditionalComputationRuleTemplate>, IConditionalComputationRuleTemplate
    {
        #region Init
        static ConditionalComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConditional, IResultException>(nameof(IConditional.BooleanExpression) + Dot + nameof(IExpression.ResolvedException)),
                new OnceReferenceSourceTemplate<IConditional, IResultException>(nameof(IConditional.Instructions) + Dot + nameof(IScope.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConditional, IResultException>(nameof(IConditional.ResolvedException)),
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
        public override bool CheckConsistency(IConditional node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression BooleanExpression = (IExpression)node.BooleanExpression;
            IScope Instructions = (IScope)node.Instructions;
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
                    ICompiledType ContractType = ResolvedResult.At(0).ValueType;

                    if (!IsValidType(ContractType, IsBooleanTypeAvailable, BooleanType) && !IsValidType(ContractType, IsEventTypeAvailable, EventType))
                    {
                        AddSourceError(new ErrorInvalidExpression(BooleanExpression));
                        Success = false;
                    }
                    else
                    {
                        IResultException ResolvedException = new ResultException();

                        ResultException.Merge(ResolvedException, BooleanExpression.ResolvedException.Item);
                        ResultException.Merge(ResolvedException, Instructions.ResolvedException.Item);

                        data = ResolvedException;
                    }
                }
            }

            return Success;
        }

        private bool IsValidType(ICompiledType contractType, bool isExisting, ICompiledType type)
        {
            return isExisting && ObjectType.TypeConformToBase(contractType, type, isConversionAllowed: true);
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConditional node, object data)
        {
            IScope Instructions = (IScope)node.Instructions;

            node.ResolvedException.Item = Instructions.ResolvedException.Item;
        }
        #endregion
    }
}
