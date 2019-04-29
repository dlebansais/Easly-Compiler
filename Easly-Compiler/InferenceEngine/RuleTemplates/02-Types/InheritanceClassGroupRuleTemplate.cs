namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IInheritanceClassGroupRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class InheritanceClassGroupRuleTemplate : RuleTemplate<IInheritance, InheritanceClassGroupRuleTemplate>, IInheritanceClassGroupRuleTemplate
    {
        #region Init
        static InheritanceClassGroupRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IInheritance, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IInheritance>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInheritance, IClass>(nameof(IInheritance.ClassGroup2)),
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
        public override bool CheckConsistency(IInheritance node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IIdentifier ClassIdentifier = null;
            IClass BaseClass = null;

            if (node.ParentType is ISimpleType AsSimpleType)
                ClassIdentifier = (IIdentifier)AsSimpleType.ClassIdentifier;
            else if (node.ParentType is IGenericType AsGenericType)
                ClassIdentifier = (IIdentifier)AsGenericType.ClassIdentifier;

            if (ClassIdentifier != null)
            {
                Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
                string ValidIdentifier = ClassIdentifier.ValidText.Item;

                IHashtableEx<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;

                if (ValidIdentifier.ToLower() == LanguageClasses.Any.Name.ToLower())
                {
                    BaseClass = Class.ClassAny;
                }
                else if (ValidIdentifier.ToLower() == LanguageClasses.AnyReference.Name.ToLower())
                {
                    BaseClass = Class.ClassAnyReference;
                }
                else if (ValidIdentifier.ToLower() == LanguageClasses.AnyValue.Name.ToLower())
                {
                    BaseClass = Class.ClassAnyValue;
                }
                else if (ImportedClassTable.ContainsKey(ValidIdentifier))
                {
                    IImportedClass Imported = ImportedClassTable[ValidIdentifier];
                    BaseClass = Imported.Item;
                }
            }

            data = BaseClass;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInheritance node, object data)
        {
            IClass BaseClass = data as IClass;

            if (BaseClass != null)
                node.ClassGroup2.Item = BaseClass;
        }
        #endregion
    }
}
