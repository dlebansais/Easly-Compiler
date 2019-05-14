﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public interface IEntityExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IEntityExpression"/>.
    /// </summary>
    public class EntityExpressionRuleTemplate : RuleTemplate<IEntityExpression, EntityExpressionRuleTemplate>, IEntityExpressionRuleTemplate
    {
        #region Init
        static EntityExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<IEntityExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateScopeStart<IEntityExpression>.Default),
                new OnceReferenceTableSourceTemplate<IEntityExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScope.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateScopeStart<IEntityExpression>.Default),
                new OnceReferenceSourceTemplate<IEntityExpression, IList<IExpressionType>>(nameof(IEntityExpression.Query) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IEntityExpression, IList<IExpressionType>>(nameof(IEntityExpression.ResolvedResult)),
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
        public override bool CheckConsistency(IEntityExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= EntityExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ICompiledFeature ResolvedFinalFeature, out IDiscrete ResolvedFinalDiscrete, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ICompiledFeature, IDiscrete, IList<IExpressionType>, IList<IIdentifier>>(ResolvedFinalFeature, ResolvedFinalDiscrete, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IEntityExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IEntityExpression node, IErrorList errorList, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            resolvedFinalFeature = null;
            resolvedFinalDiscrete = null;
            resolvedResult = null;
            resolvedExceptions = null;

            IQualifiedName Query = (IQualifiedName)node.Query;

            IClass EmbeddingClass = (Class)node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Entity.Guid, node, out ITypeName ResultTypeName, out ICompiledType ResultType))
            {
                errorList.AddError(new ErrorEntityTypeMissing(node));
                return false;
            }

            IList<IIdentifier> ValidPath = Query.ValidPath.Item;
            IIdentifier LastIdentifier = ValidPath[ValidPath.Count - 1];
            string ValidText = LastIdentifier.ValidText.Item;

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Guid EntityGuid;

            if (FinalFeature != null)
            {
                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Query.ValidResultTypePath.Item);
                resolvedFinalFeature = FinalFeature;
                EntityGuid = FinalFeature.EntityGuid;
            }
            else
            {
                Debug.Assert(FinalDiscrete != null);

                resolvedFinalDiscrete = FinalDiscrete;
                EntityGuid = LanguageClasses.NamedFeatureEntity.Guid;
            }

            ITypeName EntityTypeName = EmbeddingClass.ImportedLanguageTypeTable[EntityGuid].Item1;
            ICompiledType EntityType = EmbeddingClass.ImportedLanguageTypeTable[EntityGuid].Item2;
            IExpressionType NewEntityResult = new ExpressionType(EntityTypeName, EntityType, ValidText);

            resolvedResult = new List<IExpressionType>()
            {
                NewEntityResult
            };

            resolvedExceptions = new List<IIdentifier>();

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IEntityExpression node, object data)
        {
            ICompiledFeature ResolvedFinalFeature = ((Tuple<ICompiledFeature, IDiscrete, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            IDiscrete ResolvedFinalDiscrete = ((Tuple<ICompiledFeature, IDiscrete, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IList<IExpressionType> ResolvedResult = ((Tuple<ICompiledFeature, IDiscrete, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ICompiledFeature, IDiscrete, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;

            node.ResolvedResult.Item = ResolvedResult;
        }
        #endregion
    }
}