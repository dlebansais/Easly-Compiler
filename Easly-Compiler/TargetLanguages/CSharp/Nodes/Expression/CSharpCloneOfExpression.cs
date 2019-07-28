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
            Debug.Assert(WriteDown);

            Debug.Assert(TypeList.Count > 0);

            string CloneMethod = Source.Type == BaseNode.CloneType.Shallow ? "CloneShallow" : "Clone";

            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            SourceExpression.WriteCSharp(writer, SourceExpressionContext, -1);

            IList<string> ResultList = new List<string>();
            int ReturnValueIndex = SourceExpressionContext.ReturnValueIndex;
            string ReturnValue = SourceExpressionContext.ReturnValue;

            if (TypeList.Count == 1)
            {
                ICSharpType ClonedType = TypeList[0];
                string SourceTypeText = ClonedType.Type2CSharpString(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

                string SourceText = SourceExpressionContext.ReturnValue;
                Debug.Assert(SourceText != null);

                expressionContext.SetSingleReturnValue($"({SourceTypeText})({SourceText}).{CloneMethod}()");
            }
            else
            {
                for (int i = 0; i < SourceExpressionContext.CompleteDestinationNameList.Count; i++)
                {
                    if (i == ReturnValueIndex)
                    {
                        Debug.Assert(ReturnValue != null);
                        ResultList.Add(ReturnValue);
                    }
                    else
                        ResultList.Add(SourceExpressionContext.CompleteDestinationNameList[i]);
                }

                Debug.Assert(TypeList.Count == ResultList.Count);

                IList<string> OutgoingResultList = new List<string>();

                for (int i = 0; i < TypeList.Count; i++)
                {
                    ICSharpType ClonedType = TypeList[i];
                    string SourceTypeText = ClonedType.Type2CSharpString(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
                    string SourceText = ResultList[i];

                    OutgoingResultList.Add($"({SourceTypeText})({SourceText}).{CloneMethod}()");
                }

                expressionContext.SetMultipleResult(OutgoingResultList, ReturnValueIndex);
            }
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            SourceExpression.SetWriteDown();
        }
        #endregion
    }
}
