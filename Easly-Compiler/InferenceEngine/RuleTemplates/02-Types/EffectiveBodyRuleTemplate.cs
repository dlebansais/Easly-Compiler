namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEffectiveBody"/>.
    /// </summary>
    /// <typeparam name="TBody">Specialized type of effective body.</typeparam>
    public interface IEffectiveBodyRuleTemplate<TBody> : IRuleTemplate
        where TBody : IEffectiveBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEffectiveBody"/>.
    /// </summary>
    /// <typeparam name="TBody">Specialized type of effective body.</typeparam>
    public class EffectiveBodyRuleTemplate<TBody> : RuleTemplate<TBody, EffectiveBodyRuleTemplate<TBody>>, IEffectiveBodyRuleTemplate<TBody>
        where TBody : IEffectiveBody
    {
        #region Init
        static EffectiveBodyRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<TBody, IEntityDeclaration, IScopeAttributeFeature>(nameof(IEffectiveBody.EntityDeclarationList), nameof(IEntityDeclaration.ValidEntity)),
                new SealedTableCollectionSourceTemplate<TBody, IInstruction, string, IScopeAttributeFeature>(nameof(IEffectiveBody.BodyInstructionList), nameof(IInstruction.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<TBody, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope)),
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
        public override bool CheckConsistency(TBody node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = new HashtableEx<string, IScopeAttributeFeature>();

            foreach (IEntityDeclaration Item in node.EntityDeclarationList)
            {
                IScopeAttributeFeature LocalEntity = Item.ValidEntity.Item;
                string ValidFeatureName = LocalEntity.ValidFeatureName.Item.Name;

                if (CheckedScope.ContainsKey(ValidFeatureName))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidFeatureName));
                    Success = false;
                }
                else
                    CheckedScope.Add(ValidFeatureName, LocalEntity);
            }

            IList<string> ConflictList = new List<string>();
            ScopeHolder.RecursiveCheck(CheckedScope, node.InnerScopes, ConflictList);

            foreach (IEntityDeclaration Item in node.EntityDeclarationList)
            {
                IScopeAttributeFeature LocalEntity = Item.ValidEntity.Item;
                string ValidFeatureName = LocalEntity.ValidFeatureName.Item.Name;

                if (ConflictList.Contains(ValidFeatureName))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidFeatureName));
                    Success = false;
                }
            }

            IList<IClass> AssignedSingleClassList = new List<IClass>();
            IList<IError> CheckErrorList = new List<IError>();
            if (ScopeHolder.HasConflictingSingleAttributes(CheckedScope, node.InnerScopes, AssignedSingleClassList, node, CheckErrorList))
            {
                AddSourceErrorList(CheckErrorList);
                Success = false;
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
        public override void Apply(TBody node, object data)
        {
            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = (IHashtableEx<string, IScopeAttributeFeature>)data;

            ((IScopeHolder)node).LocalScope.Merge(CheckedScope);
            ((IScopeHolder)node).LocalScope.Seal();
            node.FullScope.Merge(((IScopeHolder)node).LocalScope);

            ScopeHolder.RecursiveAdd(node.FullScope, node.InnerScopes);

            IList<IScopeHolder> EmbeddingScopeList = ScopeHolder.EmbeddingScope(node);
            EmbeddingScopeList.Add(node);
        }
        #endregion
    }
}
