namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllTypedefsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllTypedefsRuleTemplate : RuleTemplate<IClass, AllTypedefsRuleTemplate>, IAllTypedefsRuleTemplate
    {
        #region Init
        static AllTypedefsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IClass, IClassType>(nameof(IClass.ResolvedClassType)),
                new SealedTableSourceTemplate<IClass, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IHashtableEx<IFeatureName, ITypedefType>>(nameof(IClass.InheritanceList), nameof(IInheritance.TypedefTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, ITypedefType>(nameof(IClass.TypedefTable)),
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

            Debug.Assert(node.LocalTypedefTable.IsSealed);
            IHashtableEx<IFeatureName, ITypedefType> MergedTypedefTable = node.LocalTypedefTable.CloneUnsealed();

            foreach (IInheritance InheritanceItem in node.InheritanceList)
            {
                IClassType InheritanceParent = InheritanceItem.ResolvedType.Item;
                IHashtableEx<IFeatureName, ITypedefType> InheritedTypedefTable = InheritanceParent.TypedefTable;

                foreach (KeyValuePair<IFeatureName, ITypedefType> InstanceEntry in InheritedTypedefTable)
                {
                    IFeatureName InstanceName = InstanceEntry.Key;
                    ITypedefType InstanceItem = InstanceEntry.Value;
                    bool ConflictingEntry = false;

                    foreach (KeyValuePair<IFeatureName, ITypedefType> Entry in MergedTypedefTable)
                    {
                        IFeatureName LocalName = Entry.Key;
                        ITypedefType LocalItem = Entry.Value;

                        if (InstanceName.Name == LocalName.Name)
                        {
                            if (InstanceItem != LocalItem)
                            {
                                AddSourceError(new ErrorDuplicateName(InheritanceItem, LocalName.Name));
                                ConflictingEntry = true;
                            }
                        }

                        else if (InstanceItem == LocalItem)
                        {
                            AddSourceError(new ErrorTypedefNameConflict(InheritanceItem, LocalName.Name, InstanceName.Name));
                            ConflictingEntry = true;
                        }
                    }

                    if (!ConflictingEntry)
                        MergedTypedefTable.Add(InstanceName, InstanceItem);
                }
            }

            if (Success)
                data = MergedTypedefTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            IHashtableEx<IFeatureName, ITypedefType> MergedTypedefTable = (IHashtableEx<IFeatureName, ITypedefType>)data;

            Debug.Assert(node.ResolvedClassType.IsAssigned);
            IClassType ThisClassType = node.ResolvedClassType.Item;

            ThisClassType.TypedefTable.Merge(MergedTypedefTable);
            ThisClassType.TypedefTable.Seal();

            node.TypedefTable.Merge(MergedTypedefTable);
            node.TypedefTable.Seal();

            foreach (IClassType Item in node.GenericInstanceList)
            {
                Item.TypedefTable.Merge(MergedTypedefTable);
                Item.TypedefTable.Seal();
            }
        }
        #endregion
    }
}
