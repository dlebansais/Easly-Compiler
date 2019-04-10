namespace CompilerNode
{
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IManifestNumberExpression.
    /// </summary>
    public interface IManifestNumberExpression : BaseNode.IManifestNumberExpression, IExpression
    {
        /// <summary>
        /// The valid value of <see cref="BaseNode.IManifestNumberExpression.Text"/>.
        /// </summary>
        OnceReference<string> ValidText { get; }
    }

    /// <summary>
    /// Compiler IManifestNumberExpression.
    /// </summary>
    public class ManifestNumberExpression : BaseNode.ManifestNumberExpression, IManifestNumberExpression
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// This constructor is needed to allow deserialization of objects.
        /// </summary>
        public ManifestNumberExpression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// </summary>
        /// <param name="value">Initial value.</param>
        public ManifestNumberExpression(int value)
        {
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();
            Text = value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestNumberExpression"/> class.
        /// </summary>
        /// <param name="text">Initial value.</param>
        public ManifestNumberExpression(string text)
        {
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();
            Text = text;
        }
        #endregion

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

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="engine">The engine requesting reset.</param>
        public void Reset(InferenceEngine engine)
        {
            bool IsHandled = false;

            if (engine.RuleTemplateList == RuleTemplateSet.Identifiers)
            {
                ValidText = new OnceReference<string>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The valid value of <see cref="BaseNode.IManifestNumberExpression.Text"/>.
        /// </summary>
        public OnceReference<string> ValidText { get; private set; } = new OnceReference<string>();
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return $"Manifest Number '{Text}'";
        }
        #endregion
    }
}
