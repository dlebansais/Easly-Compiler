namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ICreateInstruction"/>.
    /// </summary>
    public interface ICreateInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICreateInstruction"/>.
    /// </summary>
    public class CreateInstructionComputationRuleTemplate : RuleTemplate<ICreateInstruction, CreateInstructionComputationRuleTemplate>, ICreateInstructionComputationRuleTemplate
    {
        #region Init
        static CreateInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<ICreateInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateNodeStart<ICreateInstruction>.Default),
                new OnceReferenceTableSourceTemplate<ICreateInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateNodeStart<ICreateInstruction>.Default),
                new SealedTableSourceTemplate<ICreateInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<ICreateInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<ICreateInstruction, IArgument, IResultException>(nameof(ICreateInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICreateInstruction, IResultException>(nameof(ICreateInstruction.ResolvedException)),
                new OnceReferenceDestinationTemplate<ICreateInstruction, ICommandOverloadType>(nameof(ICreateInstruction.SelectedOverload)),
                new OnceReferenceDestinationTemplate<ICreateInstruction, ITypeName>(nameof(ICreateInstruction.ResolvedEntityTypeName)),
                new OnceReferenceDestinationTemplate<ICreateInstruction, ICompiledType>(nameof(ICreateInstruction.ResolvedEntityType)),
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
        public override bool CheckConsistency(ICreateInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IIdentifier EntityIdentifier = (IIdentifier)node.EntityIdentifier;
            string ValidText = EntityIdentifier.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            if (!GetCreatedEntity(node, out ITypeName AttributeTypeName, out ICompiledType AttributeType))
                return false;

            IList<IClassType> ConstraintClassTypeList = new List<IClassType>();

            if (AttributeType is IClassType AsClassType)
                ConstraintClassTypeList.Add(AsClassType);

            else if (AttributeType is IFormalGenericType AsFormalGenericType)
                foreach (KeyValuePair<ITypeName, ICompiledType> Entry in AsFormalGenericType.ConformanceTable)
                    if (Entry.Value is IClassType AsConformantClassType)
                        ConstraintClassTypeList.Add(AsConformantClassType);
                    else
                    {
                        AddSourceError(new ErrorClassTypeRequired(EntityIdentifier));
                        return false;
                    }

            else
            {
                AddSourceError(new ErrorClassTypeRequired(EntityIdentifier));
                return false;
            }

            IIdentifier CreationRoutineIdentifier = (IIdentifier)node.CreationRoutineIdentifier;
            ValidText = CreationRoutineIdentifier.ValidText.Item;

            IFeatureInstance CreationFeatureInstance = null;
            foreach (ClassType Item in ConstraintClassTypeList)
                if (FeatureName.TableContain(Item.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
                {
                    CreationFeatureInstance = Instance;
                    break;
                }

            if (CreationFeatureInstance == null)
            {
                AddSourceError(new ErrorUnknownIdentifier(CreationRoutineIdentifier, ValidText));
                return false;
            }

            ICompiledFeature CreationRoutineInstance = CreationFeatureInstance.Feature.Item;

            if (CreationRoutineInstance is ICreationFeature AsCreationFeature)
            {
                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                IErrorList ArgumentErrorList = new ErrorList();
                if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, ArgumentErrorList))
                {
                    AddSourceErrorList(ArgumentErrorList);
                    return false;
                }

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();

                IProcedureType AsProcedureType = (IProcedureType)AsCreationFeature.ResolvedFeatureType.Item;
                foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                    ParameterTableList.Add(Overload.ParameterTable);

                int SelectedIndex;
                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out SelectedIndex))
                    return false;

                ICommandOverloadType SelectedOverload = AsProcedureType.OverloadList[SelectedIndex];
                ITypeName CreatedObjectTypeName = AttributeTypeName;
                ICompiledType CreatedObjectType = AttributeType;

                if (node.Processor.IsAssigned)
                {
                    IQualifiedName Processor = (IQualifiedName)node.Processor.Item;
                    IList<IIdentifier> ValidPath = Processor.ValidPath.Item;

                    IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

                    if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                        return false;

                    IResultException ResolvedException = new ResultException();

                    foreach (IArgument Item in node.ArgumentList)
                        ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                    ResultException.Merge(ResolvedException, SelectedOverload.ExceptionIdentifierList);

                    data = new Tuple<IResultException, ICommandOverloadType, ITypeName, ICompiledType>(ResolvedException, SelectedOverload, CreatedObjectTypeName, CreatedObjectType);
                }
            }
            else
            {
                AddSourceError(new ErrorCreationFeatureRequired(CreationRoutineIdentifier, ValidText));
                return false;
            }

            return Success;
        }

        private bool GetCreatedEntity(ICreateInstruction node, out ITypeName attributeTypeName, out ICompiledType attributeType)
        {
            attributeTypeName = null;
            attributeType = null;
            bool Success = false;

            IIdentifier EntityIdentifier = (IIdentifier)node.EntityIdentifier;
            string ValidText = EntityIdentifier.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (LocalScope.ContainsKey(ValidText))
            {
                IScopeAttributeFeature CreatedFeature = LocalScope[ValidText];
                attributeTypeName = CreatedFeature.ResolvedFeatureTypeName.Item;
                attributeType = CreatedFeature.ResolvedFeatureType.Item;
                Success = true;
            }
            else if (FeatureName.TableContain(EmbeddingClass.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
            {
                if (Instance.Feature.Item is IAttributeFeature AsAttributeFeature)
                {
                    attributeTypeName = AsAttributeFeature.ResolvedEntityTypeName.Item;
                    attributeType = AsAttributeFeature.ResolvedEntityType.Item;
                    Success = true;
                }

                else if (Instance.Feature.Item is IPropertyFeature AsPropertyFeature)
                {
                    attributeTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                    attributeType = AsPropertyFeature.ResolvedEntityType.Item;
                    Success = true;
                }

                else
                    AddSourceError(new ErrorCreatedFeatureNotAttributeOrProperty(EntityIdentifier, ValidText));
            }
            else
                AddSourceError(new ErrorUnknownIdentifier(EntityIdentifier, ValidText));

            if (Success && !attributeType.IsReference)
            {
                AddSourceError(new ErrorReferenceTypeRequired(EntityIdentifier));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICreateInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ICommandOverloadType, ITypeName, ICompiledType>)data).Item1;
            ICommandOverloadType SelectedOverload = ((Tuple<IResultException, ICommandOverloadType, ITypeName, ICompiledType>)data).Item2;
            ITypeName CreatedObjectTypeName = ((Tuple<IResultException, ICommandOverloadType, ITypeName, ICompiledType>)data).Item3;
            ICompiledType CreatedObjectType = ((Tuple<IResultException, ICommandOverloadType, ITypeName, ICompiledType>)data).Item4;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedOverload.Item = SelectedOverload;
            node.ResolvedEntityTypeName.Item = CreatedObjectTypeName;
            node.ResolvedEntityType.Item = CreatedObjectType;

            // TODO
            /*if (CreatedObjectType is IFormalGenericType AsFormalGenericType)
                AsFormalGenericType.SetCreated();*/
        }
        #endregion
    }
}
