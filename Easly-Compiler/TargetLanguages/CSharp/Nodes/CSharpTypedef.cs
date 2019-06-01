namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# typedef.
    /// </summary>
    public interface ICSharpTypedef : ICSharpSource<ITypedef>
    {
        /// <summary>
        /// The typedef name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The associated type.
        /// </summary>
        ICSharpType Type { get; }
    }

    /// <summary>
    /// A C# typedef.
    /// </summary>
    public class CSharpTypedef : CSharpSource<ITypedef>, ICSharpTypedef
    {
        #region Init
        /// <summary>
        /// Create a new C# typedef.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="embeddingClass">The class where the C# node is created.</param>
        public static ICSharpTypedef Create(ICSharpContext context, ITypedef source, ICSharpClass embeddingClass)
        {
            return new CSharpTypedef(context, source, embeddingClass);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTypedef"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="embeddingClass">The class where the C# node is created.</param>
        protected CSharpTypedef(ICSharpContext context, ITypedef source, ICSharpClass embeddingClass)
            : base(source)
        {
            Name = ((IName)source.EntityName).ValidText.Item;
            Type = CSharpType.Create(context, source.ResolvedDefinedType.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The typedef name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The associated type.
        /// </summary>
        public ICSharpType Type { get; }
        #endregion
    }
}
