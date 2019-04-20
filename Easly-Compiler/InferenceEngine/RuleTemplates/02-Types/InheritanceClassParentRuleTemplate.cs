namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public interface IInheritanceClassParentRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInheritance"/>.
    /// </summary>
    public class InheritanceClassParentRuleTemplate : RuleTemplate<IInheritance, InheritanceClassParentRuleTemplate>, IInheritanceClassParentRuleTemplate
    {
        #region Init
        static InheritanceClassParentRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IInheritance, ITypeName>(nameof(IInheritance.ResolvedParentTypeName)),
                new OnceReferenceSourceTemplate<IInheritance, ICompiledType>(nameof(IInheritance.ResolvedParentType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInheritance, ITypeName>(nameof(IInheritance.ResolvedClassParentTypeName)),
                new OnceReferenceDestinationTemplate<IInheritance, IClassType>(nameof(IInheritance.ResolvedClassParentType)),
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
            data = null;

            IClassType ResolvedClassType = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<IClassType, IObjectType> InheritedClassTypeTable = EmbeddingClass.InheritedClassTypeTable;

            if (node.ResolvedParentType.Item is IClassType AsClassType)
            {
                ResolvedClassType = AsClassType;

                foreach (KeyValuePair<IClassType, IObjectType> Entry in InheritedClassTypeTable)
                {
                    IClassType OtherClassType = Entry.Key;
                    if (ClassType.TypesHaveIdenticalSignature(ResolvedClassType, OtherClassType))
                    {
                        AddSourceError(new ErrorTypeAlreadyInherited((IObjectType)node.ParentType));
                        Success = false;
                    }
                }
            }
            else
            {
                AddSourceError(new ErrorClassTypeRequired((IObjectType)node.ParentType));
                Success = false;
            }

            if (Success)
                data = ResolvedClassType;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInheritance node, object data)
        {
            IClassType ResolvedClassType = (IClassType)data;
            IClass EmbeddingClass = node.EmbeddingClass;
            IHashtableEx<IClassType, IObjectType> InheritedClassTypeTable = EmbeddingClass.InheritedClassTypeTable;
            IObjectType ParentType = (IObjectType)node.ParentType;

            node.ResolvedClassParentTypeName.Item = node.ResolvedParentTypeName.Item;
            node.ResolvedClassParentType.Item = ResolvedClassType;
            InheritedClassTypeTable.Add(ResolvedClassType, ParentType);
        }
        #endregion
    }
}
