namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IProcedureType"/>.
    /// </summary>
    public interface IProcedureTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IProcedureType"/>.
    /// </summary>
    public class ProcedureTypeRuleTemplate : RuleTemplate<IProcedureType, ProcedureTypeRuleTemplate>, IProcedureTypeRuleTemplate
    {
        #region Init
        static ProcedureTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IProcedureType, ITypeName>(nameof(IProcedureType.BaseType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IProcedureType, ICompiledType>(nameof(IProcedureType.BaseType) + Dot + nameof(IObjectType.ResolvedType)),
                new SealedListCollectionSourceTemplate<IProcedureType, ICommandOverloadType, IParameter>(nameof(IProcedureType.OverloadList), nameof(ICommandOverloadType.ParameterTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IProcedureType, ITypeName>(nameof(IProcedureType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IProcedureType, ICompiledType>(nameof(IProcedureType.ResolvedType)),
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
        public override bool CheckConsistency(IProcedureType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            IList<IExpressionType> CommonResults = new List<IExpressionType>();

            IList<IError> CheckErrorList = new List<IError>();
            if (!Feature.DisjoinedParameterCheck(node.OverloadList, node, CheckErrorList))
            {
                Debug.Assert(CheckErrorList.Count > 0);
                AddSourceErrorList(CheckErrorList);
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IProcedureType node, object data)
        {
            IList<IExpressionType> CommonResults = (IList<IExpressionType>)data;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType BaseTypeItem = (IObjectType)node.BaseType;
            ITypeName BaseTypeName = BaseTypeItem.ResolvedTypeName.Item;
            ICompiledType BaseType = BaseTypeItem.ResolvedType.Item;

            ProcedureType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, node.OverloadList, out ITypeName ResolvedTypeName, out ICompiledType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
