namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# procedure type.
    /// </summary>
    public interface ICSharpProcedureType : ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IProcedureType Source { get; }
    }

    /// <summary>
    /// A C# procedure type.
    /// </summary>
    public class CSharpProcedureType : CSharpType, ICSharpProcedureType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpProcedureType Create(ICSharpContext context, IProcedureType source)
        {
            return new CSharpProcedureType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpProcedureType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpProcedureType(ICSharpContext context, IProcedureType source)
            : base(context, source)
        {
        }

        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        public static ICSharpProcedureType Create(ICSharpContext context, IProcedureType source, ICSharpTypedef originatingTypedef)
        {
            return new CSharpProcedureType(context, source, originatingTypedef);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpProcedureType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        protected CSharpProcedureType(ICSharpContext context, IProcedureType source, ICSharpTypedef originatingTypedef)
            : base(context, source, originatingTypedef)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IProcedureType Source { get { return (IProcedureType)base.Source; } }

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

            return CommandOverloadType2CSharpString(DelegateName, Source.OverloadList[0]);
        }

        private string CommandOverloadType2CSharpString(string delegateName, ICommandOverloadType overload)
        {
            // TODO
            return delegateName;
        }
        #endregion
    }
}
