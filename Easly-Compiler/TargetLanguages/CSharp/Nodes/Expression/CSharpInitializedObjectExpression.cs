namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpInitializedObjectExpression : ICSharpExpression, ICSharpExpressionAsConstant
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
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public override void CheckNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            string ClassNameText = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);

            string AssignmentText = string.Empty;
            foreach (ICSharpAssignmentArgument Assignment in AssignmentList)
            {
                if (AssignmentText.Length > 0)
                    AssignmentText += ", ";

                ICSharpExpression SourceExpression = Assignment.SourceExpression;
                string ExpressionText = SingleResultExpressionText(writer, SourceExpression);

                //TODO: handle more than one parameter name
                string AssignedField = Assignment.ParameterNameList[0];
                string AssignedFieldText = CSharpNames.ToCSharpIdentifier(AssignedField);

                AssignmentText += $"{AssignedFieldText} = {ExpressionText}";
            }

            expressionContext.SetSingleReturnValue($"new {ClassNameText}() {{ {AssignmentText} }}");
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return true; } }
        #endregion
    }
}
