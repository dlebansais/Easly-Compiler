﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IQueryOverloadType"/>.
    /// </summary>
    public interface IQueryOverloadTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IQueryOverloadType"/>.
    /// </summary>
    public class QueryOverloadTypeRuleTemplate : RuleTemplate<IQueryOverloadType, QueryOverloadTypeRuleTemplate>, IQueryOverloadTypeRuleTemplate
    {
        #region Init
        static QueryOverloadTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IQueryOverloadType, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverloadType.ParameterList), nameof(IEntityDeclaration.ValidEntity)),
                new OnceReferenceCollectionSourceTemplate<IQueryOverloadType, IEntityDeclaration, IScopeAttributeFeature>(nameof(IQueryOverloadType.ResultList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedListDestinationTemplate<IQueryOverloadType, IParameter>(nameof(IQueryOverloadType.ParameterTable)),
                new UnsealedListDestinationTemplate<IQueryOverloadType, IParameter>(nameof(IQueryOverloadType.ResultTable)),
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
        public override bool CheckConsistency(IQueryOverloadType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            ISealableList<IParameter> ParameterTable = new SealableList<IParameter>();
            ISealableList<IParameter> ResultTable = new SealableList<IParameter>();

            foreach (IEntityDeclaration Item in node.ParameterList)
            {
                IName ParameterName = (IName)Item.EntityName;

                Debug.Assert(ParameterName.ValidText.IsAssigned);
                string ValidText = ParameterName.ValidText.Item;

                IScopeAttributeFeature ParameterFeature = Item.ValidEntity.Item;

                if (Parameter.TableContainsName(ParameterTable, ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(ParameterName, ValidText));
                    Success = false;
                }
                else
                    ParameterTable.Add(new Parameter(ValidText, ParameterFeature));
            }

            foreach (IEntityDeclaration Item in node.ResultList)
            {
                IName ResultName = (IName)Item.EntityName;

                Debug.Assert(ResultName.ValidText.IsAssigned);
                string ValidText = ResultName.ValidText.Item;

                IScopeAttributeFeature ResultFeature = Item.ValidEntity.Item;

                if (Parameter.TableContainsName(ResultTable, ValidText) || Parameter.TableContainsName(ParameterTable, ValidText))
                {
                    AddSourceError(new ErrorDuplicateName(ResultName, ValidText));
                    Success = false;
                }
                else
                    ResultTable.Add(new Parameter(ValidText, ResultFeature));
            }

            if (Success)
                data = new Tuple<ISealableList<IParameter>, ISealableList<IParameter>>(ParameterTable, ResultTable);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IQueryOverloadType node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;

            ISealableList<IParameter> ParameterTable = ((Tuple<ISealableList<IParameter>, ISealableList<IParameter>>)data).Item1;
            ISealableList<IParameter> ResultTable = ((Tuple<ISealableList<IParameter>, ISealableList<IParameter>>)data).Item2;

            Debug.Assert(node.ParameterTable.Count == 0);
            Debug.Assert(node.ResultTable.Count == 0);

            node.ParameterTable.AddRange(ParameterTable);
            node.ParameterTable.Seal();

            node.ResultTable.AddRange(ResultTable);
            node.ResultTable.Seal();

            foreach (IParameter Parameter in ResultTable)
            {
                Debug.Assert(Parameter.ResolvedParameter.ResolvedEffectiveType.IsAssigned);
                node.ConformantResultTable.Add(Parameter.ResolvedParameter.ResolvedEffectiveType.Item);
            }
        }
        #endregion
    }
}
