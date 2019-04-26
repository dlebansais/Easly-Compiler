namespace CompilerNode
{
    using System.Collections.Generic;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFeature.
    /// </summary>
    public interface IFeature : BaseNode.IFeature, INode, ISource
    {
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        OnceReference<IFeatureName> ValidFeatureName { get; }

        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFeature { get; }
    }

    /// <summary>
    /// Helper class for features.
    /// </summary>
    public static class Feature
    {
        /// <summary>
        /// Checks that all overloads in a list have parameters that allow them to be distinguished in a caller.
        /// TODO include conversions in the check.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        /// <param name="location">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool DisjoinedParameterCheck(IList<ICommandOverloadType> overloadList, ISource location, IList<IError> errorList)
        {
            IHashtableEx<int, IList<ICommandOverloadType>> UnmixedOverloadsTable = new HashtableEx<int, IList<ICommandOverloadType>>();

            foreach (ICommandOverloadType Overload in overloadList)
            {
                int LastParameterIndex;

                for (LastParameterIndex = Overload.ParameterTable.Count; LastParameterIndex > 0; LastParameterIndex--)
                {
                    IParameter p = Overload.ParameterTable[LastParameterIndex - 1];
                    IScopeAttributeFeature Attribute = p.ResolvedParameter;

                    if (!Attribute.DefaultValue.IsAssigned)
                        break;
                }

                if (!UnmixedOverloadsTable.ContainsKey(LastParameterIndex))
                    UnmixedOverloadsTable.Add(LastParameterIndex, new List<ICommandOverloadType>());

                IList<ICommandOverloadType> ThisOverloadMix = UnmixedOverloadsTable[LastParameterIndex];
                ThisOverloadMix.Add(Overload);
            }

            bool Success = true;

            foreach (KeyValuePair<int, IList<ICommandOverloadType>> Entry in UnmixedOverloadsTable)
            {
                IList<ICommandOverloadType> ThisOverloadMix = Entry.Value;

                for (int i = 0; i < Entry.Key; i++)
                    Success &= DisjoinedParameterCheck(ThisOverloadMix, i, location, errorList);
            }

            return Success;
        }

        private static bool DisjoinedParameterCheck(IList<ICommandOverloadType> overloadList, int index, ISource location, IList<IError> errorList)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (ICommandOverloadType Overload in overloadList)
            {
                ListTableEx<IParameter> ParameterTable = Overload.ParameterTable;
                if (index < ParameterTable.Count)
                    SameIndexList.Add(ParameterTable[index]);
            }

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            IList<IError> CheckErrorList = new List<IError>();

            bool Success = true;
            for (int i = 0; i < SameIndexList.Count && Success; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedFeatureType.Item;

                for (int j = i + 1; j < SameIndexList.Count && Success; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedFeatureType.Item;

                    Success &= DisjoinedParameterCheck(Parameter1, Parameter2, SubstitutionTypeTable, location, errorList);
                    Success &= DisjoinedParameterCheck(Parameter2, Parameter1, SubstitutionTypeTable, location, errorList);
                }
            }

            return Success;
        }

        /// <summary>
        /// Checks that all overloads in a list have parameters that allow them to be distinguished in a caller.
        /// TODO include conversions in the check.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        /// <param name="location">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool DisjoinedParameterCheck(IList<IQueryOverloadType> overloadList, ISource location, IList<IError> errorList)
        {
            bool Success = true;

            int MaxParameter = 0;
            foreach (IQueryOverloadType Overload in overloadList)
                if (MaxParameter < Overload.ParameterTable.Count)
                    MaxParameter = Overload.ParameterTable.Count;

            for (int i = 0; i < MaxParameter; i++)
                Success &= DisjoinedParameterCheck(overloadList, i, location, errorList);

            return Success;
        }

        private static bool DisjoinedParameterCheck(IList<IQueryOverloadType> overloadList, int index, ISource location, IList<IError> errorList)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ListTableEx<IParameter> ParameterTable = Overload.ParameterTable;
                if (index < ParameterTable.Count)
                    SameIndexList.Add(ParameterTable[index]);
            }

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            IList<IError> CheckErrorList = new List<IError>();

            bool Success = true;
            for (int i = 0; i < SameIndexList.Count && Success; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedFeatureType.Item;

                for (int j = i + 1; j < SameIndexList.Count && Success; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedFeatureType.Item;

                    Success &= DisjoinedParameterCheck(Parameter1, Parameter2, SubstitutionTypeTable, location, errorList);
                    Success &= DisjoinedParameterCheck(Parameter2, Parameter1, SubstitutionTypeTable, location, errorList);
                }
            }

            return Success;
        }

        private static bool DisjoinedParameterCheck(IParameter derivedParameter, IParameter baseParameter, IHashtableEx<ICompiledType, ICompiledType> substitutionTypeTable, ISource location, IList<IError> errorList)
        {
            IList<IError> CheckErrorList = new List<IError>();
            ICompiledType DerivedType = derivedParameter.ResolvedParameter.ResolvedFeatureType.Item;
            ICompiledType BaseType = baseParameter.ResolvedParameter.ResolvedFeatureType.Item;
            bool Success = true;

            if (ObjectType.TypeConformToBase(DerivedType, BaseType, substitutionTypeTable, CheckErrorList, location, false))
            {
                errorList.Add(new ErrorMoreBasicParameter(baseParameter.ResolvedParameter.EmbeddingOverload));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Gets the most common type of all overloads.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        public static IList<IExpressionType> CommonResultType(IList<IQueryOverloadType> overloadList)
        {
            IList<IExpressionType> Result = new List<IExpressionType>();

            int MaxResult = 0;
            foreach (IQueryOverloadType Overload in overloadList)
                if (MaxResult < Overload.ResultTable.Count)
                    MaxResult = Overload.ResultTable.Count;

            for (int i = 0; i < MaxResult; i++)
            {
                GetCommonResultType(overloadList, i, out ITypeName ResultTypeName, out ICompiledType ResultType);
                IExpressionType NewExpressionType = new ExpressionType(ResultTypeName, ResultType, overloadList[0].ResultTable[i].Name);
                Result.Add(NewExpressionType);
            }

            return Result;
        }

        private static void GetCommonResultType(IList<IQueryOverloadType> overloadList, int index, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ListTableEx<IParameter> ResultTable = Overload.ResultTable;
                if (index < ResultTable.Count)
                    SameIndexList.Add(ResultTable[index]);
            }

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            IList<IError> CheckErrorList = new List<IError>();
            ISource FakeLocation = overloadList[0];

            IParameter SelectedParameter = SameIndexList[0];
            ITypeName SelectedParameterTypeName = SelectedParameter.ResolvedParameter.ResolvedFeatureTypeName.Item;
            ICompiledType SelectedParameterType = SelectedParameter.ResolvedParameter.ResolvedFeatureType.Item;

            for (int i = 1; i < SameIndexList.Count; i++)
            {
                IParameter CurrentParameter = SameIndexList[i];
                ITypeName CurrentParameterTypeName = CurrentParameter.ResolvedParameter.ResolvedFeatureTypeName.Item;
                ICompiledType CurrentParameterType = CurrentParameter.ResolvedParameter.ResolvedFeatureType.Item;

                if (ObjectType.TypeConformToBase(SelectedParameterType, CurrentParameterType, SubstitutionTypeTable, CheckErrorList, FakeLocation, false))
                {
                    SelectedParameter = CurrentParameter;
                    SelectedParameterTypeName = CurrentParameterTypeName;
                    SelectedParameterType = CurrentParameterType;
                }
            }

            resultTypeName = SelectedParameterTypeName;
            resultType = SelectedParameterType;
        }

        /// <summary>
        /// Checks that a result of a list of overloads conform to a base type.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        /// <param name="index">Index of the result in the possible results.</param>
        /// <param name="baseType">The base type.</param>
        /// <param name="location">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool JoinedResultCheck(IList<IQueryOverloadType> overloadList, int index, ICompiledType baseType, ISource location, IList<IError> errorList)
        {
            bool Success = true;
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ListTableEx<IParameter> ResultTable = Overload.ResultTable;
                if (index < ResultTable.Count)
                    SameIndexList.Add(ResultTable[index]);
            }

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            IList<IError> CheckErrorList = new List<IError>();

            for (int i = 0; i < SameIndexList.Count; i++)
            {
                IParameter CurrentParameter = SameIndexList[i];
                ICompiledType CurrentParameterType = CurrentParameter.ResolvedParameter.ResolvedFeatureType.Item;

                if (!ObjectType.TypeConformToBase(CurrentParameterType, baseType, SubstitutionTypeTable, CheckErrorList, location, false))
                {
                    errorList.Add(new ErrorNonConformingType(location));
                    Success = false;
                }
            }

            return Success;
        }
    }
}
