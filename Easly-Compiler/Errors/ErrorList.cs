namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

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
        /// A fake location for errors to ignore.
        /// </summary>
        public static ISource NoLocation { get; } = new Identifier();

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
            Debug.Assert(error != null);
            Debug.Assert(!string.IsNullOrEmpty(error.Message));

            if (this != Ignored)
                Add(error);
        }

        /// <summary>
        /// Adds errors to the list.
        /// </summary>
        /// <param name="errorList">Errors to add.</param>
        public void AddErrors(IErrorList errorList)
        {
            Debug.Assert(errorList is ErrorList);

            if (this != Ignored)
            {
                foreach (IError Error in (ErrorList)errorList)
                {
                    Debug.Assert(!string.IsNullOrEmpty(Error.Message));
                    Add(Error);
                }
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
            {
                Debug.Assert(!string.IsNullOrEmpty(Error.ToString()));
                Result += $"{Environment.NewLine}  {Error.Message} ({Error.GetType().Name}) [{Error.Location}].";
            }

            return Result;
        }
    }
}
