namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public interface IQueryOverloadResultRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public class QueryOverloadResultRuleTemplate : RuleTemplate<IQueryOverload, QueryOverloadResultRuleTemplate>, IQueryOverloadResultRuleTemplate
    {
        #region Init
        static QueryOverloadResultRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IQueryOverload, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverload.ParameterList), nameof(IEntityDeclaration.ValidEntity)),
                new OnceReferenceCollectionSourceTemplate<IQueryOverload, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverload.ResultList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ParameterTable)),
                new UnsealedListDestinationTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ResultTable)),
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
        public override bool CheckConsistency(IQueryOverload node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryOverload node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IFeature EmbeddingFeature = node.EmbeddingFeature;

            ListTableEx<IParameter> ParameterTable = new ListTableEx<IParameter>();
            ListTableEx<IParameter> ResultTable = new ListTableEx<IParameter>();

            foreach (IEntityDeclaration Item in node.ParameterList)
            {
                IName SourceName = (IName)Item.EntityName;

                Debug.Assert(SourceName.ValidText.IsAssigned);
                string ValidName = SourceName.ValidText.Item;
                ParameterTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
            }

            // Ensured since the root is valid.
            Debug.Assert(node.ResultList.Count > 0);

            foreach (IEntityDeclaration Item in node.ResultList)
            {
                IName SourceName = (IName)Item.EntityName;

                Debug.Assert(SourceName.ValidText.IsAssigned);
                string ValidName = SourceName.ValidText.Item;
                ResultTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
            }

            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();
            node.ResultTable.AddRange(ResultTable);
            node.ResultTable.Seal();

            foreach (IParameter Parameter in ResultTable)
            {
                Debug.Assert(Parameter.ResolvedParameter.ResolvedFeatureType.IsAssigned);
                node.ConformantResultTable.Add(Parameter.ResolvedParameter.ResolvedFeatureType.Item);
            }

            Debug.Assert(ResultTable.Count > 0);

            ITypeName BestResultTypeName = null;
            ICompiledType BestResultType = null;
            foreach (IParameter Item in ResultTable)
            {
                Debug.Assert(Item.ResolvedParameter.ResolvedFeatureTypeName.IsAssigned);
                Debug.Assert(Item.ResolvedParameter.ResolvedFeatureType.IsAssigned);

                if (BestResultType == null || Item.Name == nameof(BaseNode.Keyword.Result))
                {
                    BestResultTypeName = Item.ResolvedParameter.ResolvedFeatureTypeName.Item;
                    BestResultType = Item.ResolvedParameter.ResolvedFeatureType.Item;
                }
            }

            node.ResolvedResultTypeName.Item = BestResultTypeName;
            node.ResolvedResultType.Item = BestResultType;

            IBody QueryBody = (IBody)node.QueryBody;
            IQueryOverloadType AssociatedType = new QueryOverloadType(node.ParameterList, node.ParameterEnd, node.ResultList, QueryBody.RequireList, QueryBody.EnsureList, QueryBody.ExceptionIdentifierList);
            AssociatedType.ParameterTable.AddRange(ParameterTable);
            AssociatedType.ParameterTable.Seal();
            AssociatedType.ResultTable.AddRange(ResultTable);
            AssociatedType.ResultTable.Seal();

            foreach (IParameter Parameter in ResultTable)
            {
                Debug.Assert(Parameter.ResolvedParameter.ResolvedFeatureType.IsAssigned);
                AssociatedType.ConformantResultTable.Add(Parameter.ResolvedParameter.ResolvedFeatureType.Item);
            }

            node.ResolvedAssociatedType.Item = AssociatedType;

            Debug.Assert(!node.ConformantResultTable.IsSealed);
            Debug.Assert(!AssociatedType.ConformantResultTable.IsSealed);
        }
        #endregion
    }
}
