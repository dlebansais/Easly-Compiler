namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Type of an expression.
    /// </summary>
    public interface IExpressionType
    {
        /// <summary>
        /// The expression type name.
        /// </summary>
        ITypeName ValueTypeName { get; }

        /// <summary>
        /// The expression type.
        /// </summary>
        ICompiledType ValueType { get; }

        /// <summary>
        /// Name of the expression value, empty if none.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The expression with this type.
        /// </summary>
        IExpression Source { get; }

        /// <summary>
        /// True if the associated name is 'Result'.
        /// </summary>
        bool IsResultName { get; }

        /// <summary>
        /// Index of this type in results of <see cref="Source"/>.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The number kind if the expression type is a number.
        /// </summary>
        NumberKinds NumberKind { get; }

        /// <summary>
        /// Sets the origin expression of this type.
        /// </summary>
        /// <param name="source">The expression with this type.</param>
        /// <param name="index">Index of this type in results of <paramref name="source"/>.</param>
        void SetSource(IExpression source, int index);

        /// <summary>
        /// Set this type name.
        /// </summary>
        /// <param name="name">The name to set.</param>
        void SetName(string name);

        /// <summary>
        /// Tentatively updates the number kind of the result.
        /// </summary>
        /// <param name="numberKind">The new number kind.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        void UpdateNumberKind(NumberKinds numberKind, ref bool isChanged);
    }

    /// <summary>
    /// Type of an expression.
    /// </summary>
    public class ExpressionType : IExpressionType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionType"/> class.
        /// </summary>
        /// <param name="valueTypeName">The expression type name.</param>
        /// <param name="valueType">The expression type.</param>
        /// <param name="name">Name of the expression value, empty if none.</param>
        public ExpressionType(ITypeName valueTypeName, ICompiledType valueType, string name)
        {
            ValueTypeName = valueTypeName;
            ValueType = valueType;
            Name = name;

            if (valueType is ICompiledNumberType AsNumberType)
                NumberKind = AsNumberType.GetDefaultNumberKind();
            else
                NumberKind = NumberKinds.NotApplicable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The expression type name.
        /// </summary>
        public ITypeName ValueTypeName { get; }

        /// <summary>
        /// The expression type.
        /// </summary>
        public ICompiledType ValueType { get; }

        /// <summary>
        /// Name of the expression value, empty if none.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The expression with this type.
        /// </summary>
        public IExpression Source { get; private set; }

        /// <summary>
        /// True if the associated name is 'Result'.
        /// </summary>
        public bool IsResultName { get { return Name == nameof(BaseNode.Keyword.Result); } }

        /// <summary>
        /// Index of this type in results of <see cref="Source"/>.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The number kind if the expression type is a number.
        /// </summary>
        public NumberKinds NumberKind { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the origin expression of this type.
        /// </summary>
        /// <param name="source">The expression with this type.</param>
        /// <param name="index">Index of this type in results of <paramref name="source"/>.</param>
        public void SetSource(IExpression source, int index)
        {
            Source = source;
            Index = index;
        }

        /// <summary>
        /// Set this type name.
        /// </summary>
        /// <param name="name">The name to set.</param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Tentatively updates the number kind of the result.
        /// </summary>
        /// <param name="numberKind">The new number kind.</param>
        /// <param name="isChanged">True if the number kind was changed.</param>
        public void UpdateNumberKind(NumberKinds numberKind, ref bool isChanged)
        {
            Debug.Assert(numberKind != NumberKinds.NotChecked);
            Debug.Assert(ValueType is ICompiledNumberType);

            if (NumberKind == NumberKinds.NotChecked)
            {
                NumberKind = numberKind;
                isChanged = true;
            }
            else
            {
                Debug.Assert(NumberKind != NumberKinds.NotApplicable || numberKind == NumberKinds.NotApplicable);

                if (NumberKind == NumberKinds.Unknown)
                {
                    if (numberKind == NumberKinds.Integer)
                    {
                        NumberKind = NumberKinds.Integer;
                        isChanged = true;
                    }

                    if (numberKind == NumberKinds.Real)
                    {
                        NumberKind = NumberKinds.Real;
                        isChanged = true;
                    }
                }
            }
        }
        #endregion
    }
}
