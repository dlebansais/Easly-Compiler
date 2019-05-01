namespace EaslyCompiler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be a sealed <see cref="IListTableEx"/>.
    /// </summary>
    public interface ISealedListCollectionSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of <typeparamref name="TItem"/> nodes, each with a property that must be a sealed <see cref="ListTableEx{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TValue">Type of <see cref="IListTableEx"/> in each item.</typeparam>
    public interface ISealedListCollectionSourceTemplate<TSource, TItem, TValue> : ISourceTemplate<TSource, IList>
        where TSource : ISource
        where TValue : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of <typeparamref name="TItem"/> nodes, each with a property that must be a sealed <see cref="ListTableEx{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TValue">Type of <see cref="IListTableEx"/> in each item.</typeparam>
    public class SealedListCollectionSourceTemplate<TSource, TItem, TValue> : SourceTemplate<TSource, IList>, ISealedListCollectionSourceTemplate<TSource, TItem, TValue>, ISealedListCollectionSourceTemplate
        where TSource : ISource
        where TValue : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SealedListCollectionSourceTemplate{TSource, TItem, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="propertyName">The name of the <see cref="ListTableEx{TValue}"/> property to check in each items of the list.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SealedListCollectionSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = BaseNodeHelper.NodeTreeHelper.GetPropertyOf(typeof(TItem), propertyName);
            Debug.Assert(ItemProperty != null);
            Debug.Assert(ItemProperty.PropertyType.GetInterface(typeof(IListTableEx).Name) != null);
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
            IList ItemList = GetSourceObject(node, out bool IsInterrupted);
            IList<ListTableEx<TValue>> ReadyValueList = new List<ListTableEx<TValue>>();

            for (int i = 0; i < ItemList.Count && !IsInterrupted; i++)
            {
                TItem Item = (TItem)ItemList[i];

                ListTableEx<TValue> Value = ItemProperty.GetValue(Item) as ListTableEx<TValue>;
                Debug.Assert(Value != null);

                if (Value.IsSealed)
                    ReadyValueList.Add(Value);
                else
                    IsInterrupted = true;
            }

            if (IsInterrupted)
            {
                data = null;
                return false;
            }
            else
            {
                data = ReadyValueList;
                return true;
            }
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return $"{Path} * {ItemProperty.Name}, from {StartingPoint}";
        }
        #endregion
    }
}
