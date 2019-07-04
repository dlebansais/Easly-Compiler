﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpBinaryOperatorExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IBinaryOperatorExpression Source { get; }

        /// <summary>
        /// The left expression.
        /// </summary>
        ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        ICSharpExpression RightExpression { get; }

        /// <summary>
        /// The operator.
        /// </summary>
        ICSharpFunctionFeature Operator { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpBinaryOperatorExpression : CSharpExpression, ICSharpBinaryOperatorExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpBinaryOperatorExpression Create(ICSharpContext context, IBinaryOperatorExpression source)
        {
            return new CSharpBinaryOperatorExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryOperatorExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpBinaryOperatorExpression(ICSharpContext context, IBinaryOperatorExpression source)
            : base(context, source)
        {
            LeftExpression = Create(context, (IExpression)source.LeftExpression);
            RightExpression = Create(context, (IExpression)source.RightExpression);

            Operator = context.GetFeature(source.SelectedFeature.Item) as ICSharpFunctionFeature;
            Debug.Assert(Operator != null);

            IResultType ResolvedLeftResult = LeftExpression.Source.ResolvedResult.Item;

            for (int i = 0; i < ResolvedLeftResult.Count; i++)
            {
                ICompiledType OperatorBaseType = ResolvedLeftResult.At(i).ValueType;
                if (OperatorBaseType is IClassType AsClassType)
                    if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Number.Guid)
                        IsCallingNumberFeature = true;
            }

            if (!LeftExpression.IsSingleResult || !RightExpression.IsSingleResult)
            {
                RightAssignment = new CSharpAssignment(context, "temp", RightExpression);
                LeftAssignment = new CSharpAssignment(context, "temp", LeftExpression);
            }
        }

        ICSharpAssignment RightAssignment;
        ICSharpAssignment LeftAssignment;
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IBinaryOperatorExpression Source { get { return (IBinaryOperatorExpression)base.Source; } }

        /// <summary>
        /// The left expression.
        /// </summary>
        public ICSharpExpression LeftExpression { get; }

        /// <summary>
        /// The right expression.
        /// </summary>
        public ICSharpExpression RightExpression { get; }

        /// <summary>
        /// The operator.
        /// </summary>
        public ICSharpFunctionFeature Operator { get; }

        /// <summary>
        /// True if calling a feature of the Number class.
        /// </summary>
        public bool IsCallingNumberFeature { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, false, false, new List<ICSharpQualifiedName>(), -1);
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            string OperatorText = Operator.Name;

            if (IsCallingNumberFeature)
            {
                switch (OperatorText)
                {
                    case "≥":
                        OperatorText = ">=";
                        break;
                    case "≤":
                        OperatorText = "<=";
                        break;
                    case "shift right":
                        OperatorText = ">>";
                        break;
                    case "shift left":
                        OperatorText = "<<";
                        break;
                    case "modulo":
                        OperatorText = "%";
                        break;
                    case "bitwise and":
                        OperatorText = "&";
                        break;
                    case "bitwise or":
                        OperatorText = "|";
                        break;
                    case "bitwise xor":
                        OperatorText = "^";
                        break;
                }
            }
            else
                OperatorText = CSharpNames.ToCSharpIdentifier(OperatorText);

            if (IsCallingNumberFeature)
            {
                string LeftText = NestedExpressionText(usingCollection, LeftExpression);
                string RightText = NestedExpressionText(usingCollection, RightExpression);

                return $"{LeftText} {OperatorText} {RightText}";
            }
            else if (LeftExpression.IsSingleResult && RightExpression.IsSingleResult)
            {
                string LeftText = NestedExpressionText(usingCollection, LeftExpression);
                string RightText = RightExpression.CSharpText(usingCollection);

                return $"{LeftText}.{OperatorText}({RightText})";
            }
            else
            {
                RightAssignment.WriteCSharp(usingCollection as ICSharpWriter, true, true, false, out IList<string> DestinationEntityList);

                string LeftText = NestedExpressionText(usingCollection, LeftExpression);
                string RightText = RightExpression.CSharpText(usingCollection);

                return $"{LeftText}.{OperatorText}({RightText})";
            }
        }

        private string NestedExpressionText(ICSharpUsingCollection usingCollection, ICSharpExpression expression)
        {
            string Result = expression.CSharpText(usingCollection);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}
