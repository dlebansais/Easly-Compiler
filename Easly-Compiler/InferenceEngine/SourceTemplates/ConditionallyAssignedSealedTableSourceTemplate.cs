namespace EaslyCompiler
{
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference"/> where the assigned node has a property that must be a sealed <see cref="IHashtableEx"/> hash table.
    /// </summary>
    public interface IConditionallyAssignedSealedTableSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference{TRef}"/> where the assigned node has a property that must be a sealed <see cref="IHashtableEx{TKey, TValue}"/> hash table.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the optional node.</typeparam>
    /// <typeparam name="TKey">Type of the hash table key.</typeparam>
    /// <typeparam name="TValue">Type of the hash table value.</typeparam>
    public interface IConditionallyAssignedSealedTableSourceTemplate<TSource, TRef, TKey, TValue> : ISourceTemplate<TSource, IOptionalReference>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference{TRef}"/> where the assigned node has a property that must be a sealed <see cref="IHashtableEx{TKey, TValue}"/> hash table.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the optional node.</typeparam>
    /// <typeparam name="TKey">Type of the hash table key.</typeparam>
    /// <typeparam name="TValue">Type of the hash table value.</typeparam>
    public class ConditionallyAssignedSealedTableSourceTemplate<TSource, TRef, TKey, TValue> : SourceTemplate<TSource, IOptionalReference>, IConditionallyAssignedSealedTableSourceTemplate<TSource, TRef, TKey, TValue>, IConditionallyAssignedSealedTableSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionallyAssignedSealedTableSourceTemplate{TSource, TRef, TKey, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the <see cref="IOptionalReference{TRef}"/> in the source object.</param>
        /// <param name="propertyName">The name of the <see cref="IHashtableEx{TKey, TValue}"/> property to check in the optional node.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public ConditionallyAssignedSealedTableSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = BaseNodeHelper.NodeTreeHelper.GetPropertyOf(typeof(TRef), propertyName);
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
            bool Result = false;

            IOptionalReference OptionalValue = GetSourceObject(node);
            if (OptionalValue.IsAssigned)
            {
                TRef Value = (TRef)OptionalValue.Item;
                IHashtableEx<TKey, TValue> ValueTable = ItemProperty.GetValue(Value) as IHashtableEx<TKey, TValue>;
                Debug.Assert(ValueTable != null);
                if (ValueTable.IsSealed)
                {
                    data = ValueTable;
                    Result = true;
                }
            }

            return Result;
        }
        #endregion
    }
}
