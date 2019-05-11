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
        /// True if the expression is a constant.
        /// </summary>
        bool IsConstant { get; }

        /// <summary>
        /// Specific constant number.
        /// </summary>
        OnceReference<ILanguageConstant> NumberConstant { get; }

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        OnceReference<IList<IIdentifier>> ResolvedExceptions { get; }
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
        /// <param name="source">The source to use when reporting errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resultTypeName">The type name upon return.</param>
        /// <param name="resultType">The type upon return.</param>
        public static bool IsLanguageTypeAvailable(Guid guid, ISource source, IErrorList errorList, out ITypeName resultTypeName, out ICompiledType resultType)
        {
            resultTypeName = null;
            resultType = null;

            IClass EmbeddingClass = source.EmbeddingClass;
            bool Found = false;
            foreach (KeyValuePair<Guid, Tuple<ITypeName, IClassType>> Entry in EmbeddingClass.ImportedLanguageTypeTable)
            {
                if (Entry.Key == guid)
                {
                    resultTypeName = Entry.Value.Item1;
                    resultType = Entry.Value.Item2;
                    Found = true;
                    break;
                }
            }

            if (!Found)
            {
                bool IsHandled = false;

                if (guid == LanguageClasses.Boolean.Guid)
                {
                    errorList.AddError(new ErrorBooleanTypeMissing(source));
                    IsHandled = true;
                }
                else if (guid == LanguageClasses.Exception.Guid)
                {
                    errorList.AddError(new ErrorExceptionTypeMissing(source));
                    IsHandled = true;
                }

                Debug.Assert(IsHandled);
            }

            return Found;
        }
    }
}
