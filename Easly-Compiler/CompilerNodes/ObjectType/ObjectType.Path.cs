namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
        /// <summary>
        /// Gets the object a path is refering to.
        /// </summary>
        /// <param name="baseClass">The class where the path is used.</param>
        /// <param name="baseType">The type at the start of the path.</param>
        /// <param name="localScope">The local scope.</param>
        /// <param name="validPath">The path.</param>
        /// <param name="index">Index of the current identifier in the path.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="finalFeature">The feature at the end of the path, if any, upon return.</param>
        /// <param name="finalDiscrete">The discrete at the end of the path, if any, upon return.</param>
        /// <param name="finalTypeName">The type name of the result.</param>
        /// <param name="finalType">The type of the result.</param>
        /// <param name="inheritBySideAttribute">Inherited from an effective body.</param>
        public static bool GetQualifiedPathFinalType(IClass baseClass, ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IErrorList errorList, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            finalFeature = null;
            finalDiscrete = null;
            finalTypeName = null;
            finalType = null;
            inheritBySideAttribute = false;

            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = baseType.FeatureTable;

            IIdentifier ValidIdentifier = validPath[index];
            string ValidText = ValidIdentifier.ValidText.Item;

            if (index == 0 && localScope.ContainsKey(ValidText))
                return GetQualifiedPathFinalTypeFromLocal(baseClass, baseType, localScope, validPath, index, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
                return GetQualifiedPathFinalTypeAsFeature(baseClass, baseType, localScope, validPath, index, errorList, Instance, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (index == 0 && index + 1 < validPath.Count && baseClass.ImportedClassTable.ContainsKey(ValidText) && baseClass.ImportedClassTable[ValidText].Item.Cloneable == BaseNode.CloneableStatus.Single)
                return GetQualifiedPathFinalTypeFromSingle(baseClass, localScope, validPath, index, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (index + 1 == validPath.Count)
                return GetQualifiedPathFinalTypeAsDiscreteOrAgent(baseType, localScope, ValidIdentifier, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else
            {
                errorList.AddError(new ErrorUnknownIdentifier(ValidIdentifier, ValidText));
                return false;
            }
        }

        private static bool GetQualifiedPathFinalTypeFromLocal(IClass baseClass, ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IErrorList errorList, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            IIdentifier ValidIdentifier = validPath[index];
            string ValidText = ValidIdentifier.ValidText.Item;

            if (index + 1 < validPath.Count)
            {
                Debug.Assert(localScope[ValidText].ResolvedEffectiveType.IsAssigned);

                ITypeName ResolvedFeatureTypeName = localScope[ValidText].ResolvedEffectiveTypeName.Item;
                ICompiledType ResolvedFeatureType = localScope[ValidText].ResolvedEffectiveType.Item;
                ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, localScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            }
            else
            {
                IScopeAttributeFeature LocalFeature = localScope[ValidText];

                finalFeature = LocalFeature;
                finalDiscrete = null;
                finalTypeName = LocalFeature.ResolvedEffectiveTypeName.Item;
                finalType = LocalFeature.ResolvedEffectiveType.Item;
                inheritBySideAttribute = false;
                return true;
            }
        }

        private static bool GetQualifiedPathFinalTypeAsFeature(IClass baseClass, ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IErrorList errorList, IFeatureInstance instance, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            ICompiledFeature SourceFeature = instance.Feature;

            if (index + 1 < validPath.Count)
            {
                ITypeName ResolvedFeatureTypeName = SourceFeature.ResolvedEffectiveTypeName.Item;
                ICompiledType ResolvedFeatureType = SourceFeature.ResolvedEffectiveType.Item;

                Debug.Assert(baseType is ICompiledTypeWithFeature);
                ResolvedFeatureType.InstanciateType((ICompiledTypeWithFeature)baseType, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, localScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            }
            else
            {
                ITypeName ResolvedFeatureTypeName = SourceFeature.ResolvedAgentTypeName.Item;
                ICompiledType ResolvedFeatureType = SourceFeature.ResolvedAgentType.Item;

                Debug.Assert(baseType is ICompiledTypeWithFeature);
                ResolvedFeatureType.InstanciateType((ICompiledTypeWithFeature)baseType, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                finalFeature = SourceFeature;
                finalDiscrete = null;
                finalTypeName = ResolvedFeatureTypeName;
                finalType = ResolvedFeatureType;
                inheritBySideAttribute = instance.InheritBySideAttribute;
                return true;
            }
        }

        private static bool GetQualifiedPathFinalTypeFromSingle(IClass baseClass, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IErrorList errorList, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            IIdentifier ValidIdentifier = validPath[index];
            string ValidText = ValidIdentifier.ValidText.Item;

            IImportedClass Imported = baseClass.ImportedClassTable[ValidText];
            ITypeName ResolvedFeatureTypeName = Imported.ResolvedClassTypeName.Item;
            ICompiledType ResolvedFeatureType = Imported.ResolvedClassType.Item;
            ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

            return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, localScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
        }

        private static bool GetQualifiedPathFinalTypeAsDiscreteOrAgent(ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IIdentifier validIdentifier, IErrorList errorList, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            finalFeature = null;
            finalDiscrete = null;
            finalTypeName = null;
            finalType = null;
            inheritBySideAttribute = false;

            string ValidText = validIdentifier.ValidText.Item;

            ISealableDictionary<IFeatureName, IDiscrete> DiscreteTable = baseType.DiscreteTable;

            if (FeatureName.TableContain(DiscreteTable, ValidText, out IFeatureName Key, out IDiscrete Discrete))
            {
                if (Expression.IsLanguageTypeAvailable(LanguageClasses.Number.Guid, validIdentifier, out finalTypeName, out finalType))
                {
                    finalDiscrete = Discrete;
                    return true;
                }
                else
                {
                    errorList.AddError(new ErrorNumberTypeMissing(validIdentifier));
                    return false;
                }
            }
            else if (localScope.ContainsKey(ValidText))
            {
                IScopeAttributeFeature LocalFeature = localScope[ValidText];
                ICompiledType EffectiveType = LocalFeature.ResolvedEffectiveType.Item;

                switch (EffectiveType)
                {
                    // With these return types, the local variable is an agent.
                    case IProcedureType AsProcedureType:
                    case IFunctionType AsFunctionType:
                    case IPropertyType AsPropertyType:
                        finalFeature = LocalFeature;
                        finalTypeName = LocalFeature.ResolvedEffectiveTypeName.Item;
                        finalType = LocalFeature.ResolvedEffectiveType.Item;
                        inheritBySideAttribute = false;
                        return true;

                    default:
                        errorList.AddError(new ErrorUnknownIdentifier(validIdentifier, ValidText));
                        return false;
                }
            }
            else
            {
                errorList.AddError(new ErrorUnknownIdentifier(validIdentifier, ValidText));
                return false;
            }
        }

        /// <summary>
        /// Update all elements along a path with their type, previously validated.
        /// </summary>
        /// <param name="baseClass">The class where the path is used.</param>
        /// <param name="baseType">The type at the start of the path.</param>
        /// <param name="localScope">The local scope.</param>
        /// <param name="validPath">The path.</param>
        /// <param name="index">Index of the current identifier in the path.</param>
        /// <param name="resultPath">The path receiving updated elements.</param>
        public static void FillResultPath(IClass baseClass, ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IList<IExpressionType> resultPath)
        {
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = baseType.FeatureTable;
            string ValidText = validPath[index].ValidText.Item;

            if (index == 0 && localScope.ContainsKey(ValidText))
            {
                IScopeAttributeFeature LocalFeature = localScope[ValidText];
                ITypeName ResolvedFeatureTypeName = LocalFeature.ResolvedEffectiveTypeName.Item;
                ICompiledType ResolvedFeatureType = LocalFeature.ResolvedEffectiveType.Item;
                ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                resultPath.Add(new ExpressionType(ResolvedFeatureTypeName, ResolvedFeatureType, LocalFeature.ValidFeatureName.Item.Name));

                if (index + 1 < validPath.Count)
                    FillResultPath(baseClass, ResolvedFeatureType, null, validPath, index + 1, resultPath);
            }
            else if (FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
            {
                ICompiledFeature SourceFeature = Instance.Feature;
                ITypeName ResolvedFeatureTypeName = SourceFeature.ResolvedAgentTypeName.Item;
                ICompiledType ResolvedFeatureType = SourceFeature.ResolvedAgentType.Item;

                Debug.Assert(baseType is ICompiledTypeWithFeature);
                ResolvedFeatureType.InstanciateType((ICompiledTypeWithFeature)baseType, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                IExpressionType PathResult;

                if (SourceFeature is IFunctionFeature AsFunctionFeature)
                    PathResult = AsFunctionFeature.MostCommonResult.Item;
                else
                    PathResult = new ExpressionType(ResolvedFeatureTypeName, ResolvedFeatureType, string.Empty);

                resultPath.Add(PathResult);

                if (index + 1 < validPath.Count)
                    FillResultPath(baseClass, ResolvedFeatureType, null, validPath, index + 1, resultPath);
            }
        }
    }
}
