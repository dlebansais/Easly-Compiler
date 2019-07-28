namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpClassConstantExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IClassConstantExpression Source { get; }

        /// <summary>
        /// The constant feature. Can be null.
        /// </summary>
        ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The constant value as an expression. Can be null.
        /// </summary>
        ICSharpExpression ConstantExpression { get; }

        /// <summary>
        /// The constant discrete. Can be null.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        ICSharpClass Class { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpClassConstantExpression : CSharpExpression, ICSharpClassConstantExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpClassConstantExpression Create(ICSharpContext context, IClassConstantExpression source)
        {
            return new CSharpClassConstantExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassConstantExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpClassConstantExpression(ICSharpContext context, IClassConstantExpression source)
            : base(context, source)
        {
            if (source.ResolvedFinalFeature.IsAssigned)
            {
                Feature = context.GetFeature(source.ResolvedFinalFeature.Item) as ICSharpConstantFeature;
                ConstantExpression = Feature.ConstantExpression;
            }

            if (source.ResolvedFinalDiscrete.IsAssigned)
                Discrete = CSharpDiscrete.Create(context, source.ResolvedFinalDiscrete.Item);

            Debug.Assert((Feature != null && Discrete == null) || (Feature == null && Discrete != null));

            Class = context.GetClass(source.ResolvedClassType.Item.BaseClass);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IClassConstantExpression Source { get { return (IClassConstantExpression)base.Source; } }

        /// <summary>
        /// The constant feature. Can be null.
        /// </summary>
        public ICSharpConstantFeature Feature { get; }

        /// <summary>
        /// The constant value as an expression. Can be null.
        /// </summary>
        public ICSharpExpression ConstantExpression { get; }

        /// <summary>
        /// The constant discrete. Can be null.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The feature class.
        /// </summary>
        public ICSharpClass Class { get; }
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
            string TypeText;
            string ClassName;
            string ConstantName;

            if (Feature != null)
            {
                TypeText = string.Empty;

                if (Class.ValidSourceName == "Microsoft .NET")
                {
                    ClassName = CSharpNames.ToDotNetIdentifier(Class.ValidClassName);
                    ConstantName = CSharpNames.ToDotNetIdentifier(Feature.Name);
                }
                else
                {
                    ClassName = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);
                    ConstantName = CSharpNames.ToCSharpIdentifier(Feature.Name);
                }
            }
            else
            {
                TypeText = "(int)";

                if (Class.HasDiscreteWithUnkownValue)
                    ClassName = CSharpNames.ToCSharpIdentifier(Class.ValidClassName) + "_Enum";
                else
                    ClassName = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);

                ConstantName = CSharpNames.ToCSharpIdentifier(Discrete.Name);
            }

            expressionContext.SetSingleReturnValue($"{TypeText}{ClassName}.{ConstantName}");
        }
        #endregion

        #region Implementation of ICSharpExpressionAsConstant
        /// <summary>
        /// True if the expression can provide its constant value directly.
        /// </summary>
        public bool IsDirectConstant { get { return false; } }
        #endregion

        #region Implementation of ICSharpComputableExpression
        /// <summary>
        /// The expression computed constant value.
        /// </summary>
        public string ComputedValue { get; private set; }

        /// <summary>
        /// Runs the compiler to compute the value as a string.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public void Compute(ICSharpWriter writer)
        {
            if (Feature != null)
                ComputeFeature(writer);
            else
                ComputeDiscrete(writer);
        }

        private void ComputeFeature(ICSharpWriter writer)
        {
            string ValueString;

            ICSharpExpressionAsConstant ExpressionAsConstant = ConstantExpression as ICSharpExpressionAsConstant;
            Debug.Assert(ExpressionAsConstant != null);

            if (ExpressionAsConstant.IsDirectConstant)
            {
                ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
                ConstantExpression.WriteCSharp(writer, SourceExpressionContext, -1);

                ValueString = SourceExpressionContext.ReturnValue;
            }
            else
            {
                ICSharpComputableExpression ComputableExpression = ExpressionAsConstant as ICSharpComputableExpression;
                Debug.Assert(ComputableExpression != null);

                ComputableExpression.Compute(writer);
                ValueString = ComputableExpression.ComputedValue;
            }

            ComputedValue = ValueString;
        }

        private void ComputeDiscrete(ICSharpWriter writer)
        {
            Debug.Assert(Discrete != null);

            ISealableDictionary<IDiscrete, string> AssignedDiscreteTable = Class.Source.AssignedDiscreteTable;

            Debug.Assert(AssignedDiscreteTable.ContainsKey(Discrete.Source));

            ComputedValue = AssignedDiscreteTable[Discrete.Source];
        }
        #endregion
    }
}
