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

            bool Exit = false;
            for (int i = 0; i < SameIndexList.Count && !Exit; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedFeatureType.Item;

                for (int j = i + 1; j < SameIndexList.Count && !Exit; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedFeatureType.Item;

                    if (ObjectType.TypeConformToBase(ParameterType1, ParameterType2, SubstitutionTypeTable, CheckErrorList, location, false))
                    {
                        errorList.Add(new ErrorMoreBasicParameter(Parameter2.ResolvedParameter));
                        Exit = true;
                    }

                    else if (ObjectType.TypeConformToBase(ParameterType2, ParameterType1, SubstitutionTypeTable, CheckErrorList, location, false))
                    {
                        errorList.Add(new ErrorMoreBasicParameter(Parameter1.ResolvedParameter));
                        Exit = true;
                    }
                }
            }

            return !Exit;
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

            bool Exit = false;
            for (int i = 0; i < SameIndexList.Count && !Exit; i++)
            {
                IParameter Parameter1 = SameIndexList[i];
                ICompiledType ParameterType1 = Parameter1.ResolvedParameter.ResolvedFeatureType.Item;

                for (int j = i + 1; j < SameIndexList.Count && !Exit; j++)
                {
                    IParameter Parameter2 = SameIndexList[j];
                    ICompiledType ParameterType2 = Parameter2.ResolvedParameter.ResolvedFeatureType.Item;

                    if (ObjectType.TypeConformToBase(ParameterType1, ParameterType2, SubstitutionTypeTable, CheckErrorList, location, false))
                    {
                        errorList.Add(new ErrorMoreBasicParameter(Parameter2.ResolvedParameter.EmbeddingOverload));
                        Exit = true;
                    }

                    else if (ObjectType.TypeConformToBase(ParameterType2, ParameterType1, SubstitutionTypeTable, CheckErrorList, location, false))
                    {
                        errorList.Add(new ErrorMoreBasicParameter(Parameter1.ResolvedParameter.EmbeddingOverload));
                        Exit = true;
                    }
                }
            }

            return !Exit;
        }
    }
}
