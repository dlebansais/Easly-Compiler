namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllDiscretesRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllDiscretesRuleTemplate : RuleTemplate<IClass, AllDiscretesRuleTemplate>, IAllDiscretesRuleTemplate
    {
        #region Init
        static AllDiscretesRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IClass, IClassType>(nameof(IClass.ResolvedClassType)),
                new SealedTableSourceTemplate<IClass, IFeatureName, IDiscrete>(nameof(IClass.LocalDiscreteTable)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IClassType>(nameof(IClass.InheritanceList), nameof(IInheritance.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IClass, IInheritance, IHashtableEx<IFeatureName, IDiscrete>>(nameof(IClass.InheritanceList), nameof(IInheritance.DiscreteTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, IDiscrete>(nameof(IClass.DiscreteTable)),
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, IExpression>(nameof(IClass.DiscreteWithValueTable)),
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

            Debug.Assert(node.LocalDiscreteTable.IsSealed);
            IHashtableEx<IFeatureName, IDiscrete> MergedDiscreteTable = node.LocalDiscreteTable.CloneUnsealed();

            foreach (IInheritance Inheritance in node.InheritanceList)
            {
                Debug.Assert(Inheritance.ResolvedType.IsAssigned);
                Debug.Assert(Inheritance.DiscreteTable.IsAssigned);

                IClassType InheritanceParent = Inheritance.ResolvedType.Item;
                IHashtableEx<IFeatureName, IDiscrete> InheritedDiscreteTable = InheritanceParent.DiscreteTable;

                // TODO: verify InheritedDiscreteTable == Inheritance.DiscreteTable since the source is on the later.
                foreach (KeyValuePair<IFeatureName, IDiscrete> InstanceEntry in InheritedDiscreteTable)
                {
                    IFeatureName InstanceName = InstanceEntry.Key;
                    IDiscrete InstanceItem = InstanceEntry.Value;
                    bool ConflictingEntry = false;

                    foreach (KeyValuePair<IFeatureName, IDiscrete> Entry in MergedDiscreteTable)
                    {
                        IFeatureName LocalName = Entry.Key;
                        IDiscrete LocalItem = Entry.Value;

                        if (InstanceName.Name == LocalName.Name)
                        {
                            if (InstanceItem != LocalItem)
                            {
                                AddSourceError(new ErrorDuplicateName(Inheritance, LocalName.Name));
                                ConflictingEntry = true;
                                Success = false;
                            }
                        }
                        else if (InstanceItem == LocalItem)
                        {
                            AddSourceError(new ErrorDiscreteNameConflict(Inheritance, LocalName.Name, InstanceName.Name));
                            ConflictingEntry = true;
                            Success = false;
                        }
                    }

                    if (!ConflictingEntry && !MergedDiscreteTable.ContainsKey(InstanceName))
                        MergedDiscreteTable.Add(InstanceName, InstanceItem);
                }
            }

            if (Success)
                data = MergedDiscreteTable;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            IHashtableEx<IFeatureName, IDiscrete> MergedDiscreteTable = (IHashtableEx<IFeatureName, IDiscrete>)data;

            Debug.Assert(node.ResolvedClassType.IsAssigned);
            IClassType ThisClassType = node.ResolvedClassType.Item;

            ThisClassType.DiscreteTable.Merge(MergedDiscreteTable);
            ThisClassType.DiscreteTable.Seal();

            node.DiscreteTable.Merge(MergedDiscreteTable);
            node.DiscreteTable.Seal();

            Debug.Assert(node.DiscreteWithValueTable.Count == 0);
            foreach (KeyValuePair<IFeatureName, IDiscrete> Entry in node.DiscreteTable)
                if (Entry.Value.NumericValue.IsAssigned)
                {
                    IExpression NumericValue = (IExpression)Entry.Value.NumericValue.Item;
                    node.DiscreteWithValueTable.Add(Entry.Key, NumericValue);
                }

            node.DiscreteWithValueTable.Seal();

            foreach (IClassType Item in node.GenericInstanceList)
            {
                Item.DiscreteTable.Merge(MergedDiscreteTable);
                Item.DiscreteTable.Seal();
            }
        }
        #endregion
    }
}
