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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpIndexerType Create(ICSharpContext context, IIndexerType source)
        {
            return new CSharpIndexerType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexerType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpIndexerType(ICSharpContext context,  IIndexerType source)
            : base(context, source)
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
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public override string Type2CSharpString(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            SetUsedInCode();

            return "<Not Supported>";
        }
        #endregion
    }
}
