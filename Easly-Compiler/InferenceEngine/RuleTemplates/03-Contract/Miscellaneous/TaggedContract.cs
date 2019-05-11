namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A contract and its associated tag.
    /// </summary>
    public interface ITaggedContract
    {
        /// <summary>
        /// The contract.
        /// </summary>
        IExpression Contract { get; }

        /// <summary>
        /// The associated tag.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// True if there is a valid tag.
        /// </summary>
        bool HasTag { get; }
    }

    /// <summary>
    /// A contract and its associated tag.
    /// </summary>
    public class TaggedContract : ITaggedContract
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedContract"/> class.
        /// </summary>
        /// <param name="contract">The contract.</param>
        public TaggedContract(IExpression contract)
        {
            Tag = string.Empty;
            Contract = contract;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedContract"/> class.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <param name="tag">The associated tag.</param>
        public TaggedContract(IExpression contract, string tag)
        {
            Tag = tag;
            Contract = contract;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The contract.
        /// </summary>
        public IExpression Contract { get; }

        /// <summary>
        /// The associated tag.
        /// </summary>
        public string Tag { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// True if there is a valid tag.
        /// </summary>
        public bool HasTag
        {
            get { return Tag.Length > 0; }
        }
        #endregion
    }
}
