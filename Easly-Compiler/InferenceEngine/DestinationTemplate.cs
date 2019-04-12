namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using BaseNodeHelper;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IDestinationTemplate
    {
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="source">The node for which the value is to be checked.</param>
        bool IsSet(ISource source);

        /// <summary>
        /// Gets the destination current value.
        /// </summary>
        /// <param name="source">The node for which the value is requested.</param>
        object GetDestinationObject(ISource source);
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the destination.</typeparam>
    public interface IDestinationTemplate<TSource, TValue>
        where TSource : ISource
    {
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="source">The node for which the value is to be checked.</param>
        bool IsSet(TSource source);

        /// <summary>
        /// Gets the destination current value.
        /// </summary>
        /// <param name="source">The node for which the value is requested.</param>
        TValue GetDestinationObject(TSource source);
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the destination.</typeparam>
    public abstract class DestinationTemplate<TSource, TValue> : IDestinationTemplate<TSource, TValue>, IDestinationTemplate
        where TSource : ISource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationTemplate{TSource, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public DestinationTemplate(string path, ITemplatePathStart<TSource> startingPoint = null)
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
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="source">The node for which the value is to be checked.</param>
        public abstract bool IsSet(TSource source);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsSet(ISource source) { return IsSet((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets the destination current value.
        /// </summary>
        /// <param name="source">The node for which the value is requested.</param>
        public virtual TValue GetDestinationObject(TSource source)
        {
            object Result = StartingPoint.GetStart(source);

            for (int i = 0; i < PropertyPath.Count; i++)
                Result = PropertyPath[i].GetValue(Result);

            return (TValue)Result;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public object GetDestinationObject(ISource source) { return GetDestinationObject((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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
