namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllInheritancesRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllInheritancesRuleTemplate : RuleTemplate<IClass, AllInheritancesRuleTemplate>, IAllInheritancesRuleTemplate
    {
        #region Init
        static AllInheritancesRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedClassParentTypeName)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedClassParentType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IClassType, IObjectType>(nameof(IClass.InheritedClassTypeTable)),
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
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IHashtableEx<IClassType, IObjectType> InheritedClassTypeTable = node.InheritedClassTypeTable;
            StableReference<SingleClassGroup> ClassGroup = new StableReference<SingleClassGroup>();

            foreach (KeyValuePair<IClassType, IObjectType> Entry in InheritedClassTypeTable)
            {
                IClassType InheritedClassType = Entry.Key;
                IClass InheritedClass = InheritedClassType.BaseClass;

                if (InheritedClass.Cloneable == BaseNode.CloneableStatus.Single)
                {
                    if (!ClassGroup.IsAssigned)
                        ClassGroup.Item = InheritedClass.ClassGroup.Item;
                    else
                    {
                        foreach (IClass Class in InheritedClass.ClassGroup.Item.ClassList)
                            ClassGroup.Item.ClassList.Add(Class);

                        InheritedClass.ClassGroup.Item = ClassGroup.Item;
                    }
                }
            }

            if (ClassGroup.IsAssigned)
                if (node.Cloneable == BaseNode.CloneableStatus.Cloneable)
                {
                    AddSourceError(new ErrorCloneableClass(node));
                    Success = false;
                }
                else
                {
                    foreach (IClass Class in node.ClassGroup.Item.ClassList)
                        ClassGroup.Item.ClassList.Add(Class);

                    node.ClassGroup.Item = ClassGroup.Item;
                }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            IHashtableEx<IClassType, IObjectType> InheritedClassTypeTable = node.InheritedClassTypeTable;
            InheritedClassTypeTable.Seal();
        }
        #endregion
    }
}
