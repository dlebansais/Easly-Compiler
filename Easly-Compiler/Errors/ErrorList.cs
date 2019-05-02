namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A list of errors.
    /// </summary>
    public interface IErrorList
    {
        /// <summary>
        /// True if the error list is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds an error to the list.
        /// </summary>
        /// <param name="error">The error to add.</param>
        void AddError(IError error);

        /// <summary>
        /// Adds errors to the list.
        /// </summary>
        /// <param name="errorList">Errors to add.</param>
        void AddErrors(IErrorList errorList);

        /// <summary>
        /// Clears the list of errors.
        /// </summary>
        void ClearErrors();

        /// <summary>
        /// Returns the n-th error in the list.
        /// </summary>
        /// <param name="index">Index of the error to return.</param>
        IError At(int index);
    }

    /// <summary>
    /// A list of errors.
    /// </summary>
    public class ErrorList : List<IError>, IErrorList
    {
        /// <summary>
        /// A list of errors that can be ignored.
        /// </summary>
        public static IErrorList Ignored { get; } = new ErrorList();

        /// <summary>
        /// True if the error list is empty.
        /// </summary>
        public bool IsEmpty { get { return Count == 0; } }

        /// <summary>
        /// Adds an error to the list.
        /// </summary>
        /// <param name="error">The error to add.</param>
        public void AddError(IError error)
        {
            if (this != Ignored)
                Add(error);
        }

        /// <summary>
        /// Adds errors to the list.
        /// </summary>
        /// <param name="errorList">Errors to add.</param>
        public void AddErrors(IErrorList errorList)
        {
            if (this != Ignored)
            {
                Debug.Assert(errorList is ErrorList);
                AddRange((ErrorList)errorList);
            }
        }

        /// <summary>
        /// Clears the list of errors.
        /// </summary>
        public void ClearErrors()
        {
            Clear();
        }

        /// <summary>
        /// Returns the n-th error in the list.
        /// </summary>
        /// <param name="index">Index of the error to return.</param>
        public IError At(int index)
        {
            Debug.Assert(index >= 0 && index < Count);
            return this[index];
        }

        /// <summary>
        /// Displays errors on the debug terminal.
        /// </summary>
        public override string ToString()
        {
            string Result = $"{Count} error(s).";

            foreach (IError Error in this)
                Result += $"{Environment.NewLine}  {Error.Message} ({Error}) [{Error.Location}].";

            return Result;
        }
    }
}
