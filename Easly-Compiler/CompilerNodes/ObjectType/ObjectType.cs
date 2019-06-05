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
    public static class ObjectType
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
                ListConvertibleTypes(derivedType, DerivedTypeList, GetTypesConvertibleFrom);

                IList<ICompiledType> BaseTypeList = new List<ICompiledType>();
                ListConvertibleTypes(baseType, BaseTypeList, GetTypesConvertibleTo);

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
                    Debug.Assert(OverloadParameter.ResolvedParameter.ResolvedFeatureType.IsAssigned);

                    ICompiledType OverloadParameterType = OverloadParameter.ResolvedParameter.ResolvedFeatureType.Item;
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
                    Debug.Assert(OverloadResult.ResolvedParameter.ResolvedFeatureType.IsAssigned);

                    ICompiledType OverloadResultType = OverloadResult.ResolvedParameter.ResolvedFeatureType.Item;
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

        private static bool TypeConformToClassType(ICompiledType derivedType, IClassType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result;

            if (derivedType is IClassType AsClassType)
                Result = ClassTypeConformToClassType(AsClassType, baseType, errorList, sourceLocation);
            else
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool ClassTypeConformToClassType(IClassType derivedType, IClassType baseType, IErrorList errorList, ISource sourceLocation)
        {
            if (derivedType.BaseClass != baseType.BaseClass)
            {
                errorList.AddError(new ErrorAncestorConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            bool Result = true;

            foreach (KeyValuePair<string, ICompiledType> DerivedEntry in derivedType.TypeArgumentTable)
            {
                string DerivedGenericName = DerivedEntry.Key;
                ICompiledType DerivedGenericType = DerivedEntry.Value;
                ICompiledType BaseGenericType = baseType.TypeArgumentTable[DerivedGenericName];

                Result &= TypeConformToBase(DerivedGenericType, BaseGenericType, errorList, sourceLocation, isConversionAllowed: false);
            }

            return Result;
        }

        private static bool TypeConformToFunctionType(ICompiledType derivedType, IFunctionType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToFunctionType(AsFunctionType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToFunctionType(AsPropertyType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToFunctionType(AsIndexerType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToFunctionType(IFunctionType derivedType, IFunctionType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false))
            {
                errorList.AddError(new ErrorBaseConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            foreach (IQueryOverloadType BaseOverload in baseType.OverloadList)
            {
                bool MatchingDerivedOverload = false;
                foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
                    MatchingDerivedOverload |= QueryOverloadConformToBase(DerivedOverload, BaseOverload, ErrorList.Ignored, ErrorList.NoLocation);

                if (!MatchingDerivedOverload)
                {
                    errorList.AddError(new ErrorOverloadMismatchConformance(sourceLocation, derivedType, baseType));
                    Result = false;
                }
            }

            return Result;
        }

        private static bool QueryOverloadConformToBase(IQueryOverloadType derivedType, IQueryOverloadType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (baseType.ParameterList.Count != derivedType.ParameterList.Count || baseType.ParameterEnd != derivedType.ParameterEnd)
            {
                errorList.AddError(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.ResultList.Count != derivedType.ResultList.Count)
            {
                errorList.AddError(new ErrorResultMismatchConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i < baseType.ParameterList.Count && i < derivedType.ParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.ParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.ParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            for (int i = 0; i < baseType.ResultList.Count && i < derivedType.ResultList.Count; i++)
            {
                IEntityDeclaration BaseResult = baseType.ResultList[i];
                IEntityDeclaration DerivedResult = derivedType.ResultList[i];

                Debug.Assert(BaseResult.ValidEntity.IsAssigned);
                Debug.Assert(BaseResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedResult.ValidEntity.Item.ResolvedFeatureType.Item, BaseResult.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool PropertyTypeConformToFunctionType(IPropertyType derivedType, IFunctionType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (derivedType.PropertyKind == BaseNode.UtilityType.WriteOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            IQueryOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count > 0 || SingleOverload.ResultList.Count != 1 || SingleOverload.ParameterEnd == BaseNode.ParameterEndStatus.Open)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            IEntityDeclaration OverloadResult = SingleOverload.ResultList[0];
            Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ValidEntity.Item.ResolvedFeatureType.Item, derivedType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool IndexerTypeConformToFunctionType(IIndexerType derivedType, IFunctionType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (derivedType.IndexerKind == BaseNode.UtilityType.WriteOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            IQueryOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != derivedType.IndexParameterList.Count || SingleOverload.ResultList.Count != 1 || SingleOverload.ParameterEnd != derivedType.ParameterEnd)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i < SingleOverload.ParameterList.Count && i < derivedType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = SingleOverload.ParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.IndexParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            IEntityDeclaration OverloadResult = SingleOverload.ResultList[0];
            Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ValidEntity.Item.ResolvedFeatureType.Item, derivedType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool TypeConformToProcedureType(ICompiledType derivedType, IProcedureType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToProcedureType(AsProcedureType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToProcedureType(AsPropertyType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToProcedureType(AsIndexerType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool ProcedureTypeConformToProcedureType(IProcedureType derivedType, IProcedureType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            IErrorList AllOverloadErrorList = new ErrorList();
            foreach (ICommandOverloadType BaseOverload in baseType.OverloadList)
            {
                bool MatchingDerivedOverload = false;
                foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
                {
                    IErrorList OverloadErrorList = new ErrorList();
                    if (!CommandOverloadConformToBase(DerivedOverload, BaseOverload, OverloadErrorList, DerivedOverload))
                    {
                        Debug.Assert(!OverloadErrorList.IsEmpty);
                        AllOverloadErrorList.AddError(OverloadErrorList.At(0));
                    }
                    else
                        MatchingDerivedOverload = true;
                }

                if (!MatchingDerivedOverload)
                {
                    errorList.AddErrors(AllOverloadErrorList);
                    errorList.AddError(new ErrorOverloadMismatchConformance(sourceLocation, derivedType, baseType));
                    Result = false;
                }
            }

            return Result;
        }

        private static bool CommandOverloadConformToBase(ICommandOverloadType derivedType, ICommandOverloadType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (baseType.ParameterList.Count != derivedType.ParameterList.Count || baseType.ParameterEnd != derivedType.ParameterEnd)
            {
                errorList.AddError(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i < baseType.ParameterList.Count && i < derivedType.ParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.ParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.ParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool PropertyTypeConformToProcedureType(IPropertyType derivedType, IProcedureType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (derivedType.PropertyKind == BaseNode.UtilityType.ReadOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            ICommandOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != 1 || SingleOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (SingleOverload.ParameterList.Count == 1)
            {
                IEntityDeclaration OverloadValue = SingleOverload.ParameterList[0];
                Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);
                Debug.Assert(OverloadValue.ValidEntity.IsAssigned);
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, OverloadValue.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
                Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);
            }

            return Result;
        }

        private static bool IndexerTypeConformToProcedureType(IIndexerType derivedType, IProcedureType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (derivedType.IndexerKind == BaseNode.UtilityType.ReadOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            ICommandOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != derivedType.IndexParameterList.Count + 1 || SingleOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i + 1 < SingleOverload.ParameterList.Count && i < derivedType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = SingleOverload.ParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.IndexParameterList[i];

                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            if (SingleOverload.ParameterList.Count == derivedType.IndexParameterList.Count + 1)
            {
                Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

                IEntityDeclaration LastParameter = SingleOverload.ParameterList[derivedType.IndexParameterList.Count];
                Debug.Assert(LastParameter.ValidEntity.IsAssigned);
                Debug.Assert(LastParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, LastParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool TypeConformToPropertyType(ICompiledType derivedType, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToPropertyType(AsFunctionType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToPropertyType(AsProcedureType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToPropertyType(AsPropertyType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToPropertyType(IFunctionType derivedType, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (baseType.PropertyKind != BaseNode.UtilityType.ReadOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            IErrorList AllOverloadErrorList = new ErrorList();
            bool MatchingDerivedOverload = false;
            foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
            {
                IErrorList OverloadErrorList = new ErrorList();
                if (!QueryOverloadHasPropertyConformingBase(DerivedOverload, baseType, OverloadErrorList, sourceLocation))
                {
                    Debug.Assert(!OverloadErrorList.IsEmpty);
                    AllOverloadErrorList.AddError(OverloadErrorList.At(0));
                }
                else
                    MatchingDerivedOverload = true;
            }

            if (!MatchingDerivedOverload)
            {
                errorList.AddErrors(AllOverloadErrorList);
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool QueryOverloadHasPropertyConformingBase(IQueryOverloadType derivedOverload, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (derivedOverload.ParameterList.Count > 0 || derivedOverload.ResultList.Count != 1 || derivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                errorList.AddError(new ErrorOverloadParameterMismatchConformance(sourceLocation, derivedOverload, baseType));
                Result = false;
            }

            if (derivedOverload.ResultList.Count == 1)
            {
                IEntityDeclaration OverloadResult = derivedOverload.ResultList[0];

                Debug.Assert(baseType.ResolvedEntityType.IsAssigned);
                Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
                Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool ProcedureTypeConformToPropertyType(IProcedureType derivedType, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (baseType.PropertyKind != BaseNode.UtilityType.WriteOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            IErrorList AllOverloadErrorList = new ErrorList();
            bool MatchingDerivedOverload = false;
            foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
            {
                IErrorList OverloadErrorList = new ErrorList();
                bool IsMatching = CommandOverloadHasPropertyConformingBase(DerivedOverload, baseType, OverloadErrorList, sourceLocation);
                if (!IsMatching)
                {
                    Debug.Assert(!OverloadErrorList.IsEmpty);
                    AllOverloadErrorList.AddError(OverloadErrorList.At(0));
                }

                MatchingDerivedOverload |= IsMatching;
            }

            if (!MatchingDerivedOverload)
            {
                errorList.AddErrors(AllOverloadErrorList);
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool CommandOverloadHasPropertyConformingBase(ICommandOverloadType derivedOverload, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (derivedOverload.ParameterList.Count != 1 || derivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                errorList.AddError(new ErrorOverloadParameterMismatchConformance(sourceLocation, derivedOverload, baseType));
                Result = false;
            }

            if (derivedOverload.ParameterList.Count == 1)
            {
                IEntityDeclaration OverloadValue = derivedOverload.ParameterList[0];
                Debug.Assert(OverloadValue.ValidEntity.IsAssigned);
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(OverloadValue.ValidEntity.Item.ResolvedFeatureType.Item, baseType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool PropertyTypeConformToPropertyType(IPropertyType derivedType, IPropertyType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if ((baseType.PropertyKind == BaseNode.UtilityType.ReadOnly && derivedType.PropertyKind == BaseNode.UtilityType.WriteOnly) ||
                (baseType.PropertyKind == BaseNode.UtilityType.WriteOnly && derivedType.PropertyKind == BaseNode.UtilityType.ReadOnly) ||
                (baseType.PropertyKind == BaseNode.UtilityType.ReadWrite && derivedType.PropertyKind != BaseNode.UtilityType.ReadWrite))
            {
                errorList.AddError(new ErrorGetterSetterConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);
            Debug.Assert(baseType.ResolvedEntityType.IsAssigned);

            /*
            if (!TypesHaveIdenticalSignature(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item))
            {
                //errorList.Add(new ConformanceError(sourceLocation));
                return false;
            }
            */

            Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);
            Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool TypeConformToIndexerType(ICompiledType derivedType, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToIndexerType(AsFunctionType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToIndexerType(AsProcedureType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToIndexerType(AsIndexerType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToIndexerType(IFunctionType derivedType, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result = TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (baseType.IndexerKind != BaseNode.UtilityType.ReadOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            bool MatchingOverload = false;
            foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
                MatchingOverload |= FunctionTypeConformToIndexerTypeOverloads(DerivedOverload, baseType, errorList, sourceLocation);

            if (!MatchingOverload)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool FunctionTypeConformToIndexerTypeOverloads(IQueryOverloadType derivedOverload, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Result &= derivedOverload.ParameterList.Count == baseType.IndexParameterList.Count && derivedOverload.ResultList.Count == 1 && derivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed;

            for (int i = 0; i < derivedOverload.ParameterList.Count && i < baseType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedOverload.ParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            IEntityDeclaration OverloadResult = derivedOverload.ResultList[0];

            Debug.Assert(baseType.ResolvedEntityType.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

            Result &= TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ValidEntity.Item.ResolvedFeatureType.Item, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool ProcedureTypeConformToIndexerType(IProcedureType derivedType, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if (baseType.IndexerKind != BaseNode.UtilityType.WriteOnly)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            bool MatchingOverload = false;
            foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
                MatchingOverload |= ProcedureTypeConformToIndexerTypeOverloads(DerivedOverload, baseType, errorList, sourceLocation);

            if (!MatchingOverload)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            return Result;
        }

        private static bool ProcedureTypeConformToIndexerTypeOverloads(ICommandOverloadType derivedOverload, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Result &= derivedOverload.ParameterList.Count == baseType.IndexParameterList.Count + 1 && derivedOverload.ParameterEnd == BaseNode.ParameterEndStatus.Closed;

            for (int i = 0; i + 1 < derivedOverload.ParameterList.Count && i < baseType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedOverload.ParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            if (derivedOverload.ParameterList.Count == baseType.IndexParameterList.Count + 1)
            {
                IEntityDeclaration OverloadValue = derivedOverload.ParameterList[baseType.IndexParameterList.Count];

                Debug.Assert(OverloadValue.ValidEntity.IsAssigned);
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(baseType.ResolvedEntityType.IsAssigned);

                Result &= TypeConformToBase(OverloadValue.ValidEntity.Item.ResolvedFeatureType.Item, baseType.ResolvedEntityType.Item, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed: false);
                Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);
            }

            return Result;
        }

        private static bool IndexerTypeConformToIndexerType(IIndexerType derivedType, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, errorList, sourceLocation, isConversionAllowed: false);

            if ((baseType.IndexerKind == BaseNode.UtilityType.ReadOnly && derivedType.IndexerKind == BaseNode.UtilityType.WriteOnly) ||
                (baseType.IndexerKind == BaseNode.UtilityType.WriteOnly && derivedType.IndexerKind == BaseNode.UtilityType.ReadOnly) ||
                (baseType.IndexerKind == BaseNode.UtilityType.ReadWrite && derivedType.IndexerKind != BaseNode.UtilityType.ReadWrite))
            {
                errorList.AddError(new ErrorGetterSetterConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            if (derivedType.IndexParameterList.Count != baseType.IndexParameterList.Count || derivedType.ParameterEnd != baseType.ParameterEnd)
            {
                errorList.AddError(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i < baseType.IndexParameterList.Count && i < derivedType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.IndexParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);
            Debug.Assert(baseType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);

            /*
            if (!TypesHaveIdenticalSignature(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item))
            {
                //errorList.Add(new ConformanceError(sourceLocation));
                return false;
            }
            */

            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);
            Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        private static bool TypeConformToTupleType(ICompiledType derivedType, ITupleType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                errorList.AddError(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                Result = TupleTypeConformToTupleType(AsTupleType, baseType, errorList, sourceLocation);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool TupleTypeConformToTupleType(ITupleType derivedType, ITupleType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            if (derivedType.EntityDeclarationList.Count < baseType.EntityDeclarationList.Count)
            {
                errorList.AddError(new ErrorFieldMismatchConformance(sourceLocation, derivedType, baseType));
                Result = false;
            }

            for (int i = 0; i < derivedType.EntityDeclarationList.Count && i < baseType.EntityDeclarationList.Count; i++)
            {
                IEntityDeclaration BaseField = baseType.EntityDeclarationList[i];
                IEntityDeclaration DerivedField = derivedType.EntityDeclarationList[i];

                Debug.Assert(DerivedField.ValidEntity.IsAssigned);
                Debug.Assert(DerivedField.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result = TypeConformToBase(DerivedField.ValidEntity.Item.ResolvedFeatureType.Item, BaseField.ValidEntity.Item.ResolvedFeatureType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            return Result;
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

            if (localScope.ContainsKey(ValidText))
                return GetQualifiedPathFinalTypeFromLocal(baseClass, baseType, localScope, validPath, index, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
                return GetQualifiedPathFinalTypeAsFeature(baseClass, baseType, localScope, validPath, index, errorList, Instance, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (index == 0 && index + 1 < validPath.Count && baseClass.ImportedClassTable.ContainsKey(ValidText) && baseClass.ImportedClassTable[ValidText].Item.Cloneable == BaseNode.CloneableStatus.Single)
                return GetQualifiedPathFinalTypeFromSingle(baseClass, validPath, index, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            else if (index + 1 == validPath.Count)
                return GetQualifiedPathFinalTypeAsDiscrete(baseType, ValidIdentifier, errorList, out finalDiscrete, out finalTypeName, out finalType);
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
                Debug.Assert(localScope[ValidText].ResolvedFeatureType.IsAssigned);

                ITypeName ResolvedFeatureTypeName = localScope[ValidText].ResolvedFeatureTypeName.Item;
                ICompiledType ResolvedFeatureType = localScope[ValidText].ResolvedFeatureType.Item;
                ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                ISealableDictionary<string, IScopeAttributeFeature> NewScope = ScopeFromType(ResolvedFeatureType);
                return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, NewScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            }
            else
            {
                finalFeature = localScope[ValidText];
                finalDiscrete = null;
                finalTypeName = finalFeature.ResolvedFeatureTypeName.Item;
                finalType = finalFeature.ResolvedFeatureType.Item;
                inheritBySideAttribute = false;
                return true;
            }
        }

        private static bool GetQualifiedPathFinalTypeAsFeature(IClass baseClass, ICompiledType baseType, ISealableDictionary<string, IScopeAttributeFeature> localScope, IList<IIdentifier> validPath, int index, IErrorList errorList, IFeatureInstance instance, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            ICompiledFeature SourceFeature = instance.Feature;
            ITypeName ResolvedFeatureTypeName = SourceFeature.ResolvedFeatureTypeName.Item;
            ICompiledType ResolvedFeatureType = SourceFeature.ResolvedFeatureType.Item;

            IPathParticipatingType PathParticipatingType = ResolvedFeatureType as IPathParticipatingType;
            Debug.Assert(PathParticipatingType != null);

            ResolvedFeatureType = PathParticipatingType.TypeAsDestinationOrSource;

            Debug.Assert(baseType is IClassType);
            ResolvedFeatureType.InstanciateType((IClassType)baseType, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

            if (index + 1 < validPath.Count)
            {
                ISealableDictionary<string, IScopeAttributeFeature> NewScope = ScopeFromType(ResolvedFeatureType);
                return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, NewScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
            }
            else
            {
                finalFeature = SourceFeature;
                finalDiscrete = null;
                finalTypeName = ResolvedFeatureTypeName;
                finalType = ResolvedFeatureType;
                inheritBySideAttribute = instance.InheritBySideAttribute;
                return true;
            }
        }

        private static bool GetQualifiedPathFinalTypeFromSingle(IClass baseClass, IList<IIdentifier> validPath, int index, IErrorList errorList, out ICompiledFeature finalFeature, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType, out bool inheritBySideAttribute)
        {
            IIdentifier ValidIdentifier = validPath[index];
            string ValidText = ValidIdentifier.ValidText.Item;

            IImportedClass Imported = baseClass.ImportedClassTable[ValidText];
            ITypeName ResolvedFeatureTypeName = Imported.ResolvedClassTypeName.Item;
            ICompiledType ResolvedFeatureType = Imported.ResolvedClassType.Item;
            ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

            ISealableDictionary<string, IScopeAttributeFeature> NewScope = ScopeFromType(ResolvedFeatureType);
            return GetQualifiedPathFinalType(baseClass, ResolvedFeatureType, NewScope, validPath, index + 1, errorList, out finalFeature, out finalDiscrete, out finalTypeName, out finalType, out inheritBySideAttribute);
        }

        private static bool GetQualifiedPathFinalTypeAsDiscrete(ICompiledType baseType, IIdentifier validIdentifier, IErrorList errorList, out IDiscrete finalDiscrete, out ITypeName finalTypeName, out ICompiledType finalType)
        {
            finalDiscrete = null;
            finalTypeName = null;
            finalType = null;

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
            else
            {
                errorList.AddError(new ErrorUnknownIdentifier(validIdentifier, ValidText));
                return false;
            }
        }

        private static ISealableDictionary<string, IScopeAttributeFeature> ScopeFromType(ICompiledType type)
        {
            ISealableDictionary<string, IScopeAttributeFeature> Result = new SealableDictionary<string, IScopeAttributeFeature>();

            if (type is ITupleType AsTupleType)
            {
                foreach (IEntityDeclaration Item in AsTupleType.EntityDeclarationList)
                {
                    IName EntityName = (IName)Item.EntityName;
                    string Name = EntityName.ValidText.Item;
                    IScopeAttributeFeature Attribute = Item.ValidEntity.Item;

                    Result.Add(Name, Attribute);
                }
            }

            return Result;
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
                ITypeName ResolvedFeatureTypeName = localScope[ValidText].ResolvedFeatureTypeName.Item;
                ICompiledType ResolvedFeatureType = localScope[ValidText].ResolvedFeatureType.Item;
                ResolvedFeatureType.InstanciateType(baseClass.ResolvedClassType.Item, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                resultPath.Add(new ExpressionType(ResolvedFeatureTypeName, ResolvedFeatureType, string.Empty));

                if (index + 1 < validPath.Count)
                    FillResultPath(baseClass, ResolvedFeatureType, null, validPath, index + 1, resultPath);
            }
            else if (FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
            {
                ICompiledFeature SourceFeature = Instance.Feature;
                ITypeName ResolvedFeatureTypeName = SourceFeature.ResolvedFeatureTypeName.Item;
                ICompiledType ResolvedFeatureType = SourceFeature.ResolvedFeatureType.Item;

                IPathParticipatingType PathParticipatingType = ResolvedFeatureType as IPathParticipatingType;
                Debug.Assert(PathParticipatingType != null);

                ResolvedFeatureType = PathParticipatingType.TypeAsDestinationOrSource;

                Debug.Assert(baseType is IClassType);
                ResolvedFeatureType.InstanciateType((IClassType)baseType, ref ResolvedFeatureTypeName, ref ResolvedFeatureType);

                resultPath.Add(new ExpressionType(ResolvedFeatureTypeName, ResolvedFeatureType, string.Empty));

                if (index + 1 < validPath.Count)
                    FillResultPath(baseClass, ResolvedFeatureType, null, validPath, index + 1, resultPath);
            }
        }
    }
}
