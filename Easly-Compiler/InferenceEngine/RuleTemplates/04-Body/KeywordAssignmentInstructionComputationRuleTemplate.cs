namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IKeywordAssignmentInstruction"/>.
    /// </summary>
    public interface IKeywordAssignmentInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IKeywordAssignmentInstruction"/>.
    /// </summary>
    public class KeywordAssignmentInstructionComputationRuleTemplate : RuleTemplate<IKeywordAssignmentInstruction, KeywordAssignmentInstructionComputationRuleTemplate>, IKeywordAssignmentInstructionComputationRuleTemplate
    {
        #region Init
        static KeywordAssignmentInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IKeywordAssignmentInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateNodeStart<IKeywordAssignmentInstruction>.Default),
                new OnceReferenceTableSourceTemplate<IKeywordAssignmentInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateNodeStart<IKeywordAssignmentInstruction>.Default),
                new SealedTableSourceTemplate<IKeywordAssignmentInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IKeywordAssignmentInstruction>.Default),
                new OnceReferenceSourceTemplate<IKeywordAssignmentInstruction, IResultException>(nameof(IKeywordAssignmentInstruction.Source) + Dot + nameof(IExpression.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IKeywordAssignmentInstruction, IResultException>(nameof(IKeywordAssignmentInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IKeywordAssignmentInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IExpression SourceExpression = (IExpression)node.Source;
            IResultType SourceResult = SourceExpression.ResolvedResult.Item;

            if (SourceResult.Count != 1)
            {
                AddSourceError(new ErrorAssignmentMismatch(node));
                return false;
            }

            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            ICompiledType SourceType = SourceResult.At(0).ValueType;
            ICompiledType DestinationType;

            if (node.Destination == BaseNode.Keyword.Result)
            {
                if (node.EmbeddingFeature is IPropertyFeature AsPropertyFeature)
                    DestinationType = AsPropertyFeature.ResolvedEntityType.Item;
                else if (node.EmbeddingFeature is IIndexerFeature AsIndexerFeature)
                    DestinationType = AsIndexerFeature.ResolvedEntityType.Item;
                else
                {
                    AddSourceError(new ErrorInvalidAssignment(node));
                    return false;
                }
            }
            else if (node.Destination == BaseNode.Keyword.Retry)
            {
                if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType))
                {
                    AddSourceError(new ErrorBooleanTypeMissing(node));
                    return false;
                }

                DestinationType = BooleanType;
            }
            else
            {
                AddSourceError(new ErrorInvalidAssignment(node));
                return false;
            }

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);
            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

            if (!ObjectType.TypeConformToBase(SourceType, DestinationType, SubstitutionTypeTable, ErrorList, node))
            {
                AddSourceError(new ErrorAssignmentMismatch(node));
                return false;
            }

            data = SourceExpression.ResolvedException.Item;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IKeywordAssignmentInstruction node, object data)
        {
            IResultException ResolvedException = (IResultException)data;

            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
