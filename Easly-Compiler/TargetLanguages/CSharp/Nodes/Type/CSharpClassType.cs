namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# class type.
    /// </summary>
    public interface ICSharpClassType : ICSharpTypeWithFeature
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IClassType Source { get; }

        /// <summary>
        /// The list of generic arguments.
        /// </summary>
        IList<ICSharpType> TypeArgumentList { get; }

        /// <summary>
        /// The class on which this type is based.
        /// </summary>
        ICSharpClass Class { get; }
    }

    /// <summary>
    /// A C# class type.
    /// </summary>
    public class CSharpClassType : CSharpType, ICSharpClassType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpClassType Create(ICSharpContext context, IClassType source)
        {
            return new CSharpClassType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpClassType(ICSharpContext context, IClassType source)
            : base(context, source)
        {
            Class = context.GetClass(source.BaseClass);

            foreach (ICSharpGeneric Generic in Class.GenericList)
            {
                string GenericName = Generic.Name;
                Debug.Assert(Source.TypeArgumentTable.ContainsKey(GenericName));

                ICompiledType Type = Source.TypeArgumentTable[GenericName];
                ICSharpType TypeArgument = Create(context, Type);

                TypeArgumentList.Add(TypeArgument);
            }

            ConformingClassTypeList.Add(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IClassType Source { get { return (IClassType)base.Source; } }

        /// <summary>
        /// The list of generic arguments.
        /// </summary>
        public IList<ICSharpType> TypeArgumentList { get; } = new List<ICSharpType>();

        /// <summary>
        /// The class on which this type is based.
        /// </summary>
        public ICSharpClass Class { get; private set; }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        public override bool HasInterfaceText { get { return Class.Source.Cloneable != BaseNode.CloneableStatus.Single && Class.ValidSourceName != "Microsoft .NET"; } }

        /// <summary>
        /// The list of class types this type conforms to.
        /// </summary>
        public IList<ICSharpClassType> ConformingClassTypeList { get; } = new List<ICSharpClassType>();
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

            Debug.Assert(Class != null);
            Debug.Assert(Class.Source != null);

            Guid BaseClassGuid = Class.Source.ClassGuid;
            bool AsInterface = cSharpTypeFormat == CSharpTypeFormats.AsInterface;
            bool TypeArgumentsWithInterface = true;
            bool TypeArgumentsWithImplementation = false;
            string Result = null;

            if (BaseClassGuid == LanguageClasses.BitFieldEnumeration.Guid)
                Result = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);
            else if (BaseClassGuid == LanguageClasses.DetachableReference.Guid)
                Result = "DetachableReference" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.Hashtable.Guid)
                Result = "Hashtable" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.KeyValuePair.Guid)
                Result = "KeyValuePair" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.List.Guid)
            {
                string ClassName;

                if (AsInterface)
                    ClassName = "IList";
                else
                    ClassName = "List";

                Result = ClassName + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);

                usingCollection.AddUsing("System.Collections.Generic");
            }
            else if (BaseClassGuid == LanguageClasses.OnceReference.Guid)
                Result = "OnceReference" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.OptionalReference.Guid)
                Result = "OptionalReference" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.OverLoopSource.Guid)
                Result = "Enumerable" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.SealableHashtable.Guid)
                Result = "Hashtable" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.SpecializedTypeEntity.Guid)
                Result = "SpecializedTypeEntity" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, false, true);
            else if (BaseClassGuid == LanguageClasses.StableReference.Guid)
                Result = "StableReference" + TypeArguments2CSharpName(usingCollection, TypeArgumentList, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (LanguageClasses.GuidToName.ContainsKey(BaseClassGuid))
                Result = CSharpLanguageClasses.GuidToName[BaseClassGuid];
            else
            {
                string ClassName;

                if (Class.Source.IsEnumeration)
                    cSharpTypeFormat = CSharpTypeFormats.Normal;
                ClassName = Class.BasicClassName2CSharpClassName(usingCollection, cSharpTypeFormat, cSharpNamespaceFormat);

                Result = ClassName + TypeArguments2CSharpName(usingCollection, TypeArgumentList, true, true);
            }

            return Result;
        }

        /// <summary>
        /// Gets the singleton text corresponding to this type, if any.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        /// <param name="text">The singleton text upon return, if successful.</param>
        public override bool GetSingletonString(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat, out string text)
        {
            text = null;

            if (Class.Source.Cloneable != BaseNode.CloneableStatus.Single)
                return false;

            string ClassTypeText = Type2CSharpString(usingCollection, cSharpTypeFormat, cSharpNamespaceFormat);

            text = $"{ClassTypeText}.Singleton";
            return true;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return Source.ToString();
        }
        #endregion
    }
}
