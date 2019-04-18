﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using BaseNodeHelper;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface ISourceTemplate
    {
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="source">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        bool IsReady(ISource source, out object data);
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the source.</typeparam>
    public interface ISourceTemplate<TSource, TValue>
        where TSource : ISource
    {
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="source">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        bool IsReady(TSource source, out object data);

        /// <summary>
        /// Gets the source's current value.
        /// </summary>
        /// <param name="source">The node for which the value is requested.</param>
        /// <param name="isInterrupted">True is progressing through the path was interrupted.</param>
        TValue GetSourceObject(TSource source, out bool isInterrupted);
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the source.</typeparam>
    public abstract class SourceTemplate<TSource, TValue> : ISourceTemplate<TSource, TValue>, ISourceTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTemplate{TSource, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SourceTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
        {
            StartingPoint = startingPoint ?? TemplateNodeStart<TSource>.Default;

            List<PropertyInfo> PropertyPath = new List<PropertyInfo>();
            TemplateHelper.BuildPropertyPath(StartingPoint.PropertyType, path, PropertyPath);
            Debug.Assert(PropertyPath.Count > 0);

            this.PropertyPath = PropertyPath.AsReadOnly();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The starting point for the path.
        /// </summary>
        public ITemplatePathStart<TSource> StartingPoint { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="source">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        public abstract bool IsReady(TSource source, out object data);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsReady(ISource source, out object data) { return IsReady((TSource)source, out data); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets the source's current value.
        /// </summary>
        /// <param name="source">The node for which the value is requested.</param>
        /// <param name="isInterrupted">True is progressing through the path was interrupted.</param>
        public virtual TValue GetSourceObject(TSource source, out bool isInterrupted)
        {
            return TemplateHelper.GetPropertyPathValue<TSource, TValue>(source, StartingPoint, PropertyPath, out isInterrupted);
        }

        /// <summary></summary>
        protected virtual IReadOnlyList<PropertyInfo> PropertyPath { get; private set; }
        #endregion
    }
}
