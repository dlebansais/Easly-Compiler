namespace CompilerNode
{
    using System.Collections;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ITypeArgument.
    /// </summary>
    public interface ITypeArgument : BaseNode.ITypeArgument, INode, ISource
    {
        /// <summary>
        /// Name of the resolved source type.
        /// </summary>
        OnceReference<ITypeName> ResolvedSourceTypeName { get; }

        /// <summary>
        /// The resolved source type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedSourceType { get; }

        /// <summary>
        /// Gets a string representation of the type argument.
        /// </summary>
        string TypeArgumentToString { get; }
    }

    /// <summary>
    /// Type Argument helper class.
    /// </summary>
    public static class TypeArgument
    {
        /// <summary>
        /// Gets a string representation of a list of type arguments.
        /// </summary>
        /// <param name="argumentList">The list of type arguments.</param>
        public static string TypeArgumentListToString(IEnumerable argumentList)
        {
            string Result = string.Empty;

            foreach (ITypeArgument TypeArgument in argumentList)
            {
                if (Result.Length > 0)
                    Result += ", ";
                Result += TypeArgument.TypeArgumentToString;
            }

            return Result;
        }
    }
}
