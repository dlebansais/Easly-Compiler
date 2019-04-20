﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public interface ITypedefSourceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ITypedef"/>.
    /// </summary>
    public class TypedefSourceRuleTemplate : RuleTemplate<ITypedef, TypedefSourceRuleTemplate>, ITypedefSourceRuleTemplate
    {
        #region Init
        static TypedefSourceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<ITypedef, IFeatureName>(nameof(ITypedef.ValidTypedefName)),
                new OnceReferenceSourceTemplate<ITypedef, ITypeName>(nameof(ITypedef.DefinedType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<ITypedef, ICompiledType>(nameof(ITypedef.DefinedType) + Dot + nameof(IObjectType.ResolvedType)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ITypedef, ITypeName>(nameof(ITypedef.ResolvedDefinedTypeName)),
                new OnceReferenceDestinationTemplate<ITypedef, ICompiledType>(nameof(ITypedef.ResolvedDefinedType)),
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
        public override bool CheckConsistency(ITypedef node, IDictionary<ISourceTemplate, object> dataList, out object data)
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
        public override void Apply(ITypedef node, object data)
        {
            IObjectType DefinedTypeItem = (IObjectType)node.DefinedType;

            ITypeName DefinedTypeName = DefinedTypeItem.ResolvedTypeName.Item;
            ICompiledType DefinedType = DefinedTypeItem.ResolvedType.Item;

            node.ResolvedDefinedTypeName.Item = DefinedTypeName;
            node.ResolvedDefinedType.Item = DefinedType;
            DefinedType.OriginatingTypedef.Item = node;
        }
        #endregion
    }
}