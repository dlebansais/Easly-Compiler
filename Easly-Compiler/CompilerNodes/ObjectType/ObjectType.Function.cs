namespace CompilerNode
{
    using EaslyCompiler;
    using System.Diagnostics;

    /// <summary>
    /// Type hepler class.
    /// </summary>
    public static partial class ObjectType
    {
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
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.Item, derivedType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
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
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            IEntityDeclaration OverloadResult = SingleOverload.ResultList[0];
            Debug.Assert(OverloadResult.ValidEntity.IsAssigned);
            Debug.Assert(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
            Debug.Assert(derivedType.ResolvedEntityType.IsAssigned);

            Result &= TypeConformToBase(OverloadResult.ValidEntity.Item.ResolvedEffectiveType.Item, derivedType.ResolvedEntityType.Item, errorList, sourceLocation, isConversionAllowed: false);
            Result &= ExceptionListConformToBase(derivedType.GetExceptionIdentifierList, SingleOverload.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }

        /// <summary>
        /// Checks that a query overload conforms to another.
        /// </summary>
        /// <param name="derivedType">The derived type.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="sourceLocation">The location for reporting errors.</param>
        public static bool QueryOverloadConformToBase(IQueryOverloadType derivedType, IQueryOverloadType baseType, IErrorList errorList, ISource sourceLocation)
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
                Debug.Assert(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.IsAssigned);
                Debug.Assert(BaseParameter.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedParameter.ValidEntity.Item.ResolvedEffectiveType.Item, BaseParameter.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            for (int i = 0; i < baseType.ResultList.Count && i < derivedType.ResultList.Count; i++)
            {
                IEntityDeclaration BaseResult = baseType.ResultList[i];
                IEntityDeclaration DerivedResult = derivedType.ResultList[i];

                Debug.Assert(BaseResult.ValidEntity.IsAssigned);
                Debug.Assert(BaseResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.IsAssigned);
                Debug.Assert(DerivedResult.ValidEntity.Item.ResolvedEffectiveType.IsAssigned);

                Result &= TypeConformToBase(DerivedResult.ValidEntity.Item.ResolvedEffectiveType.Item, BaseResult.ValidEntity.Item.ResolvedEffectiveType.Item, errorList, sourceLocation, isConversionAllowed: false);
            }

            Result &= ExceptionListConformToBase(derivedType.ExceptionIdentifierList, baseType.ExceptionIdentifierList, errorList, sourceLocation);

            return Result;
        }
    }
}
