namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public interface ITypedefRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public class TypedefRuleTemplate : RuleTemplate<ITypedef, TypedefRuleTemplate>, ITypedefRuleTemplate
    {
        #region Init
        static TypedefRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<ITypedef, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<ITypedef>.Default),
                new SealedTableSourceTemplate<ITypedef, string, ICompiledType>(nameof(IClass.LocalGenericTable), TemplateClassStart<ITypedef>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ITypedef, IFeatureName>(nameof(ITypedef.ValidTypedefName)),
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
        public override bool CheckConsistency(ITypedef node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;
            IFeatureName Key;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            if (EmbeddingClass.ImportedClassTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (EmbeddingClass.LocalGenericTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalExportTable, ValidText, out Key, out IHashtableEx<string, IClass> Export))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (FeatureName.TableContain(EmbeddingClass.LocalTypedefTable, ValidText, out Key, out ITypedefType TypedefType))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else
            {
                // This is checked in the corresponding step of a discrete rule.
                bool IsDiscreteName = FeatureName.TableContain(EmbeddingClass.LocalDiscreteTable, ValidText, out Key, out IDiscrete Discrete);
                Debug.Assert(!IsDiscreteName);

                // This is checked in the corresponding step of a feature rule.
                bool IsFeatureName = FeatureName.TableContain(EmbeddingClass.LocalFeatureTable, ValidText, out Key, out IFeatureInstance Feature);
                Debug.Assert(!IsFeatureName);
            }

            if (node.DefinedType is IAnchoredType AsAnchoredType)
            {
                AddSourceError(new ErrorInvalidAnchoredType(AsAnchoredType));
                Success = false;
            }
            else if (node.DefinedType is IKeywordAnchoredType AsKeywordAnchoredType)
            {
                bool IsAllowed;
                switch (AsKeywordAnchoredType.Anchor)
                {
                    case BaseNode.Keyword.True:
                    case BaseNode.Keyword.False:
                    case BaseNode.Keyword.Retry:
                    case BaseNode.Keyword.Exception:
                        IsAllowed = true;
                        break;

                    default:
                        IsAllowed = false;
                        break;
                }

                if (!IsAllowed)
                {
                    AddSourceError(new ErrorInvalidAnchoredType(AsKeywordAnchoredType));
                    Success = false;
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ITypedef node, object data)
        {
            IName EntityName = (IName)node.EntityName;
            IClass EmbeddingClass = node.EmbeddingClass;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IFeatureName ResolvedTypedefTypeName = new FeatureName(ValidText);
            ITypedefType ResolvedTypedefType = new TypedefType();

            EmbeddingClass.LocalTypedefTable.Add(ResolvedTypedefTypeName, ResolvedTypedefType);
            node.ValidTypedefName.Item = ResolvedTypedefTypeName;

#if DEBUG
            // TODO: remove this code, for code coverage purpose only.
            string TypeString = ResolvedTypedefType.ToString();
#endif
        }
        #endregion
    }
}
