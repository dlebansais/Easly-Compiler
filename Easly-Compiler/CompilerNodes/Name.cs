namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IName.
    /// </summary>
    public interface IName : BaseNode.IName, INode, ISource
    {
        /// <summary>
        /// The valid value of <see cref="BaseNode.IName.Text"/>.
        /// </summary>
        OnceReference<string> ValidText { get; }
    }

    /// <summary>
    /// Compiler IName.
    /// </summary>
    public class Name : BaseNode.Name, IName
    {
        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IQueryOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The valid value of <see cref="BaseNode.IName.Text"/>.
        /// </summary>
        public OnceReference<string> ValidText { get; } = new OnceReference<string>();
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Name '{Text}'";
        }
        #endregion
    }
}
