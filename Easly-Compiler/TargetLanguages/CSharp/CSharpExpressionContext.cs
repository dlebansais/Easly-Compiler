namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Context when evaluating an expression.
    /// </summary>
    public interface ICSharpExpressionContext
    {
        /// <summary>
        /// Name to use for destination variables.
        /// </summary>
        IList<string> DestinationNameList { get; }
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpExpressionContext"/> class.
        /// </summary>
        public CSharpExpressionContext(IList<string> destinationNameList)
        {
            DestinationNameList = destinationNameList;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Name to use for destination variables.
        /// </summary>
        public IList<string> DestinationNameList { get; }
        #endregion
    }
}
