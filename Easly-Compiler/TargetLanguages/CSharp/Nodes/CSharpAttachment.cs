namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# attachment node.
    /// </summary>
    public interface ICSharpAttachment : ICSharpSource<IAttachment>
    {
        /// <summary>
        /// The list of attaching types.
        /// </summary>
        IList<ICSharpType> AttachTypeList { get; }

        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharp(ICSharpWriter writer);
    }

    /// <summary>
    /// A C# attachment node.
    /// </summary>
    public class CSharpAttachment : CSharpSource<IAttachment>, ICSharpAttachment
    {
        #region Init
        /// <summary>
        /// Create a new C# attachment.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpAttachment Create(ICSharpContext context, IAttachment source)
        {
            return new CSharpAttachment(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAttachment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpAttachment(ICSharpContext context, IAttachment source)
            : base(source)
        {
            foreach (IScopeAttributeFeature Entity in source.ResolvedLocalEntitiesList)
            {
                ICSharpType NewType = CSharpType.Create(context, Entity.ResolvedFeatureType2.Item);
                AttachTypeList.Add(NewType);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of attaching types.
        /// </summary>
        public IList<ICSharpType> AttachTypeList { get; } = new List<ICSharpType>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# attachment.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public virtual void WriteCSharp(ICSharpWriter writer)
        {
            //TODO
        }
        #endregion
    }
}
