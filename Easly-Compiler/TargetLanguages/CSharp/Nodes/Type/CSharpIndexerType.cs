namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# indexer type.
    /// </summary>
    public interface ICSharpIndexerType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IIndexerType Source { get; }
    }

    /// <summary>
    /// A C# indexer type.
    /// </summary>
    public class CSharpIndexerType : CSharpType, ICSharpIndexerType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpIndexerType Create(IIndexerType source)
        {
            return new CSharpIndexerType(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexerType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpIndexerType(IIndexerType source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IIndexerType Source { get { return (IIndexerType)base.Source; } }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        public override bool HasInterfaceText { get { return false; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public override string Type2CSharpString(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            SetUsedInCode();

            return "<Not Supported>";
        }
        #endregion
    }
}
