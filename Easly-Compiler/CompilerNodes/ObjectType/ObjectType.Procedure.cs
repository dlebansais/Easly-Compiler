namespace CompilerNode
{
    using EaslyCompiler;
    using System.Diagnostics;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
                Debug.Assert(OverloadValue.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, OverloadValue.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
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
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.IsAssigned);
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            if (SingleOverload.ParameterList.Count == derivedType.IndexParameterList.Count + 1)
            {
                Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

                IEntityDeclaration LastParameter = SingleOverload.ParameterList[derivedType.IndexParameterList.Count];
                Debug.Assert(LastParameter.ValidEntity.IsAssigned);
                Debug.Assert(LastParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(derivedType.ResolvedEntityType.Item, LastParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.SetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        /// <summary>
        /// Checks that a command overload conforms to another.
        /// </summary>
        /// <param name="derivedType">The derived type.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="sourceLocation">The location for reporting errors.</param>
        public static bool CommandOverloadConformToBase(ICommandOverloadType derivedType, ICommandOverloadType baseType, IErrorList errorList, ISource sourceLocation)
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
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }
    }
}
