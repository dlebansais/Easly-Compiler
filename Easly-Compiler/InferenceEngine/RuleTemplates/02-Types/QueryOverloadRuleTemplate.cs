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
                new OnceReferenceCollectionSourceTemplate<IQueryOverload, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverload.ParameterList), nameof(IEntityDeclaration.ValidEntity)),
                new OnceReferenceCollectionSourceTemplate<IQueryOverload, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverload.ResultList), nameof(IEntityDeclaration.ValidEntity)),
                new SealedTableSourceTemplate<IQueryOverload, string, IScopeAttributeFeature>(nameof(IQueryOverload.QueryBody) + Dot + nameof(IBody.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ParameterTable)),
                new UnsealedListDestinationTemplate<IQueryOverload, IParameter>(nameof(IQueryOverload.ResultTable)),
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

            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = new HashtableEx<string, IScopeAttributeFeature>();
            ListTableEx<IParameter> ParameterTable = new ListTableEx<IParameter>();
            ListTableEx<IParameter> ResultTable = new ListTableEx<IParameter>();

            foreach (IEntityDeclaration Item in node.ParameterList)
            {
                IName SourceName = (IName)Item.EntityName;

                Debug.Assert(SourceName.ValidText.IsAssigned);
                string ValidName = SourceName.ValidText.Item;

                if (CheckedScope.ContainsKey(ValidName))
                {
                    AddSourceError(new ErrorDuplicateName(SourceName, ValidName));
                    Success = false;
                }
                else
                {
                    CheckedScope.Add(ValidName, Item.ValidEntity.Item);
                    ParameterTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
                }
            }

            // Ensured since the root is valid.
            Debug.Assert(node.ResultList.Count > 0);

            foreach (IEntityDeclaration Item in node.ResultList)
            {
                IName SourceName = (IName)Item.EntityName;

                Debug.Assert(SourceName.ValidText.IsAssigned);
                string ValidName = SourceName.ValidText.Item;

                if (CheckedScope.ContainsKey(ValidName))
                {
                    AddSourceError(new ErrorDuplicateName(SourceName, ValidName));
                    Success = false;
                }
                else
                {
                    CheckedScope.Add(ValidName, Item.ValidEntity.Item);
                    ResultTable.Add(new Parameter(ValidName, Item.ValidEntity.Item));
                }
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
                data = new Tuple<IHashtableEx<string, IScopeAttributeFeature>, ListTableEx<IParameter>, ListTableEx<IParameter>>(CheckedScope, ParameterTable, ResultTable);

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

            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = ((Tuple<IHashtableEx<string, IScopeAttributeFeature>, ListTableEx<IParameter>, ListTableEx<IParameter>>)data).Item1;
            ListTableEx<IParameter> ParameterTable = ((Tuple<IHashtableEx<string, IScopeAttributeFeature>, ListTableEx<IParameter>, ListTableEx<IParameter>>)data).Item2;
            ListTableEx<IParameter> ResultTable = ((Tuple<IHashtableEx<string, IScopeAttributeFeature>, ListTableEx<IParameter>, ListTableEx<IParameter>>)data).Item3;

            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();
            node.ResultTable.AddRange(ResultTable);
            node.ResultTable.Seal();

            IBody QueryBody = (IBody)node.QueryBody;
            IQueryOverloadType AssociatedType = new QueryOverloadType(node.ParameterList, node.ParameterEnd, node.ResultList, QueryBody.RequireList, QueryBody.EnsureList, QueryBody.ExceptionIdentifierList);
            AssociatedType.ParameterTable.AddRange(ParameterTable);
            AssociatedType.ParameterTable.Seal();
            AssociatedType.ResultTable.AddRange(ResultTable);
            AssociatedType.ResultTable.Seal();

            node.ResolvedAssociatedType.Item = AssociatedType;

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
