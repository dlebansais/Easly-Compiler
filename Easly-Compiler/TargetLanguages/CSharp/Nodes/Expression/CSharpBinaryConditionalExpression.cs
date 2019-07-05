﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpBinaryConditionalExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IBinaryConditionalExpression Source { get; }

        /// <summary>
        /// The left expression.
        /// </summary>
        ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        ICSharpExpression RightExpression { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpBinaryConditionalExpression : CSharpExpression, ICSharpBinaryConditionalExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpBinaryConditionalExpression Create(ICSharpContext context, IBinaryConditionalExpression source)
        {
            return new CSharpBinaryConditionalExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryConditionalExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpBinaryConditionalExpression(ICSharpContext context, IBinaryConditionalExpression source)
            : base(context, source)
        {
            LeftExpression = Create(context, (IExpression)source.LeftExpression);
            RightExpression = Create(context, (IExpression)source.RightExpression);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IBinaryConditionalExpression Source { get { return (IBinaryConditionalExpression)base.Source; } }

        /// <summary>
        /// The left expression.
        /// </summary>
        public ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        public ICSharpExpression RightExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override string CSharpText(ICSharpWriter writer)
        {
            WriteCSharp(writer, false, false, new List<ICSharpQualifiedName>(), -1, out string LastExpressionText);
            return LastExpressionText;
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        public override void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText)
        {
            if (IsEventExpression)
                lastExpressionText = CSharpTextEvent(writer, destinationList);
            else
                WriteCSharpBoolean(writer, destinationList, out lastExpressionText);
        }

        private string CSharpTextEvent(ICSharpWriter writer, IList<ICSharpQualifiedName> destinationList)
        {
            string LeftText = NestedExpressionText(writer, LeftExpression);
            string RightText = NestedExpressionText(writer, RightExpression);

            string OperatorName = null;

            switch (Source.Conditional)
            {
                case BaseNode.ConditionalTypes.And:
                    OperatorName = "&&";
                    break;

                case BaseNode.ConditionalTypes.Or:
                    OperatorName = "||";
                    break;

                case BaseNode.ConditionalTypes.Xor:
                    OperatorName = "^";
                    break;

                case BaseNode.ConditionalTypes.Implies:
                    OperatorName = "/";
                    break;
            }

            Debug.Assert(OperatorName != null);

            return $"{LeftText} {OperatorName} {RightText}";
        }

        private void WriteCSharpBoolean(ICSharpWriter writer, IList<ICSharpQualifiedName> destinationList, out string lastExpressionText)
        {
            if (LeftExpression.IsSingleResult && RightExpression.IsSingleResult)
            {
                string LeftText = NestedExpressionText(writer, LeftExpression);
                string RightText = NestedExpressionText(writer, RightExpression);

                if (Source.Conditional == BaseNode.ConditionalTypes.Implies)
                    lastExpressionText = $"!{LeftText} || {RightText}";
                else
                {
                    string OperatorName = null;

                    switch (Source.Conditional)
                    {
                        case BaseNode.ConditionalTypes.And:
                            OperatorName = "&&";
                            break;

                        case BaseNode.ConditionalTypes.Or:
                            OperatorName = "||";
                            break;

                        case BaseNode.ConditionalTypes.Xor:
                            OperatorName = "^";
                            break;
                    }

                    Debug.Assert(OperatorName != null);

                    lastExpressionText = $"{LeftText} {OperatorName} {RightText}";
                }
            }
            else
            {
                //TODO
                lastExpressionText = "TODO";
            }
        }

        private string NestedExpressionText(ICSharpWriter writer, ICSharpExpression expression)
        {
            string Result = expression.CSharpText(writer);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
