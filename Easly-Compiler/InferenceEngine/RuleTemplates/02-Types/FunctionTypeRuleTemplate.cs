namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public interface IFunctionTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IFunctionType"/>.
    /// </summary>
    public class FunctionTypeRuleTemplate : RuleTemplate<IFunctionType, FunctionTypeRuleTemplate>, IFunctionTypeRuleTemplate
    {
        #region Init
        static FunctionTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.BaseType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.BaseType) + Dot + nameof(IObjectType.ResolvedType)),
                new SealedListCollectionSourceTemplate<IFunctionType, IQueryOverloadType, IParameter>(nameof(IFunctionType.OverloadList), nameof(IQueryOverloadType.ParameterTable)),
                new SealedListCollectionSourceTemplate<IFunctionType, IQueryOverloadType, IParameter>(nameof(IFunctionType.OverloadList), nameof(IQueryOverloadType.ResultTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IFunctionType, ITypeName>(nameof(IFunctionType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IFunctionType, ICompiledType>(nameof(IFunctionType.ResolvedType)),
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
        public override bool CheckConsistency(IFunctionType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            // This is ensured because the root node is valid.
            Debug.Assert(node.OverloadList.Count > 0);

            IErrorList CheckErrorList = new ErrorList();
            if (!Feature.DisjoinedParameterCheck(node.OverloadList, node, CheckErrorList))
            {
                Debug.Assert(!CheckErrorList.IsEmpty);
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
        public override void Apply(IFunctionType node, object data)
        {
            IResultType CommonResults = (IResultType)data;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType BaseType = (IObjectType)node.BaseType;
            ITypeName BaseTypeName = BaseType.ResolvedTypeName.Item;
            ICompiledTypeWithFeature ResolvedBaseType = BaseType.ResolvedType.Item as ICompiledTypeWithFeature;
            Debug.Assert(BaseType != null);

            FunctionType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, ResolvedBaseType, node.OverloadList, out ITypeName ResolvedTypeName, out IFunctionType ResolvedType);

            node.ResolvedTypeName.Item = ResolvedTypeName;
            node.ResolvedType.Item = ResolvedType;
        }
        #endregion
    }
}
