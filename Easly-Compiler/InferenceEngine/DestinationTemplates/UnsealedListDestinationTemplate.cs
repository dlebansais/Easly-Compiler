namespace EaslyCompiler
{
    using System.Diagnostics;
    using Easly;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unsealed <see cref="IListTableEx"/>.
    /// </summary>
    public interface IUnsealedListDestinationTemplate : IDestinationTemplate
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unsealed <see cref="ListTableEx{TItem}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of the item.</typeparam>
    public interface IUnsealedListDestinationTemplate<TSource, TItem> : IDestinationTemplate<TSource, ListTableEx<TItem>>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// The destination is an unsealed <see cref="ListTableEx{TItem}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of the item.</typeparam>
    public class UnsealedListDestinationTemplate<TSource, TItem> : DestinationTemplate<TSource, ListTableEx<TItem>>, IUnsealedListDestinationTemplate<TSource, TItem>, IUnsealedListDestinationTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsealedListDestinationTemplate{TSource, TItem}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public UnsealedListDestinationTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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
            ListTableEx<TItem> ListValue = GetDestinationObject(node);
            Debug.Assert(ListValue == GetDestinationObject((ISource)node));

            return ListValue.IsSealed;
        }
        #endregion
    }
}
