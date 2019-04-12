namespace EaslyCompiler
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
        TValue GetSourceObject(TSource source);
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
            BuildPropertyPath(StartingPoint.PropertyType, path, PropertyPath);
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
        public virtual TValue GetSourceObject(TSource source)
        {
            object Result = StartingPoint.GetStart(source);

            for (int i = 0; i < PropertyPath.Count; i++)
                Result = PropertyPath[i].GetValue(Result);

            return (TValue)Result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Recursively build a path of properties from a base node to the final property.
        /// </summary>
        /// <param name="type">The current object type.</param>
        /// <param name="path">The remaining path to parse.</param>
        /// <param name="propertyPath">Accumulated properties in the path so far.</param>
        private void BuildPropertyPath(Type type, string path, List<PropertyInfo> propertyPath)
        {
            if (path.Length == 0)
                return;

            int Index = path.IndexOf(InferenceEngine.Dot);
            int ThisPathIndex = (Index >= 0) ? Index : path.Length;
            string PropertyName = path.Substring(0, ThisPathIndex);
            int NextPathIndex = (Index >= 0) ? Index + 1 : path.Length;
            string NextPath = path.Substring(NextPathIndex);

            PropertyInfo Property = NodeTreeHelper.GetPropertyOf(type, PropertyName);
            Debug.Assert(Property != null);

            propertyPath.Add(Property);

            Type NestedType = Property.PropertyType;
            BuildPropertyPath(NestedType, NextPath, propertyPath);
        }

        /// <summary></summary>
        protected virtual IReadOnlyList<PropertyInfo> PropertyPath { get; private set; }
        #endregion
    }
}
