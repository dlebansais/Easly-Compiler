namespace EaslyCompiler
{
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference"/> where the assigned node has a property that must be an assigned <see cref="IOnceReference"/>.
    /// </summary>
    public interface IConditionallyAssignedReferenceSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference{TRef}"/> where the assigned node has a property that must be an assigned <see cref="OnceReference{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the optional node.</typeparam>
    /// <typeparam name="TValue">Type of the once reference value.</typeparam>
    public interface IConditionallyAssignedReferenceSourceTemplate<TSource, TRef, TValue> : ISourceTemplate<TSource, IOptionalReference>
        where TSource : ISource
        where TRef : class
        where TValue : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOptionalReference{TRef}"/> where the assigned node has a property that must be an assigned <see cref="OnceReference{TValue}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the optional node.</typeparam>
    /// <typeparam name="TValue">Type of the once reference value.</typeparam>
    public class ConditionallyAssignedReferenceSourceTemplate<TSource, TRef, TValue> : SourceTemplate<TSource, IOptionalReference>, IConditionallyAssignedReferenceSourceTemplate<TSource, TRef, TValue>, IConditionallyAssignedReferenceSourceTemplate
        where TSource : ISource
        where TRef : class
        where TValue : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionallyAssignedReferenceSourceTemplate{TSource, TRef, TValue}"/> class.
        /// </summary>
        /// <param name="path">Path to the <see cref="IOptionalReference{TRef}"/> in the source object.</param>
        /// <param name="propertyName">The name of the <see cref="OnceReference{TValue}"/> property to check in the optional node.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public ConditionallyAssignedReferenceSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = typeof(TRef).GetProperty(propertyName);
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
            bool Result = false;

            IOptionalReference OptionalValue = GetSourceObject(node, out bool IsInterrupted);
            if (!IsInterrupted)
            {
                if (OptionalValue.IsAssigned)
                {
                    TRef Value = (TRef)OptionalValue.Item;
                    OnceReference<TValue> Reference = ItemProperty.GetValue(Value) as OnceReference<TValue>;
                    Debug.Assert(Reference != null);
                    if (Reference.IsAssigned)
                    {
                        data = Reference.Item;
                        Result = true;
                    }
                }
                else
                    Result = true;
            }

            return Result;
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
