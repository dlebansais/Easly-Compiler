namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
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
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// The feature called. Can be null.
        /// </summary>
        ICSharpFeature Feature { get; }

        /// <summary>
        /// The discrete read. Can be null.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The query.
        /// </summary>
        ICSharpQualifiedName Query { get; }
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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpQueryExpression Create(ICSharpContext context, IQueryExpression source)
        {
            return new CSharpQueryExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQueryExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpQueryExpression(ICSharpContext context, IQueryExpression source)
            : base(context, source)
        {
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);

            if (Source.ResolvedFinalFeature.IsAssigned)
            {
                ICompiledFeature ResolvedFeature = Source.ResolvedFinalFeature.Item;
                if (ResolvedFeature is IScopeAttributeFeature AsScopeAttributeFeature)
                {
                    ICSharpClass Owner = context.GetClass(source.EmbeddingClass);
                    Feature = CSharpScopeAttributeFeature.Create(context, Owner, AsScopeAttributeFeature);
                }
                else
                    Feature = context.GetFeature(Source.ResolvedFinalFeature.Item);
            }

            if (Source.ResolvedFinalDiscrete.IsAssigned)
            {
                ICSharpClass Class = context.GetClass(Source.ResolvedFinalDiscrete.Item.EmbeddingClass);

                foreach (ICSharpDiscrete Item in Class.DiscreteList)
                    if (Item.Source == Source.ResolvedFinalDiscrete.Item)
                    {
                        Debug.Assert(Discrete == null);
                        Discrete = Item;
                    }
            }

            Debug.Assert((Feature != null && Discrete == null) || (Feature == null && Discrete != null));

            Query = CSharpQualifiedName.Create(context, (IQualifiedName)Source.Query, Feature, Discrete, source.InheritBySideAttribute);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IQueryExpression Source { get { return (IQueryExpression)base.Source; } }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// The feature called. Can be null.
        /// </summary>
        public ICSharpFeature Feature { get; }

        /// <summary>
        /// The discrete read. Can be null.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The query.
        /// </summary>
        public ICSharpQualifiedName Query { get; }
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
            string ArgumentListText = CSharpArgument.CSharpArgumentList(usingCollection, FeatureCall, destinationList, skippedIndex);
            string QueryText = Query.CSharpText(usingCollection, 0);

            bool IsAgent;
            switch (Feature)
            {
                case ICSharpAttributeFeature AsAttributeFeature:
                case ICSharpConstantFeature AsConstantFeature:
                case ICSharpFunctionFeature AsFunctionFeature:
                case ICSharpPropertyFeature AsPropertyFeature:
                    IsAgent = false;
                    break;

                case ICSharpScopeAttributeFeature AsScopeAttributeFeature:
                    switch (AsScopeAttributeFeature.Type)
                    {
                        case ICSharpProcedureType AsProcedureType:
                        case ICSharpFunctionType AsFunctionType:
                        case ICSharpPropertyType AsPropertyType:
                            IsAgent = true;
                            break;

                        default:
                            IsAgent = false;
                            break;
                    }
                    break;

                default:
                    IsAgent = true;
                    break;
            }

            if (IsAgent)
            {
                IIdentifier AgentIdentifier = (IIdentifier)Source.Query.Path[Source.Query.Path.Count - 1];
                string AgentIdentifierText = CSharpNames.ToCSharpIdentifier(AgentIdentifier.ValidText.Item);

                if (Source.Query.Path.Count > 1)
                    QueryText = Query.CSharpText(usingCollection, 1);
                else
                    QueryText = "this";

                if (FeatureCall.ArgumentList.Count > 0)
                    return $"{AgentIdentifierText}({QueryText}, {ArgumentListText})";
                else
                    return $"{AgentIdentifierText}({QueryText})";
            }
            else
            {
                if (ArgumentListText.Length > 0)
                    return $"{QueryText}({ArgumentListText})";
                else if (Feature is ICSharpFunctionFeature)
                    return $"{QueryText}()";
                else
                    return QueryText;
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
