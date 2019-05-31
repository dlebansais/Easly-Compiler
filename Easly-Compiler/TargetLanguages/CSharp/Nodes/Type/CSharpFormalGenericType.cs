namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# formal generic type.
    /// </summary>
    public interface ICSharpFormalGenericType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IFormalGenericType Source { get; }

        /// <summary>
        /// The associated generic.
        /// </summary>
        ICSharpGeneric Generic { get; }

        /// <summary>
        /// Sets the <see cref="Generic"/> property.
        /// </summary>
        /// <param name="generic">The associated generic.</param>
        void SetGeneric(ICSharpGeneric generic);
    }

    /// <summary>
    /// A C# formal generic type.
    /// </summary>
    public class CSharpFormalGenericType : CSharpType, ICSharpFormalGenericType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpFormalGenericType Create(ICSharpContext context, IFormalGenericType source)
        {
            return new CSharpFormalGenericType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFormalGenericType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpFormalGenericType(ICSharpContext context, IFormalGenericType source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IFormalGenericType Source { get { return (IFormalGenericType)base.Source; } }

        /// <summary>
        /// The associated generic.
        /// </summary>
        public ICSharpGeneric Generic { get; private set; }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        public override bool HasInterfaceText { get { return false; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the <see cref="Generic"/> property.
        /// </summary>
        /// <param name="generic">The associated generic.</param>
        public void SetGeneric(ICSharpGeneric generic)
        {
            Debug.Assert(generic != null && generic.Type == this);
            Debug.Assert(Generic == null);

            Generic = generic;
        }

        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public override string Type2CSharpString(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            SetUsedInCode();

            return ((cSharpTypeFormat == CSharpTypeFormats.AsInterface) ? "I" : string.Empty) + CSharpNames.ToCSharpIdentifier(Generic.Name);
        }
        #endregion
    }
}
