namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IExpression.
    /// </summary>
    public interface IExpression : BaseNode.IExpression, INode, ISource
    {
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        string ExpressionToString { get; }

        /// <summary>
        /// Types of results of the expression.
        /// </summary>
        OnceReference<IList<IExpressionType>> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        OnceReference<IList<IIdentifier>> ResolvedExceptions { get; }

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        ListTableEx<IExpression> ConstantSourceList { get; }

        /// <summary>
        /// The constant expression, if assigned.
        /// </summary>
        OnceReference<ILanguageConstant> ExpressionConstant { get; }
    }

    /// <summary>
    /// Expression helper class.
    /// </summary>
    public static class Expression
    {
        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IExpression expression1, IExpression expression2)
        {
            if (expression1.GetType() != expression2.GetType())
                return false;

            bool Result = false;
            bool IsHandled = false;

            switch (expression1)
            {
                case IAgentExpression AsAgentExpression:
                    Result = AgentExpression.IsExpressionEqual(expression1 as IAgentExpression, expression2 as IAgentExpression);
                    IsHandled = true;
                    break;

                case IAssertionTagExpression AsAssertionTagExpression:
                    Result = AssertionTagExpression.IsExpressionEqual(expression1 as IAssertionTagExpression, expression2 as IAssertionTagExpression);
                    IsHandled = true;
                    break;

                case IBinaryConditionalExpression AsBinaryConditionalExpression:
                    Result = BinaryConditionalExpression.IsExpressionEqual(expression1 as IBinaryConditionalExpression, expression2 as IBinaryConditionalExpression);
                    IsHandled = true;
                    break;

                case IBinaryOperatorExpression AsBinaryOperatorExpression:
                    Result = BinaryOperatorExpression.IsExpressionEqual(expression1 as IBinaryOperatorExpression, expression2 as IBinaryOperatorExpression);
                    IsHandled = true;
                    break;

                case IClassConstantExpression AsClassConstantExpression:
                    Result = ClassConstantExpression.IsExpressionEqual(expression1 as IClassConstantExpression, expression2 as IClassConstantExpression);
                    IsHandled = true;
                    break;

                case ICloneOfExpression AsCloneOfExpression:
                    Result = CloneOfExpression.IsExpressionEqual(expression1 as ICloneOfExpression, expression2 as ICloneOfExpression);
                    IsHandled = true;
                    break;

                case IEntityExpression AsEntityExpression:
                    Result = EntityExpression.IsExpressionEqual(expression1 as IEntityExpression, expression2 as IEntityExpression);
                    IsHandled = true;
                    break;

                case IEqualityExpression AsEqualityExpression:
                    Result = EqualityExpression.IsExpressionEqual(expression1 as IEqualityExpression, expression2 as IEqualityExpression);
                    IsHandled = true;
                    break;

                case IIndexQueryExpression AsIndexQueryExpression:
                    Result = IndexQueryExpression.IsExpressionEqual(expression1 as IIndexQueryExpression, expression2 as IIndexQueryExpression);
                    IsHandled = true;
                    break;

                case IInitializedObjectExpression AsInitializedObjectExpression:
                    Result = InitializedObjectExpression.IsExpressionEqual(expression1 as IInitializedObjectExpression, expression2 as IInitializedObjectExpression);
                    IsHandled = true;
                    break;

                case IKeywordExpression AsKeywordExpression:
                    Result = KeywordExpression.IsExpressionEqual(expression1 as IKeywordExpression, expression2 as IKeywordExpression);
                    IsHandled = true;
                    break;

                case IManifestCharacterExpression AsManifestCharacterExpression:
                    Result = ManifestCharacterExpression.IsExpressionEqual(expression1 as IManifestCharacterExpression, expression2 as IManifestCharacterExpression);
                    IsHandled = true;
                    break;

                case IManifestNumberExpression AsManifestNumberExpression:
                    Result = ManifestNumberExpression.IsExpressionEqual(expression1 as IManifestNumberExpression, expression2 as IManifestNumberExpression);
                    IsHandled = true;
                    break;

                case IManifestStringExpression AsManifestStringExpression:
                    Result = ManifestStringExpression.IsExpressionEqual(expression1 as IManifestStringExpression, expression2 as IManifestStringExpression);
                    IsHandled = true;
                    break;

                case INewExpression AsNewExpression:
                    Result = NewExpression.IsExpressionEqual(expression1 as INewExpression, expression2 as INewExpression);
                    IsHandled = true;
                    break;

                case IOldExpression AsOldExpression:
                    Result = OldExpression.IsExpressionEqual(expression1 as IOldExpression, expression2 as IOldExpression);
                    IsHandled = true;
                    break;

                case IPrecursorExpression AsPrecursorExpression:
                    Result = PrecursorExpression.IsExpressionEqual(expression1 as IPrecursorExpression, expression2 as IPrecursorExpression);
                    IsHandled = true;
                    break;

                /*
                 * Two precursor index expressions cannot be compared because it happens only when comparing different features, and there can be only one indexer.
                case IPrecursorIndexExpression AsPrecursorIndexExpression:
                    Result = PrecursorIndexExpression.IsExpressionEqual(expression1 as IPrecursorIndexExpression, expression2 as IPrecursorIndexExpression);
                    IsHandled = true;
                    break;
                */
                case IQueryExpression AsQueryExpression:
                    Result = QueryExpression.IsExpressionEqual(expression1 as IQueryExpression, expression2 as IQueryExpression);
                    IsHandled = true;
                    break;

                case IResultOfExpression AsResultOfExpression:
                    Result = ResultOfExpression.IsExpressionEqual(expression1 as IResultOfExpression, expression2 as IResultOfExpression);
                    IsHandled = true;
                    break;

                case IUnaryNotExpression AsUnaryNotExpression:
                    Result = UnaryNotExpression.IsExpressionEqual(expression1 as IUnaryNotExpression, expression2 as IUnaryNotExpression);
                    IsHandled = true;
                    break;

                case IUnaryOperatorExpression AsUnaryOperatorExpression:
                    Result = UnaryOperatorExpression.IsExpressionEqual(expression1 as IUnaryOperatorExpression, expression2 as IUnaryOperatorExpression);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
            return Result;
        }

        /// <summary>
        /// Checks if a built-in language type is imported, and return the corresponding name and type.
        /// </summary>
        /// <param name="guid">The language type guid.</param>
        /// <param name="source">The location where the language type is needed.</param>
        /// <param name="resultTypeName">The type name upon return.</param>
        /// <param name="resultType">The type upon return.</param>
        public static bool IsLanguageTypeAvailable(Guid guid, ISource source, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;

            IClass EmbeddingClass = source.EmbeddingClass;
            bool Found = false;
            foreach (KeyValuePair<Guid, Tuple<ITypeName, IClassType>> Entry in EmbeddingClass.ImportedLanguageTypeTable)
                if (Entry.Key == guid)
                {
                    resultTypeName = Entry.Value.Item1;
                    resultType = Entry.Value.Item2;
                    Found = true;
                    break;
                }

            return Found;
        }

        /// <summary>
        /// Gets the class type of an expression, if any.
        /// </summary>
        /// <param name="booleanOrEventExpression">A boolean or event expression.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="expressionClassType">The class type upon return, if successful.</param>
        public static bool GetClassTypeOfExpression(IExpression booleanOrEventExpression, IErrorList errorList, out IClassType expressionClassType)
        {
            expressionClassType = null;
            bool Result = false;

            IClass EmbeddingClass = booleanOrEventExpression.EmbeddingClass;
            IList<IExpressionType> ExpressionResult = booleanOrEventExpression.ResolvedResult.Item;

            ICompiledType BooleanOrEventExpressionType = null;

            if (ExpressionResult.Count == 1)
                BooleanOrEventExpressionType = ExpressionResult[0].ValueType;
            else
                foreach (IExpressionType Item in ExpressionResult)
                    if (Item.Name == nameof(BaseNode.Keyword.Result))
                    {
                        BooleanOrEventExpressionType = Item.ValueType;
                        break;
                    }

            if (BooleanOrEventExpressionType is IClassType AsClassType)
            {
                if (CheckForBooleanOrEventType(booleanOrEventExpression, AsClassType, errorList))
                {
                    expressionClassType = AsClassType;
                    Result = true;
                }
            }
            else
                errorList.AddError(new ErrorInvalidExpression(booleanOrEventExpression));

            return Result;
        }

        /// <summary>
        /// Checks that the expected type, either boolean or event, is available.
        /// </summary>
        /// <param name="booleanOrEventExpression">The expression to check.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="errorList">The list of errors found.</param>
        public static bool CheckForBooleanOrEventType(IExpression booleanOrEventExpression, IClassType expectedType, IErrorList errorList)
        {
            bool IsTypeFound = false;
            bool IsBooleanAvailable = IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, booleanOrEventExpression, out ITypeName BooleanTypeName, out ICompiledType BooleanType);
            bool IsEventAvailable = IsLanguageTypeAvailable(LanguageClasses.Event.Guid, booleanOrEventExpression, out ITypeName EventTypeName, out ICompiledType EventType);

            if (!IsBooleanAvailable && !IsEventAvailable)
            {
                if (expectedType.BaseClass.EntityName.Text != LanguageClasses.Event.Name)
                    errorList.AddError(new ErrorBooleanTypeMissing(booleanOrEventExpression));
                if (expectedType.BaseClass.EntityName.Text != LanguageClasses.Boolean.Name)
                    errorList.AddError(new ErrorEventTypeMissing(booleanOrEventExpression));

                Debug.Assert(!errorList.IsEmpty);
            }
            else if ((!IsBooleanAvailable || expectedType != BooleanType) && (!IsEventAvailable || expectedType != EventType))
                errorList.AddError(new ErrorInvalidExpression(booleanOrEventExpression));
            else
                IsTypeFound = true;

            return IsTypeFound;
        }

        /// <summary>
        /// Merge two lists of exceptions source code can throw.
        /// </summary>
        /// <param name="mergedExceptions">The list with the merged content upon return.</param>
        /// <param name="additionalExceptions">The list of exception identifiers to merge with <paramref name="mergedExceptions"/>.</param>
        public static void MergeExceptions(IList<IIdentifier> mergedExceptions, IList<IIdentifier> additionalExceptions)
        {
            foreach (IIdentifier Item in additionalExceptions)
            {
                bool Found = false;
                foreach (IIdentifier OtherItem in mergedExceptions)
                    if (OtherItem.ValidText.Item == Item.ValidText.Item)
                    {
                        Found = true;
                        break;
                    }

                if (!Found)
                    mergedExceptions.Add(Item);
            }
        }

        /// <summary>
        /// Gets the final constant in a chain of expressions.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public static ILanguageConstant FinalConstant(IExpression expression)
        {
            ILanguageConstant Result = null;

            Debug.Assert(expression.ExpressionConstant.IsAssigned);
            ILanguageConstant ExpressionConstant = expression.ExpressionConstant.Item;

            if (ExpressionConstant == NeutralLanguageConstant.NotConstant)
                Result = ExpressionConstant;
            else
            {
                switch (ExpressionConstant)
                {
                    case IAgentLanguageConstant AsAgentLanguageConstant:
                    case IBooleanLanguageConstant AsBooleanLanguageConstant:
                    case ICharacterLanguageConstant AsCharacterLanguageConstant:
                    case IEntityLanguageConstant AsEntityLanguageConstant:
                    case INumberLanguageConstant AsNumberLanguageConstant:
                    case IObjectLanguageConstant AsObjectLanguageConstant:
                    case IStringLanguageConstant AsStringLanguageConstant:
                        Result = ExpressionConstant;
                        break;

                    case IDiscreteLanguageConstant AsDiscreteLanguageConstant:
                        if (AsDiscreteLanguageConstant.IsValueKnown && AsDiscreteLanguageConstant.Discrete.NumericValue.IsAssigned)
                            Result = FinalConstant((IExpression)AsDiscreteLanguageConstant.Discrete.NumericValue.Item);
                        else
                            Result = ExpressionConstant;
                        break;
                }

                Debug.Assert(Result != null);
            }

            return Result;
        }
    }
}
