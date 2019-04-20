namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public interface IForLoopInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IForLoopInstruction"/>.
    /// </summary>
    public class ForLoopInstructionRuleTemplate : RuleTemplate<IForLoopInstruction, ForLoopInstructionRuleTemplate>, IForLoopInstructionRuleTemplate
    {
        #region Init
        static ForLoopInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IForLoopInstruction, IEntityDeclaration, IScopeAttributeFeature>(nameof(IForLoopInstruction.EntityDeclarationList), nameof(IEntityDeclaration.ValidEntity)),
                new SealedTableCollectionSourceTemplate<IForLoopInstruction, IInstruction, string, IScopeAttributeFeature>(nameof(IForLoopInstruction.InitInstructionList), nameof(IContinuation.LocalScope)),
                new SealedTableCollectionSourceTemplate<IForLoopInstruction, IInstruction, string, IScopeAttributeFeature>(nameof(IForLoopInstruction.LoopInstructionList), nameof(IContinuation.LocalScope)),
                new SealedTableCollectionSourceTemplate<IForLoopInstruction, IInstruction, string, IScopeAttributeFeature>(nameof(IForLoopInstruction.IterationInstructionList), nameof(IContinuation.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IForLoopInstruction, string, IScopeAttributeFeature>(nameof(IForLoopInstruction.LocalScope)),
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
        public override bool CheckConsistency(IForLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
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

            if (Success)
                data = CheckedScope;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IForLoopInstruction node, object data)
        {
            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = (IHashtableEx<string, IScopeAttributeFeature>)data;

            node.LocalScope.Merge(CheckedScope);
            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullScope, node.InnerScopes);

            IList<IScopeHolder> EmbeddingScopeList = ScopeHolder.EmbeddingScope(node);
            EmbeddingScopeList.Add(node);
        }
        #endregion
    }
}
