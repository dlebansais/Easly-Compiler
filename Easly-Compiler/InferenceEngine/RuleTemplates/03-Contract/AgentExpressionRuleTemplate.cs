﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IAgentExpression"/>.
    /// </summary>
    public interface IAgentExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAgentExpression"/>.
    /// </summary>
    public class AgentExpressionRuleTemplate : RuleTemplate<IAgentExpression, AgentExpressionRuleTemplate>, IAgentExpressionRuleTemplate
    {
        #region Init
        static AgentExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAgentExpression, IList<IExpressionType>>(nameof(IAgentExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IAgentExpression, ICompiledType>(nameof(IAgentExpression.ResolvedAncestorType)),
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
        public override bool CheckConsistency(IAgentExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= AgentExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ICompiledFeature ResolvedFeature, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedException);

            if (Success)
                data = new Tuple<ICompiledFeature, IList<IExpressionType>, IList<IIdentifier>>(ResolvedFeature, ResolvedResult, ResolvedException);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IAgentExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedFeature">The feature found upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IAgentExpression node, IErrorList errorList, out ICompiledFeature resolvedFeature, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resolvedFeature = null;
            resolvedResult = null;
            resolvedExceptions = null;

            IIdentifier Delegated = (IIdentifier)node.Delegated;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            Debug.Assert(Delegated.ValidText.IsAssigned);
            string ValidText = Delegated.ValidText.Item;

            if (!FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Value))
            {
                errorList.AddError(new ErrorUnknownIdentifier(node, ValidText));
                return false;
            }

            Debug.Assert(Value.Feature.IsAssigned);
            resolvedFeature = Value.Feature.Item;
            resolvedExceptions = new List<IIdentifier>();
            resolvedResult = new List<IExpressionType>
            {
                new ExpressionType(resolvedFeature.ResolvedFeatureTypeName.Item, resolvedFeature.ResolvedFeatureType.Item, string.Empty)
            };

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAgentExpression node, object data)
        {
            ICompiledFeature ResolvedFeature = ((Tuple<ICompiledFeature, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IList<IExpressionType> ResolvedResult = ((Tuple<ICompiledFeature, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ICompiledFeature, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedAncestorTypeName.Item = ResolvedFeature.ResolvedFeatureTypeName.Item;
            node.ResolvedAncestorType.Item = ResolvedFeature.ResolvedFeatureType.Item;
            node.ResolvedFeature.Item = ResolvedFeature;
        }
        #endregion
    }
}