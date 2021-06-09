namespace CompilerNode
{
    using EaslyCompiler;
    using System.Diagnostics;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
                Debug.Assert(DerivedField.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.IsAssigned);
                Debug.Assert(BaseField.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result = TypeConformToBase(DerivedField.ValidEntity.Item.ResolvedEffectiveType.Item, BaseField.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            return Result;
        }
    }
}
