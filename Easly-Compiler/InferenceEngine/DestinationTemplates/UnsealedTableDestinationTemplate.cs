namespace EaslyCompiler
{
    using System.Diagnostics;
    using Easly;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IUnsealedTableDestinationTemplate : IDestinationTemplate
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IUnsealedTableDestinationTemplate<TSource, TKey, TValue> : IDestinationTemplate<TSource, IHashtableEx<TKey, TValue>>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class UnsealedTableDestinationTemplate<TSource, TKey, TValue> : DestinationTemplate<TSource, IHashtableEx<TKey, TValue>>, IUnsealedTableDestinationTemplate<TSource, TKey, TValue>, IUnsealedTableDestinationTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsealedTableDestinationTemplate{TSource, TKey, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public UnsealedTableDestinationTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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
            IHashtableEx<TKey, TValue> Value = GetDestinationObject(node);
            Debug.Assert(Value == GetDestinationObject((ISource)node));

            return Value.IsSealed;
        }
        #endregion
    }
}
