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

                IClass Class = node.EmbeddingClass;
                IErrorList ErrorList = new ErrorList();
                if (IsPathReady(Path, Class.LocalFeatureTable, Class.FeatureTable, ErrorList, out ITypeName ResolvedPathTypeName, out ICompiledType ResolvedPathType))
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
        /// <param name="startingLocalFeatureTable">The local feature table at the begining of the path.</param>
        /// <param name="startingFeatureTable">The feature table at the begining of the path.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if the path could be resolved to the target.</returns>
        public static bool IsPathReady(IList<IIdentifier> path, IHashtableEx<IFeatureName, IFeatureInstance> startingLocalFeatureTable, IHashtableEx<IFeatureName, IFeatureInstance> startingFeatureTable, IErrorList errorList, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            resolvedPathTypeName = null;
            resolvedPathType = null;

            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable;

            // We start with a feature table. The full table if available, the local table otherwise.
            /*
            if (startingFeatureTable.IsSealed)
                FeatureTable = startingFeatureTable;
            else if (startingLocalFeatureTable.IsSealed)
                FeatureTable = startingLocalFeatureTable;
            else
                return false;
            */

            if (startingLocalFeatureTable.IsSealed)
                FeatureTable = startingLocalFeatureTable;
            else
                return false;

            Debug.Assert(path.Count > 0);

            bool IsInterrupted = false;
            bool IsReady = true;
            int i = 0;

            // If an error is found, the error will be processed in CheckConsistency.
            for (; i + 1 < path.Count && !IsInterrupted && IsReady; i++)
                IsReady &= IsPathItemReady(path[i], path[i + 1], ref FeatureTable, errorList, ref IsInterrupted, out resolvedPathTypeName, out resolvedPathType);

            // This loop executes once if IsReady is true, and doesn't otherwise.
            for (; i < path.Count && !IsInterrupted && IsReady; i++)
                IsReady &= IsLastPathItemReady(path[i], FeatureTable, errorList, ref IsInterrupted, out resolvedPathTypeName, out resolvedPathType);

            return IsReady || IsInterrupted;
        }

        /// <summary>
        /// Checks if an intermediate step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The current step.</param>
        /// <param name="nextItem">The step after <paramref name="item"/>.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="isInterrupted">Set if an error is found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>False to stop; True to continue with the next step.</returns>
        public static bool IsPathItemReady(IIdentifier item, IIdentifier nextItem, ref IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IErrorList errorList, ref bool isInterrupted, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            resolvedPathTypeName = null;
            resolvedPathType = null;

            string Text = item.ValidText.Item;

            // Check that the name is known in the table of features for the current step.
            if (!FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance))
            {
                errorList.AddError(new ErrorUnknownIdentifier(item, Text));
                isInterrupted = true;
                return false;
            }

            ICompiledType ItemType = null;
            ICompiledFeature ItemFeature = ItemInstance.Feature.Item;

            Debug.Assert(nextItem != null);
            Debug.Assert(nextItem.ValidText.IsAssigned);
            string NextItemText = nextItem.ValidText.Item;

            bool IsReady = false;
            bool IsHandled = false;
            switch (ItemFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                    IsReady = AsAttributeFeature.ResolvedEntityType.IsAssigned;
                    ItemType = IsReady ? AsAttributeFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    IsReady = AsConstantFeature.ResolvedEntityType.IsAssigned;
                    ItemType = IsReady ? AsConstantFeature.ResolvedEntityType.Item : null;
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
                    ItemType = IsReady ? AsFunctionFeature.MostCommonType.Item : null;
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    IsReady = AsPropertyFeature.ResolvedEntityType.IsAssigned;
                    ItemType = IsReady ? AsPropertyFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;

                case IIndexerFeature AsIndexerFeature:
                    IsReady = AsIndexerFeature.ResolvedEntityType.IsAssigned;
                    ItemType = IsReady ? AsIndexerFeature.ResolvedEntityType.Item : null;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (IsReady)
            {
                Debug.Assert(ItemType != null);
                featureTable = ItemType.FeatureTable;

                IsReady = featureTable.IsSealed;
            }

            return IsReady;
        }

        /// <summary>
        /// Checks if the last step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The last step.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="isInterrupted">Set if an error is found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if the path step could be resolved, or an error was found.</returns>
        public static bool IsLastPathItemReady(IIdentifier item, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IErrorList errorList, ref bool isInterrupted, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            resolvedPathTypeName = null;
            resolvedPathType = null;

            string Text = item.ValidText.Item;

            if (!FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance))
            {
                errorList.AddError(new ErrorUnknownIdentifier(item, Text));
                isInterrupted = true;
                return false;
            }

            ICompiledFeature ItemFeature = ItemInstance.Feature.Item;
            bool Result = false;
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
