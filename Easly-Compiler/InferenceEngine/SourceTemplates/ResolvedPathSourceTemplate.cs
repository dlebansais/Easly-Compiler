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
    public class ResolvedPathSourceTemplate<TSource> : SourceTemplate<TSource, OnceReference<IList<IIdentifier>>>, IResolvedPathSourceTemplate
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

        #region Properties
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

            OnceReference<IList<IIdentifier>> Value = GetSourceObject(node);
            if (Value.IsAssigned)
            {
                IList<IIdentifier> Path = Value.Item;
                foreach (IIdentifier Identifier in Path)
                    Debug.Assert(Identifier.ValidText.IsAssigned);

                IClass Class = node.EmbeddingClass;
                IList<IError> ErrorList = new List<IError>();
                if (IsPathReady(Path, Class.LocalFeatureTable, Class.FeatureTable, ErrorList, out ITypeName ResolvedPathTypeName, out ICompiledType ResolvedPathType))
                {
                    data = new Tuple<IList<IError>, ITypeName, ICompiledType>(ErrorList, ResolvedPathTypeName, ResolvedPathType);
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
        public static bool IsPathReady(IList<IIdentifier> path, IHashtableEx<IFeatureName, IFeatureInstance> startingLocalFeatureTable, IHashtableEx<IFeatureName, IFeatureInstance> startingFeatureTable, IList<IError> errorList, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            resolvedPathTypeName = null;
            resolvedPathType = null;

            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable;

            // We start with a feature table. The full table if available, the local table otherwise.
            if (startingFeatureTable.IsSealed)
                FeatureTable = startingFeatureTable;
            else if (startingLocalFeatureTable.IsSealed)
                FeatureTable = startingLocalFeatureTable;
            else
                return false;

            Debug.Assert(path.Count > 0);

            for (int i = 0; i + 1 < path.Count; i++)
            {
                // Return if an error is found, the error will be processed in CheckConsistency.
                if (IsPathItemReady(path[i], path[i + 1], ref FeatureTable, errorList, out resolvedPathTypeName, out resolvedPathType))
                    return true;

                if (!FeatureTable.IsSealed)
                    return false;
            }

            if (IsLastPathItemReady(path[path.Count - 1], FeatureTable, errorList, out resolvedPathTypeName, out resolvedPathType))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if an intermediate step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The current step.</param>
        /// <param name="nextItem">The step after <paramref name="item"/>.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if there is an error; False to continue with the next step.</returns>
        public static bool IsPathItemReady(IIdentifier item, IIdentifier nextItem, ref IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IList<IError> errorList, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            resolvedPathTypeName = null;
            resolvedPathType = null;

            string Text = item.ValidText.Item;

            // Check that the name is known in the table of features for the current step.
            if (!FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance))
            {
                errorList.Add(new ErrorUnknownIdentifier(item, Text));
                return true;
            }

            ICompiledType ItemType = null;
            ICompiledFeature ItemFeature = ItemInstance.Feature.Item;

            Debug.Assert(nextItem != null);

            Debug.Assert(nextItem.ValidText.IsAssigned);
            string NextItemText = nextItem.ValidText.Item;

            bool IsHandled = false;
            switch (ItemFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                    ItemType = AsAttributeFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    ItemType = AsConstantFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                // Creation and procedure features don't return anything.
                case ICreationFeature AsCreationFeature:
                case IProcedureFeature AsProcedureFeature:
                    errorList.Add(new ErrorUnknownIdentifier(nextItem, NextItemText));
                    return true;

                case IFunctionFeature AsFunctionFeature:
                    ItemType = AsFunctionFeature.MostCommonType.Item;
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    ItemType = AsPropertyFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                case IIndexerFeature AsIndexerFeature:
                    ItemType = AsIndexerFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (ItemType == null)
            {
                errorList.Add(new ErrorUnknownIdentifier(nextItem, NextItemText));
                return true;
            }

            featureTable = ItemType.FeatureTable;
            return false;
        }

        /// <summary>
        /// Checks if the last step in a path to a target type is resolved.
        /// </summary>
        /// <param name="item">The last step.</param>
        /// <param name="featureTable">The feature table to use.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedPathTypeName">The target type name upon return.</param>
        /// <param name="resolvedPathType">The target type upon return.</param>
        /// <returns>True if the path step could be resolved, or an error was found.</returns>
        public static bool IsLastPathItemReady(IIdentifier item, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, IList<IError> errorList, out ITypeName resolvedPathTypeName, out ICompiledType resolvedPathType)
        {
            Debug.Assert(featureTable.IsSealed);
            Debug.Assert(item.ValidText.IsAssigned);

            resolvedPathTypeName = null;
            resolvedPathType = null;

            string Text = item.ValidText.Item;

            if (!FeatureName.TableContain(featureTable, Text, out IFeatureName ItemName, out IFeatureInstance ItemInstance))
            {
                errorList.Add(new ErrorUnknownIdentifier(item, Text));
                return true;
            }

            ICompiledFeature ItemFeature = ItemInstance.Feature.Item;

            bool IsHandled = false;
            switch (ItemFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                    if (!AsAttributeFeature.ResolvedEntityTypeName.IsAssigned || !AsAttributeFeature.ResolvedEntityType.IsAssigned)
                        return false;

                    resolvedPathTypeName = AsAttributeFeature.ResolvedEntityTypeName.Item;
                    resolvedPathType = AsAttributeFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    if (!AsConstantFeature.ResolvedEntityTypeName.IsAssigned || !AsConstantFeature.ResolvedEntityType.IsAssigned)
                        return false;

                    resolvedPathTypeName = AsConstantFeature.ResolvedEntityTypeName.Item;
                    resolvedPathType = AsConstantFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                case ICreationFeature AsCreationFeature:
                case IProcedureFeature AsProcedureFeature:
                    errorList.Add(new ErrorNotAnchor(item, Text));
                    return true;

                case IFunctionFeature AsFunctionFeature:
                    if (!AsFunctionFeature.MostCommonTypeName.IsAssigned || !AsFunctionFeature.MostCommonType.IsAssigned)
                        return false;

                    resolvedPathTypeName = AsFunctionFeature.MostCommonTypeName.Item;
                    resolvedPathType = AsFunctionFeature.MostCommonType.Item;
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    if (!AsPropertyFeature.ResolvedEntityTypeName.IsAssigned || !AsPropertyFeature.ResolvedEntityType.IsAssigned)
                        return false;

                    resolvedPathTypeName = AsPropertyFeature.ResolvedEntityTypeName.Item;
                    resolvedPathType = AsPropertyFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;

                case IIndexerFeature AsIndexerFeature:
                    if (!AsIndexerFeature.ResolvedEntityTypeName.IsAssigned || !AsIndexerFeature.ResolvedEntityType.IsAssigned)
                        return false;

                    resolvedPathTypeName = AsIndexerFeature.ResolvedEntityTypeName.Item;
                    resolvedPathType = AsIndexerFeature.ResolvedEntityType.Item;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
            return true;
        }
        #endregion
    }
}
