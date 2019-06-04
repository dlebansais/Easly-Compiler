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
    public interface IQueryOverloadRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverload"/>.
    /// </summary>
    public class QueryOverloadRuleTemplate : RuleTemplate<IQueryOverload, QueryOverloadRuleTemplate>, IQueryOverloadRuleTemplate
    {
        #region Init
        static QueryOverloadRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ParameterTable)),
                new SealedListSourceTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ResultTable)),
                new SealedTableSourceTemplate<IQueryOverload, string, IScopeAttributeFeature>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IQueryOverload, string, IScopeAttributeFeature>(nameof(IQueryOverload.LocalScope)),
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

            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = new SealableDictionary<string, IScopeAttributeFeature>();

            Debug.Assert(node.ParameterTable.Count == node.ParameterList.Count);
            for (int i = 0; i < node.ParameterTable.Count; i++)
            {
                IParameter Parameter = node.ParameterTable[i];
                IEntityDeclaration EntityDeclaration = node.ParameterList[i];

                if (CheckedScope.ContainsKey(Parameter.Name))
                {
                    AddSourceError(new ErrorDuplicateName(EntityDeclaration, Parameter.Name));
                    Success = false;
                }
                else
                    CheckedScope.Add(Parameter.Name, EntityDeclaration.ValidEntity.Item);
            }

            Debug.Assert(node.ResultTable.Count == node.ResultList.Count);
            for (int i = 0; i < node.ResultTable.Count; i++)
            {
                IParameter Result = node.ResultTable[i];
                IEntityDeclaration EntityDeclaration = node.ResultList[i];

                if (CheckedScope.ContainsKey(Result.Name))
                {
                    AddSourceError(new ErrorDuplicateName(EntityDeclaration, Result.Name));
                    Success = false;
                }
                else
                    CheckedScope.Add(Result.Name, EntityDeclaration.ValidEntity.Item);
            }

            IList<string> ConflictList = new List<string>();
            ScopeHolder.RecursiveCheck(CheckedScope, node.InnerScopes, ConflictList);

            foreach (IEntityDeclaration Item in node.ParameterList)
            {
                IScopeAttributeFeature LocalEntity = Item.ValidEntity.Item;
                string ValidFeatureName = LocalEntity.ValidFeatureName.Item.Name;

                if (ConflictList.Contains(ValidFeatureName))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidFeatureName));
                    Success = false;
                }
            }

            foreach (IEntityDeclaration Item in node.ResultList)
            {
                IScopeAttributeFeature LocalEntity = Item.ValidEntity.Item;
                string ValidFeatureName = LocalEntity.ValidFeatureName.Item.Name;

                if (ConflictList.Contains(ValidFeatureName))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidFeatureName));
                    Success = false;
                }
            }

            if (Success)
                data = CheckedScope;

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

            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = (ISealableDictionary<string, IScopeAttributeFeature>)data;

            node.LocalScope.Merge(CheckedScope);
            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullScope, node.InnerScopes);

            EmbeddingClass.BodyList.Add((IBody)node.QueryBody);
            EmbeddingClass.QueryOverloadList.Add(node);
        }
        #endregion
    }
}
