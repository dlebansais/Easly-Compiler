namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IArgument.
    /// </summary>
    public interface IArgument : BaseNode.IArgument, INode, ISource
    {
        /// <summary>
        /// Gets a string representation of the argument.
        /// </summary>
        string ArgumentToString { get; }

        /// <summary>
        /// Types of expression results for the argument.
        /// </summary>
        OnceReference<IList<IExpressionType>> ResolvedResult { get; }

        /// <summary>
        /// True if the argument is a constant.
        /// </summary>
        bool IsConstant { get; }
    }

    /// <summary>
    /// Argument helper class.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Compares two lists of arguments.
        /// </summary>
        /// <param name="argumentList1">The first list.</param>
        /// <param name="argumentList2">The second list.</param>
        public static bool IsArgumentListEqual(IList<IArgument> argumentList1, IList<IArgument> argumentList2)
        {
            bool Result = true;

            Result &= argumentList1.Count == argumentList2.Count;

            for (int i = 0; i < argumentList1.Count && i < argumentList2.Count; i++)
            {
                IArgument Argument1 = argumentList1[i];
                IArgument Argument2 = argumentList2[i];
                Result &= IsArgumentEqual(Argument1, Argument2);
            }

            return Result;
        }

        /// <summary>
        /// Compares two arguments.
        /// </summary>
        /// <param name="argument1">The first argument.</param>
        /// <param name="argument2">The second argument.</param>
        private static bool IsArgumentEqual(IArgument argument1, IArgument argument2)
        {
            bool Result = false;
            bool IsHandled = false;

            if (argument1 is IPositionalArgument AsPositionalArgument1)
            {
                if (argument2 is IAssignmentArgument AsAssignmentArgument2)
                    IsHandled = true;
                else if (argument2 is IPositionalArgument AsPositionalArgument2)
                {
                    Result = PositionalArgument.IsPositionalArgumentEqual(AsPositionalArgument1, AsPositionalArgument2);
                    IsHandled = true;
                }
            }
            else if (argument1 is IAssignmentArgument AsAssignmentArgument1)
            {
                if (argument2 is IPositionalArgument AsPositionalArgument2)
                    IsHandled = true;
                else if (argument2 is IAssignmentArgument AsAssignmentArgument2)
                {
                    Result = AssignmentArgument.IsAssignmentArgumentEqual(AsAssignmentArgument1, AsAssignmentArgument2);
                    IsHandled = true;
                }
            }

            Debug.Assert(IsHandled);

            return Result;
        }

        /// <summary>
        /// Gets a string representation of a list of arguments.
        /// </summary>
        /// <param name="argumentList">The list of arguments.</param>
        public static string ArgumentListToString(IEnumerable argumentList)
        {
            string Result = string.Empty;

            foreach (IArgument Argument in argumentList)
            {
                if (Result.Length > 0)
                    Result += ", ";
                Result += Argument.ArgumentToString;
            }

            return Result;
        }

        /// <summary>
        /// Validate and merge a list of arguments.
        /// </summary>
        /// <param name="argumentList">The list of arguments.</param>
        /// <param name="mergedArgumentList">The merged result.</param>
        /// <param name="argumentStyle">The validated style.</param>
        /// <param name="errorList">List of errors found.</param>
        public static bool Validate(IList<IArgument> argumentList, List<IExpressionType> mergedArgumentList, out TypeArgumentStyles argumentStyle, IErrorList errorList)
        {
            argumentStyle = TypeArgumentStyles.None;

            bool IsHandled;

            foreach (IArgument Argument in argumentList)
            {
                IsHandled = false;

                switch (Argument)
                {
                    case IPositionalArgument AsPositionalArgument:
                        if (argumentStyle == TypeArgumentStyles.None)
                            argumentStyle = TypeArgumentStyles.Positional;

                        else if (argumentStyle == TypeArgumentStyles.Assignment)
                        {
                            errorList.AddError(new ErrorArgumentMixed(AsPositionalArgument));
                            return false;
                        }

                        IsHandled = true;
                        break;

                    case IAssignmentArgument AsAssignmentArgument:
                        if (argumentStyle == TypeArgumentStyles.None)
                            argumentStyle = TypeArgumentStyles.Assignment;

                        else if (argumentStyle == TypeArgumentStyles.Positional)
                        {
                            errorList.AddError(new ErrorArgumentMixed(AsAssignmentArgument));
                            return false;
                        }

                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            IHashtableEx<string, IArgument> DuplicateNameTable = new HashtableEx<string, IArgument>();
            IsHandled = false;

            switch (argumentStyle)
            {
                case TypeArgumentStyles.None:
                    IsHandled = true;
                    break;

                case TypeArgumentStyles.Positional:
                    for (int i = 0; i < argumentList.Count; i++)
                    {
                        IArgument Argument = argumentList[i];
                        IExpression Source = null;

                        switch (Argument)
                        {
                            case IPositionalArgument AsPositionalArgument:
                                Source = (IExpression)AsPositionalArgument.Source;
                                break;

                            case IAssignmentArgument AsAssignmentArgument:
                                Source = (IExpression)AsAssignmentArgument.Source;
                                break;
                        }

                        Debug.Assert(Source != null);

                        for (int j = 0; j < Argument.ResolvedResult.Item.Count; j++)
                        {
                            IExpressionType Item = Argument.ResolvedResult.Item[j];
                            Item.SetSource(Source, j);
                            mergedArgumentList.Add(Item);
                        }
                    }

                    IsHandled = true;
                    break;

                case TypeArgumentStyles.Assignment:
                    for (int i = 0; i < argumentList.Count; i++)
                    {
                        IArgument Argument = argumentList[i];
                        IExpression Source = null;
                        IList<IIdentifier> ParameterList;

                        switch (Argument)
                        {
                            case IPositionalArgument AsPositionalArgument:
                                Source = (IExpression)AsPositionalArgument.Source;

                                for (int j = 0; j < Argument.ResolvedResult.Item.Count; j++)
                                {
                                    IExpressionType Item = Argument.ResolvedResult.Item[j];
                                    Item.SetSource(Source, j);
                                    mergedArgumentList.Add(Item);
                                }
                                break;

                            case IAssignmentArgument AsAssignmentArgument:
                                Source = (IExpression)AsAssignmentArgument.Source;
                                ParameterList = AsAssignmentArgument.ParameterList;

                                for (int j = 0; j < Argument.ResolvedResult.Item.Count; j++)
                                {
                                    IExpressionType Item = Argument.ResolvedResult.Item[j];
                                    Item.SetName(ParameterList[j].ValidText.Item);

                                    if (mergedArgumentList.Exists((IExpressionType other) => { return Item.Name == other.Name; }))
                                        if (!DuplicateNameTable.ContainsKey(Item.Name))
                                            DuplicateNameTable.Add(Item.Name, Argument);

                                    Item.SetSource(Source, j);
                                    mergedArgumentList.Add(Item);
                                }
                                break;
                        }

                        Debug.Assert(Source != null);
                    }

                    if (DuplicateNameTable.Count > 0)
                    {
                        foreach (KeyValuePair<string, IArgument> Entry in DuplicateNameTable)
                            errorList.AddError(new ErrorDuplicateName(Entry.Value, Entry.Key));

                        return false;
                    }

                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return true;
        }

        /// <summary>
        /// Checks if actual arguments of a call conform to expected parameters.
        /// </summary>
        /// <param name="parameterTableList">The list of expected parameters.</param>
        /// <param name="arguments">The list of arguments.</param>
        /// <param name="argumentStyle">The argument-passing style.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="source">The source to use for errors.</param>
        /// <param name="selectedIndex">The selected index in the list of overloads upon return.</param>
        public static bool ArgumentsConformToParameters(IList<ListTableEx<IParameter>> parameterTableList, IList<IExpressionType> arguments, TypeArgumentStyles argumentStyle, IErrorList errorList, ISource source, out int selectedIndex)
        {
            selectedIndex = -1;
            bool Result = false;
            bool IsHandled = false;

            switch (argumentStyle)
            {
                case TypeArgumentStyles.None:
                    Result = NoneArgumentsConformToParameters(parameterTableList, arguments, errorList, source, out selectedIndex);
                    IsHandled = true;
                    break;

                case TypeArgumentStyles.Positional:
                    Result = PositionalArgumentsConformToParameters(parameterTableList, arguments, errorList, source, out selectedIndex);
                    IsHandled = true;
                    break;

                case TypeArgumentStyles.Assignment:
                    Result = AssignmentArgumentsConformToParameters(parameterTableList, arguments, errorList, source, out selectedIndex);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
            Debug.Assert(selectedIndex >= 0 || !Result);

            return Result;
        }

        private static bool NoneArgumentsConformToParameters(IList<ListTableEx<IParameter>> parameterTableList, IList<IExpressionType> arguments, IErrorList errorList, ISource source, out int selectedIndex)
        {
            Debug.Assert(arguments.Count <= 1);

            return PositionalArgumentsConformToParameters(parameterTableList, arguments, errorList, source, out selectedIndex);
        }

        private static bool PositionalArgumentsConformToParameters(IList<ListTableEx<IParameter>> parameterTableList, IList<IExpressionType> arguments, IErrorList errorList, ISource source, out int selectedIndex)
        {
            OnceReference<ListTableEx<IParameter>> SelectedOverload = new OnceReference<ListTableEx<IParameter>>();
            selectedIndex = -1;

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            for (int i = 0; i < parameterTableList.Count; i++)
            {
                ListTableEx<IParameter> OverloadParameterList = parameterTableList[i];

                int j;
                bool IsMatching = true;
                for (j = 0; j < arguments.Count && j < OverloadParameterList.Count && IsMatching; j++)
                {
                    ICompiledType ArgumentType = arguments[j].ValueType;
                    IParameter OverloadParameter = OverloadParameterList[j];
                    Debug.Assert(OverloadParameter.ResolvedParameter.ResolvedFeatureType.IsAssigned);

                    ICompiledType ParameterType = null;
                    switch (OverloadParameter.ResolvedParameter.ResolvedFeatureType.Item)
                    {
                        case IFunctionType AsFunctionType:
                            ParameterType = AsFunctionType;
                            break;

                        case IProcedureType AsProcedureType:
                            ParameterType = AsProcedureType;
                            break;

                        case IClassType AsClassType:
                            ParameterType = AsClassType;
                            break;

                        case IFormalGenericType AsFormalGenericType:
                            ParameterType = AsFormalGenericType;
                            break;
                    }

                    Debug.Assert(ParameterType != null);

                    IErrorList CheckErrorList = new ErrorList();
                    IsMatching &= ObjectType.TypeConformToBase(ArgumentType, ParameterType, SubstitutionTypeTable, CheckErrorList, source);
                }

                if (IsMatching)
                {
                    for (; j < OverloadParameterList.Count && IsMatching; j++)
                    {
                        IParameter OverloadParameter = OverloadParameterList[j];
                        IsMatching &= OverloadParameter.ResolvedParameter.DefaultValue.IsAssigned;
                    }
                }

                if (IsMatching)
                    if (SelectedOverload.IsAssigned)
                    {
                        errorList.AddError(new ErrorInvalidExpression(source));
                        return false;
                    }
                    else
                    {
                        SelectedOverload.Item = OverloadParameterList;
                        selectedIndex = i;
                    }
            }

            if (!SelectedOverload.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(source));
                return false;
            }

            if (SelectedOverload.Item.Count < arguments.Count)
            {
                errorList.AddError(new ErrorTooManyArguments(source, arguments.Count, SelectedOverload.Item.Count));
                return false;
            }

            return true;
        }

        private static bool AssignmentArgumentsConformToParameters(IList<ListTableEx<IParameter>> parameterTableList, IList<IExpressionType> arguments, IErrorList errorList, ISource source, out int selectedIndex)
        {
            OnceReference<ListTableEx<IParameter>> SelectedOverload = new OnceReference<ListTableEx<IParameter>>();
            selectedIndex = -1;

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
            for (int i = 0; i < parameterTableList.Count; i++)
            {
                ListTableEx<IParameter> OverloadParameterList = parameterTableList[i];

                bool IsMatching = true;
                for (int j = 0; j < arguments.Count && IsMatching; j++)
                {
                    ICompiledType ArgumentType = arguments[j].ValueType;
                    string ArgumentName = arguments[j].Name;

                    OnceReference<IParameter> MatchingParameter = new OnceReference<IParameter>();
                    foreach (IParameter p in OverloadParameterList)
                        if (p.Name == ArgumentName)
                        {
                            MatchingParameter.Item = p;
                            break;
                        }

                    if (!MatchingParameter.IsAssigned)
                    {
                        errorList.AddError(new ErrorArgumentNameMismatch(source, ArgumentName));
                        return false;
                    }

                    IParameter OverloadParameter = MatchingParameter.Item;
                    ICompiledType ParameterType = OverloadParameter.ResolvedParameter.ResolvedFeatureType.Item;

                    IErrorList CheckErrorList = new ErrorList();
                    IsMatching &= ObjectType.TypeConformToBase(ArgumentType, ParameterType, SubstitutionTypeTable, CheckErrorList, source);
                }

                if (IsMatching)
                    if (SelectedOverload.IsAssigned)
                    {
                        errorList.AddError(new ErrorInvalidExpression(source));
                        return false;
                    }
                    else
                    {
                        SelectedOverload.Item = OverloadParameterList;
                        selectedIndex = i;
                    }
            }

            if (!SelectedOverload.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(source));
                return false;
            }
            else
                return true;
        }
    }
}
