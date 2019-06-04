namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

            Debug.Assert(AttributeType.IsReference);

            IList<IClassType> ConstraintClassTypeList = new List<IClassType>();
            if (!CheckConstraints(node, AttributeType, ConstraintClassTypeList))
                return false;

            if (!CheckCreationRoutine(node, ConstraintClassTypeList, out ICreationFeature CreationFeature))
                return false;

            if (!CheckCall(node, AttributeTypeName, AttributeType, CreationFeature, out ICommandOverloadType SelectedOverload, out IFeatureCall FeatureCall))
                return false;

            ITypeName CreatedObjectTypeName = AttributeTypeName;
            ICompiledType CreatedObjectType = AttributeType;

            if (node.Processor.IsAssigned)
            {
                IQualifiedName Processor = (IQualifiedName)node.Processor.Item;
                IList<IIdentifier> ValidPath = Processor.ValidPath.Item;

                ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

                if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                    return false;
            }

            IResultException ResolvedException = new ResultException();

            foreach (IArgument Item in node.ArgumentList)
                ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

            ResultException.Merge(ResolvedException, SelectedOverload.ExceptionIdentifierList);

            data = new Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>(ResolvedException, SelectedOverload, FeatureCall, CreatedObjectTypeName, CreatedObjectType);

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
            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (LocalScope.ContainsKey(ValidText))
            {
                IScopeAttributeFeature CreatedFeature = LocalScope[ValidText];
                attributeTypeName = CreatedFeature.ResolvedFeatureTypeName.Item;
                attributeType = CreatedFeature.ResolvedFeatureType.Item;
                Success = true;
            }
            else if (FeatureName.TableContain(EmbeddingClass.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
            {
                if (Instance.Feature is IAttributeFeature AsAttributeFeature)
                {
                    attributeTypeName = AsAttributeFeature.ResolvedEntityTypeName.Item;
                    attributeType = AsAttributeFeature.ResolvedEntityType.Item;
                    Success = true;
                }
                else if (Instance.Feature is IPropertyFeature AsPropertyFeature)
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

        private bool CheckConstraints(ICreateInstruction node, ICompiledType attributeType, IList<IClassType> constraintClassTypeList)
        {
            IIdentifier EntityIdentifier = (IIdentifier)node.EntityIdentifier;
            string ValidText = EntityIdentifier.ValidText.Item;
            bool IsHandled = false;

            switch (attributeType)
            {
                case IClassType AsClassType:
                    constraintClassTypeList.Add(AsClassType);
                    IsHandled = true;
                    break;

                case IFormalGenericType AsFormalGenericType:
                    foreach (KeyValuePair<ITypeName, ICompiledType> Entry in AsFormalGenericType.ConformanceTable)
                        if (Entry.Value is IClassType AsConformantClassType)
                            constraintClassTypeList.Add(AsConformantClassType);
                        else
                        {
                            AddSourceError(new ErrorClassTypeRequired(EntityIdentifier));
                            return false;
                        }

                    IsHandled = true;
                    break;
            }

            // Since AttributeType is a reference type, it can only be one of the two cases above.
            Debug.Assert(IsHandled);

            return true;
        }

        private bool CheckCreationRoutine(ICreateInstruction node, IList<IClassType> constraintClassTypeList, out ICreationFeature creationFeature)
        {
            creationFeature = null;

            IIdentifier CreationRoutineIdentifier = (IIdentifier)node.CreationRoutineIdentifier;
            string ValidText = CreationRoutineIdentifier.ValidText.Item;

            IFeatureInstance CreationFeatureInstance = null;
            foreach (IClassType Item in constraintClassTypeList)
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

            ICompiledFeature CreationRoutineInstance = CreationFeatureInstance.Feature;

            if (CreationRoutineInstance is ICreationFeature AsCreationFeature)
            {
                creationFeature = AsCreationFeature;
                return true;
            }
            else
            {
                AddSourceError(new ErrorCreationFeatureRequired(CreationRoutineIdentifier, ValidText));
                return false;
            }
        }

        private bool CheckCall(ICreateInstruction node, ITypeName attributeTypeName, ICompiledType attributeType, ICreationFeature creationFeature, out ICommandOverloadType selectedOverload, out IFeatureCall featureCall)
        {
            selectedOverload = null;
            featureCall = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, ErrorList))
                return false;

            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();

            IProcedureType AsProcedureType = (IProcedureType)creationFeature.ResolvedFeatureType.Item;
            foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                ParameterTableList.Add(Overload.ParameterTable);

            if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, ErrorList, node, out int SelectedIndex))
                return false;

            selectedOverload = AsProcedureType.OverloadList[SelectedIndex];
            featureCall = new FeatureCall(ParameterTableList[SelectedIndex], node.ArgumentList, MergedArgumentList, TypeArgumentStyle);

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICreateInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>)data).Item1;
            ICommandOverloadType SelectedOverload = ((Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>)data).Item2;
            IFeatureCall FeatureCall = ((Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>)data).Item3;
            ITypeName CreatedObjectTypeName = ((Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>)data).Item4;
            ICompiledType CreatedObjectType = ((Tuple<IResultException, ICommandOverloadType, IFeatureCall, ITypeName, ICompiledType>)data).Item5;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedOverload.Item = SelectedOverload;
            node.FeatureCall.Item = FeatureCall;
            node.ResolvedEntityTypeName.Item = CreatedObjectTypeName;
            node.ResolvedEntityType.Item = CreatedObjectType;

            if (CreatedObjectType is IFormalGenericType AsFormalGenericType)
                AsFormalGenericType.SetIsUsedToCreate();
        }
        #endregion
    }
}
