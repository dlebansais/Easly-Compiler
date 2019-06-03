namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned collection of <see cref="IIdentifier"/>, and each of them must be the name of a resolved type.
    /// </summary>
    public interface IResolvedPathSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned collection of <see cref="IIdentifier"/>, and each of them must be the name of a resolved type.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    public interface IResolvedPathSourceTemplate<TSource> : ISourceTemplate<TSource, OnceReference<IList<IIdentifier>>>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned collection of <see cref="IIdentifier"/>, and each of them must be the name of a resolved type.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    public class ResolvedPathSourceTemplate<TSource> : SourceTemplate<TSource, OnceReference<IList<IIdentifier>>>, IResolvedPathSourceTemplate<TSource>, IResolvedPathSourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvedPathSourceTemplate{TSource}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public ResolvedPathSourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        public override bool IsReady(TSource node, out object data)
        {
            data = null;
            bool Result = false;

            OnceReference<IList<IIdentifier>> Value = GetSourceObject(node, out bool IsInterrupted);
            if (!IsInterrupted && Value.IsAssigned)
            {
                IList<IIdentifier> Path = Value.Item;
                foreach (IIdentifier Identifier in Path)
                    Debug.Assert(Identifier.ValidText.IsAssigned);

                List<IEntityDeclaration> LocalEntityList = new List<IEntityDeclaration>();

                if (node.EmbeddingOverload is ICommandOverload AsCommandOverload)
                    LocalEntityList.AddRange(AsCommandOverload.ParameterList);
                else if (node.EmbeddingOverload is IQueryOverload AsQueryOverload)
                {
                    LocalEntityList.AddRange(AsQueryOverload.ParameterList);
                    LocalEntityList.AddRange(AsQueryOverload.ResultList);
                }

                if (node.EmbeddingBody is IEffectiveBody AsEffectiveBody)
                    LocalEntityList.AddRange(AsEffectiveBody.EntityDeclarationList);

                IClass Class = node.EmbeddingClass;
                IHashtableEx<IFeatureName, IFeatureInstance> LocalFeatureTable = Class.LocalFeatureTable;
                IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = Class.FeatureTable;

                IErrorList ErrorList = new ErrorList();
                if (IsPathReady(Path, LocalEntityList, LocalFeatureTable, FeatureTable, ErrorList, out ITypeName ResolvedPathTypeName, out ICompiledType ResolvedPathType))
                {
                    data = new Tuple<IErrorList, ITypeName, ICompiledType>(ErrorList, ResolvedPathTypeName, ResolvedPathType);
                    Result = true;
                }
            }

            return Result;
        }

        /// <summary>
        /// Checks if a path to a target type is made of resolved elements.
        /// </summary>
        /// <param name="path">The path to the target.</param>
        /// <param name="localEntityList">The list of available local variables.</param>
        /// <param name="localFeatureTable">The local feature table at the begining of the path.</param>
        /// <param name="featureTable">The feature table at the begining of the path.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if the path could be resolved to the target.</returns>
        public static bool IsPathReady(IList<IIdentifier> path, List<IEntityDeclaration> localEntityList, IHashtableEx<IFeatureName, IFeatureInstance> localFeatureTable, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IErrorList errorList, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            resolvedPathTypeName = null;
            resolvedPathType = null;

            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable;

            // We start with a feature table. The full table if available, the local table otherwise.
            /*
            if (featureTable.IsSealed)
                FeatureTable = featureTable;
            else if (localFeatureTable.IsSealed)
                FeatureTable = localFeatureTable;
            else
                return false;
            */

            if (localFeatureTable.IsSealed)
                FeatureTable = localFeatureTable;
            else
                return false;

            Debug.Assert(path.Count > 0);

            bool IsInterrupted = false;
            bool IsReady = true;
            int i = 0;

            // If an error is found, the error will be processed in CheckConsistency.
            for (; i + 1 < path.Count && !IsInterrupted && IsReady; i++)
                IsReady &= IsPathItemReady(path[i], path[i + 1], ref localEntityList, ref FeatureTable, errorList, ref IsInterrupted);

            // This loop executes once if IsReady is true, and doesn't otherwise.
            for (; i < path.Count && !IsInterrupted && IsReady; i++)
                IsReady &= IsLastPathItemReady(path[i], localEntityList, FeatureTable, errorList, ref IsInterrupted, out resolvedPathTypeName, out resolvedPathType);

            return IsReady || IsInterrupted;
        }

        /// <summary>
        /// Checks if an intermediate step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The current step.</param>
        /// <param name="nextItem">The step after <paramref name="item"/>.</param>
        /// <param name="localEntityList">The list of available local variables.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="isInterrupted">Set if an error is found.</param>
        /// <returns>False to stop; True to continue with the next step.</returns>
        public static bool IsPathItemReady(IIdentifier item, IIdentifier nextItem, ref List<IEntityDeclaration> localEntityList, ref IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IErrorList errorList, ref bool isInterrupted)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            string Text = item.ValidText.Item;
            bool IsLocal = IsLocalEntity(Text, localEntityList, out IEntityDeclaration Entity);
            bool IsFeature = FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance);

            // Check that the name is known in the table of features for the current step.
            if (!IsLocal && !IsFeature)
            {
                errorList.AddError(new ErrorUnknownIdentifier(item, Text));
                isInterrupted = true;
                return false;
            }

            bool IsReady = false;
            ICompiledType ItemType = null;

            if (IsLocal)
            {
                IsReady = Entity.ValidEntity.IsAssigned && Entity.ValidEntity.Item.ResolvedFeatureType.IsAssigned;
                ItemType = IsReady ? Entity.ValidEntity.Item.ResolvedFeatureType.Item : null;
            }
            else
            {
                ICompiledFeature ItemFeature = ItemInstance.Feature;
                IsReady = IsPathGlobalItemReady(ItemFeature, nextItem, errorList, ref isInterrupted, out ItemType);
            }

            if (IsReady)
            {
                Debug.Assert(ItemType != null);
                featureTable = ItemType.FeatureTable;

                IsReady = featureTable.IsSealed;
            }

            return IsReady;
        }

        private static bool IsPathGlobalItemReady(ICompiledFeature itemFeature, IIdentifier nextItem, IErrorList errorList, ref bool isInterrupted, out ICompiledType itemType)
        {
            bool IsReady = false;
            itemType = null;

            Debug.Assert(nextItem != null);
            Debug.Assert(nextItem.ValidText.IsAssigned);
            string NextItemText = nextItem.ValidText.Item;

            bool IsHandled = false;
            switch (itemFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                    IsReady = AsAttributeFeature.ResolvedEntityType.IsAssigned;
                    itemType = IsReady ? AsAttributeFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    IsReady = AsConstantFeature.ResolvedEntityType.IsAssigned;
                    itemType = IsReady ? AsConstantFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;

                // Creation and procedure features don't return anything.
                case ICreationFeature AsCreationFeature:
                case IProcedureFeature AsProcedureFeature:
                    errorList.AddError(new ErrorUnknownIdentifier(nextItem, NextItemText));
                    isInterrupted = true;
                    return false;

                case IFunctionFeature AsFunctionFeature:
                    IsReady = AsFunctionFeature.MostCommonType.IsAssigned;
                    itemType = IsReady ? AsFunctionFeature.MostCommonType.Item : null;
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    IsReady = AsPropertyFeature.ResolvedEntityType.IsAssigned;
                    itemType = IsReady ? AsPropertyFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;

                case IIndexerFeature AsIndexerFeature:
                    IsReady = AsIndexerFeature.ResolvedEntityType.IsAssigned;
                    itemType = IsReady ? AsIndexerFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return IsReady;
        }

        /// <summary>
        /// Checks if the last step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The last step.</param>
        /// <param name="localEntityList">The list of available local variables.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="isInterrupted">Set if an error is found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if the path step could be resolved, or an error was found.</returns>
        public static bool IsLastPathItemReady(IIdentifier item, List<IEntityDeclaration> localEntityList, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IErrorList errorList, ref bool isInterrupted, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            resolvedPathTypeName = null;
            resolvedPathType = null;

            string Text = item.ValidText.Item;
            bool IsLocal = IsLocalEntity(Text, localEntityList, out IEntityDeclaration Entity);
            bool IsFeature = FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance);

            // Check that the name is known in the table of features for the current step.
            if (!IsLocal && !IsFeature)
            {
                errorList.AddError(new ErrorUnknownIdentifier(item, Text));
                isInterrupted = true;
                return false;
            }

            bool Result = false;

            if (IsLocal)
            {
                if (Entity.ValidEntity.IsAssigned && Entity.ValidEntity.Item.ResolvedFeatureType.IsAssigned)
                {
                    resolvedPathTypeName = Entity.ValidEntity.Item.ResolvedFeatureTypeName.Item;
                    resolvedPathType = Entity.ValidEntity.Item.ResolvedFeatureType.Item;
                    Result = true;
                }
            }
            else
            {
                ICompiledFeature ItemFeature = ItemInstance.Feature;
                bool IsHandled = false;

                switch (ItemFeature)
                {
                    case IAttributeFeature AsAttributeFeature:
                        Result = IsAttributeFeatureReady(AsAttributeFeature, out resolvedPathTypeName, out resolvedPathType);
                        IsHandled = true;
                        break;

                    case IConstantFeature AsConstantFeature:
                        Result = IsConstantFeatureReady(AsConstantFeature, out resolvedPathTypeName, out resolvedPathType);
                        IsHandled = true;
                        break;

                    case ICreationFeature AsCreationFeature:
                    case IProcedureFeature AsProcedureFeature:
                        errorList.AddError(new ErrorNotAnchor(item, Text));
                        isInterrupted = true;
                        return false;

                    case IFunctionFeature AsFunctionFeature:
                        Result = IsFunctionFeatureReady(AsFunctionFeature, out resolvedPathTypeName, out resolvedPathType);
                        IsHandled = true;
                        break;

                    case IPropertyFeature AsPropertyFeature:
                        Result = IsPropertyFeatureReady(AsPropertyFeature, out resolvedPathTypeName, out resolvedPathType);
                        IsHandled = true;
                        break;

                    case IIndexerFeature AsIndexerFeature:
                        Result = IsIndexerFeatureReady(AsIndexerFeature, out resolvedPathTypeName, out resolvedPathType);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return Result;
        }

        private static bool IsLocalEntity(string text, List<IEntityDeclaration> localEntityList, out IEntityDeclaration entity)
        {
            entity = null;
            bool Result = false;

            foreach (IEntityDeclaration Item in localEntityList)
            {
                IName EntityName = (IName)Item.EntityName;
                Debug.Assert(EntityName.ValidText.IsAssigned);
                if (text == EntityName.ValidText.Item)
                {
                    Debug.Assert(entity == null);

                    entity = Item;
                    Result = true;
                }
            }

            return Result;
        }

        private static bool IsAttributeFeatureReady(IAttributeFeature feature, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            bool Result = false;
            resolvedPathTypeName = null;
            resolvedPathType = null;

            if (feature.ResolvedEntityTypeName.IsAssigned && feature.ResolvedEntityType.IsAssigned)
            {
                resolvedPathTypeName = feature.ResolvedEntityTypeName.Item;
                resolvedPathType = feature.ResolvedEntityType.Item;
                Result = true;
            }

            return Result;
        }

        private static bool IsConstantFeatureReady(IConstantFeature feature, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            bool Result = false;
            resolvedPathTypeName = null;
            resolvedPathType = null;

            if (feature.ResolvedEntityTypeName.IsAssigned && feature.ResolvedEntityType.IsAssigned)
            {
                resolvedPathTypeName = feature.ResolvedEntityTypeName.Item;
                resolvedPathType = feature.ResolvedEntityType.Item;
                Result = true;
            }

            return Result;
        }

        private static bool IsFunctionFeatureReady(IFunctionFeature feature, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            bool Result = false;
            resolvedPathTypeName = null;
            resolvedPathType = null;

            if (feature.MostCommonTypeName.IsAssigned && feature.MostCommonType.IsAssigned)
            {
                resolvedPathTypeName = feature.MostCommonTypeName.Item;
                resolvedPathType = feature.MostCommonType.Item;
                Result = true;
            }

            return Result;
        }

        private static bool IsPropertyFeatureReady(IPropertyFeature feature, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            bool Result = false;
            resolvedPathTypeName = null;
            resolvedPathType = null;

            if (feature.ResolvedEntityTypeName.IsAssigned && feature.ResolvedEntityType.IsAssigned)
            {
                resolvedPathTypeName = feature.ResolvedEntityTypeName.Item;
                resolvedPathType = feature.ResolvedEntityType.Item;
                Result = true;
            }

            return Result;
        }

        private static bool IsIndexerFeatureReady(IIndexerFeature feature, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            bool Result = false;
            resolvedPathTypeName = null;
            resolvedPathType = null;

            if (feature.ResolvedEntityTypeName.IsAssigned && feature.ResolvedEntityType.IsAssigned)
            {
                resolvedPathTypeName = feature.ResolvedEntityTypeName.Item;
                resolvedPathType = feature.ResolvedEntityType.Item;
                Result = true;
            }

            return Result;
        }
        #endregion
    }
}
