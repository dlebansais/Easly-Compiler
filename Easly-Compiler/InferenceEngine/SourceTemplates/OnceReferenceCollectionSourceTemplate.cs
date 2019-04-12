namespace EaslyCompiler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    public interface IOnceReferenceCollectionSourceTemplate : ISourceTemplate
    {
        /// <summary>
        /// The list of reference values if ready.
        /// </summary>
        IList<IOnceReference> ReadyReferenceList { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference in each item.</typeparam>
    public interface IOnceReferenceCollectionSourceTemplate<TSource, TRef> : ISourceTemplate<TSource, IList>
        where TSource : ISource
        where TRef : class
    {
        /// <summary>
        /// The list of reference values if ready.
        /// </summary>
        IList<IOnceReference> ReadyReferenceList { get; }
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is a collection of nodes, each with a property that must be an assigned <see cref="OnceReference{Tref}"/>.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference in each item.</typeparam>
    public class OnceReferenceCollectionSourceTemplate<TSource, TRef> : SourceTemplate<TSource, IList>, IOnceReferenceCollectionSourceTemplate<TSource, TRef>, IOnceReferenceCollectionSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceCollectionSourceTemplate{TSource, TRef}"/> class.
        /// </summary>
        /// <param name="path">Path to the source object.</param>
        /// <param name="propertyName">The name of the <see cref="OnceReference{TRef}"/> property to check in each item of the list.</param>
        /// <param name="startingPoint">The starting point for the path.</param>
        public OnceReferenceCollectionSourceTemplate(string path, string propertyName, ITemplatePathStart<TSource> startingPoint = null)
            : base(path, startingPoint)
        {
            ItemProperty = typeof(TRef).GetProperty(propertyName);
            Debug.Assert(ItemProperty != null);
        }

        private PropertyInfo ItemProperty;
        #endregion

        #region Properties
        /// <summary>
        /// The list of reference values if ready.
        /// </summary>
        public IList<IOnceReference> ReadyReferenceList { get; } = new List<IOnceReference>();
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
            ReadyReferenceList.Clear();

            IList ValueList = GetSourceObject(node);
            foreach (object Item in ValueList)
            {
                TRef Value = Item as TRef;
                Debug.Assert(Value != null);

                IOnceReference Reference = ItemProperty.GetValue(Value) as IOnceReference;
                Debug.Assert(Value != null);

                if (!Reference.IsAssigned)
                {
                    Result = false;
                    break;
                }

                ReadyReferenceList.Add(Reference);
            }

            return Result;
        }
        #endregion
    }
}
