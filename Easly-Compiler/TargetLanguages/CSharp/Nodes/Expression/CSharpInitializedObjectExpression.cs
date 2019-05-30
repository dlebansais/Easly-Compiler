﻿namespace EaslyCompiler
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
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpInitializedObjectExpression Create(IInitializedObjectExpression source, ICSharpContext context)
        {
            return new CSharpInitializedObjectExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInitializedObjectExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpInitializedObjectExpression(IInitializedObjectExpression source, ICSharpContext context)
            : base(source, context)
        {
            Class = context.GetClass(source.ResolvedClassType.Item.BaseClass);

            foreach (IAssignmentArgument Argument in source.AssignmentList)
            {
                ICSharpAssignmentArgument NewAssignment = CSharpAssignmentArgument.Create(Argument, context);
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
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

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
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpText(cSharpNamespace, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public virtual string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string ClassNameText = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);

            string AssignmentText = string.Empty;
            foreach (ICSharpAssignmentArgument Assignment in AssignmentList)
            {
                if (AssignmentText.Length > 0)
                    AssignmentText += ", ";

                ICSharpExpression SourceExpression = Assignment.SourceExpression;
                string ExpressionText = NestedExpressionText(SourceExpression, cSharpNamespace);

                //TODO: handle more than one parameter name
                string AssignedField = Assignment.ParameterNameList[0];
                string AssignedFieldText = CSharpNames.ToCSharpIdentifier(AssignedField);

                AssignmentText += $"{AssignedFieldText} = {ExpressionText}";
            }

            return $"new {ClassNameText}() {{ {AssignmentText} }}";
        }

        private string NestedExpressionText(ICSharpExpression expression, string cSharpNamespace)
        {
            string Result = expression.CSharpText(cSharpNamespace);

            if (expression.IsComplex)
                Result = $"({Result})";

            return Result;
        }
        #endregion
    }
}