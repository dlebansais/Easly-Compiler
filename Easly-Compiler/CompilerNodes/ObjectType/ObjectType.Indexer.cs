namespace CompilerNode
{
    using EaslyCompiler;
    using System.Diagnostics;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
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

        private static bool FunctionTypeConformToIndexerTypeOverloads(IQueryOverloadType derivedOverload, IIndexerType baseType, IErrorList errorList, ISource sourceLocation)
        {
            bool Result = true;

            Result &= derivedOverload.ParameterList.Count == baseType.IndexParameterList.Count && derivedOverload.ResultList.Count == 1 && derivedOverload.ParameterEnd != BaseNode.ParameterEndStatus.Closed;

            for (int i = 0; i < derivedOverload.ParameterList.Count && i < baseType.IndexParameterList.Count; i++)
            {
                IEntityDeclaration BaseParameter = baseType.IndexParameterList[i];
                IEntityDeclaration DerivedParameter = derivedOverload.ParameterList[i];

                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            IEntityDeclaration OverloadResult = derivedOverload.ResultList[0];

            Debug.Assert(baseType.ResolvedEntityType.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

            Result &= TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ValidEntity.Item.ResolvedEffectiveType.Item, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);

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
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            if (derivedOverload.ParameterList.Count == baseType.IndexParameterList.Count + 1)
            {
                IEntityDeclaration OverloadValue = derivedOverload.ParameterList[baseType.IndexParameterList.Count];

                Debug.Assert(OverloadValue.ValidEntity.IsAssigned);
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(baseType.ResolvedEntityType.IsAssigned);

                Result &= TypeConformToBase(OverloadValue.ValidEntity.Item.ResolvedEffectiveType.Item, baseType.ResolvedEntityType.Item, ErrorList.Ignored, ErrorList.NoLocation, isConversionAllowed: false);
                Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);
            }

            return Result;
        }
    }
}
