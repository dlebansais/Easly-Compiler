namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Context when evaluating an expression.
    /// </summary>
    public interface ICSharpExpressionContext
    {
        /// <summary>
        /// Name to use for destination variables.
        /// </summary>
        IList<ICSharpVariableContext> DestinationNameList { get; }

        /// <summary>
        /// Copy of <see cref="DestinationNameList"/> but with temporary variables added.
        /// </summary>
        IList<string> CompleteDestinationNameList { get; }

        /// <summary>
        /// Table of filled destinations.
        /// </summary>
        IDictionary<string, string> FilledDestinationTable { get; }

        /// <summary>
        /// The value returned directly. Can be null.
        /// </summary>
        string ReturnValue { get; }

        /// <summary>
        /// All returned results, presented as argument for a call.
        /// </summary>
        string ResultListAsArgument { get; }

        /// <summary>
        /// Sets the single return value of an expression.
        /// </summary>
        /// <param name="value">The expression return value in plain text.</param>
        void SetSingleReturnValue(string value);

        /// <summary>
        /// Sets results of an expression, with one of them the returned value.
        /// </summary>
        /// <param name="outgoingResultList">The list of results.</param>
        /// <param name="returnValueIndex">The expression return value in plain text.</param>
        void SetMultipleResult(IList<string> outgoingResultList, int returnValueIndex);
    }

    /// <summary>
    /// Context when evaluating an expression.
    /// </summary>
    public class CSharpExpressionContext : ICSharpExpressionContext
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpressionContext"/> class.
        /// </summary>
        public CSharpExpressionContext()
        {
            DestinationNameList = new List<ICSharpVariableContext>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpressionContext"/> class.
        /// </summary>
        /// <param name="destinationNameList">The list of variables to use as destination.</param>
        public CSharpExpressionContext(IList<ICSharpVariableContext> destinationNameList)
        {
            DestinationNameList = destinationNameList;

            foreach (ICSharpVariableContext Item in DestinationNameList)
                CompleteDestinationNameList.Add(Item.Name);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Name to use for destination variables.
        /// Contains names that can be assigned directly as 'out' results. May contain less names than results, and these extra results are allowed to be lost.
        /// </summary>
        public IList<ICSharpVariableContext> DestinationNameList { get; }

        /// <summary>
        /// Copy of <see cref="DestinationNameList"/> but with temporary variables added.
        /// </summary>
        public IList<string> CompleteDestinationNameList { get; } = new List<string>();

        /// <summary>
        /// Table of filled destinations.
        /// Key: all strings in DestinationNameList, plus temporary variables.
        /// Value: Variable names or expressions in plain text. If null, it's in <see cref="ReturnValue"/>.
        /// </summary>
        public IDictionary<string, string> FilledDestinationTable { get; } = new Dictionary<string, string>();

        /// <summary>
        /// The value returned directly. Can be null.
        /// </summary>
        public string ReturnValue { get; private set; }

        /// <summary>
        /// Index of the value returned directly. -1 if none.
        /// </summary>
        public int ReturnValueIndex { get; private set; }

        /// <summary>
        /// All returned results, presented as argument for a call.
        /// </summary>
        public string ResultListAsArgument
        {
            get
            {
                string Result = string.Empty;
                int i;

                for (i = 0; i < ReturnValueIndex; i++)
                {
                    if (Result.Length > 0)
                        Result += ", ";

                    string OutgoingResult = CompleteDestinationNameList[i];
                    Result += OutgoingResult;
                }

                if (ReturnValue != null)
                {
                    if (Result.Length > 0)
                        Result += ", ";

                    Result += ReturnValue;
                }

                for (; i < CompleteDestinationNameList.Count; i++)
                {
                    if (Result.Length > 0)
                        Result += ", ";

                    string OutgoingResult = CompleteDestinationNameList[i];
                    Result += OutgoingResult;
                }

                return Result;
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the single return value of an expression.
        /// </summary>
        /// <param name="value">The expression return value in plain text.</param>
        public void SetSingleReturnValue(string value)
        {
            Debug.Assert(DestinationNameList.Count <= 1);

            ReturnValue = value;

            if (DestinationNameList.Count == 1)
                FilledDestinationTable.Add(DestinationNameList[0].Name, null);
        }

        /// <summary>
        /// Sets results of an expression, with one of them the returned value.
        /// </summary>
        /// <param name="outgoingResultList">The list of results.</param>
        /// <param name="returnValueIndex">The expression return value in plain text.</param>
        public void SetMultipleResult(IList<string> outgoingResultList, int returnValueIndex)
        {
            for (int i = 0; i < outgoingResultList.Count; i++)
            {
                string OutgoingResult = outgoingResultList[i];

                if (i != returnValueIndex)
                    CompleteDestinationNameList.Add(OutgoingResult);
                else
                {
                    ReturnValue = OutgoingResult;
                    OutgoingResult = null;
                }

                if (i < DestinationNameList.Count)
                    FilledDestinationTable.Add(DestinationNameList[i].Name, OutgoingResult);
            }

            ReturnValueIndex = returnValueIndex;
        }
        #endregion
    }
}
