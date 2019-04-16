namespace EaslyCompiler
{
    using System.Diagnostics;
    using Easly;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unassigned <see cref="IOnceReference"/>.
    /// </summary>
    public interface IOnceReferenceDestinationTemplate : IDestinationTemplate
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unassigned <see cref="OnceReference{TRef}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceDestinationTemplate<TSource, TRef> : IDestinationTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unassigned <see cref="OnceReference{TRef}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceDestinationTemplate<TSource, TRef> : DestinationTemplate<TSource, OnceReference<TRef>>, IOnceReferenceDestinationTemplate<TSource, TRef>, IOnceReferenceDestinationTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceDestinationTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceDestinationTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="node">The node for which the value is to be checked.</param>
        public override bool IsSet(TSource node)
        {
            OnceReference<TRef> Value = GetDestinationObject(node);
            Debug.Assert(Value == GetDestinationObject((ISource)node));

            return Value.IsAssigned;
        }
        #endregion
    }
}
