namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllGenericsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllGenericsRuleTemplate : RuleTemplate<IClass, AllGenericsRuleTemplate>, IAllGenericsRuleTemplate
    {
        #region Init
        static AllGenericsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IGeneric, ITypeName>(nameof(IClass.GenericList), nameof(IGeneric.ResolvedGenericTypeName)),
                new OnceReferenceCollectionSourceTemplate<IClass, IGeneric, IFormalGenericType>(nameof(IClass.GenericList), nameof(IGeneric.ResolvedGenericType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClass, ITypeName>(nameof(IClass.ResolvedClassTypeName)),
                new OnceReferenceDestinationTemplate<IClass, IClassType>(nameof(IClass.ResolvedClassType)),
                new UnsealedTableDestinationTemplate<IClass, string, ICompiledType>(nameof(IClass.LocalGenericTable)),
                new UnsealedTableDestinationTemplate<IClass, string, ICompiledType>(nameof(IClass.GenericTable)),
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

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            IHashtableEx<string, ICompiledType> LocalGenericTable = node.LocalGenericTable;

            LocalGenericTable.Seal();
            node.LocalNamespaceTable.Add("Generic", LocalGenericTable);

            IClassType NewClassType = new ClassType(node, LocalGenericTable);
            ITypeName NewTypeName = new TypeName(NewClassType.TypeFriendlyName);

            node.ResolvedClassTypeName.Item = NewTypeName;
            node.ResolvedClassType.Item = NewClassType;
            node.ResolvedAsCompiledType.Item = NewClassType;
            node.GenericTable.Merge(LocalGenericTable);
            node.GenericTable.Seal();
        }
        #endregion
    }
}
