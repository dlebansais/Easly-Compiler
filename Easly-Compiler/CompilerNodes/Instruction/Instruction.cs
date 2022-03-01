namespace CompilerNode
{
    using System.Collections.Generic;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInstruction.
    /// </summary>
    public interface IInstruction : INode, ISource, IScopeHolder
    {
        /// <summary>
        /// Types of results of the instruction.
        /// </summary>
        OnceReference<IResultType> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the instruction can throw.
        /// </summary>
        OnceReference<IResultException> ResolvedException { get; }

        /// <summary>
        /// Gets a string representation of the instruction.
        /// </summary>
        string InstructionToString { get; }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        void RestartNumberType(ref bool isChanged);

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        void CheckNumberType(ref bool isChanged);

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        void ValidateNumberType(IErrorList errorList);
    }
}
