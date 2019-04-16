﻿namespace EaslyCompiler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be an assigned <see cref="IOnceReference"/>.
    /// </summary>
    public interface IOnceReferenceCollectionSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of <typeparamref name="TItem"/> nodes, each with a property that must be an assigned <see cref="OnceReference{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TValue">Type of <see cref="IOnceReference"/> in each item.</typeparam>
    public interface IOnceReferenceCollectionSourceTemplate<TSource, TItem, TValue> : ISourceTemplate<TSource, IList>
        where TSource : ISource
        where TValue : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of <typeparamref name="TItem"/> nodes, each with a property that must be an assigned <see cref="OnceReference{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TItem">Type of items in the source.</typeparam>
    /// <typeparam name="TValue">Type of <see cref="IOnceReference"/> in each item.</typeparam>
    public class OnceReferenceCollectionSourceTemplate<TSource, TItem, TValue> : SourceTemplate<TSource, IList>, IOnceReferenceCollectionSourceTemplate<TSource, TItem, TValue>, IOnceReferenceCollectionSourceTemplate
        where TSource : ISource
        where TValue : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceCollectionSourceTemplate{TSource, TItem, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="propertyName">The name of the <see cref="OnceReference{TValue}"/> property to check in each items of the list.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceCollectionSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = BaseNodeHelper.NodeTreeHelper.GetPropertyOf(typeof(TItem), propertyName);
            Debug.Assert(ItemProperty != null);
            Debug.Assert(ItemProperty.PropertyType.GetInterface(typeof(IOnceReference).Name) != null);
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
            IList<TValue> ReadyValueList = new List<TValue>();

            foreach (TItem Value in ValueList)
            {
                OnceReference<TValue> Reference = ItemProperty.GetValue(Value) as OnceReference<TValue>;
                Debug.Assert(Reference != null);

                if (!Reference.IsAssigned)
                {
                    Result = false;
                    break;
                }

                ReadyValueList.Add(Reference.Item);
            }

            if (Result)
                data = ReadyValueList;

            return Result;
        }
        #endregion
    }
}
