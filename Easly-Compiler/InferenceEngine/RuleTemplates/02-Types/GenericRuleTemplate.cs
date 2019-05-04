namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public interface IGenericRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IGeneric"/>.
    /// </summary>
    public class GenericRuleTemplate : RuleTemplate<IGeneric, GenericRuleTemplate>, IGenericRuleTemplate
    {
        #region Init
        static GenericRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IGeneric, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IGeneric>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IGeneric, ITypeName>(nameof(IGeneric.ResolvedGenericTypeName)),
                new OnceReferenceDestinationTemplate<IGeneric, IFormalGenericType>(nameof(IGeneric.ResolvedGenericType)),
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
        public override bool CheckConsistency(IGeneric node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IHashtableEx<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;
            IHashtableEx<string, ICompiledType> LocalGenericTable = EmbeddingClass.LocalGenericTable;

            if (ValidText.ToLower() == LanguageClasses.Any.Name.ToLower())
            {
                AddSourceError(new ErrorReservedName(EntityName, ValidText));
                Success = false;
            }
            else if (ImportedClassTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }
            else if (LocalGenericTable.ContainsKey(ValidText))
            {
                AddSourceError(new ErrorDuplicateName(EntityName, ValidText));
                Success = false;
            }

            foreach (IConstraint Constraint in node.ConstraintList)
            {
                IObjectType ConstraintType = (IObjectType)Constraint.ParentType;

                if (ConstraintType is IAnchoredType AsAnchoredType)
                {
                    AddSourceError(new ErrorInvalidAnchoredType(AsAnchoredType));
                    Success = false;
                }
                else if (ConstraintType is IKeywordAnchoredType AsKeywordAnchoredType)
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
        public override void Apply(IGeneric node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IName EntityName = (IName)node.EntityName;

            Debug.Assert(EntityName.ValidText.IsAssigned);
            string ValidText = EntityName.ValidText.Item;

            IHashtableEx<string, ICompiledType> LocalGenericTable = EmbeddingClass.LocalGenericTable;

            TypeName ResolvedTypeName = new TypeName(ValidText);
            IFormalGenericType GenericFormalType = new FormalGenericType(node, ResolvedTypeName);

#if DEBUG
            // TODO: remove this code, for code coverage purpose only.
            string TypeString = GenericFormalType.ToString();
#endif

            Debug.Assert(!node.ResolvedGenericTypeName.IsAssigned);
            Debug.Assert(!node.ResolvedGenericType.IsAssigned);

            node.ResolvedGenericTypeName.Item = ResolvedTypeName;
            node.ResolvedGenericType.Item = GenericFormalType;
            LocalGenericTable.Add(ValidText, GenericFormalType);
        }
        #endregion
    }
}
