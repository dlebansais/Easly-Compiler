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

        /// <summary>
        /// The pass name when the error occured.
        /// </summary>
        string PassName { get; }
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
        /// <param name="nameList">List of nodes with cyclic dependencies by names.</param>
        /// <param name="passName">The pass name when the error occured.</param>
        public ErrorCyclicDependency(IList<string> nameList, string passName)
            : base(ErrorLocation.NoLocation)
        {
            Debug.Assert(nameList.Count >= 1);

            NameList = nameList;
            PassName = passName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of nodes with cyclic dependencies by names.
        /// </summary>
        public IList<string> NameList { get; }

        /// <summary>
        /// The pass name when the error occured.
        /// </summary>
        public string PassName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        public override string Message
        {
            get
            {
                Debug.Assert(NameList.Count >= 1);

                string Result = $"'{NameList[0]}'";

                for (int i = 1; i + 1 < NameList.Count; i++)
                    Result += $", '{NameList[i]}'";

                Result += $" and '{NameList[NameList.Count - 1]}'";

                return $"Cyclic dependencies in pass {PassName} detected in {Result}.";
            }
        }
        #endregion
    }
}
