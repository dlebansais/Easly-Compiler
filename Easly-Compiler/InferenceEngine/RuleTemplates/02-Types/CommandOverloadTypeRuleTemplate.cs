namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ICommandOverloadType"/>.
    /// </summary>
    public interface ICommandOverloadTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICommandOverloadType"/>.
    /// </summary>
    public class CommandOverloadTypeRuleTemplate : RuleTemplate<ICommandOverloadType, CommandOverloadTypeRuleTemplate>, ICommandOverloadTypeRuleTemplate
    {
        #region Init
        static CommandOverloadTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<ICommandOverloadType, IEntityDeclaration, IScopeAttributeFeature>(nameof(ICommandOverloadType.ParameterList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<ICommandOverloadType, IParameter>(nameof(ICommandOverloadType.ParameterTable)),
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
        public override bool CheckConsistency(ICommandOverloadType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            ListTableEx<IParameter> ParameterTable = new ListTableEx<IParameter>();

            foreach (IEntityDeclaration Item in node.ParameterList)
            {
                IName ParameterName = (IName)Item.EntityName;

                Debug.Assert(ParameterName.ValidText.IsAssigned);
                string ValidText = ParameterName.ValidText.Item;

                Debug.Assert(Item.ValidEntity.IsAssigned);
                IScopeAttributeFeature ParameterFeature = Item.ValidEntity.Item;

                if (Parameter.TableContainsName(ParameterTable, ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(ParameterName, ValidText));
                    Success = false;
                }
                else
                    ParameterTable.Add(new Parameter(ValidText, ParameterFeature));
            }

            if (Success)
                data = ParameterTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICommandOverloadType node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ListTableEx<IParameter> ParameterTable = (ListTableEx<IParameter>)data;

            Debug.Assert(node.ParameterTable.Count == 0);
            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();
        }
        #endregion
    }
}
