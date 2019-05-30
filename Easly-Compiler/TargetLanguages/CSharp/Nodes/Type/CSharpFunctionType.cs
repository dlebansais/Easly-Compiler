namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# function type.
    /// </summary>
    public interface ICSharpFunctionType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IFunctionType Source { get; }
    }

    /// <summary>
    /// A C# function type.
    /// </summary>
    public class CSharpFunctionType : CSharpType, ICSharpFunctionType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpFunctionType Create(IFunctionType source)
        {
            return new CSharpFunctionType(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpFunctionType(IFunctionType source)
            : base(source)
        {
        }

        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        public static ICSharpFunctionType Create(IFunctionType source, ICSharpTypedef originatingTypedef)
        {
            return new CSharpFunctionType(source, originatingTypedef);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        protected CSharpFunctionType(IFunctionType source, ICSharpTypedef originatingTypedef)
            : base(source, originatingTypedef)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IFunctionType Source { get { return (IFunctionType)base.Source; } }

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

            Debug.Assert(OriginatingTypedef != null);

            // TODO: detect delegate call parameters to select the proper overload
            string DelegateName = CSharpNames.ToCSharpIdentifier(OriginatingTypedef.Name);

            return QueryOverloadType2CSharpString(DelegateName, Source.OverloadList[0]);
        }

        private string QueryOverloadType2CSharpString(string delegateName, IQueryOverloadType overload)
        {
            // TODO
            return delegateName;
        }
        #endregion
    }
}
