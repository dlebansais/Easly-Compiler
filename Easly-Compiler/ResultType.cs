namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// The result type(s) of a node.
    /// </summary>
    public interface IResultType : IReadOnlyCollection<IExpressionType>
    {
        /// <summary>
        /// Index of the result type to select with a 'result of' expression. -1 if none.
        /// </summary>
        int ResultNameIndex { get; }

        /// <summary>
        /// Gets the type at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        IExpressionType At(int index);

        /// <summary>
        /// Get the type of the result if only one, or the one named 'result' if more than one.
        /// </summary>
        /// <param name="type">The result type upon return, if successful.</param>
        bool TryGetResult(out ICompiledType type);

        /// <summary>
        /// Gets the list of types.
        /// </summary>
        IReadOnlyList<IExpressionType> ToList();
    }

    /// <summary>
    /// The result type(s) of a node.
    /// </summary>
    public class ResultType : List<IExpressionType>, IResultType
    {
        #region Init
        /// <summary>
        /// A special value that contains no result.
        /// </summary>
        public static IResultType Empty { get; } = new ResultType();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultType"/> class.
        /// </summary>
        protected ResultType()
        {
            ResultNameIndex = GetResultNameIndex();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultType"/> class.
        /// </summary>
        /// <param name="item">The single result.</param>
        public ResultType(IExpressionType item)
        {
            Add(item);
            ResultNameIndex = GetResultNameIndex();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultType"/> class.
        /// </summary>
        /// <param name="resultList">The list of result type(s).</param>
        public ResultType(IList<IExpressionType> resultList)
        {
            AddRange(resultList);
            ResultNameIndex = GetResultNameIndex();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultType"/> class.
        /// </summary>
        /// <param name="valueTypeName">The expression type name.</param>
        /// <param name="valueType">The expression type.</param>
        /// <param name="name">Name of the expression value, empty if none.</param>
        public ResultType(ITypeName valueTypeName, ICompiledType valueType, string name)
        {
            Add(new ExpressionType(valueTypeName, valueType, name));
            ResultNameIndex = GetResultNameIndex();
        }

        private int GetResultNameIndex()
        {
            int Index = -1;

            for (int i = 0; i < Count; i++)
            {
                IExpressionType Item = this[i];
                if (Item.IsResultName)
                {
                    Debug.Assert(Index == -1);
                    Index = i;
                }
            }

            return Index;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Index of the result type to select with a 'result of' expression. -1 if none.
        /// </summary>
        public int ResultNameIndex { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the type at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        public IExpressionType At(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Get the type of the result if only one, or the one named 'result' if more than one.
        /// </summary>
        /// <param name="type">The result type upon return, if successful.</param>
        public virtual bool TryGetResult(out ICompiledType type)
        {
            type = null;

            if (Count == 1)
                type = At(0).ValueType;
            else
                foreach (ExpressionType Item in this)
                    if (Item.Name == nameof(BaseNode.Keyword.Result))
                    {
                        Debug.Assert(type == null);
                        type = Item.ValueType;
                        Debug.Assert(type != null);
                    }

            return type != null;
        }

        /// <summary>
        /// Gets the list of types.
        /// </summary>
        public IReadOnlyList<IExpressionType> ToList()
        {
            return this;
        }
        #endregion
    }
}
