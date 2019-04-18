namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a hash table of nodes, each with a property that must be an assigned <see cref="IOnceReference"/>.
    /// </summary>
    public interface IOnceReferenceTableSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a <typeparamref name="TKey"/>, <typeparamref name="TValue"/> hash table of nodes, each with a property that must be an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key for each item.</typeparam>
    /// <typeparam name="TValue">Type of the value for each item.</typeparam>
    /// <typeparam name="TRef">Type of the reference in each item value.</typeparam>
    public interface IOnceReferenceTableSourceTemplate<TSource, TKey, TValue, TRef> : ISourceTemplate<TSource, IHashtableEx<TKey, TValue>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a <typeparamref name="TKey"/>, <typeparamref name="TValue"/> hash table of nodes, each with a property that must be an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TKey">Type of the key for each item.</typeparam>
    /// <typeparam name="TValue">Type of the value for each item.</typeparam>
    /// <typeparam name="TRef">Type of the reference in each item value.</typeparam>
    public class OnceReferenceTableSourceTemplate<TSource, TKey, TValue, TRef> : SourceTemplate<TSource, IHashtableEx<TKey, TValue>>, IOnceReferenceTableSourceTemplate<TSource, TKey, TValue, TRef>, IOnceReferenceTableSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceTableSourceTemplate{TSource, TKey, TValue, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="propertyName">The name of the <see cref="OnceReference{TRef}"/> property to check in each item of the list.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceTableSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = BaseNodeHelper.NodeTreeHelper.GetPropertyOf(typeof(TValue), propertyName);
            Debug.Assert(ItemProperty != null);
        }

        private PropertyInfo ItemProperty;
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        public override bool IsReady(TSource node, out object data)
        {
            data = null;
            bool Result = true;

            IList<TRef> ReadyReferenceList = new List<TRef>();

            IHashtableEx<TKey, TValue> ValueTable = GetSourceObject(node, out bool IsInterrupted);

            if (IsInterrupted)
                Result = false;
            else
                foreach (KeyValuePair<TKey, TValue> Entry in ValueTable)
                {
                    TValue Value = Entry.Value;
                    Debug.Assert(Value != null);

                    OnceReference<TRef> Reference = ItemProperty.GetValue(Value) as OnceReference<TRef>;
                    Debug.Assert(Reference != null);

                    if (!Reference.IsAssigned)
                    {
                        Result = false;
                        break;
                    }

                    ReadyReferenceList.Add(Reference.Item);
                }

            if (Result)
                data = ReadyReferenceList;

            return Result;
        }
        #endregion
    }
}
