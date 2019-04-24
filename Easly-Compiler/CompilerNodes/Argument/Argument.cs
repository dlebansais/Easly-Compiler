namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        public static bool IsArgumentEqual(IArgument argument1, IArgument argument2)
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
    }
}
