namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public interface ISimpleTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    /// <typeparam name="T">Type used to have separate static constructor.</typeparam>
    public abstract class SimpleTypeRuleTemplate<T> : RuleTemplate<ISimpleType, SimpleTypeRuleTemplate<T>>, ISimpleTypeRuleTemplate
    {
        #region Init
        static SimpleTypeRuleTemplate()
        {
            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ISimpleType, ITypeName>(nameof(ISimpleType.TypeNameSource)),
                new OnceReferenceDestinationTemplate<ISimpleType, ICompiledType>(nameof(ISimpleType.TypeSource)),
                new OnceReferenceDestinationTemplate<ISimpleType, string>(nameof(ISimpleType.ValidTypeSource)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(ISimpleType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IFeature EmbeddingFeature = node.EmbeddingFeature;

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;

            Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            IHashtableEx<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;
            IHashtableEx<string, ICompiledType> LocalGenericTable = EmbeddingClass.LocalGenericTable;
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> LocalExportTable = EmbeddingClass.LocalExportTable;
            IHashtableEx<IFeatureName, ITypedefType> LocalTypedefTable = EmbeddingClass.LocalTypedefTable;
            IHashtableEx<IFeatureName, IDiscrete> LocalDiscreteTable = EmbeddingClass.LocalDiscreteTable;
            IHashtableEx<IFeatureName, IFeatureInstance> LocalFeatureTable = EmbeddingClass.LocalFeatureTable;

            ITypeName ValidTypeName = null;
            ICompiledType ValidType = null;

            if (ValidIdentifier.ToLower() == LanguageClasses.Any.Name.ToLower())
            {
                IClass BaseClass = Class.ClassAny;
                Success &= CheckValidityAsClass(BaseClass, node, out ValidTypeName, out ValidType);
                Debug.Assert(ValidType is IClassType);
            }
            else if (ValidIdentifier.ToLower() == LanguageClasses.AnyReference.Name.ToLower())
            {
                IClass BaseClass = Class.ClassAnyReference;
                Success &= CheckValidityAsClass(BaseClass, node, out ValidTypeName, out ValidType);
                Debug.Assert(ValidType is IClassType);
            }
            else if (ValidIdentifier.ToLower() == LanguageClasses.AnyValue.Name.ToLower())
            {
                IClass BaseClass = Class.ClassAnyValue;
                Success &= CheckValidityAsClass(BaseClass, node, out ValidTypeName, out ValidType);
                Debug.Assert(ValidType is IClassType);
            }
            else if (ImportedClassTable.ContainsKey(ValidIdentifier))
            {
                IImportedClass Imported = ImportedClassTable[ValidIdentifier];
                IClass BaseClass = Imported.Item;
                Success &= CheckValidityAsClass(BaseClass, node, out ValidTypeName, out ValidType);
                Debug.Assert(!Success || ValidType is IClassType);
            }
            else if (LocalGenericTable.ContainsKey(ValidIdentifier))
            {
                IFormalGenericType FormalGeneric = (IFormalGenericType)LocalGenericTable[ValidIdentifier];
                node.FormalGenericSource.Item = FormalGeneric;
                node.FormalGenericNameSource.Item = FormalGeneric.ResolvedTypeName;
                CheckValidityAsGeneric(node.FormalGenericNameSource.Item, node.FormalGenericSource.Item, out ValidTypeName, out ValidType);
                Debug.Assert(!(ValidType is IClassType));
            }
            else if (FeatureName.TableContain(LocalTypedefTable, ValidIdentifier, out IFeatureName Key, out ITypedefType DefinedType))
            {
                CheckValidityAsTypedef(DefinedType, out ValidTypeName, out ValidType);
                Debug.Assert(!(ValidType is IClassType));
            }
            else
            {
                AddSourceError(new ErrorUnknownIdentifier(ClassIdentifier, ValidIdentifier));
                Success = false;
            }

            if (Success)
                data = new Tuple<ITypeName, ICompiledType>(ValidTypeName, ValidType);

            return Success;
        }

        private bool CheckValidityAsClass(IClass baseClass, ISimpleType node, out ITypeName validTypeName, out ICompiledType validType)
        {
            validTypeName = null;
            validType = null;

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            Debug.Assert(baseClass.ResolvedClassType.IsAssigned && baseClass.GenericTable.IsSealed);

            if (baseClass.GenericTable.Count > 0)
            {
                AddSourceError(new ErrorGenericClass(ClassIdentifier, ValidIdentifier));
                return false;
            }

            validTypeName = baseClass.ResolvedClassTypeName.Item;
            validType = baseClass.ResolvedAsCompiledType.Item;
            return true;
        }

        private void CheckValidityAsGeneric(ITypeName genericNameSource, ICompiledType genericSource, out ITypeName validTypeName, out ICompiledType validType)
        {
            // TODO check: Debug.Assert(c.TypeTable.Contains(FormalGeneric.ResolvedTypeName));

            validTypeName = genericNameSource;
            validType = genericSource;
        }

        private void CheckValidityAsTypedef(ITypedefType definedType, out ITypeName validTypeName, out ICompiledType validType)
        {
            validTypeName = definedType.ReferencedTypeName.Item;
            validType = definedType.ReferencedType.Item;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ISimpleType node, object data)
        {
            ITypeName ValidTypeName = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            ICompiledType ValidType = ((Tuple<ITypeName, ICompiledType>)data).Item2;

            node.TypeNameSource.Item = ValidTypeName;
            node.TypeSource.Item = ValidType;
            node.ValidTypeSource.Item = "Set";
        }
        #endregion
    }
}
