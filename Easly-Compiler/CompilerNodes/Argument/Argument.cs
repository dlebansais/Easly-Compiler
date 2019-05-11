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
    }
}
