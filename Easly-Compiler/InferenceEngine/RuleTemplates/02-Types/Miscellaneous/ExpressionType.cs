namespace EaslyCompiler
{
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
        #endregion
    }
}
