namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# function type.
    /// </summary>
    public interface ICSharpTupleType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new ITupleType Source { get; }
    }

    /// <summary>
    /// A C# function type.
    /// </summary>
    public class CSharpTupleType : CSharpType, ICSharpTupleType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpTupleType Create(ICSharpContext context, ITupleType source)
        {
            return new CSharpTupleType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTupleType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpTupleType(ICSharpContext context, ITupleType source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new ITupleType Source { get { return (ITupleType)base.Source; } }

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
