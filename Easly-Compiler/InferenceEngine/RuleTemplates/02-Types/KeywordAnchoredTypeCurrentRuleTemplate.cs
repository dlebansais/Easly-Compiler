namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IKeywordAnchoredType"/>.
    /// </summary>
    public interface IKeywordAnchoredTypeCurrentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IKeywordAnchoredType"/>.
    /// </summary>
    public class KeywordAnchoredTypeCurrentRuleTemplate : RuleTemplate<IKeywordAnchoredType, KeywordAnchoredTypeCurrentRuleTemplate>, IKeywordAnchoredTypeCurrentRuleTemplate
    {
        #region Init
        static KeywordAnchoredTypeCurrentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IKeywordAnchoredType, ITypeName, IClassType>(nameof(IClass.ResolvedImportedClassTable), TemplateClassStart<IKeywordAnchoredType>.Default),
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, ITypeName>(nameof(IClass.ResolvedClassTypeName), TemplateClassStart<IKeywordAnchoredType>.Default),
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IKeywordAnchoredType>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ITypeName>(nameof(IKeywordAnchoredType.ResolvedCurrentTypeName)),
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ICompiledType>(nameof(IKeywordAnchoredType.ResolvedCurrentType)),
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

            if (node.Anchor == BaseNode.Keyword.Current)
            {
                IClass EmbeddingClass = node.EmbeddingClass;
                IErrorList CheckErrorList = new ErrorList();

                // 'Current' is always available.
                Success = KeywordExpression.IsKeywordAvailable(node, node.Anchor, CheckErrorList, out ITypeName ResultTypeName, out ICompiledType ResultType);
                Debug.Assert(Success);

                data = new Tuple<ITypeName, ICompiledType>(ResultTypeName, ResultType);
            }
            else
                data = new Tuple<ITypeName, ICompiledType>(Class.ClassAny.ResolvedClassTypeName.Item, Class.ClassAny.ResolvedClassType.Item);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IKeywordAnchoredType node, object data)
        {
            ITypeName ResultTypeName = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            ICompiledType ResultType = ((Tuple<ITypeName, ICompiledType>)data).Item2;

            node.ResolvedCurrentTypeName.Item = ResultTypeName;
            node.ResolvedCurrentType.Item = ResultType;

            if (node.Anchor == BaseNode.Keyword.Current)
            {
                node.ResolvedTypeName.Item = ResultTypeName;
                node.ResolvedType.Item = ResultType;

                if (!node.ResolvedOtherTypeName.IsAssigned && !node.ResolvedOtherType.IsAssigned)
                {
                    node.ResolvedOtherTypeName.Item = Class.ClassAny.ResolvedClassTypeName.Item;
                    node.ResolvedOtherType.Item = Class.ClassAny.ResolvedClassType.Item;
                }
            }
        }
        #endregion
    }
}
