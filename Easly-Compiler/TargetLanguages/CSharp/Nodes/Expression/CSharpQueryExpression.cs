namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpQueryExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IQueryExpression Source { get; }

        /// <summary>
        /// The query.
        /// </summary>
        ICSharpQualifiedName Query { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpQueryExpression : CSharpExpression, ICSharpQueryExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpQueryExpression Create(IQueryExpression source, ICSharpContext context)
        {
            return new CSharpQueryExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQueryExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpQueryExpression(IQueryExpression source, ICSharpContext context)
            : base(source, context)
        {
            Query = CSharpQualifiedName.Create((IQualifiedName)Source.Query, context);
            FeatureCall = new CSharpFeatureCall(source.SelectedParameterList, source.ArgumentList, source.ArgumentStyle, context);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IQueryExpression Source { get { return (IQueryExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The query.
        /// </summary>
        public ICSharpQualifiedName Query { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }
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
            string ArgumentListText = CSharpArgument.CSharpArgumentList(cSharpNamespace, FeatureCall, destinationList);

            if (ArgumentListText.Length > 0)
            {
                string QueryText = Query.CSharpText(cSharpNamespace, 0);
                return $"{QueryText}({ArgumentListText})";
            }
            /*
            else
            {
                IList<IIdentifier> ValidPath = Query.Source.ValidPath.Item;
                IList<IExpressionType> ValidResultTypePath = Query.Source.ValidResultTypePath.Item;

                if (ValidResultTypePath.Count >= 2)
                {
                    ExpressionType CallerExpressionType = ValidResultTypePath[ValidResultTypePath.Count - 2];
                    string CalledFeature = ValidPath[ValidPath.Count - 1].ValidText.Item;
                    if (CalledFeature == "Has Handler")
                    {
                        foreach (KeyValuePair<TypeName, ICompiledType> Entry in CallerExpressionType.ValueType.ConformanceTable)
                        {
                            ClassType AsClassType;
                            if ((AsClassType = Entry.Value as ClassType) != null)
                            {
                                Class BaseClass = (Class)AsClassType.BaseClass;
                                if (BaseClass.InheritFromDotNetEvent)
                                {
                                    bool SkipParenthesis = false;
                                    if (ParentSource is Conditional)
                                        SkipParenthesis = true;

                                    return (SkipParenthesis ? "" : "(") + Query.DecoratedCSharpText(Context, 1) + " " + "!=" + " " + "null" + (SkipParenthesis ? "" : ")");
                                }
                            }
                        }
                    }
                }

                return Query.DecoratedCSharpText(cSharpNamespace, 0);
            }*/
        }
        #endregion
    }
}
