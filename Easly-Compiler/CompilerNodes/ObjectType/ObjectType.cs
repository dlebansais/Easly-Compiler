namespace CompilerNode
{
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
            if (type1.GetType() != type2.GetType())
                return false;

            bool IsHandled = false;
            bool Result = false;

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

            return Result;
        }

        /// <summary>
        /// Checks that a type conforms to a base type.
        /// </summary>
        /// <param name="derivedType">The type to check.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="substitutionTypeTable">A table of type substitutions describing the context of the check.</param>
        /// <param name="errorList">The list of errors found if <paramref name="reportError"/> is set.</param>
        /// <param name="sourceLocation">The location for reporting errors.</param>
        /// <param name="reportError">True if the checked type must conform to the base type; False if just exploring alternatives.</param>
        public static bool TypeConformToBase(ICompiledType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result;

            if (derivedType.ConformanceTable.IsSealed)
            {
                Result = TypeConformToBaseWithTable(derivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                Debug.Assert(Result == TypeConformToBaseWithoutTable(derivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError));
            }
            else
                Result = TypeConformToBaseWithoutTable(derivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool TypeConformToBaseWithTable(ICompiledType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool IsConforming = false;

            Debug.Assert(derivedType.ConformanceTable.IsSealed);

            IsConforming |= ConformToBaseAny(derivedType, baseType);

            IList<IError> DirectConformanceErrorList = new List<IError>();
            IsConforming |= TypeConformDirectlyToBase(derivedType, baseType, substitutionTypeTable, DirectConformanceErrorList, sourceLocation, reportError);

            IList<IError> FakeErrorList = new List<IError>();
            foreach (KeyValuePair<ITypeName, ICompiledType> Entry in derivedType.ConformanceTable)
            {
                ICompiledType Conformance = Entry.Value;
                IsConforming |= TypeConformToBase(Conformance, baseType, substitutionTypeTable, FakeErrorList, sourceLocation, reportError);
            }

            if (!IsConforming && reportError)
                foreach (IError Error in DirectConformanceErrorList)
                    errorList.Add(Error);

            return IsConforming;
        }

        private static bool TypeConformToBaseWithoutTable(ICompiledType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (IsDirectDescendantOf(derivedType, baseType, substitutionTypeTable, sourceLocation))
                return true;
            else
                return TypeConformDirectlyToBase(derivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
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

        private static bool IsDirectDescendantOf(ICompiledType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, ISource sourceLocation)
        {
            bool Result = false;

            if (baseType is IClassType BaseClassType)
            {
                if (derivedType is IClassType DerivedClassType)
                    Result = IsDirectClassDescendantOf(DerivedClassType, BaseClassType, substitutionTypeTable, sourceLocation);
                else if (derivedType is IFormalGenericType DerivedFormalGenericType)
                    Result = IsDirectFormalGenericDescendantOf(DerivedFormalGenericType, BaseClassType, substitutionTypeTable, sourceLocation);
            }

            return Result;
        }

        private static bool IsDirectClassDescendantOf(IClassType derivedType, IClassType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, ISource sourceLocation)
        {
            IList<IError> FakeErrorList = new List<IError>();
            bool Result = false;

            Result |= ConformToBaseAny(derivedType, baseType);

            foreach (IInheritance Inheritance in derivedType.BaseClass.InheritanceList)
                if (Inheritance.ResolvedParentType.IsAssigned && Inheritance.ResolvedParentType.Item is IClassType Parent)
                {
                    Result |= TypeConformDirectlyToBase(Parent, baseType, substitutionTypeTable, FakeErrorList, sourceLocation, false);
                    Result |= IsDirectClassDescendantOf(Parent, baseType, substitutionTypeTable, sourceLocation);
                }

            return Result;
        }

        private static bool IsDirectFormalGenericDescendantOf(IFormalGenericType derivedType, IClassType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, ISource sourceLocation)
        {
            IList<IError> FakeErrorList = new List<IError>();
            bool Result = false;

            Result |= ConformToBaseAny(derivedType, baseType);

            foreach (IConstraint Item in derivedType.FormalGeneric.ConstraintList)
                if (Item.ResolvedParentType.Item is IClassType Parent)
                    Result |= TypeConformDirectlyToBase(Parent, baseType, substitutionTypeTable, FakeErrorList, sourceLocation, false);

            return Result;
        }

        private static bool TypeConformDirectlyToBase(ICompiledType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            while (substitutionTypeTable.ContainsKey(derivedType))
                derivedType = substitutionTypeTable[derivedType];

            while (substitutionTypeTable.ContainsKey(baseType))
                baseType = substitutionTypeTable[baseType];

            if (derivedType == baseType)
                return true;

            if (baseType is IFormalGenericType AsFormalGenericBaseType)
                return TypeConformToFormalGenericType(derivedType, AsFormalGenericBaseType, substitutionTypeTable, errorList, sourceLocation, reportError);
            else if (derivedType is FormalGenericType AsFormalGenericDerivedType)
                return FormalGenericTypeConformToBase(AsFormalGenericDerivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
            else
            {
                bool Result = false;
                bool IsHandled = false;

                switch (baseType)
                {
                    case IClassType AsClassType:
                        Result = TypeConformToClassType(derivedType, AsClassType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;

                    case IFunctionType AsFunctionType:
                        Result = TypeConformToFunctionType(derivedType, AsFunctionType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;

                    case IProcedureType AsProcedureType:
                        Result = TypeConformToProcedureType(derivedType, AsProcedureType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;

                    case IPropertyType AsPropertyType:
                        Result = TypeConformToPropertyType(derivedType, AsPropertyType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;

                    case IIndexerType AsIndexerType:
                        Result = TypeConformToIndexerType(derivedType, AsIndexerType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;

                    case ITupleType AsTupleType:
                        Result = TypeConformToTupleType(derivedType, AsTupleType, substitutionTypeTable, errorList, sourceLocation, reportError);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
                return Result;
            }
        }

        private static bool TypeConformToFormalGenericType(ICompiledType derivedType, IFormalGenericType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Result &= baseType.FormalGeneric.ResolvedConformanceTable.IsSealed;

            /* TODO: Is this bad to not have any constraint?
            if (baseType.FormalGeneric.ResolvedConformanceTable.Count == 0)
            {
                //errorList.Add(new ConformanceError(sourceLocation));
                return false;
            }*/

            foreach (KeyValuePair<ITypeName, ICompiledType> ConformingEntry in baseType.FormalGeneric.ResolvedConformanceTable)
            {
                ICompiledType ConformingType = ConformingEntry.Value;
                Result &= TypeConformToBase(derivedType, ConformingType, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            if (!derivedType.IsReference && baseType.IsReference)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (!derivedType.IsValue && baseType.IsValue)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            return Result;
        }

        private static bool FormalGenericTypeConformToBase(IFormalGenericType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Result &= derivedType.FormalGeneric.ResolvedConformanceTable.IsSealed;

            if (derivedType.FormalGeneric.ResolvedConformanceTable.Count > 0)
                Result &= FormalGenericTypeConformToBaseWithConformance(derivedType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
            else if (baseType != ClassType.ClassAnyType && baseType != ClassType.ClassAnyReferenceType && baseType != ClassType.ClassAnyValueType)
            {
                if (reportError)
                    errorList.Add(new ErrorInsufficientConstraintConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.IsReference && !derivedType.IsReference)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.IsValue && !derivedType.IsValue)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            return Result;
        }

        private static bool FormalGenericTypeConformToBaseWithConformance(IFormalGenericType derivedType, ICompiledType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool ConformantConstraintFound = false;

            foreach (KeyValuePair<ITypeName, ICompiledType> ConformingEntry in derivedType.FormalGeneric.ResolvedConformanceTable)
            {
                ICompiledType ConformingType = ConformingEntry.Value;
                IList<IError> FakeErrorList = new List<IError>();

                ConformantConstraintFound |= TypeConformToBase(ConformingType, baseType, substitutionTypeTable, FakeErrorList, sourceLocation, false);
            }

            if (!ConformantConstraintFound)
            {
                if (reportError)
                    errorList.Add(new ErrorInsufficientConstraintConformance(sourceLocation, derivedType, baseType));
            }

            return ConformantConstraintFound;
        }

        private static bool TypeConformToClassType(ICompiledType derivedType, IClassType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result;

            if (derivedType is IClassType AsClassType)
                Result = ClassTypeConformToClassType(AsClassType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
            else
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            return Result;
        }

        private static bool ClassTypeConformToClassType(IClassType derivedType, IClassType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (derivedType.BaseClass != baseType.BaseClass)
            {
                if (reportError)
                    errorList.Add(new ErrorAncestorConformance(sourceLocation, derivedType, baseType));

                return false;
            }

            bool Result = true;

            foreach (KeyValuePair<string, ICompiledType> DerivedEntry in derivedType.TypeArgumentTable)
            {
                string DerivedGenericName = DerivedEntry.Key;
                ICompiledType DerivedGenericType = DerivedEntry.Value;
                ICompiledType BaseGenericType = baseType.TypeArgumentTable[DerivedGenericName];

                Result &= TypeConformToBase(DerivedGenericType, BaseGenericType, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            return Result;
        }

        private static bool TypeConformToFunctionType(ICompiledType derivedType, IFunctionType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToFunctionType(AsFunctionType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToFunctionType(AsPropertyType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToFunctionType(AsIndexerType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToFunctionType(IFunctionType derivedType, IFunctionType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
            {
                if (reportError)
                    errorList.Add(new ErrorBaseConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            foreach (IQueryOverloadType BaseOverload in baseType.OverloadList)
            {
                IList<IError> FakeErrorList = new List<IError>();

                bool MatchingDerivedOverload = false;
                foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
                    MatchingDerivedOverload |= QueryOverloadConformToBase(DerivedOverload, BaseOverload, substitutionTypeTable, FakeErrorList, sourceLocation, false);

                if (!MatchingDerivedOverload)
                {
                    if (reportError)
                        errorList.Add(new ErrorOverloadMismatchConformance(sourceLocation, derivedType, baseType));

                    Result = false;
                }
            }

            return Result;
        }

        private static bool QueryOverloadConformToBase(IQueryOverloadType derivedType, IQueryOverloadType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            if (baseType.ParameterList.Count != derivedType.ParameterList.Count || baseType.ParameterEnd != derivedType.ParameterEnd)
            {
                if (reportError)
                    errorList.Add(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.ResultList.Count != derivedType.ResultList.Count)
            {
                if (reportError)
                    errorList.Add(new ErrorResultMismatchConformance(sourceLocation, derivedType, baseType));

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

                Result &= TypeConformToBase(BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            for (int i = 0; i < baseType.ResultList.Count && i < derivedType.ResultList.Count; i++)
            {
                IEntityDeclaration BaseResult = baseType.ResultList[i];
                IEntityDeclaration DerivedResult = derivedType.ResultList[i];

                Debug.Assert(BaseResult.ValidEntity.IsAssigned);
                Debug.Assert(BaseResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                Result &= TypeConformToBase(DerivedResult.ValidEntity.Item.ResolvedFeatureType.Item, BaseResult.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool PropertyTypeConformToFunctionType(IPropertyType derivedType, IFunctionType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);

            if (derivedType.PropertyKind == BaseNode.UtilityType.WriteOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            IQueryOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count > 0 || SingleOverload.ResultList.Count != 1 || SingleOverload.ParameterEnd == BaseNode.ParameterEndStatus.Open)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            IEntityDeclaration OverloadResult = SingleOverload.ResultList[0];
            Debug.Assert(OverloadResult.ResolvedEntityType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ResolvedEntityType.Item, derivedType.ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool IndexerTypeConformToFunctionType(IIndexerType derivedType, IFunctionType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);

            if (derivedType.IndexerKind == BaseNode.UtilityType.WriteOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            IQueryOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != derivedType.IndexParameterList.Count || SingleOverload.ResultList.Count != 1 || SingleOverload.ParameterEnd != derivedType.ParameterEnd)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

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

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            IEntityDeclaration OverloadResult = SingleOverload.ResultList[0];
            Debug.Assert(OverloadResult.ResolvedEntityType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ResolvedEntityType.Item, derivedType.ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool TypeConformToProcedureType(ICompiledType derivedType, IProcedureType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToProcedureType(AsProcedureType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToProcedureType(AsPropertyType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToProcedureType(AsIndexerType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool ProcedureTypeConformToProcedureType(IProcedureType derivedType, IProcedureType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);

            foreach (ICommandOverloadType BaseOverload in baseType.OverloadList)
            {
                IList<IError> FakeErrorList = new List<IError>();

                bool MatchingDerivedOverload = false;
                foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
                    MatchingDerivedOverload |= CommandOverloadConformToBase(DerivedOverload, BaseOverload, substitutionTypeTable, FakeErrorList, sourceLocation, false);

                if (!MatchingDerivedOverload)
                {
                    if (reportError)
                        errorList.Add(new ErrorOverloadMismatchConformance(sourceLocation, derivedType, baseType));

                    Result = false;
                }
            }

            return Result;
        }

        private static bool CommandOverloadConformToBase(ICommandOverloadType derivedType, ICommandOverloadType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            if (baseType.ParameterList.Count != derivedType.ParameterList.Count || baseType.ParameterEnd != derivedType.ParameterEnd)
            {
                if (reportError)
                    errorList.Add(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));

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

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool PropertyTypeConformToProcedureType(IPropertyType derivedType, IProcedureType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = true;

            Debug.Assert(derivedType.ResolvedBaseType.IsAssigned);
            Debug.Assert(baseType.ResolvedBaseType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);

            if (derivedType.PropertyKind == BaseNode.UtilityType.ReadOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            ICommandOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != 1 || SingleOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
            }

            IEntityDeclaration OverloadValue = SingleOverload.ParameterList[0];
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);
            Debug.Assert(OverloadValue.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, OverloadValue.ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError);
            Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation, reportError);

            return Result;
        }

        private static bool IndexerTypeConformToProcedureType(IIndexerType derivedType, IProcedureType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (derivedType.IndexerKind == BaseNode.UtilityType.ReadOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            if (baseType.OverloadList.Count > 1)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            ICommandOverloadType SingleOverload = baseType.OverloadList[0];
            if (SingleOverload.ParameterList.Count != derivedType.IndexParameterList.Count + 1 || SingleOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            for (int i = 0; i + 1 < SingleOverload.ParameterList.Count && i + 1 < derivedType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = SingleOverload.ParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.IndexParameterList[i];

                if (!DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned || !BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned)
                    return false;

                if (!TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                    return false;
            }

            if (!derivedType.ResolvedEntityType.IsAssigned || !SingleOverload.ParameterList[derivedType.IndexParameterList.Count].ResolvedEntityType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedEntityType.Item, SingleOverload.ParameterList[derivedType.IndexParameterList.Count].ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (!ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            return true;
        }

        private static bool TypeConformToPropertyType(ICompiledType derivedType, IPropertyType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToPropertyType(AsFunctionType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToPropertyType(AsProcedureType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                Result = PropertyTypeConformToPropertyType(AsPropertyType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToPropertyType(IFunctionType derivedType, IPropertyType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (baseType.PropertyKind != BaseNode.UtilityType.ReadOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            bool MatchingOverload = false;
            foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
            {
                if (DerivedOverload.ParameterList.Count > 0 || DerivedOverload.ResultList.Count != 1 || DerivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
                    continue;

                IList<IError> FakeErrorList = new List<IError>();
                IEntityDeclaration OverloadResult = DerivedOverload.ResultList[0];

                if (!baseType.ResolvedEntityType.IsAssigned || !OverloadResult.ResolvedEntityType.IsAssigned)
                    return false;

                if (!TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ResolvedEntityType.Item, substitutionTypeTable, FakeErrorList, sourceLocation, reportError))
                    continue;

                if (!ExceptionListConformToBase(DerivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation, reportError))
                    return false;

                MatchingOverload = true;
                break;
            }
            if (!MatchingOverload)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            return true;
        }

        private static bool ProcedureTypeConformToPropertyType(IProcedureType derivedType, IPropertyType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (baseType.PropertyKind != BaseNode.UtilityType.WriteOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            bool MatchingOverload = false;
            foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
            {
                if (DerivedOverload.ParameterList.Count != 1 || DerivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
                    continue;

                IList<IError> FakeErrorList = new List<IError>();

                IEntityDeclaration OverloadValue = DerivedOverload.ParameterList[0];
                if (!OverloadValue.ResolvedEntityType.IsAssigned || !baseType.ResolvedEntityType.IsAssigned)
                    return false;

                if (!TypeConformToBase(OverloadValue.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, substitutionTypeTable, FakeErrorList, sourceLocation, reportError))
                    continue;

                if (!ExceptionListConformToBase(DerivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation, reportError))
                    return false;

                MatchingOverload = true;
                break;
            }
            if (!MatchingOverload)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            return true;
        }

        private static bool PropertyTypeConformToPropertyType(IPropertyType derivedType, IPropertyType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if ((baseType.PropertyKind == BaseNode.UtilityType.ReadOnly && derivedType.PropertyKind == BaseNode.UtilityType.WriteOnly) ||
                (baseType.PropertyKind == BaseNode.UtilityType.WriteOnly && derivedType.PropertyKind == BaseNode.UtilityType.ReadOnly) ||
                (baseType.PropertyKind == BaseNode.UtilityType.ReadWrite && derivedType.PropertyKind != BaseNode.UtilityType.ReadWrite))
            {
                if (reportError)
                    errorList.Add(new ErrorGetterSetterConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            if (!derivedType.ResolvedEntityType.IsAssigned || !baseType.ResolvedEntityType.IsAssigned)
                return false;

            /*
            if (!TypesHaveIdenticalSignature(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item))
            {
                //errorList.Add(new ConformanceError(sourceLocation));
                return false;
            }
            */

            if (!TypeConformToBase(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (!ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            if (!ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            return true;
        }

        private static bool TypeConformToIndexerType(ICompiledType derivedType, IIndexerType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                Result = FunctionTypeConformToIndexerType(AsFunctionType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                Result = ProcedureTypeConformToIndexerType(AsProcedureType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                Result = IndexerTypeConformToIndexerType(AsIndexerType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                Result = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool FunctionTypeConformToIndexerType(IFunctionType derivedType, IIndexerType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (baseType.IndexerKind != BaseNode.UtilityType.ReadOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            bool MatchingOverload = false;
            foreach (IQueryOverloadType DerivedOverload in derivedType.OverloadList)
            {
                if (FunctionTypeConformToIndexerTypeOverloads(DerivedOverload, baseType, substitutionTypeTable, errorList, sourceLocation, reportError, ref MatchingOverload))
                    continue;

                if (!MatchingOverload)
                    return false;
                else
                    break;
            }
            if (!MatchingOverload)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            return true;
        }

        private static bool FunctionTypeConformToIndexerTypeOverloads(IQueryOverloadType derivedOverload, IIndexerType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError, ref bool matchingOverload)
        {
            if (derivedOverload.ParameterList.Count != baseType.IndexParameterList.Count || derivedOverload.ResultList.Count != 1 || derivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
                return true;

            for (int i = 0; i < derivedOverload.ParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedOverload.ParameterList[i];

                if (!DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned || !BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned)
                    return false;

                if (!TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                    return false;
            }

            IList<IError> FakeErrorList = new List<IError>();
            IEntityDeclaration OverloadResult = derivedOverload.ResultList[0];

            if (!baseType.ResolvedEntityType.IsAssigned || !OverloadResult.ResolvedEntityType.IsAssigned)
                return false;

            if (!TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ResolvedEntityType.Item, substitutionTypeTable, FakeErrorList, sourceLocation, reportError))
                return true;

            if (!ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            matchingOverload = true;
            return false;
        }

        private static bool ProcedureTypeConformToIndexerType(IProcedureType derivedType, IIndexerType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if (baseType.IndexerKind != BaseNode.UtilityType.WriteOnly)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            bool MatchingOverload = false;
            foreach (ICommandOverloadType DerivedOverload in derivedType.OverloadList)
            {
                if (DerivedOverload.ParameterList.Count != baseType.IndexParameterList.Count + 1 || DerivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed)
                    continue;

                for (int i = 0; i < DerivedOverload.ParameterList.Count; i++)
                {
                    IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                    IEntityDeclaration DerivedParameter = DerivedOverload.ParameterList[i];

                    if (!DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned || !BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned)
                        return false;

                    if (!TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                        return false;
                }

                IList<IError> FakeErrorList = new List<IError>();
                IEntityDeclaration OverloadValue = DerivedOverload.ParameterList[baseType.IndexParameterList.Count];

                if (!OverloadValue.ResolvedEntityType.IsAssigned || !baseType.ResolvedEntityType.IsAssigned)
                    return false;

                if (!TypeConformToBase(OverloadValue.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, substitutionTypeTable, FakeErrorList, sourceLocation, reportError))
                    continue;

                if (!ExceptionListConformToBase(DerivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation, reportError))
                    return false;

                MatchingOverload = true;
                break;
            }
            if (!MatchingOverload)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            return true;
        }

        private static bool IndexerTypeConformToIndexerType(IIndexerType derivedType, IIndexerType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!derivedType.ResolvedBaseType.IsAssigned || !baseType.ResolvedBaseType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedBaseType.Item, baseType.ResolvedBaseType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            if ((baseType.IndexerKind == BaseNode.UtilityType.ReadOnly && derivedType.IndexerKind == BaseNode.UtilityType.WriteOnly) ||
                (baseType.IndexerKind == BaseNode.UtilityType.WriteOnly && derivedType.IndexerKind == BaseNode.UtilityType.ReadOnly) ||
                (baseType.IndexerKind == BaseNode.UtilityType.ReadWrite && derivedType.IndexerKind != BaseNode.UtilityType.ReadWrite))
            {
                if (reportError)
                    errorList.Add(new ErrorGetterSetterConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            if (derivedType.IndexParameterList.Count != baseType.IndexParameterList.Count || derivedType.ParameterEnd != baseType.ParameterEnd)
            {
                if (reportError)
                    errorList.Add(new ErrorParameterMismatchConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            for (int i = 0; i < baseType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedType.IndexParameterList[i];

                if (!DerivedParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned || !BaseParameter.ValidEntity.Item.ResolvedFeatureType.IsAssigned)
                    return false;

                if (!TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedFeatureType.Item, BaseParameter.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                    return false;
            }

            if (!derivedType.ResolvedEntityType.IsAssigned || !baseType.ResolvedEntityType.IsAssigned)
                return false;

            if (!TypeConformToBase(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                return false;

            /*
            if (!TypesHaveIdenticalSignature(derivedType.ResolvedEntityType.Item, baseType.ResolvedEntityType.Item))
            {
                //errorList.Add(new ConformanceError(sourceLocation));
                return false;
            }
            */

            if (!ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            if (!ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation, reportError))
                return false;

            return true;
        }

        private static bool TypeConformToTupleType(ICompiledType derivedType, ITupleType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            bool Result = false;
            bool IsHandled = false;

            if (derivedType is IClassType AsClassType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IFunctionType AsFunctionType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IProcedureType AsProcedureType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IPropertyType AsPropertyType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is IIndexerType AsIndexerType)
            {
                if (reportError)
                    errorList.Add(new ErrorTypeKindConformance(sourceLocation, derivedType, baseType));

                Result = false;
                IsHandled = true;
            }
            else if (derivedType is ITupleType AsTupleType)
            {
                Result = TupleTypeConformToTupleType(AsTupleType, baseType, substitutionTypeTable, errorList, sourceLocation, reportError);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        private static bool TupleTypeConformToTupleType(ITupleType derivedType, ITupleType baseType, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (derivedType.EntityDeclarationList.Count < baseType.EntityDeclarationList.Count)
            {
                if (reportError)
                    errorList.Add(new ErrorFieldMismatchConformance(sourceLocation, derivedType, baseType));
                return false;
            }

            for (int i = 0; i < baseType.EntityDeclarationList.Count; i++)
            {
                IEntityDeclaration BaseField = baseType.EntityDeclarationList[i];
                IEntityDeclaration DerivedField = derivedType.EntityDeclarationList[i];

                Debug.Assert(DerivedField.ValidEntity.IsAssigned);
                Debug.Assert(DerivedField.ValidEntity.Item.ResolvedFeatureType.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.Item.ResolvedFeatureType.IsAssigned);

                if (!TypeConformToBase(DerivedField.ValidEntity.Item.ResolvedFeatureType.Item, BaseField.ValidEntity.Item.ResolvedFeatureType.Item, substitutionTypeTable, errorList, sourceLocation, reportError))
                    return false;
            }

            return true;
        }

        private static bool TypeConformToGeneric(ICompiledType actual, IFormalGenericType formal, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (!formal.FormalGeneric.ResolvedConformanceTable.IsSealed)
                return false;

            foreach (KeyValuePair<ITypeName, ICompiledType> ConformingEntry in formal.FormalGeneric.ResolvedConformanceTable)
            {
                ICompiledType ConformingType = ConformingEntry.Value;
                if (!TypeConformToBase(actual, ConformingType, substitutionTypeTable, errorList, sourceLocation, reportError))
                    return false;
            }

            if (!actual.IsReference && formal.IsReference)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, actual, formal));
                return false;
            }

            if (!actual.IsValue && formal.IsValue)
            {
                if (reportError)
                    errorList.Add(new ErrorReferenceValueConformance(sourceLocation, actual, formal));
                return false;
            }

            return true;
        }

        private static bool ExceptionListConformToBase(IList<IIdentifier> derivedExceptionIdentifierList, IList<IIdentifier> baseExceptionIdentifierList, IList<IError> errorList, ISource sourceLocation, bool reportError)
        {
            if (derivedExceptionIdentifierList.Count > baseExceptionIdentifierList.Count)
            {
                if (reportError)
                    errorList.Add(new ErrorExceptionConformance(sourceLocation, derivedExceptionIdentifierList, baseExceptionIdentifierList));
                return false;
            }

            for (int i = 0; i < derivedExceptionIdentifierList.Count; i++)
            {
                IIdentifier DerivedIdentifier = derivedExceptionIdentifierList[i];

                bool Found = false;
                for (int j = 0; j < baseExceptionIdentifierList.Count; j++)
                {
                    IIdentifier BaseIdentifier = baseExceptionIdentifierList[j];

                    if (DerivedIdentifier.ValidText.Item == BaseIdentifier.ValidText.Item)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    if (reportError)
                        errorList.Add(new ErrorExceptionConformance(sourceLocation, derivedExceptionIdentifierList, baseExceptionIdentifierList));
                    return false;
                }
            }

            return true;
        }
    }
}
