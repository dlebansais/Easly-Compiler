namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# conditional node.
    /// </summary>
    public interface ICSharpConditional : ICSharpSource<IConditional>
    {
        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The condition.
        /// </summary>
        ICSharpExpression BooleanExpression { get; }

        /// <summary>
        /// The conditional instructions.
        /// </summary>
        ICSharpScope Instructions { get; }

        /// <summary>
        /// Writes down the C# conditional instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isElseIf">True if the conditional is not the first.</param>
        void WriteCSharp(ICSharpWriter writer, bool isElseIf);
    }

    /// <summary>
    /// A C# conditional node.
    /// </summary>
    public class CSharpConditional : CSharpSource<IConditional>, ICSharpConditional
    {
        #region Init
        /// <summary>
        /// Create a new C# conditional.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpConditional Create(ICSharpContext context, ICSharpFeature parentFeature, IConditional source)
        {
            return new CSharpConditional(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConditional"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpConditional(ICSharpContext context, ICSharpFeature parentFeature, IConditional source)
            : base(source)
        {
            ParentFeature = parentFeature;

            BooleanExpression = CSharpExpression.Create(context, (IExpression)source.BooleanExpression);
            Instructions = CSharpScope.Create(context, parentFeature, (IScope)source.Instructions);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The condition.
        /// </summary>
        public ICSharpExpression BooleanExpression { get; }

        /// <summary>
        /// The conditional instructions.
        /// </summary>
        public ICSharpScope Instructions { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# conditional instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isElseIf">True if the conditional is not the first.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, bool isElseIf)
        {
            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
            BooleanExpression.WriteCSharp(writer, ExpressionContext, -1);

            string ExpressionString = ExpressionContext.ReturnValue;
            Debug.Assert(ExpressionString != null);

            if (BooleanExpression.IsEventExpression)
                if (BooleanExpression.IsComplex)
                    ExpressionString = $"({ExpressionString}).IsSignaled";
                else
                    ExpressionString += ".IsSignaled";

            string Condition;
            if (isElseIf)
                Condition = "else if";
            else
                Condition = "if";

            Condition += $" ({ExpressionString})";
            writer.WriteIndentedLine(Condition);

            Instructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Mandatory, false);
        }
        #endregion
    }
}
