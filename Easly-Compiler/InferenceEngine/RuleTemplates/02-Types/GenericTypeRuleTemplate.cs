namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IGenericType"/>.
    /// </summary>
    public interface IGenericTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IGenericType"/>.
    /// </summary>
    public class GenericTypeRuleTemplate : RuleTemplate<IGenericType, GenericTypeRuleTemplate>, IGenericTypeRuleTemplate
    {
        #region Init
        static GenericTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IGenericType, string, IIdentifier>(nameof(IGenericType.ArgumentIdentifierTable)),
                new SealedTableSourceTemplate<IGenericType, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<IGenericType>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IGenericType, IClass>(nameof(IGenericType.BaseClass)),
                new OnceReferenceDestinationTemplate<IGenericType, IHashtableEx<string, ICompiledType>>(nameof(IGenericType.ResolvedTypeArgumentTable)),
                new OnceReferenceDestinationTemplate<IGenericType, IHashtableEx<string, IObjectType>>(nameof(IGenericType.ResolvedArgumentLocationTable)),
                new OnceReferenceDestinationTemplate<IGenericType, ITypeName>(nameof(IGenericType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IGenericType, ICompiledType>(nameof(IGenericType.ResolvedType)),
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
        public override bool CheckConsistency(IGenericType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = ClassIdentifier.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, IImportedClass> ImportedClassTable = EmbeddingClass.ImportedClassTable;

            IHashtableEx<string, ICompiledType> ResolvedTable = new HashtableEx<string, ICompiledType>();
            IHashtableEx<string, IObjectType> LocationTable = new HashtableEx<string, IObjectType>();

            if (!ImportedClassTable.ContainsKey(ValidIdentifier))
            {
                AddSourceError(new ErrorUnknownIdentifier(ClassIdentifier, ValidIdentifier));
                Success = false;
            }
            else
            {
                TypeArgumentStyles ArgumentStyle = TypeArgumentStyles.None;

                foreach (ITypeArgument Item in node.TypeArgumentList)
                    Success &= IsTypeArgumentValid(Item, ref ArgumentStyle);

                IImportedClass Imported = ImportedClassTable[ValidIdentifier];
                IClass BaseClass = Imported.Item;

                if (BaseClass.GenericTable.Count == 0)
                {
                    AddSourceError(new ErrorGenericClass(node, ValidIdentifier));
                    Success = false;
                }

                if (Success)
                {
                    bool IsHandled = false;

                    switch (ArgumentStyle)
                    {
                        case TypeArgumentStyles.None:
                        case TypeArgumentStyles.Positional:
                            Success = CheckPositionalTypeArgumentsValidity(node, BaseClass, ResolvedTable, LocationTable);
                            IsHandled = true;
                            break;

                        case TypeArgumentStyles.Assignment:
                            Success = CheckAssignmentTypeArgumentsValidity(node, BaseClass, ResolvedTable, LocationTable);
                            IsHandled = true;
                            break;
                    }

                    Debug.Assert(IsHandled);

                    if (Success)
                    {
                        ClassType.ResolveType(EmbeddingClass.TypeTable, BaseClass, ResolvedTable, EmbeddingClass.ResolvedClassType.Item, out ITypeName ValidResolvedTypeName, out ICompiledType ValidResolvedType);
                        data = new Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>(BaseClass, ArgumentStyle, ResolvedTable, LocationTable, ValidResolvedTypeName, ValidResolvedType);
                    }
                }
            }

            return Success;
        }

        private bool IsTypeArgumentValid(ITypeArgument item, ref TypeArgumentStyles argumentStyle)
        {
            bool Success = true;
            bool IsHandled = false;

            if (item is IPositionalTypeArgument AsPositionalTypeArgument)
            {
                if (argumentStyle == TypeArgumentStyles.None)
                    argumentStyle = TypeArgumentStyles.Positional;
                else if (argumentStyle == TypeArgumentStyles.Assignment)
                {
                    AddSourceError(new ErrorTypeArgumentMixed(item));
                    Success = false;
                }

                IsHandled = true;
            }
            else if (item is IAssignmentTypeArgument AsAssignmentTypeArgument)
            {
                if (argumentStyle == TypeArgumentStyles.None)
                    argumentStyle = TypeArgumentStyles.Assignment;
                else if (argumentStyle == TypeArgumentStyles.Positional)
                {
                    AddSourceError(new ErrorTypeArgumentMixed(item));
                    Success = false;
                }

                IsHandled = true;
            }

            Debug.Assert(IsHandled);

            return Success;
        }

        private bool CheckPositionalTypeArgumentsValidity(IGenericType node, IClass baseClass, IHashtableEx<string, ICompiledType> resolvedTable, IHashtableEx<string, IObjectType> locationTable)
        {
            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            int LastDefaultCount = 0;
            foreach (KeyValuePair<string, ICompiledType> Entry in baseClass.GenericTable)
            {
                IFormalGenericType FormalGeneric = (IFormalGenericType)Entry.Value;
                if (FormalGeneric.FormalGeneric.DefaultValue.IsAssigned)
                    LastDefaultCount++;
                else
                    LastDefaultCount = 0;
            }

            int MinimumArgumentCount = baseClass.GenericTable.Count - LastDefaultCount;

            if (node.TypeArgumentList.Count > baseClass.GenericTable.Count)
            {
                AddSourceError(new ErrorTooManyTypeArguments(node, ValidIdentifier, baseClass.GenericTable.Count));
                return false;
            }
            else if (node.TypeArgumentList.Count < MinimumArgumentCount)
            {
                AddSourceError(new ErrorTypeArgumentCount(node, ValidIdentifier, MinimumArgumentCount));
                return false;
            }

            for (int i = 0; i < node.TypeArgumentList.Count; i++)
            {
                IName EntityName = (IName)baseClass.GenericList[i].EntityName;
                string GenericName = EntityName.ValidText.Item;
                IPositionalTypeArgument ActualArgument = (IPositionalTypeArgument)node.TypeArgumentList[i];
                ICompiledType ActualArgumentType = ActualArgument.ResolvedSourceType.Item;

                resolvedTable.Add(GenericName, ActualArgumentType);
                locationTable.Add(GenericName, (IObjectType)ActualArgument.Source);
            }

            return true;
        }

        private bool CheckAssignmentTypeArgumentsValidity(IGenericType node, IClass baseClass, IHashtableEx<string, ICompiledType> resolvedTable, IHashtableEx<string, IObjectType> locationTable)
        {
            bool Result = true;

            foreach (IAssignmentTypeArgument Item in node.TypeArgumentList)
            {
                IIdentifier ParameterIdentifier = (IIdentifier)Item.ParameterIdentifier;
                string GenericName = ParameterIdentifier.ValidText.Item;

                // This is checked in a separate rule.
                Debug.Assert(!resolvedTable.ContainsKey(GenericName));

                if (!baseClass.LocalGenericTable.ContainsKey(GenericName))
                {
                    AddSourceError(new ErrorUnknownIdentifier(ParameterIdentifier, GenericName));
                    Result = false;
                }

                ICompiledType ActualArgumentType = Item.ResolvedSourceType.Item;

                resolvedTable.Add(GenericName, ActualArgumentType);
                locationTable.Add(GenericName, (IObjectType)Item.Source);
            }

            foreach (KeyValuePair<string, ICompiledType> Entry in baseClass.GenericTable)
            {
                string GenericName = Entry.Key;
                if (!resolvedTable.ContainsKey(GenericName))
                {
                    FormalGenericType Generic = (FormalGenericType)Entry.Value;
                    if (!Generic.FormalGeneric.DefaultValue.IsAssigned)
                    {
                        AddSourceError(new ErrorMissingTypeArgument(node, GenericName));
                        Result = false;
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IGenericType node, object data)
        {
            node.BaseClass.Item = ((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item1;
            node.SetArgumentStyle(((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item2);
            node.ResolvedTypeArgumentTable.Item = ((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item3;
            node.ResolvedArgumentLocationTable.Item = ((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item4;
            node.ResolvedTypeName.Item = ((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item5;
            node.ResolvedType.Item = ((Tuple<IClass, TypeArgumentStyles, IHashtableEx<string, ICompiledType>, IHashtableEx<string, IObjectType>, ITypeName, ICompiledType>)data).Item6;
        }
        #endregion
    }
}
