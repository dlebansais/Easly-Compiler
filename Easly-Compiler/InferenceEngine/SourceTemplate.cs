namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using CompilerNode;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface ISourceTemplate
    {
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        bool IsReady(object node);
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the source.</typeparam>
    public interface ISourceTemplate<TNode, TValue>
        where TNode : INode
    {
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        bool IsReady(TNode node);

        /// <summary>
        /// Gets the source's current value.
        /// </summary>
        /// <param name="node">The node for which the value is requested.</param>
        TValue GetSourceObject(TNode node);
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the source.</typeparam>
    public abstract class SourceTemplate<TNode, TValue> : ISourceTemplate<TNode, TValue>, ISourceTemplate
        where TNode : INode
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTemplate{TNode, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public SourceTemplate(string path, ITemplatePathStart startingPoint = null)
        {
            StartingPoint = startingPoint ?? TemplateNodeStart.Default;

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
        public ITemplatePathStart StartingPoint { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        public abstract bool IsReady(TNode node);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsReady(object node) { return IsReady((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets the source's current value.
        /// </summary>
        /// <param name="node">The node for which the value is requested.</param>
        public virtual TValue GetSourceObject(TNode node)
        {
            object Result = StartingPoint.GetStart(node);

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

            int Index = path.IndexOf('.');
            if (Index < 0)
                Index = path.Length;

            string PropertyName = path.Substring(0, Index);
            PropertyInfo Property = type.GetProperty(PropertyName);
            Debug.Assert(Property != null);

            propertyPath.Add(Property);

            string NestedPath = path.Substring(Index + 1);
            Type NestedType = Property.PropertyType;

            BuildPropertyPath(NestedType, NestedPath, propertyPath);
        }

        /// <summary></summary>
        protected virtual IReadOnlyList<PropertyInfo> PropertyPath { get; private set; }
        #endregion
    }
}
