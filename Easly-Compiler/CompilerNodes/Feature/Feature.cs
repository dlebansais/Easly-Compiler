namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
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

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);
    }

    /// <summary>
    /// Helper class for features.
    /// </summary>
    public static class Feature
    {
        /// <summary>
        /// Checks that all overloads in a list have parameters that allow them to be distinguished in a caller.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool DisjoinedParameterCheck(IList<ICommandOverloadType> overloadList, IErrorList errorList)
        {
            ISealableDictionary<int, IList<ICommandOverloadType>> UnmixedOverloadsTable = new SealableDictionary<int, IList<ICommandOverloadType>>();

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
                    Success &= DisjoinedParameterCheck(ThisOverloadMix, i, errorList);
            }

            return Success;
        }

        private static bool DisjoinedParameterCheck(IList<ICommandOverloadType> overloadList, int index, IErrorList errorList)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (ICommandOverloadType Overload in overloadList)
            {
                ISealableList<IParameter> ParameterTable = Overload.ParameterTable;
                if (index < ParameterTable.Count)
                    SameIndexList.Add(ParameterTable[index]);
            }

            bool Success = true;
            for (int i = 0; i < SameIndexList.Count && Success; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedEffectiveType.Item;

                for (int j = i + 1; j < SameIndexList.Count && Success; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedEffectiveType.Item;

                    Success &= DisjoinedParameterCheck(Parameter1, Parameter2, errorList);
                    Success &= DisjoinedParameterCheck(Parameter2, Parameter1, errorList);
                }
            }

            return Success;
        }

        /// <summary>
        /// Checks that all overloads in a list have parameters that allow them to be distinguished in a caller.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool DisjoinedParameterCheck(IList<IQueryOverloadType> overloadList, IErrorList errorList)
        {
            ISealableDictionary<int, IList<IQueryOverloadType>> UnmixedOverloadsTable = new SealableDictionary<int, IList<IQueryOverloadType>>();

            foreach (IQueryOverloadType Overload in overloadList)
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
                    UnmixedOverloadsTable.Add(LastParameterIndex, new List<IQueryOverloadType>());

                IList<IQueryOverloadType> ThisOverloadMix = UnmixedOverloadsTable[LastParameterIndex];
                ThisOverloadMix.Add(Overload);
            }

            bool Success = true;

            foreach (KeyValuePair<int, IList<IQueryOverloadType>> Entry in UnmixedOverloadsTable)
            {
                IList<IQueryOverloadType> ThisOverloadMix = Entry.Value;

                for (int i = 0; i < Entry.Key; i++)
                    Success &= DisjoinedParameterCheck(ThisOverloadMix, i, errorList);
            }

            return Success;
        }

        private static bool DisjoinedParameterCheck(IList<IQueryOverloadType> overloadList, int index, IErrorList errorList)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ISealableList<IParameter> ParameterTable = Overload.ParameterTable;
                if (index < ParameterTable.Count)
                    SameIndexList.Add(ParameterTable[index]);
            }

            bool Success = true;
            for (int i = 0; i < SameIndexList.Count && Success; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedEffectiveType.Item;

                for (int j = i + 1; j < SameIndexList.Count && Success; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedEffectiveType.Item;

                    Success &= DisjoinedParameterCheck(Parameter1, Parameter2, errorList);
                    Success &= DisjoinedParameterCheck(Parameter2, Parameter1, errorList);
                }
            }

            return Success;
        }

        private static bool DisjoinedParameterCheck(IParameter derivedParameter, IParameter baseParameter, IErrorList errorList)
        {
            ICompiledType DerivedType = derivedParameter.ResolvedParameter.ResolvedEffectiveType.Item;
            ICompiledType BaseType = baseParameter.ResolvedParameter.ResolvedEffectiveType.Item;
            bool Success = true;

            if (DerivedType == BaseType)
            {
                errorList.AddError(new ErrorEqualParameters(baseParameter.ResolvedParameter.Location));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Gets the most common type of all overloads.
        /// </summary>
        /// <param name="overloadList">The list of overloads.</param>
        public static IResultType CommonResultType(IList<IQueryOverloadType> overloadList)
        {
            int MaxResult = 0;
            int MaxResultIndex = 0;
            for (int i = 0; i < overloadList.Count; i++)
            {
                IQueryOverloadType Overload = overloadList[i];
                if (MaxResult < Overload.ResultTable.Count)
                {
                    MaxResult = Overload.ResultTable.Count;
                    MaxResultIndex = i;
                }
            }

            IList<IExpressionType> ResultList = new List<IExpressionType>();
            for (int i = 0; i < MaxResult; i++)
            {
                GetCommonResultType(overloadList, i, out ITypeName ResultTypeName, out ICompiledType ResultType);

                Debug.Assert(i < overloadList[MaxResultIndex].ResultTypeList.Count);

                IExpressionType NewExpressionType = new ExpressionType(ResultTypeName, ResultType, overloadList[MaxResultIndex].ResultTypeList[i].Name);
                ResultList.Add(NewExpressionType);
            }

            return new ResultType(ResultList);
        }

        private static void GetCommonResultType(IList<IQueryOverloadType> overloadList, int index, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ISealableList<IParameter> ResultTable = Overload.ResultTable;
                if (index < ResultTable.Count)
                    SameIndexList.Add(ResultTable[index]);
            }

            IParameter SelectedParameter = SameIndexList[0];
            ITypeName SelectedParameterTypeName = SelectedParameter.ResolvedParameter.ResolvedEffectiveTypeName.Item;
            ICompiledType SelectedParameterType = SelectedParameter.ResolvedParameter.ResolvedEffectiveType.Item;

            for (int i = 1; i < SameIndexList.Count; i++)
            {
                IParameter CurrentParameter = SameIndexList[i];
                ITypeName CurrentParameterTypeName = CurrentParameter.ResolvedParameter.ResolvedEffectiveTypeName.Item;
                ICompiledType CurrentParameterType = CurrentParameter.ResolvedParameter.ResolvedEffectiveType.Item;

                if (ObjectType.TypeConformToBase(SelectedParameterType, CurrentParameterType, isConversionAllowed: false))
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
        public static bool JoinedResultCheck(IList<IQueryOverloadType> overloadList, int index, ICompiledType baseType, ISource location, IErrorList errorList)
        {
            bool Success = true;
            IList<IParameter> SameIndexList = new List<IParameter>();

            foreach (IQueryOverloadType Overload in overloadList)
            {
                ISealableList<IParameter> ResultTable = Overload.ResultTable;
                if (index < ResultTable.Count)
                    SameIndexList.Add(ResultTable[index]);
            }

            for (int i = 0; i < SameIndexList.Count; i++)
            {
                IParameter CurrentParameter = SameIndexList[i];
                ICompiledType CurrentParameterType = CurrentParameter.ResolvedParameter.ResolvedEffectiveType.Item;

                if (!ObjectType.TypeConformToBase(CurrentParameterType, baseType, isConversionAllowed: false))
                {
                    errorList.AddError(new ErrorNonConformingType(location));
                    Success = false;
                }
            }

            return Success;
        }
    }
}
