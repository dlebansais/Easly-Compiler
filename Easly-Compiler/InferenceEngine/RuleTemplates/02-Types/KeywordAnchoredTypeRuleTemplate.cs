﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

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
                new SealedTableSourceTemplate<IKeywordAnchoredType, ITypeName, IClassType>(nameof(IClass.ResolvedImportedClassTable), TemplateClassStart<IKeywordAnchoredType>.Default),
                new OnceReferenceResultSourceTemplate<IKeywordAnchoredType, ITypeName>(nameof(INodeWithResult.ResolvedResultTypeName)),
                new OnceReferenceResultSourceTemplate<IKeywordAnchoredType, ICompiledType>(nameof(INodeWithResult.ResolvedResultType)),
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, ITypeName>(nameof(IClass.ResolvedClassTypeName), TemplateClassStart<IKeywordAnchoredType>.Default),
                new OnceReferenceSourceTemplate<IKeywordAnchoredType, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IKeywordAnchoredType>.Default),
                new ConditionallyAssignedReferenceSourceTemplate<IKeywordAnchoredType, IIndexerFeature, ICompiledType>(nameof(IClass.ClassIndexer), nameof(IIndexerFeature.ResolvedAgentType), TemplateClassStart<IKeywordAnchoredType>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ITypeName>(nameof(IKeywordAnchoredType.ResolvedOtherTypeName)),
                new OnceReferenceDestinationTemplate<IKeywordAnchoredType, ICompiledType>(nameof(IKeywordAnchoredType.ResolvedOtherType)),
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

            // The 'Current' case is handled in KeywordAnchoredTypeCurrentRuleTemplate.
            Debug.Assert(node.Anchor != BaseNode.Keyword.Current);

            IClass EmbeddingClass = node.EmbeddingClass;
            IErrorList CheckErrorList = new ErrorList();
            if (!KeywordExpression.IsKeywordAvailable(node.Anchor, node, CheckErrorList, out ITypeName ResultTypeName, out ICompiledType ResultType))
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
            ITypeName ResultTypeName = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            ICompiledType ResultType = ((Tuple<ITypeName, ICompiledType>)data).Item2;

            node.ResolvedOtherTypeName.Item = ResultTypeName;
            node.ResolvedOtherType.Item = ResultType;

            if (node.Anchor != BaseNode.Keyword.Current)
            {
                node.ResolvedTypeName.Item = ResultTypeName;
                node.ResolvedType.Item = ResultType;

                if (!node.ResolvedCurrentTypeName.IsAssigned && !node.ResolvedCurrentType.IsAssigned)
                {
                    node.ResolvedCurrentTypeName.Item = Class.ClassAny.ResolvedClassTypeName.Item;
                    node.ResolvedCurrentType.Item = Class.ClassAny.ResolvedClassType.Item;
                }
            }
        }
        #endregion
    }
}
