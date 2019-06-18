namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# assertion node.
    /// </summary>
    public interface ICSharpAssertion : ICSharpSource<IAssertion>
    {
        /// <summary>
        /// The assertion tag. Can be null.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// The assertion expression.
        /// </summary>
        ICSharpExpression BooleanExpression { get; }

        /// <summary>
        /// Writes down the C# assertion.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharp(ICSharpWriter writer);
    }

    /// <summary>
    /// A C# assertion node.
    /// </summary>
    public class CSharpAssertion : CSharpSource<IAssertion>, ICSharpAssertion
    {
        #region Init
        /// <summary>
        /// Create a new C# assertion.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpAssertion Create(ICSharpContext context, IAssertion source)
        {
            return new CSharpAssertion(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssertion"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpAssertion(ICSharpContext context, IAssertion source)
            : base(source)
        {
            if (source.Tag.IsAssigned)
                Tag = ((IName)source.Tag.Item).ValidText.Item;

            BooleanExpression = CSharpExpression.Create(context, (IExpression)source.BooleanExpression);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The assertion tag. Can be null.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// The assertion expression.
        /// </summary>
        public ICSharpExpression BooleanExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# assertion.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public virtual void WriteCSharp(ICSharpWriter writer)
        {
            string AssertionString = BooleanExpression.CSharpText(writer);
            string TagString = string.IsNullOrEmpty(Tag) ? string.Empty : $" // {Tag}?";

            writer.WriteIndentedLine($"Debug.Assert({AssertionString});{TagString}");

            writer.AddUsing("System.Diagnostics");
        }

        /// <summary>
        /// Writes down a contract.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="requireList">The list of require assertions in the contract.</param>
        /// <param name="ensureList">The list of ensure assertions in the contract.</param>
        /// <param name="contractLocation">Where the contract appears in the feature.</param>
        /// <param name="writeEmptyContract">True if the contract must be written, even if empty.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public static void WriteContract(ICSharpWriter writer, IList<ICSharpAssertion> requireList, IList<ICSharpAssertion> ensureList, CSharpContractLocations contractLocation, bool writeEmptyContract, ref bool isFirstFeature, ref bool isMultiline)
        {
            if (writeEmptyContract || requireList.Count > 0 || ensureList.Count > 0)
            {
                if (!isFirstFeature)
                    writer.WriteEmptyLine();

                isMultiline = true;

                if (requireList.Count == 0 && ensureList.Count == 0)
                    writer.WriteIndentedLine("// Contract: None");
                else
                {
                    bool IsHandled = false;

                    switch (contractLocation)
                    {
                        case CSharpContractLocations.Getter:
                            writer.WriteIndentedLine("// Contract (getter):");
                            IsHandled = true;
                            break;

                        case CSharpContractLocations.Setter:
                            writer.WriteIndentedLine("// Contract (setter):");
                            IsHandled = true;
                            break;

                        case CSharpContractLocations.Other:
                            writer.WriteIndentedLine("// Contract:");
                            IsHandled = true;
                            break;
                    }

                    Debug.Assert(IsHandled);
                }
            }

            isFirstFeature = false;
        }
        #endregion
    }
}
