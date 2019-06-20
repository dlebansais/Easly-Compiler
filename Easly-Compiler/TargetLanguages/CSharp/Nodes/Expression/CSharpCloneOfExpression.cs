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
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>(), -1);
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            // TODO clone of multiple result

            ICSharpType ClonedType = TypeList[0];
            string SourceTypeText = ClonedType.Type2CSharpString(usingCollection, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string SourceText = SourceExpression.CSharpText(usingCollection);

            return $"({SourceTypeText})({SourceText}).Clone()";
        }
        #endregion
    }
}
