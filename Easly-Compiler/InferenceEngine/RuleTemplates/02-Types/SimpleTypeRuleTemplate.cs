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

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;

            Debug.Assert(ClassIdentifier.ValidText.IsAssigned);
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            Debug.Assert(SourceTemplateList.Count > 0);
            IOnceReferenceTypeSourceTemplate TypeSourceTemplate = SourceTemplateList[0] as IOnceReferenceTypeSourceTemplate;
            Debug.Assert(dataList.ContainsKey(TypeSourceTemplate));
            Tuple<ITypeName, ICompiledType, IError> TypeSourceData = dataList[TypeSourceTemplate] as Tuple<ITypeName, ICompiledType, IError>;

            ITypeName ValidTypeName = TypeSourceData.Item1;
            ICompiledType ValidType = TypeSourceData.Item2;
            IError Error = TypeSourceData.Item3;

            if (Error != null)
            {
                Debug.Assert(ValidTypeName == null);
                Debug.Assert(ValidType == null);

                AddSourceError(Error);
                Success = false;
            }
            else
            {
                Debug.Assert(ValidTypeName != null);
                Debug.Assert(ValidType != null);

                data = new Tuple<ITypeName, ICompiledType>(ValidTypeName, ValidType);
            }

            return Success;
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

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            string ValidIdentifier = ClassIdentifier.ValidText.Item;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<string, ICompiledType> LocalGenericTable = EmbeddingClass.LocalGenericTable;

            if (LocalGenericTable.ContainsKey(ValidIdentifier))
            {
                IFormalGenericType FormalGeneric = (IFormalGenericType)LocalGenericTable[ValidIdentifier];
                node.FormalGenericSource.Item = FormalGeneric;
                node.FormalGenericNameSource.Item = FormalGeneric.ResolvedTypeName;
            }
        }
        #endregion
    }
}
