namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ICommandOverload"/>.
    /// </summary>
    public interface ICommandOverloadRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICommandOverload"/>.
    /// </summary>
    public class CommandOverloadRuleTemplate : RuleTemplate<ICommandOverload, CommandOverloadRuleTemplate>, ICommandOverloadRuleTemplate
    {
        #region Init
        static CommandOverloadRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<ICommandOverload, IEntityDeclaration, IScopeAttributeFeature>(nameof(ICommandOverload.ParameterList), nameof(IEntityDeclaration.ValidEntity)),
                new SealedTableSourceTemplate<ICommandOverload, string, IScopeAttributeFeature>(nameof(ICommandOverload.CommandBody) + Dot + nameof(IBody.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<ICommandOverload, IParameter>(nameof(ICommandOverload.ParameterTable)),
                new UnsealedTableDestinationTemplate<ICommandOverload, string, IScopeAttributeFeature>(nameof(ICommandOverload.LocalScope)),
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
        public override bool CheckConsistency(ICommandOverload node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = new SealableDictionary<string, IScopeAttributeFeature>();
            SealableList<IParameter> ParameterTable = new SealableList<IParameter>();

            foreach (EntityDeclaration Item in node.ParameterList)
            {
                IName SourceName = (IName)Item.EntityName;
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

            if (Success)
                data = new Tuple<ISealableDictionary<string, IScopeAttributeFeature>, SealableList<IParameter>>(CheckedScope, ParameterTable);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICommandOverload node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = ((Tuple<ISealableDictionary<string, IScopeAttributeFeature>, SealableList<IParameter>>)data).Item1;
            SealableList<IParameter> ParameterTable = ((Tuple<ISealableDictionary<string, IScopeAttributeFeature>, SealableList<IParameter>>)data).Item2;

            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();

            IBody CommandBody = (IBody)node.CommandBody;
            ICommandOverloadType AssociatedType = new CommandOverloadType(node.ParameterList, BaseNode.ParameterEndStatus.Closed, CommandBody.RequireList, CommandBody.EnsureList, CommandBody.ExceptionIdentifierList);
            AssociatedType.ParameterTable.AddRange(ParameterTable);
            AssociatedType.ParameterTable.Seal();

            node.ResolvedAssociatedType.Item = AssociatedType;

            node.LocalScope.Merge(CheckedScope);
            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullScope, node.InnerScopes);

            EmbeddingClass.BodyList.Add((IBody)node.CommandBody);
            EmbeddingClass.CommandOverloadList.Add(node);
        }
        #endregion
    }
}
