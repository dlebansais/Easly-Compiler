namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="IOnceReference"/> holding a type.
    /// </summary>
    public interface IOnceReferenceTypeSourceTemplate : ISourceTemplate
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> holding a type.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public interface IOnceReferenceTypeSourceTemplate<TSource, TRef> : ISourceTemplate<TSource, OnceReference<TRef>>
        where TSource : ISource
        where TRef : class
    {
    }

    /// <summary>
    /// Specifies a source for a <see cref="IRuleTemplate"/>.
    /// The source is an assigned <see cref="OnceReference{Tref}"/> holding a type.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRef">Type of the reference.</typeparam>
    public class OnceReferenceTypeSourceTemplate<TSource, TRef> : SourceTemplate<TSource, OnceReference<TRef>>, IOnceReferenceTypeSourceTemplate<TSource, TRef>, IOnceReferenceTypeSourceTemplate
        where TSource : ISource
        where TRef : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OnceReferenceTypeSourceTemplate{TSource, TRef}"/> class.
        /// </summary>
        public OnceReferenceTypeSourceTemplate()
            : base(string.Empty)
        {
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a node source is ready.
        /// </summary>
        /// <param name="node">The node for which the value is checked.</param>
        /// <param name="data">Optional data returned to the caller.</param>
        public override bool IsReady(TSource node, out object data)
        {
            data = null;
            bool Result = false;

            ISimpleType SimpleType = node as ISimpleType;
            Debug.Assert(SimpleType != null);

            Result = IsTypeReady(SimpleType, out data);

            return Result;
        }

        private bool IsTypeReady(ISimpleType node, out object data)
        {
            data = null;
            bool IsReady = true;

            ITypeName ValidTypeName = null;
            ICompiledType ValidType = null;
            IError Error = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IFeature EmbeddingFeature = node.EmbeddingFeature;

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;

            Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            ISealableDictionary<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;
            ISealableDictionary<string, ICompiledType> LocalGenericTable = EmbeddingClass.LocalGenericTable;
            ISealableDictionary<IFeatureName, ISealableDictionary<string, IClass>> LocalExportTable = EmbeddingClass.LocalExportTable;
            ISealableDictionary<IFeatureName, ITypedefType> LocalTypedefTable = EmbeddingClass.LocalTypedefTable;
            ISealableDictionary<IFeatureName, IDiscrete> LocalDiscreteTable = EmbeddingClass.LocalDiscreteTable;
            ISealableDictionary<IFeatureName, IFeatureInstance> LocalFeatureTable = EmbeddingClass.LocalFeatureTable;

            IsReady &= ImportedClassTable.IsSealed;
            IsReady &= LocalGenericTable.IsSealed;
            IsReady &= LocalExportTable.IsSealed;
            IsReady &= LocalTypedefTable.IsSealed;
            IsReady &= LocalDiscreteTable.IsSealed;
            IsReady &= LocalFeatureTable.IsSealed;
            IsReady &= EmbeddingClass.ResolvedClassType.IsAssigned;

            if (IsReady)
            {
                if (ValidIdentifier.ToLower() == LanguageClasses.Any.Name.ToLower())
                    GetBaseClassType(Class.ClassAny, out ValidTypeName, out ValidType);
                else if (ValidIdentifier.ToLower() == LanguageClasses.AnyReference.Name.ToLower())
                    GetBaseClassType(Class.ClassAnyReference, out ValidTypeName, out ValidType);
                else if (ValidIdentifier.ToLower() == LanguageClasses.AnyValue.Name.ToLower())
                    GetBaseClassType(Class.ClassAnyValue, out ValidTypeName, out ValidType);
                else if (ImportedClassTable.ContainsKey(ValidIdentifier))
                {
                    IImportedClass Imported = ImportedClassTable[ValidIdentifier];
                    IClass BaseClass = Imported.Item;
                    IsReady = CheckValidityAsClass(BaseClass, out ValidTypeName, out ValidType, out bool IsInvalidGeneric);

                    if (IsInvalidGeneric)
                        Error = new ErrorGenericClass(ClassIdentifier, ValidIdentifier);
                }
                else if (LocalGenericTable.ContainsKey(ValidIdentifier))
                {
                    IFormalGenericType FormalGeneric = (IFormalGenericType)LocalGenericTable[ValidIdentifier];
                    GetGenericType(FormalGeneric, out ValidTypeName, out ValidType);
                }
                else if (FeatureName.TableContain(LocalTypedefTable, ValidIdentifier, out IFeatureName Key, out ITypedefType DefinedType))
                    IsReady = CheckValidityAsTypedef(DefinedType, out ValidTypeName, out ValidType);
                else
                    Error = new ErrorUnknownIdentifier(ClassIdentifier, ValidIdentifier);
            }

            if (IsReady)
                data = new Tuple<ITypeName, ICompiledType, IError>(ValidTypeName, ValidType, Error);

            return IsReady;
        }

        private void GetBaseClassType(IClass baseClass, out ITypeName validTypeName, out ICompiledType validType)
        {
            Debug.Assert(baseClass.ResolvedClassTypeName.IsAssigned && baseClass.ResolvedClassType.IsAssigned);

            validTypeName = baseClass.ResolvedClassTypeName.Item;
            validType = baseClass.ResolvedAsCompiledType.Item;

            Debug.Assert(validType is IClassType);
        }

        private bool CheckValidityAsClass(IClass baseClass, out ITypeName validTypeName, out ICompiledType validType, out bool isInvalidGeneric)
        {
            validTypeName = null;
            validType = null;
            isInvalidGeneric = false;

            bool IsReady = false;

            if (baseClass.GenericList.Count > 0)
            {
                isInvalidGeneric = true;
                IsReady = true;
            }
            else if (baseClass.ResolvedClassTypeName.IsAssigned && baseClass.ResolvedClassType.IsAssigned && baseClass.GenericTable.IsSealed)
            {
                validTypeName = baseClass.ResolvedClassTypeName.Item;
                validType = baseClass.ResolvedAsCompiledType.Item;
                IsReady = true;

                Debug.Assert(validType is IClassType);
            }

            return IsReady;
        }

        private void GetGenericType(IFormalGenericType genericSource, out ITypeName validTypeName, out ICompiledType validType)
        {
            validTypeName = genericSource.ResolvedTypeName;
            validType = genericSource;

            Debug.Assert(!(validType is IClassType));
        }

        private bool CheckValidityAsTypedef(ITypedefType definedType, out ITypeName validTypeName, out ICompiledType validType)
        {
            validTypeName = null;
            validType = null;
            bool IsReady = false;

            if (definedType.ReferencedTypeName.IsAssigned && definedType.ReferencedType.IsAssigned)
            {
                validTypeName = definedType.ReferencedTypeName.Item;
                validType = definedType.ReferencedType.Item;
                IsReady = true;
            }

            return IsReady;
        }
        #endregion
    }
}
