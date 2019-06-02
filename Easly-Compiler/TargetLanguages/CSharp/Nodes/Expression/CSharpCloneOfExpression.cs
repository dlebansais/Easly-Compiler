namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpCloneOfExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new ICloneOfExpression Source { get; }

        /// <summary>
        /// The source expression.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of cloned object types.
        /// </summary>
        IList<ICSharpType> TypeList { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpCloneOfExpression : CSharpExpression, ICSharpCloneOfExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpCloneOfExpression Create(ICSharpContext context, ICloneOfExpression source)
        {
            return new CSharpCloneOfExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCloneOfExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpCloneOfExpression(ICSharpContext context, ICloneOfExpression source)
            : base(context, source)
        {
            SourceExpression = Create(context, (IExpression)source.Source);

            IResultType SourceResult = SourceExpression.Source.ResolvedResult.Item;
            Debug.Assert(SourceResult.Count > 0);

            foreach (IExpressionType ExpressionType in SourceResult)
            {
                ICompiledType ClonedType = ExpressionType.ValueType;
                ICSharpType Type = CSharpType.Create(context, ClonedType);
                TypeList.Add(Type);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new ICloneOfExpression Source { get { return (ICloneOfExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return true; } }

        /// <summary>
        /// The source expression.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of cloned object types.
        /// </summary>
        public IList<ICSharpType> TypeList { get; } = new List<ICSharpType>();
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
        public override string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            // TODO clone of multiple result

            ICSharpType ClonedType = TypeList[0];
            string SourceTypeText = ClonedType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string SourceText = SourceExpression.CSharpText(cSharpNamespace);

            return $"({SourceTypeText})({SourceText}).Clone()";
        }
        #endregion
    }
}
