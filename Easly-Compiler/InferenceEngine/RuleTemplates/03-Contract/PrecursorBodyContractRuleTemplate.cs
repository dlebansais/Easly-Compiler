namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    public interface IPrecursorBodyContractRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IPrecursorBodyContractRuleTemplate<TBody> : IRuleTemplate<TBody, PrecursorBodyContractRuleTemplate<TBody>>
        where TBody : IPrecursorBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class PrecursorBodyContractRuleTemplate<TBody> : RuleTemplate<TBody, PrecursorBodyContractRuleTemplate<TBody>>, IPrecursorBodyContractRuleTemplate<TBody>, IPrecursorBodyContractRuleTemplate
        where TBody : IPrecursorBody
    {
        #region Init
        static PrecursorBodyContractRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IPrecursorBody.ResolvedRequireList)),
                new OnceReferenceSourceTemplate<TBody, IList<IAssertion>>(nameof(IPrecursorBody.ResolvedEnsureList)),
                new OnceReferenceSourceTemplate<TBody, IList<IIdentifier>>(nameof(IPrecursorBody.ResolvedExceptionIdentifierList)),
                new ConditionallyAssignedReferenceSourceTemplate<TBody, IObjectType, ITypeName>(nameof(IPrecursorBody.AncestorType), nameof(IObjectType.ResolvedTypeName)),
                new ConditionallyAssignedReferenceSourceTemplate<TBody, IObjectType, ICompiledType>(nameof(IPrecursorBody.AncestorType), nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TBody, ITypeName>(nameof(IPrecursorBody.ResolvedAncestorTypeName)),
                new OnceReferenceDestinationTemplate<TBody, ICompiledType>(nameof(IPrecursorBody.ResolvedAncestorType)),
                new OnceReferenceDestinationTemplate<TBody, IList<IExpressionType>>(nameof(IPrecursorBody.ResolvedResult)),
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
        public override bool CheckConsistency(TBody node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IFeature InnerFeature = node.EmbeddingFeature;
            IFeatureName InnerFeatureName = InnerFeature.ValidFeatureName.Item;
            IFeatureInstance InnerFeatureInstance = EmbeddingClass.FeatureTable[InnerFeatureName];
            IList<IPrecursorInstance> PrecursorList = InnerFeatureInstance.PrecursorList;

            ITypeName AncestorTypeName = null;
            ICompiledType AncestorType = null;

            if (node.AncestorType.IsAssigned)
            {
                IObjectType DeclaredAncestor = (IObjectType)node.AncestorType.Item;

                bool Found = false;

                if (DeclaredAncestor.ResolvedType.Item is IClassType AsClassTypeAncestor)
                {
                    foreach (IPrecursorInstance Item in PrecursorList)
                        if (Item.Ancestor.BaseClass == AsClassTypeAncestor.BaseClass)
                        {
                            Found = true;
                            AncestorTypeName = Item.Ancestor.BaseClass.ResolvedClassTypeName.Item;
                            AncestorType = Item.Ancestor.BaseClass.ResolvedClassType.Item;
                            break;
                        }
                }

                if (!Found)
                {
                    AddSourceError(new ErrorInvalidPrecursor(node));
                    Success = false;
                }
            }
            else if (PrecursorList.Count > 1)
            {
                AddSourceError(new ErrorInvalidPrecursor(node));
                Success = false;
            }
            else
            {
                AncestorTypeName = PrecursorList[0].Ancestor.BaseClass.ResolvedClassTypeName.Item;
                AncestorType = PrecursorList[0].Ancestor.BaseClass.ResolvedClassType.Item;
            }

            if (Success)
                data = new Tuple<ITypeName, ICompiledType>(AncestorTypeName, AncestorType);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(TBody node, object data)
        {
            ITypeName AncestorTypeName = ((Tuple<ITypeName, ICompiledType>)data).Item1;
            ICompiledType AncestorType = ((Tuple<ITypeName, ICompiledType>)data).Item2;

            node.ResolvedAncestorTypeName.Item = AncestorTypeName;
            node.ResolvedAncestorType.Item = AncestorType;
            node.ResolvedResult.Item = new List<IExpressionType>();
        }
        #endregion
    }
}
