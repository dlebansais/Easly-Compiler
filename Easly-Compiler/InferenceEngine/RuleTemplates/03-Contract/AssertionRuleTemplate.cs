namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssertion"/>.
    /// </summary>
    public interface IAssertionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertion"/>.
    /// </summary>
    public class AssertionRuleTemplate : RuleTemplate<IAssertion, AssertionRuleTemplate>, IAssertionRuleTemplate
    {
        #region Init
        static AssertionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IAssertion, IList<IExpressionType>>(nameof(IAssertion.BooleanExpression) + Dot + nameof(IExpression.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssertion, ITaggedContract>(nameof(IAssertion.ResolvedContract)),
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
        public override bool CheckConsistency(IAssertion node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IExpression BooleanExpression = (IExpression)node.BooleanExpression;

            Debug.Assert(BooleanExpression.ResolvedResult.IsAssigned);
            IList<IExpressionType> ResolvedResult = BooleanExpression.ResolvedResult.Item;

            if (ResolvedResult.Count != 1)
            {
                AddSourceError(new ErrorInvalidExpression(BooleanExpression));
                Success = false;
            }
            else if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType))
            {
                AddSourceError(new ErrorBooleanTypeMissing(node));
                Success = false;
            }
            else if (ResolvedResult[0].ValueType != BooleanType)
            {
                AddSourceError(new ErrorInvalidExpression(BooleanExpression));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssertion node, object data)
        {
            IExpression BooleanExpression = (IExpression)node.BooleanExpression;

            ITaggedContract NewContract;
            if (node.Tag.IsAssigned)
                NewContract = new TaggedContract(BooleanExpression, node.Tag.Item.Text);
            else
                NewContract = new TaggedContract(BooleanExpression);

            node.ResolvedContract.Item = NewContract;
        }
        #endregion
    }
}
