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
        public ExpressionType(ITypeName valueTypeName, ICompiledType valueType)
            : this(valueTypeName, valueType, string.Empty)
        {
        }

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
        #endregion

        /*
        public void SetSource(IExpression Source, int Index)
        {
            this.Source = Source;
            this.Index = Index;
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public IExpression Source { get; private set; }
        public int Index { get; private set; }
        */
    }
}
