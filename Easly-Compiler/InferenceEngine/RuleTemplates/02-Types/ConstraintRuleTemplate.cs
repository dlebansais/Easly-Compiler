namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public interface IConstraintRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IConstraint"/>.
    /// </summary>
    public class ConstraintRuleTemplate : RuleTemplate<IConstraint, ConstraintRuleTemplate>, IConstraintRuleTemplate
    {
        #region Init
        static ConstraintRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IConstraint, ITypeName>(nameof(IConstraint.ResolvedConformingTypeName)),
                new OnceReferenceSourceTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ResolvedConformingType)),
                new SealedTableSourceTemplate<IConstraint, IFeatureName, IFeatureInstance>(nameof(IConstraint.ResolvedConformingType) + Dot + nameof(ICompiledType.FeatureTable)),
                new SealedTableSourceTemplate<IConstraint, IFeatureName, IDiscrete>(nameof(IConstraint.ResolvedConformingType) + Dot + nameof(ICompiledType.DiscreteTable)),
                new SealedTableSourceTemplate<IConstraint, IIdentifier, IIdentifier>(nameof(IConstraint.RenameTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IConstraint, ICompiledType>(nameof(IConstraint.ResolvedTypeWithRename)),
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
        public override bool CheckConsistency(IConstraint node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            ITypeName DestinationTypeName = null;
            ICompiledType DestinationType = null;

            if (node.ResolvedParentType.Item is IClassType AsClassType)
                Success = ResolveClassTypeRenames(AsClassType, node, out DestinationTypeName, out DestinationType);
            else if (node.ResolvedParentType.Item is ITupleType AsTupleType)
                Success = ResolveTupleTypeRenames(AsTupleType, node, out DestinationTypeName, out DestinationType);
            else if (node.RenameTable.Count == 0)
            {
                DestinationTypeName = node.ResolvedParentTypeName.Item;
                DestinationType = node.ResolvedParentType.Item;
            }
            else
            {
                AddSourceError(new ErrorRenameNotAllowed(node));
                Success = false;
            }

            if (Success)
            {
                Debug.Assert(DestinationTypeName != null);
                Debug.Assert(DestinationType != null);

                data = new Tuple<ITypeName, ICompiledType>(DestinationTypeName, DestinationType);
            }

            return Success;
        }

        private bool ResolveClassTypeRenames(IClassType classType, IConstraint node, out ITypeName destinationTypeName, out ICompiledType destinationType)
        {
            bool Success = false;
            destinationType = null;
            destinationTypeName = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, string> SourceIdentifierTable = new HashtableEx<string, string>(); // string (source) -> string (destination)
            IHashtableEx<string, string> DestinationIdentifierTable = new HashtableEx<string, string>(); // string (destination) -> string (source)
            IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> RenamedExportTable = classType.ExportTable.CloneUnsealed();
            IHashtableEx<IFeatureName, ITypedefType> RenamedTypedefTable = classType.TypedefTable.CloneUnsealed();
            IHashtableEx<IFeatureName, IDiscrete> RenamedDiscreteTable = classType.DiscreteTable.CloneUnsealed();
            IHashtableEx<IFeatureName, IFeatureInstance> RenamedFeatureTable = classType.FeatureTable.CloneUnsealed();

            bool AllRenameValid = true;
            foreach (KeyValuePair<IIdentifier, IIdentifier> Entry in node.RenameTable)
                if (!CheckRename(Entry, new IHashtableIndex<IFeatureName>[] { RenamedExportTable, RenamedTypedefTable, RenamedDiscreteTable, RenamedFeatureTable }, SourceIdentifierTable, DestinationIdentifierTable, (IFeatureName item) => item.Name, (string name) => new FeatureName(name)))
                    AllRenameValid = false;

            if (AllRenameValid)
            {
                if (classType.CloneWithRenames(RenamedExportTable, RenamedTypedefTable, RenamedDiscreteTable, RenamedFeatureTable, EmbeddingClass.ResolvedClassType.Item, ErrorList, out IClassType ClonedType))
                {
                    destinationType = ClonedType;
                    destinationTypeName = new TypeName(destinationType.TypeFriendlyName);
                    Success = true;
                }
            }

            return Success;
        }

        private bool ResolveTupleTypeRenames(ITupleType tupleType, IConstraint node, out ITypeName destinationTypeName, out ICompiledType destinationType)
        {
            bool Success = false;
            destinationType = null;
            destinationTypeName = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, string> SourceIdentifierTable = new HashtableEx<string, string>(); // string (source) -> string (destination)
            IHashtableEx<string, string> DestinationIdentifierTable = new HashtableEx<string, string>(); // string (destination) -> string (source)
            IHashtableEx<IFeatureName, IFeatureInstance> RenamedFieldTable = tupleType.FeatureTable.CloneUnsealed();

            bool AllRenameValid = true;
            foreach (KeyValuePair<IIdentifier, IIdentifier> Entry in node.RenameTable)
                if (!CheckRename(Entry, new IHashtableEx<IFeatureName, IFeatureInstance>[] { RenamedFieldTable }, SourceIdentifierTable, DestinationIdentifierTable, (IFeatureName item) => item.Name, (string name) => new FeatureName(name)))
                    AllRenameValid = false;

            if (AllRenameValid)
            {
                destinationType = tupleType.CloneWithRenames(RenamedFieldTable);
                destinationTypeName = new TypeName(destinationType.TypeFriendlyName);
                Success = true;
            }

            return Success;
        }

        private bool CheckRename(KeyValuePair<IIdentifier, IIdentifier> entry, IHashtableIndex<IFeatureName>[] renamedItemTables, IHashtableEx<string, string> sourceIdentifierTable, IHashtableEx<string, string> destinationIdentifierTable, Func<IFeatureName, string> key2String, Func<string, IFeatureName> string2Key)
        {
            IIdentifier SourceIdentifier = entry.Key;
            IIdentifier DestinationIdentifier = entry.Value;

            OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>> SourceTable = new OnceReference<IHashtableEx<IFeatureName, IFeatureInstance>>();
            OnceReference<IFeatureName> SourceKey = new OnceReference<IFeatureName>();
            OnceReference<IFeatureInstance> SourceItem = new OnceReference<IFeatureInstance>();
            foreach (IHashtableEx<IFeatureName, IFeatureInstance> Table in renamedItemTables)
            {
                foreach (KeyValuePair<IFeatureName, IFeatureInstance> SourceEntry in Table)
                {
                    string ValidName = key2String(SourceEntry.Key);
                    if (ValidName == SourceIdentifier.Text)
                    {
                        SourceTable.Item = Table;
                        SourceKey.Item = SourceEntry.Key;
                        SourceItem.Item = SourceEntry.Value;
                        break;
                    }
                }
                if (SourceTable.IsAssigned)
                    break;
            }

            if (!SourceTable.IsAssigned)
            {
                ErrorList.Add(new ErrorUnknownIdentifier(SourceIdentifier, SourceIdentifier.Text));
                return false;
            }

            foreach (IHashtableEx<IFeatureName, IFeatureInstance> Table in renamedItemTables)
                foreach (KeyValuePair<IFeatureName, IFeatureInstance> DestinationEntry in Table)
                {
                    string ValidName = key2String(DestinationEntry.Key);
                    if (ValidName == DestinationIdentifier.Text)
                    {
                        ErrorList.Add(new ErrorIdentifierAlreadyListed(DestinationIdentifier, DestinationIdentifier.Text));
                        return false;
                    }
                }

            sourceIdentifierTable.Add(SourceIdentifier.Text, DestinationIdentifier.Text);
            destinationIdentifierTable.Add(DestinationIdentifier.Text, SourceIdentifier.Text);

            SourceTable.Item.Remove(SourceKey.Item);
            SourceTable.Item.Add(string2Key(DestinationIdentifier.Text), SourceItem.Item);

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IConstraint node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            ITypeName DestinationTypeName = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            ICompiledType DestinationType = ((Tuple<ITypeName, ICompiledType>)data).Item2;

            EmbeddingClass.TypeTable.Remove(node.ResolvedConformingTypeName.Item);
            EmbeddingClass.TypeTable.Add(DestinationTypeName, DestinationType);

            node.ResolvedTypeWithRename.Item = DestinationType;
        }
        #endregion
    }
}
