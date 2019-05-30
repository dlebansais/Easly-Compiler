namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# type.
    /// </summary>
    public interface ICSharpType
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        ICompiledType Source { get; }

        /// <summary>
        /// The typedef where this type is declared. Can be null.
        /// </summary>
        ICSharpTypedef OriginatingTypedef { get; }

        /// <summary>
        /// True if the type is used in output source code.
        /// </summary>
        bool IsUsedInCode { get; }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        bool HasInterfaceText { get; }

        /// <summary>
        /// Sets the <see cref="IsUsedInCode"/> property.
        /// </summary>
        void SetUsedInCode();

        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        string Type2CSharpString(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);
    }

    /// <summary>
    /// A C# type.
    /// </summary>
    public abstract class CSharpType : ICSharpType
    {
        #region Init
        /// <summary>
        /// Creates a new C# type.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpType Create(ICompiledType source)
        {
            ICSharpType Result = null;

            // A typedef is required for IFunctionType and IProcedureType.
            switch (source)
            {
                case IClassType AsClassType:
                    Result = CSharpClassType.Create(AsClassType);
                    break;

                case IFormalGenericType AsFormalGenericType:
                    Result = CSharpFormalGenericType.Create(AsFormalGenericType);
                    break;

                case IFunctionType AsFunctionType:
                    Result = CSharpFunctionType.Create(AsFunctionType);
                    break;

                case IProcedureType AsProcedureType:
                    Result = CSharpProcedureType.Create(AsProcedureType);
                    break;

                case IIndexerType AsIndexerType:
                    Result = CSharpIndexerType.Create(AsIndexerType);
                    break;

                case IPropertyType AsPropertyType:
                    Result = CSharpPropertyType.Create(AsPropertyType);
                    break;

                case ITupleType AsTupleType:
                    Result = CSharpTupleType.Create(AsTupleType);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Creates a new C# type associated to a typedef.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="typedef">The typedef.</param>
        public static ICSharpType Create(ICompiledType source, ICSharpTypedef typedef)
        {
            ICSharpType Result = null;

            switch (source)
            {
                case IClassType AsClassType:
                    Result = CSharpClassType.Create(AsClassType);
                    break;

                case IFormalGenericType AsFormalGenericType:
                    Result = CSharpFormalGenericType.Create(AsFormalGenericType);
                    break;

                case IFunctionType AsFunctionType:
                    Result = CSharpFunctionType.Create(AsFunctionType, typedef);
                    break;

                case IProcedureType AsProcedureType:
                    Result = CSharpProcedureType.Create(AsProcedureType, typedef);
                    break;

                case IIndexerType AsIndexerType:
                    Result = CSharpIndexerType.Create(AsIndexerType);
                    break;

                case IPropertyType AsPropertyType:
                    Result = CSharpPropertyType.Create(AsPropertyType);
                    break;

                case ITupleType AsTupleType:
                    Result = CSharpTupleType.Create(AsTupleType);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpType(ICompiledType source)
        {
            Debug.Assert(source != null);

            Source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        /// <param name="originatingTypedef">The typedef where this type is declared.</param>
        protected CSharpType(ICompiledType source, ICSharpTypedef originatingTypedef)
        {
            Debug.Assert(source != null);
            Debug.Assert(originatingTypedef != null);

            Source = source;
            OriginatingTypedef = originatingTypedef;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public ICompiledType Source { get; }

        /// <summary>
        /// The typedef where this type is declared. Can be null.
        /// </summary>
        public ICSharpTypedef OriginatingTypedef { get; }

        /// <summary>
        /// True if the type is used in output source code.
        /// </summary>
        public bool IsUsedInCode { get; private set; }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        public abstract bool HasInterfaceText { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public abstract string Type2CSharpString(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);

        /// <summary>
        /// Sets the <see cref="IsUsedInCode"/> property.
        /// </summary>
        public void SetUsedInCode()
        {
            IsUsedInCode = true;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the string corresponding to the enumeration of C# generic arguments.
        /// </summary>
        /// <param name="typeArgumentList">The list of arguments.</param>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="isWithInterface">If true, include the interface type.</param>
        /// <param name="isWithImplementation">If true, include the implementation type.</param>
        protected string TypeArguments2CSharpName(IList<ICSharpType> typeArgumentList, string cSharpNamespace, bool isWithInterface, bool isWithImplementation)
        {
            Debug.Assert(isWithInterface || isWithImplementation);

            string GenericNames = string.Empty;

            foreach (ICSharpType TypeArgument in typeArgumentList)
            {
                if (GenericNames.Length > 0)
                    GenericNames += ", ";

                if (isWithInterface)
                    GenericNames += TypeArgument.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                if (isWithImplementation)
                {
                    if (isWithInterface)
                        GenericNames += "," + " ";

                    GenericNames += TypeArgument.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
                }
            }

            if (GenericNames.Length > 0)
                GenericNames = "<" + GenericNames + ">";

            return GenericNames;
        }
        #endregion
    }
}
