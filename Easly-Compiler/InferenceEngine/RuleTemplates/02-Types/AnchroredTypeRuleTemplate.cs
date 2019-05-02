namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAnchoredType"/>.
    /// </summary>
    public interface IAnchoredTypeRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAnchoredType"/>.
    /// </summary>
    public class AnchoredTypeRuleTemplate : RuleTemplate<IAnchoredType, AnchoredTypeRuleTemplate>, IAnchoredTypeRuleTemplate
    {
        #region Init
        static AnchoredTypeRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new ResolvedPathSourceTemplate<IAnchoredType>(nameof(IAnchoredType.AnchoredName) + Dot + nameof(IQualifiedName.ValidPath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAnchoredType, ITypeName>(nameof(IAnchoredType.ResolvedTypeName)),
                new OnceReferenceDestinationTemplate<IAnchoredType, ICompiledType>(nameof(IAnchoredType.ResolvedType)),
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
        public override bool CheckConsistency(IAnchoredType node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IQualifiedName AnchoredName = (IQualifiedName)node.AnchoredName;
            Debug.Assert(AnchoredName.ValidPath.IsAssigned);
            IList<IIdentifier> ValidPath = AnchoredName.ValidPath.Item;

            Debug.Assert(dataList.Count == 1);
            Debug.Assert(dataList.ContainsKey(SourceTemplateList[0]));
            Tuple<IErrorList, ITypeName, ICompiledType> SourceData = dataList[SourceTemplateList[0]] as Tuple<IErrorList, ITypeName, ICompiledType>;
            Debug.Assert(SourceData != null);

            if (!SourceData.Item1.IsEmpty)
            {
                AddSourceErrorList(SourceData.Item1);
                Success = false;
            }
            else
                data = new Tuple<ITypeName, ICompiledType>(SourceData.Item2, SourceData.Item3);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAnchoredType node, object data)
        {
            node.ResolvedTypeName.Item = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            node.ResolvedType.Item = ((Tuple<ITypeName, ICompiledType>)data).Item2;
        }
        #endregion
    }
}
