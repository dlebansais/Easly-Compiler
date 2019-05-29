namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# class type.
    /// </summary>
    public interface ICSharpClassType : ICSharpType
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

        /// <summary>
        /// Sets the <see cref="Class"/> property
        /// </summary>
        /// <param name="cSharpClass">The class on which this type is based.</param>
        void SetClass(ICSharpClass cSharpClass);
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
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpClassType Create(IClassType source)
        {
            return new CSharpClassType(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClassType"/> class.
        /// </summary>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpClassType(IClassType source)
            : base(source)
        {
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
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the <see cref="Class"/> property
        /// </summary>
        /// <param name="cSharpClass">The class on which this type is based.</param>
        public void SetClass(ICSharpClass cSharpClass)
        {
            Debug.Assert(cSharpClass != null);
            Debug.Assert(cSharpClass.Type == this);
            Debug.Assert(Class == null);

            Class = cSharpClass;

            foreach (ICSharpGeneric Generic in Class.GenericList)
            {
                string GenericName = Generic.Name;
                Debug.Assert(Source.TypeArgumentTable.ContainsKey(GenericName));

                ICompiledType Type = Source.TypeArgumentTable[GenericName];
                ICSharpType TypeArgument = Create(Type);

                TypeArgumentList.Add(TypeArgument);
            }
        }

        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public override string Type2CSharpString(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            SetUsedInCode();

            Guid BaseClassGuid = Class.Source.ClassGuid;
            bool AsInterface = cSharpTypeFormat == CSharpTypeFormats.AsInterface;
            bool TypeArgumentsWithInterface = true;
            bool TypeArgumentsWithImplementation = false;
            string Result = null;

            if (BaseClassGuid == LanguageClasses.BitFieldEnumeration.Guid)
                Result = CSharpNames.ToCSharpIdentifier(Class.ValidClassName);
            else if (BaseClassGuid == LanguageClasses.DetachableReference.Guid)
                Result = "DetachableReference" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.Hashtable.Guid)
                Result = "Hashtable" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.KeyValuePair.Guid)
                Result = "KeyValuePair" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.List.Guid)
            {
                string ClassName;

                if (AsInterface)
                    ClassName = "IList";
                else
                    ClassName = "List";

                Result = ClassName + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            }
            else if (BaseClassGuid == LanguageClasses.OnceReference.Guid)
                Result = "OnceReference" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.OptionalReference.Guid)
                Result = "OptionalReference" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.OverLoopSource.Guid)
                Result = "Enumerable" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.SealableHashtable.Guid)
                Result = "Hashtable" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (BaseClassGuid == LanguageClasses.SpecializedTypeEntity.Guid)
                Result = "SpecializedTypeEntity" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, false, true);
            else if (BaseClassGuid == LanguageClasses.StableReference.Guid)
                Result = "StableReference" + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, TypeArgumentsWithInterface, TypeArgumentsWithImplementation);
            else if (LanguageClasses.GuidToName.ContainsKey(BaseClassGuid))
                Result = LanguageClasses.GuidToName[BaseClassGuid];
            else
            {
                string ClassName;

                if (Class.Source.IsEnumeration)
                    cSharpTypeFormat = CSharpTypeFormats.Normal;
                ClassName = Class.BasicClassName2CSharpClassName(cSharpNamespace, cSharpTypeFormat, cSharpNamespaceFormat);

                Result = ClassName + TypeArguments2CSharpName(TypeArgumentList, cSharpNamespace, true, true);
            }

            return Result;
        }
        #endregion
    }
}
