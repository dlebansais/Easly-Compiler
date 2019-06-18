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
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string ClassNameText = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);

            string AssignmentText = string.Empty;
            foreach (ICSharpAssignmentArgument Assignment in AssignmentList)
            {
                if (AssignmentText.Length > 0)
                    AssignmentText += ", ";

                ICSharpExpression SourceExpression = Assignment.SourceExpression;
                string ExpressionText = NestedExpressionText(usingCollection, SourceExpression);

                //TODO: handle more than one parameter name
                string AssignedField = Assignment.ParameterNameList[0];
                string AssignedFieldText = CSharpNames.ToCSharpIdentifier(AssignedField);

                AssignmentText += $"{AssignedFieldText} = {ExpressionText}";
            }

            return $"new {ClassNameText}() {{ {AssignmentText} }}";
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
