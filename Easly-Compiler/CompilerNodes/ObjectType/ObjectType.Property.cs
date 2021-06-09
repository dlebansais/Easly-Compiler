namespace CompilerNode
{
    using EaslyCompiler;
    using System.Diagnostics;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
                Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(baseType.ResolvedEntityType.Item, OverloadResult.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.GetExceptionIdentifierList, errorList, sourceLocation);

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
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(OverloadValue.ValidEntity.Item.ResolvedEffectiveType.Item, baseType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedOverload.ExceptionIdentifierList, baseType.SetExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }
    }
}
