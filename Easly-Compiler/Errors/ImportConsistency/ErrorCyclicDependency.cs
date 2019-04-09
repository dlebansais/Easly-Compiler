namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Cyclic dependency.
    /// </summary>
    public interface IErrorCyclicDependency : IError
    {
        /// <summary>
        /// List of nodes with cyclic dependencies by names.
        /// </summary>
        IList<string> NameList { get; }
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
        /// <param name="nameList">List of nodes with cyclic dependencies by names</param>
        public ErrorCyclicDependency(ISource source, IList<string> nameList)
            : base(source)
        {
            Debug.Assert(nameList.Count >= 2);

            NameList = nameList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of nodes with cyclic dependencies by names.
        /// </summary>
        public IList<string> NameList { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                Debug.Assert(NameList.Count >= 2);

                string Result = $"'{NameList[0]}'";

                for (int i = 1; i + 1 < NameList.Count; i++)
                    Result += $", '{NameList[i]}'";

                Result += $" and '{NameList[NameList.Count - 1]}'";

                return $"Cyclic dependencies detected in {Result}.";
            }
        }
        #endregion
    }
}
