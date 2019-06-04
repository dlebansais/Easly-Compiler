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
        OnceReference<IResultType> ResolvedResult { get; }

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        OnceReference<IResultException> ResolvedException { get; }

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        SealableList<IExpression> ConstantSourceList { get; }

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

            IComparableExpression ComparableExpression1 = expression1 as IComparableExpression;
            Debug.Assert(ComparableExpression1 != null);
            IComparableExpression ComparableExpression2 = expression2 as IComparableExpression;
            Debug.Assert(ComparableExpression1 != null);

#if DEBUG
            bool Result1 = ComparableExpression1.IsExpressionEqual(ComparableExpression2);
            bool Result2 = ComparableExpression2.IsExpressionEqual(ComparableExpression1);

            Debug.Assert(Result1 == Result2);

            return Result1;
#else
            return ComparableExpression1.IsExpressionEqual(ComparableExpression2);
#endif
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
            IResultType ExpressionResult = booleanOrEventExpression.ResolvedResult.Item;

            if (ExpressionResult.TryGetResult(out ICompiledType BooleanOrEventExpressionType) && BooleanOrEventExpressionType is IClassType AsClassType)
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
