namespace CompilerNode
{
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IExpression.
    /// </summary>
    public interface IExpression : BaseNode.IExpression, INode, ISource
    {
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

                case IPrecursorIndexExpression AsPrecursorIndexExpression:
                    Result = PrecursorIndexExpression.IsExpressionEqual(expression1 as IPrecursorIndexExpression, expression2 as IPrecursorIndexExpression);
                    IsHandled = true;
                    break;

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
    }
}
