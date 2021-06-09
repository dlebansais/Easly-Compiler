namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IObjectType.
    /// </summary>
    public interface IObjectType : BaseNode.IObjectType, INode, ISource
    {
        /// <summary>
        /// The resolved type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedTypeName { get; }

        /// <summary>
        /// The resolved type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedType { get; }

        /// <summary>
        /// Gets a string representation of the type.
        /// </summary>
        string TypeToString { get; }
    }

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
        /// <summary>
        /// Compares two types.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveIdenticalSignature(ICompiledType type1, ICompiledType type2)
        {
            bool Result = false;

            if (type1.GetType() == type2.GetType())
            {
                bool IsHandled = false;

                switch (type1)
                {
                    case IFormalGenericType AsFormalGenericType:
                        Result = FormalGenericType.TypesHaveIdenticalSignature((IFormalGenericType)type1, (IFormalGenericType)type2);
                        IsHandled = true;
                        break;

                    case IClassType AsClassType:
                        Result = ClassType.TypesHaveIdenticalSignature((IClassType)type1, (IClassType)type2);
                        IsHandled = true;
                        break;

                    case IFunctionType AsFunctionType:
                        Result = FunctionType.TypesHaveIdenticalSignature((IFunctionType)type1, (IFunctionType)type2);
                        IsHandled = true;
                        break;

                    case IProcedureType AsProcedureType:
                        Result = ProcedureType.TypesHaveIdenticalSignature((IProcedureType)type1, (IProcedureType)type2);
                        IsHandled = true;
                        break;

                    case IPropertyType AsPropertyType:
                        Result = PropertyType.TypesHaveIdenticalSignature((IPropertyType)type1, (IPropertyType)type2);
                        IsHandled = true;
                        break;

                    case IIndexerType AsIndexerType:
                        Result = IndexerType.TypesHaveIdenticalSignature((IIndexerType)type1, (IIndexerType)type2);
                        IsHandled = true;
                        break;

                    case ITupleType AsTupleType:
                        Result = TupleType.TypesHaveIdenticalSignature((ITupleType)type1, (ITupleType)type2);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return Result;
        }

        /// <summary>
        /// Checks that a type conforms to a base type.
        /// </summary>
        /// <param name="derivedType">The type to check.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="isConversionAllowed">True if the method should try to find a conversion path from base to derived.</param>
        public static bool TypeConformToBase(ICompiledType derivedType, ICompiledType baseType, bool isConversionAllowed)
        {
            return TypeConformToBase(derivedType, baseType, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed);
        }

        /// <summary>
        /// Checks that a type conforms to a base type.
        /// </summary>
        /// <param name="derivedType">The type to check.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="sourceLocation">The location for reporting errors.</param>
        /// <param name="isConversionAllowed">True if the method should try to find a conversion path from base to derived.</param>
        public static bool TypeConformToBase(ICompiledType derivedType, ICompiledType baseType, IErrorList errorList, ISource sourceLocation, bool isConversionAllowed)
        {
            if (isConversionAllowed)
            {
                IList<ICompiledType> DerivedTypeList = new List<ICompiledType>();
                ListConvertibleTypes(derivedType, DerivedTypeList, GetTypesConvertibleTo);

                IList<ICompiledType> BaseTypeList = new List<ICompiledType>();
                ListConvertibleTypes(baseType, BaseTypeList, GetTypesConvertibleFrom);

                return TypeConformToBase(DerivedTypeList, BaseTypeList, errorList, sourceLocation);
            }
            else
                return ConvertedTypeConformToBase(derivedType, baseType, errorList, sourceLocation);
        }

        private static void ListConvertibleTypes(ICompiledType type, IList<ICompiledType> typeList, Func<IClassType, IList<ICompiledType>> handler)
        {
            typeList.Add(type);

            if (type is IClassType AsClassType)
            {
                IList<ICompiledType> ConvertibleTypeList = handler(AsClassType);

                foreach (ICompiledType ConvertibleType in ConvertibleTypeList)
                    typeList.Add(ConvertibleType);
            }
        }

        private static IList<ICompiledType> GetTypesConvertibleFrom(IClassType type)
        {
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = type.FeatureTable;
            ISealableDictionary<IFeatureName, ICreationFeature> ConversionFromTable = type.BaseClass.ConversionFromTable;
            IList<ICompiledType> Result = new List<ICompiledType>();

            foreach (KeyValuePair<IFeatureName, ICreationFeature> Entry in ConversionFromTable)
            {
                ICreationFeature Feature = Entry.Value;

                foreach (ICommandOverload Overload in Feature.OverloadList)
                {
                    ISealableList<IParameter> ParameterTable = Overload.ParameterTable;
                    Debug.Assert(ParameterTable.IsSealed);
                    Debug.Assert(ParameterTable.Count == 1);

                    IParameter OverloadParameter = ParameterTable[0];
                    Debug.Assert(OverloadParameter.ResolvedParameter.ResolvedEffectiveType.IsAssigned);

                    ICompiledType OverloadParameterType = OverloadParameter.ResolvedParameter.ResolvedEffectiveType.Item;
                    Result.Add(OverloadParameterType);
                }
            }

            return Result;
        }

        private static IList<ICompiledType> GetTypesConvertibleTo(IClassType type)
        {
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = type.FeatureTable;
            ISealableDictionary<IFeatureName, IFunctionFeature> ConversionToTable = type.BaseClass.ConversionToTable;
            IList<ICompiledType> Result = new List<ICompiledType>();

            foreach (KeyValuePair<IFeatureName, IFunctionFeature> Entry in ConversionToTable)
            {
                IFunctionFeature Feature = Entry.Value;

                foreach (IQueryOverload Overload in Feature.OverloadList)
                {
                    ISealableList<IParameter> ParameterTable = Overload.ParameterTable;
                    Debug.Assert(ParameterTable.IsSealed);
                    Debug.Assert(ParameterTable.Count == 0);

                    ISealableList<IParameter> ResultTable = Overload.ResultTable;
                    Debug.Assert(ResultTable.IsSealed);
                    Debug.Assert(ResultTable.Count == 1);

                    IParameter OverloadResult = ResultTable[0];
                    Debug.Assert(OverloadResult.ResolvedParameter.ResolvedEffectiveType.IsAssigned);

                    ICompiledType OverloadResultType = OverloadResult.ResolvedParameter.ResolvedEffectiveType.Item;
                    Result.Add(OverloadResultType);
                }
            }

            return Result;
        }

        private static bool TypeConformToBase(IList<ICompiledType> derivedTypeList, IList<ICompiledType> baseTypeList, IErrorList errorList, ISource sourceLocation)
        {
            bool Success = false;
            IErrorList DirectErrorList = new ErrorList();

            for (int derivedIndex = 0; derivedIndex < derivedTypeList.Count; derivedIndex++)
            {
                ICompiledType DerivedType = derivedTypeList[derivedIndex];

                for (int baseIndex = 0; baseIndex < baseTypeList.Count; baseIndex++)
                {
                    ICompiledType BaseType = baseTypeList[baseIndex];

                    if (derivedIndex == 0 && baseIndex == 0)
                        Success |= ConvertedTypeConformToBase(DerivedType, BaseType, DirectErrorList, sourceLocation);
                    else
                        Success |= ConvertedTypeConformToBase(DerivedType, BaseType, ErrorList.Ignored, ErrorList.NoLocation);
                }
            }

            if (!Success)
                errorList.AddErrors(DirectErrorList);

            return Success;
        }

        private static bool ConvertedTypeConformToBase(ICompiledType derivedType, ICompiledType baseType, IErrorList errorList, ISource sourceLocation)
        {
            if (IsDirectDescendantOf(derivedType, baseType, sourceLocation))
                return true;
            else if (TypeConformDirectlyToBase(derivedType, baseType, errorList, sourceLocation))
                return true;
            else
                return false;
        }

        private static bool ConformToBaseAny(ICompiledType derivedType, ICompiledType baseType)
        {
            if (derivedType is IClassType DerivedClassType)
            {
                if (DerivedClassType.BaseClass.InheritanceTable.Count == 0)
                {
                    bool IsConforming = false;

                    IsConforming |= (baseType == ClassType.ClassAnyReferenceType || baseType == ClassType.ClassAnyType) && DerivedClassType.BaseClass.CopySpecification == BaseNode.CopySemantic.Reference;
                    IsConforming |= (baseType == ClassType.ClassAnyValueType || baseType == ClassType.ClassAnyType) && DerivedClassType.BaseClass.CopySpecification == BaseNode.CopySemantic.Value;
                    IsConforming |= baseType == ClassType.ClassAnyType && DerivedClassType.BaseClass.CopySpecification == BaseNode.CopySemantic.Any;

                    return IsConforming;
                }
            }

            return false;
        }

        private static bool IsDirectDescendantOf(ICompiledType derivedType, ICompiledType baseType, ISource sourceLocation)
        {
            bool Result = false;

            if (baseType is IClassType BaseClassType)
            {
                if (derivedType is IClassType DerivedClassType)
                    Result = IsDirectClassDescendantOf(DerivedClassType, BaseClassType, sourceLocation);
                else if (derivedType is IFormalGenericType DerivedFormalGenericType)
                    Result = IsDirectFormalGenericDescendantOf(DerivedFormalGenericType, BaseClassType);
            }

            return Result;
        }

        private static bool IsDirectClassDescendantOf(IClassType derivedType, IClassType baseType, ISource sourceLocation)
        {
            bool Result = false;

            Result |= ConformToBaseAny(derivedType, baseType);

            foreach (IInheritance Inheritance in derivedType.BaseClass.InheritanceList)
                if (Inheritance.ResolvedParentType.IsAssigned && Inheritance.ResolvedParentType.Item is IClassType Parent)
                {
                    Result |= TypeConformDirectlyToBase(Parent, baseType, ErrorList.Ignored, ErrorList.NoLocation);
                    Result |= IsDirectClassDescendantOf(Parent, baseType, sourceLocation);
                }

            return Result;
        }

        private static bool IsDirectFormalGenericDescendantOf(IFormalGenericType derivedType, IClassType baseType)
        {
            bool Result = false;

            Result |= ConformToBaseAny(derivedType, baseType);

            foreach (IConstraint Item in derivedType.FormalGeneric.ConstraintList)
                if (Item.ResolvedParentType.Item is IClassType Parent)
                    Result |= TypeConformDirectlyToBase(Parent, baseType, ErrorList.Ignored, ErrorList.NoLocation);

            return Result;
        }

        private static bool TypeConformDirectlyToBase(ICompiledType derivedType, ICompiledType baseType, IErrorList errorList, ISource sourceLocation)
        {
            if (derivedType == baseType)
                return true;

            if (baseType is IFormalGenericType AsFormalGenericBaseType)
                return TypeConformToFormalGenericType(derivedType, AsFormalGenericBaseType, errorList, sourceLocation);
            else if (derivedType is FormalGenericType AsFormalGenericDerivedType)
                return FormalGenericTypeConformToBase(AsFormalGenericDerivedType, baseType, errorList, sourceLocation);
            else
            {
                bool Result = false;
                bool IsHandled = false;

                switch (baseType)
                {
                    case IClassType AsClassType:
                        Result = TypeConformToClassType(derivedType, AsClassType, errorList, sourceLocation);
                        IsHandled = true;
                        break;

                    case IFunctionType AsFunctionType:
                        Result = TypeConformToFunctionType(derivedType, AsFunctionType, errorList, sourceLocation);
                        IsHandled = true;
                        break;

                    case IProcedureType AsProcedureType:
                        Result = TypeConformToProcedureType(derivedType, AsProcedureType, errorList, sourceLocation);
                        IsHandled = true;
                        break;

                    case IPropertyType AsPropertyType:
                        Result = TypeConformToPropertyType(derivedType, AsPropertyType, errorList, sourceLocation);
                        IsHandled = true;
                        break;

                    case IIndexerType AsIndexerType:
                        Result = TypeConformToIndexerType(derivedType, AsIndexerType, errorList, sourceLocation);
                        IsHandled = true;
                        break;

                    case ITupleType AsTupleType:
                        Result = TypeConformToTupleType(derivedType, AsTupleType, errorList, sourceLocation);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
                return Result;
            }
        }

        private static bool TypeConformToFormalGenericType(ICompiledType derivedType, IFormalGenericType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Result &= baseType.FormalGeneric.ResolvedConformanceTable.IsSealed;

            foreach (KeyValuePair<ITypeName, ICompiledType> ConformingEntry in baseType.FormalGeneric.ResolvedConformanceTable)
            {
                ICompiledType ConformingType = ConformingEntry.Value;
                Result &= TypeConformToBase(derivedType, ConformingType, errorList, sourceLocation, isConversionAllowed: false);
            }

            if (!derivedType.IsReference && baseType.IsReference)
            {
                errorList.AddError(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (!derivedType.IsValue && baseType.IsValue)
            {
                errorList.AddError(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool FormalGenericTypeConformToBase(IFormalGenericType derivedType, ICompiledType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Result &= derivedType.FormalGeneric.ResolvedConformanceTable.IsSealed;

            if (derivedType.FormalGeneric.ResolvedConformanceTable.Count > 0)
                Result &= FormalGenericTypeConformToBaseWithConformance(derivedType, baseType, errorList, sourceLocation);
            else if (baseType != ClassType.ClassAnyType && baseType != ClassType.ClassAnyReferenceType && baseType != ClassType.ClassAnyValueType)
            {
                errorList.AddError(new ErrorInsufficientConstraintConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.IsReference && !derivedType.IsReference)
            {
                errorList.AddError(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.IsValue && !derivedType.IsValue)
            {
                errorList.AddError(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool FormalGenericTypeConformToBaseWithConformance(IFormalGenericType derivedType, ICompiledType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool ConformantConstraintFound = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> ConformingEntry in derivedType.FormalGeneric.ResolvedConformanceTable)
            {
                ICompiledType ConformingType = ConformingEntry.Value;
                ConformantConstraintFound |= TypeConformToBase(ConformingType, baseType, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed: false);
            }

            if (!ConformantConstraintFound)
                errorList.AddError(new ErrorInsufficientConstraintConformance(sourceLocation, derivedType, baseType));

            return ConformantConstraintFound;
        }

        private static bool ExceptionListConformToBase(IList<IIdentifier> derivedExceptionIdentifierList, IList<IIdentifier> baseExceptionIdentifierList, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (derivedExceptionIdentifierList.Count > baseExceptionIdentifierList.Count)
            {
                errorList.AddError(new ErrorExceptionConformance(sourceLocation, derivedExceptionIdentifierList, baseExceptionIdentifierList));
                Result = false;
            }

            bool AllIdentifiersMatch = true;
            for (int i = 0; i < derivedExceptionIdentifierList.Count; i++)
            {
                IIdentifier DerivedIdentifier = derivedExceptionIdentifierList[i];

                bool Found = false;
                for (int j = 0; j < baseExceptionIdentifierList.Count; j++)
                {
                    IIdentifier BaseIdentifier = baseExceptionIdentifierList[j];
                    Found |= DerivedIdentifier.ValidText.Item == BaseIdentifier.ValidText.Item;
                }

                AllIdentifiersMatch &= Found;
            }

            if (!AllIdentifiersMatch)
            {
                errorList.AddError(new ErrorExceptionConformance(sourceLocation, derivedExceptionIdentifierList, baseExceptionIdentifierList));
                Result = false;
            }

            return Result;
        }

        /// <summary>
        /// Checks if two types have a common descendant.
        /// </summary>
        /// <param name="embeddingClass">The embedding class with all registered types.</param>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        public static bool TypesHaveCommonDescendant(IClass embeddingClass, ICompiledType type1, ICompiledType type2)
        {
            if (TypeConformToBase(type1, type2, isConversionAllowed: false))
                return true;

            if (TypeConformToBase(type2, type1, isConversionAllowed: false))
                return false; // Acceptable, but useless. Change this to a warning.

            bool IsConformant = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in embeddingClass.TypeTable)
            {
                ICompiledType Descendant = Entry.Value;

                IsConformant |= TypeConformToBase(Descendant, type1, isConversionAllowed: false) && TypeConformToBase(Descendant, type2, isConversionAllowed: false);
                IsConformant |= TypeConformToBase(Descendant, type2, isConversionAllowed: false) && TypeConformToBase(Descendant, type1, isConversionAllowed: false);
            }

            return IsConformant;
        }
    }
}
