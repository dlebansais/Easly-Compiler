﻿namespace EaslyCompiler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be a sealed <see cref="IHashtableEx"/>.
    /// </summary>
    public interface ISealedTableCollectionSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be a sealed <see cref="IHashtableEx{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TKey">Type of the key in each item.</typeparam>
    /// <typeparam name="TValue">Type of the value in each item.</typeparam>
    public interface ISealedTableCollectionSourceTemplate<TSource, TItem, TKey, TValue> : ISourceTemplate<TSource, IList>
        where TSource : ISource
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be a sealed <see cref="IHashtableEx{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TKey">Type of the key in each item.</typeparam>
    /// <typeparam name="TValue">Type of the value in each item.</typeparam>
    public class SealedTableCollectionSourceTemplate<TSource, TItem, TKey, TValue> : SourceTemplate<TSource, IList>, ISealedTableCollectionSourceTemplate<TSource, TItem, TKey, TValue>, ISealedTableCollectionSourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SealedTableCollectionSourceTemplate{TSource, TItem, TKey, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="propertyName">The name of the <see cref="IHashtableEx{TKey, TValue}"/> property to check in each items of the list.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SealedTableCollectionSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = BaseNodeHelper.NodeTreeHelper.GetPropertyOf(typeof(TItem), propertyName);
            Debug.Assert(ItemProperty != null);
            Debug.Assert(ItemProperty.PropertyType.GetInterface(typeof(IHashtableEx).Name) != null);
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

            IList ValueList = GetSourceObject(node);
            IList<IHashtableEx<TKey, TValue>> ReadyValueList = new List<IHashtableEx<TKey, TValue>>();

            foreach (TItem Value in ValueList)
            {
                IHashtableEx<TKey, TValue> Table = ItemProperty.GetValue(Value) as IHashtableEx<TKey, TValue>;
                Debug.Assert(Table != null);

                if (!Table.IsSealed)
                {
                    Result = false;
                    break;
                }

                ReadyValueList.Add(Table);
            }

            if (Result)
                data = ReadyValueList;

            return Result;
        }
        #endregion
    }
}