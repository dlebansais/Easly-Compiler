namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IOnceReferenceDestinationTemplate : IDestinationTemplate
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceDestinationTemplate<TNode, TRef> : IDestinationTemplate<TNode, OnceReference<TRef>>
        where TNode : INode
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceDestinationTemplate<TNode, TRef> : DestinationTemplate<TNode, OnceReference<TRef>>, IOnceReferenceDestinationTemplate<TNode, TRef>, IOnceReferenceDestinationTemplate
        where TNode : INode
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceDestinationTemplate{TNode, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceDestinationTemplate(string path, ITemplatePathStart startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="node">The node for which the value is to be checked.</param>
        public override bool IsSet(TNode node)
        {
            OnceReference<TRef> Value = GetDestinationObject(node);
            return Value.IsAssigned;
        }
        #endregion
    }
}
