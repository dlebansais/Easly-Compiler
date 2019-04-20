namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IKeywordAnchoredType"/>.
    /// </summary>
    public interface IKeywordAnchoredTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IKeywordAnchoredType"/>.
    /// </summary>
    public class KeywordAnchoredTypeRuleTemplate : RuleTemplate<IKeywordAnchoredType, KeywordAnchoredTypeRuleTemplate>, IKeywordAnchoredTypeRuleTemplate
    {
        #region Init
        static KeywordAnchoredTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, ITypeName>(nameof(IClass.ResolvedClassTypeName), TemplateClassStart<IKeywordAnchoredType>.Default),
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IKeywordAnchoredType>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ITypeName>(nameof(IKeywordAnchoredType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ICompiledType>(nameof(IKeywordAnchoredType.ResolvedType)),
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
        public override bool CheckConsistency(IKeywordAnchoredType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IList<IError> CheckErrorList = new List<IError>();
            if (!KeywordExpression.IsKeywordAvailable(node, node.Anchor, CheckErrorList, out ITypeName ResultTypeName, out ICompiledType ResultType))
            {
                AddSourceErrorList(CheckErrorList);
                Success = false;
            }
            else
                data = new Tuple<ITypeName, ICompiledType>(ResultTypeName, ResultType);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IKeywordAnchoredType node, object data)
        {
            node.ResolvedTypeName.Item = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            node.ResolvedType.Item = ((Tuple<ITypeName, ICompiledType>)data).Item2;
        }
        #endregion
    }
}
