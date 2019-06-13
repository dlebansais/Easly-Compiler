namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// The base type.
        /// </summary>
        ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The list of overloads.
        /// </summary>
        IList<ICSharpCommandOverloadType> OverloadTypeList { get; }
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
            Debug.Assert(source.OverloadList.Count > 0);

            BaseType = Create(context, source.ResolvedBaseType.Item) as ICSharpTypeWithFeature;
            Debug.Assert(BaseType != null);

            foreach (ICommandOverloadType OverloadType in source.OverloadList)
            {
                ICSharpCommandOverloadType NewOverloadType = CSharpCommandOverloadType.Create(context, OverloadType, null);
                OverloadTypeList.Add(NewOverloadType);
            }
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
            ICSharpClass Owner = context.GetClass(source.EmbeddingClass);

            Debug.Assert(source.OverloadList.Count > 0);

            foreach (ICommandOverloadType OverloadType in source.OverloadList)
            {
                ICSharpCommandOverloadType NewOverloadType = CSharpCommandOverloadType.Create(context, OverloadType, Owner);
                OverloadTypeList.Add(NewOverloadType);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IProcedureType Source { get { return (IProcedureType)base.Source; } }

        /// <summary>
        /// The base type.
        /// </summary>
        public ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The list of overloads.
        /// </summary>
        public IList<ICSharpCommandOverloadType> OverloadTypeList { get; } = new List<ICSharpCommandOverloadType>();

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

            string Result;

            // TODO: detect delegate call parameters to select the proper overload

            if (OriginatingTypedef != null)
            {
                string DelegateName = CSharpNames.ToCSharpIdentifier(OriginatingTypedef.Name);

                Result = CommandOverloadType2CSharpString(DelegateName, Source.OverloadList[0]);
            }
            else
            {
                ICSharpCommandOverloadType OverloadType = OverloadTypeList[0];

                string ActionArgumentText = BaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                foreach (ICSharpParameter Parameter in OverloadType.ParameterList)
                {
                    ICSharpType ParameterType = Parameter.Feature.Type;
                    CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;
                    string ParameterText = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);

                    ActionArgumentText += $", {ParameterText}";
                }

                Result = $"Action<{ActionArgumentText}>";

                usingCollection.AddUsing(nameof(System));
            }

            return Result;
        }

        private string CommandOverloadType2CSharpString(string delegateName, ICommandOverloadType overload)
        {
            // TODO
            return delegateName;
        }
        #endregion
    }
}
