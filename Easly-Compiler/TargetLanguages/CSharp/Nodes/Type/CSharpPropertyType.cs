namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# property type.
    /// </summary>
    public interface ICSharpPropertyType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IPropertyType Source { get; }
    }

    /// <summary>
    /// A C# property type.
    /// </summary>
    public class CSharpPropertyType : CSharpType, ICSharpPropertyType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpPropertyType Create(ICSharpContext context, IPropertyType source)
        {
            return new CSharpPropertyType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPropertyType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpPropertyType(ICSharpContext context, IPropertyType source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IPropertyType Source { get { return (IPropertyType)base.Source; } }

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
