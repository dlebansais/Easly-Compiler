namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public interface IOverLoopInstructionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IOverLoopInstruction"/>.
    /// </summary>
    public class OverLoopInstructionRuleTemplate : RuleTemplate<IOverLoopInstruction, OverLoopInstructionRuleTemplate>, IOverLoopInstructionRuleTemplate
    {
        #region Init
        static OverLoopInstructionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IOverLoopInstruction, string, IScopeAttributeFeature>(nameof(IOverLoopInstruction.LoopInstructions) + Dot + nameof(IScope.LocalScope)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IOverLoopInstruction, string, IScopeAttributeFeature>(nameof(IOverLoopInstruction.LocalScope)),
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
        public override bool CheckConsistency(IOverLoopInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = new HashtableEx<string, IScopeAttributeFeature>();
            IClass EmbeddingClass = node.EmbeddingClass;

            foreach (IName Item in node.IndexerList)
            {
                Debug.Assert(Item.ValidText.IsAssigned);
                string ValidText = Item.ValidText.Item;

                if (CheckedScope.ContainsKey(ValidText))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item, ValidText));
                    Success = false;
                }
                else
                {
                    IScopeAttributeFeature NewEntity = ScopeAttributeFeature.Create(Item, ValidText);
                    CheckedScope.Add(ValidText, NewEntity);
                }
            }

            IList<string> ConflictList = new List<string>();
            ScopeHolder.RecursiveCheck(CheckedScope, node.InnerScopes, ConflictList);

            foreach (KeyValuePair<string, IScopeAttributeFeature> Item in CheckedScope)
                if (ConflictList.Contains(Item.Key))
                {
                    AddSourceError(new ErrorVariableAlreadyDefined(Item.Value.Location, Item.Key));
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
        public override void Apply(IOverLoopInstruction node, object data)
        {
            IHashtableEx<string, IScopeAttributeFeature> CheckedScope = (IHashtableEx<string, IScopeAttributeFeature>)data;
            node.InnerLoopScope.Merge(CheckedScope);
            node.InnerLoopScope.Seal();

            node.LocalScope.Seal();

            ScopeHolder.RecursiveAdd(CheckedScope, node.InnerScopes);

            IList<IScopeHolder> EmbeddingScopeList = ScopeHolder.EmbeddingScope(node);
            EmbeddingScopeList.Add(node);
        }
        #endregion
    }
}
