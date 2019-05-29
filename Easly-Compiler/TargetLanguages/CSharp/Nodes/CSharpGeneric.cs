namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# generic node.
    /// </summary>
    public interface ICSharpGeneric : ICSharpSource<IGeneric>
    {
        /// <summary>
        /// The generic name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The corresponding type.
        /// </summary>
        ICSharpFormalGenericType Type { get; }
    }

    /// <summary>
    /// A C# generic node.
    /// </summary>
    public class CSharpGeneric : CSharpSource<IGeneric>, ICSharpGeneric
    {
        #region Init
        /// <summary>
        /// Create a new C# generic.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpGeneric Create(IGeneric source)
        {
            return new CSharpGeneric(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpGeneric"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpGeneric(IGeneric source)
            : base(source)
        {
            Name = ((IName)source.EntityName).ValidText.Item;
            Type = CSharpFormalGenericType.Create(source.ResolvedGenericType.Item);
            Type.SetGeneric(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The generic name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The corresponding type.
        /// </summary>
        public ICSharpFormalGenericType Type { get; }
        #endregion
    }
}
