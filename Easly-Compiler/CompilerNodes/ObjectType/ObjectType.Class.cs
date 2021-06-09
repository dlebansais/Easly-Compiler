namespace CompilerNode
{
    using EaslyCompiler;
    using System.Collections.Generic;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
    }
}
