namespace EaslyCompiler
{
    using System.Collections.Generic;
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

        /// <summary>
        /// The base type.
        /// </summary>
        ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The list of overloads.
        /// </summary>
        IList<ICSharpQueryOverloadType> OverloadTypeList { get; }
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
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpFunctionType Create(ICSharpContext context, IFunctionType source)
        {
            return new CSharpFunctionType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpFunctionType(ICSharpContext context, IFunctionType source)
            : base(context, source)
        {
            Debug.Assert(source.OverloadList.Count > 0);

            ICSharpClass Owner = source.EmbeddingClass != null ? context.GetClass(source.EmbeddingClass) : null;

            BaseType = Create(context, source.ResolvedBaseType.Item) as ICSharpTypeWithFeature;
            Debug.Assert(BaseType != null);

            foreach (IQueryOverloadType OverloadType in source.OverloadList)
            {
                ICSharpQueryOverloadType NewOverloadType = CSharpQueryOverloadType.Create(context, OverloadType, Owner);
                OverloadTypeList.Add(NewOverloadType);
            }
        }

        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        public static ICSharpFunctionType Create(ICSharpContext context, IFunctionType source, ICSharpTypedef originatingTypedef)
        {
            return new CSharpFunctionType(context, source, originatingTypedef);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        protected CSharpFunctionType(ICSharpContext context, IFunctionType source, ICSharpTypedef originatingTypedef)
            : base(context, source, originatingTypedef)
        {
            Debug.Assert(source.OverloadList.Count > 0);

            ICSharpClass Owner = context.GetClass(source.EmbeddingClass);

            BaseType = Create(context, source.ResolvedBaseType.Item) as ICSharpTypeWithFeature;
            Debug.Assert(BaseType != null);

            foreach (IQueryOverloadType OverloadType in source.OverloadList)
            {
                ICSharpQueryOverloadType NewOverloadType = CSharpQueryOverloadType.Create(context, OverloadType, Owner);
                OverloadTypeList.Add(NewOverloadType);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IFunctionType Source { get { return (IFunctionType)base.Source; } }

        /// <summary>
        /// The base type.
        /// </summary>
        public ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The list of overloads.
        /// </summary>
        public IList<ICSharpQueryOverloadType> OverloadTypeList { get; } = new List<ICSharpQueryOverloadType>();

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

            if (OriginatingTypedef != null)
            {
                // TODO: detect delegate call parameters to select the proper overload
                string DelegateName = CSharpNames.ToCSharpIdentifier(OriginatingTypedef.Name);

                Result = QueryOverloadType2CSharpString(DelegateName, Source.OverloadList[0]);
            }
            else
            {
                ICSharpQueryOverloadType OverloadType = OverloadTypeList[0];

                string ActionArgumentText = BaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                foreach (ICSharpParameter Parameter in OverloadType.ParameterList)
                {
                    ICSharpType ParameterType = Parameter.Feature.Type;
                    CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;
                    string ParameterText = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);

                    ActionArgumentText += $", {ParameterText}";
                }

                Debug.Assert(OverloadType.ResultList.Count >= 1);

                if (OverloadType.ResultList.Count == 1)
                {
                    ICSharpParameter Parameter = OverloadType.ResultList[0];
                    ICSharpType ResultType = Parameter.Feature.Type;
                    CSharpTypeFormats ResultFormat = ResultType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;
                    string ResultText = ResultType.Type2CSharpString(usingCollection, ResultFormat, CSharpNamespaceFormats.None);

                    ActionArgumentText += $", {ResultText}";
                }
                else
                {
                    string FuncResultText = string.Empty;

                    foreach (ICSharpParameter Parameter in OverloadType.ResultList)
                    {
                        if (FuncResultText.Length > 0)
                            FuncResultText += ", ";

                        ICSharpType ResultType = Parameter.Feature.Type;
                        CSharpTypeFormats ResultFormat = ResultType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;
                        string ResultText = ResultType.Type2CSharpString(usingCollection, ResultFormat, CSharpNamespaceFormats.None);

                        FuncResultText += $", {ResultText}";
                    }

                    ActionArgumentText += $", Tuple<{FuncResultText}>";
                }

                Result = $"Func<{ActionArgumentText}>";

                usingCollection.AddUsing(nameof(System));
            }

            return Result;
        }

        private string QueryOverloadType2CSharpString(string delegateName, IQueryOverloadType overload)
        {
            // TODO
            return delegateName;
        }
        #endregion
    }
}
