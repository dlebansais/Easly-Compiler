namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using CompilerNode;

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    public interface IDestinationTemplate
    {
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="node">The node for which the value is to be checked.</param>
        bool IsSet(object node);
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the destination.</typeparam>
    public interface IDestinationTemplate<TNode, TValue>
        where TNode : INode
    {
        /// <summary>
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="node">The node for which the value is to be checked.</param>
        bool IsSet(TNode node);

        /// <summary>
        /// Sets the destination new value.
        /// </summary>
        /// <param name="node">The node for which the value is to be set.</param>
        /// <param name="value">The value.</param>
        void SetDestinationObject(TNode node, TValue value);
    }

    /// <summary>
    /// Specifies a destination for a <see cref="IRuleTemplate"/>.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TValue">Type of the destination.</typeparam>
    public abstract class DestinationTemplate<TNode, TValue> : IDestinationTemplate<TNode, TValue>, IDestinationTemplate
        where TNode : INode
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationTemplate{TNode, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the destination object.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public DestinationTemplate(string path, ITemplatePathStart startingPoint = null)
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
        /// True if the destination value has been set;
        /// </summary>
        /// <param name="node">The node for which the value is to be checked.</param>
        public abstract bool IsSet(TNode node);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsSet(object node) { return IsSet((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Sets the destination new value.
        /// </summary>
        /// <param name="node">The node for which the value is to be set.</param>
        /// <param name="value">The value.</param>
        public virtual void SetDestinationObject(TNode node, TValue value)
        {
            object Reference = StartingPoint.GetStart(node);

            for (int i = 0; i + 1 < PropertyPath.Count; i++)
                Reference = PropertyPath[i].GetValue(Reference);

            PropertyInfo LastProperty = PropertyPath[PropertyPath.Count - 1];
            LastProperty.SetValue(Reference, value);
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
