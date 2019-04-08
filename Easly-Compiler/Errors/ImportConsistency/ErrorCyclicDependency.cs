namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Cyclic dependency.
    /// </summary>
    public interface IErrorCyclicDependency : IError
    {
        /// <summary>
        /// List of libraries with cyclic dependencies.
        /// </summary>
        IList<ILibrary> LibraryList { get; }
    }

    /// <summary>
    /// Cyclic dependency.
    /// </summary>
    internal class ErrorCyclicDependency : Error, IErrorCyclicDependency
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCyclicDependency"/> class.
        /// </summary>
        /// <param name="source">The error location.</param>
        /// <param name="libraryList">List of libraries with cyclic dependencies.</param>
        public ErrorCyclicDependency(ISource source, IList<ILibrary> libraryList)
            : base(source)
        {
            Debug.Assert(libraryList.Count >= 2);

            LibraryList = libraryList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of libraries with cyclic dependencies.
        /// </summary>
        public IList<ILibrary> LibraryList { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                Debug.Assert(LibraryList.Count >= 2);

                string NameList = $"'{LibraryList[0].ValidLibraryName}'";

                for (int i = 1; i + 1 < LibraryList.Count; i++)
                    NameList += $", '{LibraryList[i].ValidLibraryName}'";

                NameList += $" and '{LibraryList[LibraryList.Count - 1].ValidLibraryName}'";

                return $"Cyclic dependencies detected in {NameList}.";
            }
        }
        #endregion
    }
}
