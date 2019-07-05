namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpInitializedObjectExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IInitializedObjectExpression Source { get; }

        /// <summary>
        /// Class of the initialized object.
        /// </summary>
        ICSharpClass Class { get; }

        /// <summary>
        /// The list of assignments.
        /// </summary>
        IList<ICSharpAssignmentArgument> AssignmentList { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpInitializedObjectExpression : CSharpExpression, ICSharpInitializedObjectExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpInitializedObjectExpression Create(ICSharpContext context, IInitializedObjectExpression source)
        {
            return new CSharpInitializedObjectExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInitializedObjectExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpInitializedObjectExpression(ICSharpContext context, IInitializedObjectExpression source)
            : base(context, source)
        {
            Class = context.GetClass(source.ResolvedClassType.Item.BaseClass);

            foreach (IAssignmentArgument Argument in source.AssignmentList)
            {
                ICSharpAssignmentArgument NewAssignment = CSharpAssignmentArgument.Create(context, Argument);
                AssignmentList.Add(NewAssignment);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IInitializedObjectExpression Source { get { return (IInitializedObjectExpression)base.Source; } }

        /// <summary>
        /// Class of the initialized object.
        /// </summary>
        public ICSharpClass Class { get; }

        /// <summary>
        /// The list of assignments.
        /// </summary>
        public IList<ICSharpAssignmentArgument> AssignmentList { get; } = new List<ICSharpAssignmentArgument>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="lastExpressionText">The text to use for the expression upon return.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isNeverSimple, bool isDeclaredInPlace, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string lastExpressionText)
        {
            string ClassNameText = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);

            string AssignmentText = string.Empty;
            foreach (ICSharpAssignmentArgument Assignment in AssignmentList)
            {
                if (AssignmentText.Length > 0)
                    AssignmentText += ", ";

                ICSharpExpression SourceExpression = Assignment.SourceExpression;
                string ExpressionText = NestedExpressionText(writer, SourceExpression);

                //TODO: handle more than one parameter name
                string AssignedField = Assignment.ParameterNameList[0];
                string AssignedFieldText = CSharpNames.ToCSharpIdentifier(AssignedField);

                AssignmentText += $"{AssignedFieldText} = {ExpressionText}";
            }

            lastExpressionText = $"new {ClassNameText}() {{ {AssignmentText} }}";
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
