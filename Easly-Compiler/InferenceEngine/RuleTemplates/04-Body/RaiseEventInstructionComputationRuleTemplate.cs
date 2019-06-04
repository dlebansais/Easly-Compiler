namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public interface IRaiseEventInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IRaiseEventInstruction"/>.
    /// </summary>
    public class RaiseEventInstructionComputationRuleTemplate : RuleTemplate<IRaiseEventInstruction, RaiseEventInstructionComputationRuleTemplate>, IRaiseEventInstructionComputationRuleTemplate
    {
        #region Init
        static RaiseEventInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IRaiseEventInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IRaiseEventInstruction>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IRaiseEventInstruction, IResultException>(nameof(IRaiseEventInstruction.ResolvedException)),
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
        public override bool CheckConsistency(IRaiseEventInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IIdentifier QueryIdentifier = (IIdentifier)node.QueryIdentifier;
            string ValidText = QueryIdentifier.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            if (!FeatureName.TableContain(EmbeddingClass.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
            {
                AddSourceError(new ErrorUnknownIdentifier(QueryIdentifier, ValidText));
                Success = false;
            }
            else
            {
                ICompiledFeature EventFeatureInstance = Instance.Feature;
                IFeatureWithEvents SelectedFeature = null;
                ICompiledType SelectedEntityType = null;

                if (EventFeatureInstance is IAttributeFeature AsAttributeFeature)
                {
                    SelectedFeature = AsAttributeFeature;
                    SelectedEntityType = AsAttributeFeature.ResolvedEntityType.Item;
                }
                else if (EventFeatureInstance is IPropertyFeature AsPropertyFeature)
                {
                    SelectedFeature = AsPropertyFeature;
                    SelectedEntityType = AsPropertyFeature.ResolvedEntityType.Item;
                }
                else
                {
                    AddSourceError(new ErrorAttributeOrPropertyRequired(QueryIdentifier, ValidText));
                    Success = false;
                }

                if (Success)
                {
                    Debug.Assert(SelectedFeature != null);
                    Debug.Assert(SelectedEntityType != null);

                    if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Event.Guid, node, out ITypeName EventTypeName, out ICompiledType EventType))
                    {
                        AddSourceError(new ErrorEventTypeMissing(node));
                        Success = false;
                    }
                    else if (!ObjectType.TypeConformToBase(SelectedEntityType, EventType))
                    {
                        AddSourceError(new ErrorInvalidInstruction(node));
                        Success = false;
                    }
                    else
                    {
                        SelectedFeature.SetEventType(node.Event, out bool IsConflicting);

                        if (IsConflicting)
                        {
                            AddSourceError(new ErrorInvalidInstruction(node));
                            Success = false;
                        }
                        else
                        {
                            Debug.Assert(SelectedFeature.ResolvedEventType == node.Event);

                            IResultException ResolvedException = new ResultException();

                            data = new Tuple<IFeatureWithEvents, ICompiledType, IResultException>(SelectedFeature, SelectedEntityType, ResolvedException);
                        }
                    }
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IRaiseEventInstruction node, object data)
        {
            IFeatureWithEvents SelectedFeature = ((Tuple<IFeatureWithEvents, ICompiledType, IResultException>)data).Item1;
            ICompiledType SelectedEntityType = ((Tuple<IFeatureWithEvents, ICompiledType, IResultException>)data).Item2;
            IResultException ResolvedException = ((Tuple<IFeatureWithEvents, ICompiledType, IResultException>)data).Item3;

            node.ResolvedFeature.Item = SelectedFeature;
            node.ResolvedEntityType.Item = SelectedEntityType;
            node.ResolvedException.Item = ResolvedException;
        }
        #endregion
    }
}
