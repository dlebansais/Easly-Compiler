namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpQueryExpression : ICSharpExpression, ICSharpExpressionAsConstant, ICSharpComputableExpression
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

        /// <summary>
        /// The selected overload type. Can be null.
        /// </summary>
        ICSharpQueryOverloadType SelectedOverloadType { get; }
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

            if (Source.SelectedOverloadType.IsAssigned)
            {
                Debug.Assert(Feature != null);

                SelectedOverloadType = CSharpQueryOverloadType.Create(context, Source.SelectedOverloadType.Item, Feature.Owner);
            }
            else
                SelectedOverloadType = null;

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

        /// <summary>
        /// The selected overload type. Can be null.
        /// </summary>
        public ICSharpQueryOverloadType SelectedOverloadType { get; }

        /// <summary>
        /// True if calling an agent.
        /// </summary>
        public bool IsAgent
        {
            get
            {
                bool Result;

                switch (Feature)
                {
                    case ICSharpAttributeFeature AsAttributeFeature:
                    case ICSharpConstantFeature AsConstantFeature:
                    case ICSharpFunctionFeature AsFunctionFeature:
                    case ICSharpPropertyFeature AsPropertyFeature:
                        Result = false;
                        break;

                    case ICSharpScopeAttributeFeature AsScopeAttributeFeature:
                        switch (AsScopeAttributeFeature.Type)
                        {
                            case ICSharpProcedureType AsProcedureType:
                            case ICSharpFunctionType AsFunctionType:
                            case ICSharpPropertyType AsPropertyType:
                                Result = true;
                                break;

                            default:
                                Result = false;
                                break;
                        }
                        break;

                    default:
                        Result = true;
                        break;
                }

                return Result;
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            if (IsAgent)
                WriteCSharpAgentCall(writer, expressionContext, skippedIndex);
            else if (Discrete != null)
                WriteCSharpDiscreteCall(writer, expressionContext, skippedIndex);
            else
                WriteCSharpFeatureCall(writer, expressionContext, skippedIndex);

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

        private void WriteCSharpAgentCall(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            CSharpArgument.CSharpArgumentList(writer, expressionContext, FeatureCall, skippedIndex, true, out string ArgumentListText, out IList<string> OutgoingResultList);
            string QueryText = Query.CSharpText(writer, 0);

            IIdentifier AgentIdentifier = (IIdentifier)Source.Query.Path[Source.Query.Path.Count - 1];
            string AgentIdentifierText = CSharpNames.ToCSharpIdentifier(AgentIdentifier.ValidText.Item);

            if (Source.Query.Path.Count > 1)
                QueryText = Query.CSharpText(writer, 1);
            else
                QueryText = "this";

            if (FeatureCall.ArgumentList.Count > 0)
                expressionContext.SetSingleReturnValue($"{AgentIdentifierText}({QueryText}, {ArgumentListText})");
            else
                expressionContext.SetSingleReturnValue($"{AgentIdentifierText}({QueryText})");
        }

        private void WriteCSharpDiscreteCall(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            Debug.Assert(FeatureCall.ParameterList.Count == 0);
            Debug.Assert(FeatureCall.ResultList.Count == 0);

            expressionContext.SetSingleReturnValue(Query.CSharpText(writer, 0));
        }

        private void WriteCSharpFeatureCall(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            Feature.GetOutputFormat(SelectedOverloadType, out int OutgoingParameterCount, out int ReturnValueIndex);

            CSharpArgument.CSharpArgumentList(writer, expressionContext, FeatureCall, ReturnValueIndex, false, out string ArgumentListText, out IList<string> OutgoingResultList);
            string QueryText = Query.CSharpText(writer, 0);

            Debug.Assert(OutgoingParameterCount > 0);

            if (OutgoingParameterCount == 1)
            {
                if (ArgumentListText.Length > 0)
                    expressionContext.SetSingleReturnValue($"{QueryText}({ArgumentListText})");
                else if (Feature is ICSharpFunctionFeature)
                    expressionContext.SetSingleReturnValue($"{QueryText}()");
                else
                {
                    if (writer.AttachmentMap.ContainsKey(QueryText))
                        QueryText = writer.AttachmentMap[QueryText];

                    expressionContext.SetSingleReturnValue(QueryText);
                }
            }
            else
            {
                if (ReturnValueIndex >= 0)
                {
                    string TemporaryResultName = writer.GetTemporaryName();
                    writer.WriteIndentedLine($"var {TemporaryResultName} = {QueryText}({ArgumentListText});");

                    OutgoingResultList.Insert(ReturnValueIndex, TemporaryResultName);
                }
                else
                    writer.WriteIndentedLine($"{QueryText}({ArgumentListText});");

                expressionContext.SetMultipleResult(OutgoingResultList, ReturnValueIndex);
            }
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
            //TODO
        }
        #endregion
    }
}
