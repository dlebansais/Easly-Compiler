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
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="embeddingClass">The class where the C# node is created.</param>
        public static ICSharpTypedef Create(ITypedef source, ICSharpClass embeddingClass)
        {
            return new CSharpTypedef(source, embeddingClass);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTypedef"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="embeddingClass">The class where the C# node is created.</param>
        protected CSharpTypedef(ITypedef source, ICSharpClass embeddingClass)
            : base(source)
        {
            Name = ((IName)source.EntityName).ValidText.Item;
            Type = CSharpType.Create(source.ResolvedDefinedType.Item, this);

            if (Type is ICSharpFormalGenericType AsFormalGenericType)
            {
                ICSharpGeneric Generic = null;
                foreach (ICSharpGeneric Item in embeddingClass.GenericList)
                    if (Item.Source == AsFormalGenericType.Source.FormalGeneric)
                    {
                        Debug.Assert(Generic == null);
                        Generic = Item;
                    }

                AsFormalGenericType.SetGeneric(Generic);
            }
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
