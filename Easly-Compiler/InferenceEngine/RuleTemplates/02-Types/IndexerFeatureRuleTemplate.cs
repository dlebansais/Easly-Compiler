﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IIndexerFeature"/>.
    /// </summary>
    public interface IIndexerFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IIndexerFeature"/>.
    /// </summary>
    public class IndexerFeatureRuleTemplate : RuleTemplate<IIndexerFeature, IndexerFeatureRuleTemplate>, IIndexerFeatureRuleTemplate
    {
        #region Init
        static IndexerFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IIndexerFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IIndexerFeature>.Default),
                new OnceReferenceSourceTemplate<IIndexerFeature, ITypeName>(nameof(IIndexerFeature.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IIndexerFeature, ICompiledType>(nameof(IIndexerFeature.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
                new OnceReferenceCollectionSourceTemplate<IIndexerFeature, IEntityDeclaration, IScopeAttributeFeature>(nameof(IIndexerFeature.IndexParameterList), nameof(IEntityDeclaration.ValidEntity)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IIndexerFeature, ITypeName>(nameof(IIndexerFeature.ResolvedAgentTypeName)),
                new OnceReferenceDestinationTemplate<IIndexerFeature, ICompiledType>(nameof(IIndexerFeature.ResolvedAgentType)),
                new OnceReferenceDestinationTemplate<IIndexerFeature, ITypeName>(nameof(IIndexerFeature.ResolvedEffectiveTypeName)),
                new OnceReferenceDestinationTemplate<IIndexerFeature, ICompiledType>(nameof(IIndexerFeature.ResolvedEffectiveType)),
                new OnceReferenceDestinationTemplate<IIndexerFeature, ICompiledFeature>(nameof(IIndexerFeature.ResolvedFeature)),
                new UnsealedTableDestinationTemplate<IIndexerFeature, string, IScopeAttributeFeature>(nameof(IIndexerFeature.LocalScope)),
                new UnsealedTableDestinationTemplate<IIndexerFeature, string, IScopeAttributeFeature>(nameof(IIndexerFeature.LocalGetScope)),
                new UnsealedTableDestinationTemplate<IIndexerFeature, string, IScopeAttributeFeature>(nameof(IIndexerFeature.LocalSetScope)),
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
        public override bool CheckConsistency(IIndexerFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType TypeToResolve = (IObjectType)node.EntityType;

            IScopeAttributeFeature ResultFeature = ScopeAttributeFeature.CreateResultFeature(TypeToResolve, EmbeddingClass, node);
            IScopeAttributeFeature ValueFeature = ScopeAttributeFeature.CreateValueFeature(TypeToResolve, EmbeddingClass, node);

            Success = CheckResultValueConsistency(node, dataList, ResultFeature, ValueFeature, out data);

            return Success;
        }

        private bool CheckResultValueConsistency(IIndexerFeature node, IDictionary<ISourceTemplate, object> dataList, IScopeAttributeFeature resultFeature, IScopeAttributeFeature valueFeature, out object data)
        {
            data = null;
            bool Success = true;

            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = new SealableDictionary<string, IScopeAttributeFeature>();
            ISealableDictionary<string, IScopeAttributeFeature> CheckedGetScope = new SealableDictionary<string, IScopeAttributeFeature>();
            ISealableDictionary<string, IScopeAttributeFeature> CheckedSetScope = new SealableDictionary<string, IScopeAttributeFeature>();

            string NameResult = resultFeature.ValidFeatureName.Item.Name;
            string NameValue = valueFeature.ValidFeatureName.Item.Name;

            Success &= CheckParameters(node, resultFeature, valueFeature, CheckedScope);

            CheckedGetScope.Merge(CheckedScope);
            CheckedGetScope.Add(NameResult, resultFeature);

            CheckedSetScope.Merge(CheckedScope);
            CheckedSetScope.Add(NameValue, valueFeature);

            List<string> ConflictList = new List<string>();
            ScopeHolder.RecursiveCheck(CheckedGetScope, node.InnerScopes, ConflictList);
            ScopeHolder.RecursiveCheck(CheckedSetScope, node.InnerScopes, ConflictList);

            Debug.Assert(node.InnerScopes.Count == 0);

            // This has been checked in another rule.
            Debug.Assert(!ConflictList.Contains(NameResult));
            Debug.Assert(!ConflictList.Contains(NameValue));

            Success &= CheckIndexerKind(node, out BaseNode.UtilityType IndexerKind);

            if (Success)
                data = new Tuple<BaseNode.UtilityType, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>>(IndexerKind, CheckedScope, CheckedGetScope, CheckedSetScope);

            return Success;
        }

        private bool CheckParameters(IIndexerFeature node, IScopeAttributeFeature resultFeature, IScopeAttributeFeature valueFeature, ISealableDictionary<string, IScopeAttributeFeature> checkedScope)
        {
            bool Success = true;
            string NameResult = resultFeature.ValidFeatureName.Item.Name;
            string NameValue = valueFeature.ValidFeatureName.Item.Name;

            foreach (IEntityDeclaration Item in node.IndexParameterList)
            {
                IName SourceName = (IName)Item.EntityName;
                string ValidName = SourceName.ValidText.Item;

                if (checkedScope.ContainsKey(ValidName))
                {
                    AddSourceError(new ErrorDuplicateName(SourceName, ValidName));
                    Success = false;
                }
                else if (ValidName == NameResult)
                {
                    AddSourceError(new ErrorNameResultNotAllowed(SourceName));
                    Success = false;
                }
                else if (ValidName == NameValue)
                {
                    AddSourceError(new ErrorNameValueNotAllowed(SourceName));
                    Success = false;
                }
                else
                    checkedScope.Add(ValidName, Item.ValidEntity.Item);
            }

            return Success;
        }

        private bool CheckIndexerKind(IIndexerFeature node, out BaseNode.UtilityType indexerKind)
        {
            bool Success = true;
            indexerKind = (BaseNode.UtilityType)(-1);

            if (node.GetterBody.IsAssigned && node.SetterBody.IsAssigned)
            {
                indexerKind = BaseNode.UtilityType.ReadWrite;

                ICompiledBody AsCompiledGetter = (ICompiledBody)node.GetterBody.Item;
                ICompiledBody AsCompiledSetter = (ICompiledBody)node.SetterBody.Item;

                if (AsCompiledGetter.IsDeferredBody != AsCompiledSetter.IsDeferredBody)
                {
                    AddSourceError(new ErrorIndexerBodyTypeMismatch(node));
                    Success = false;
                }
            }
            else if (node.GetterBody.IsAssigned)
                indexerKind = BaseNode.UtilityType.ReadOnly;
            else if (node.SetterBody.IsAssigned)
                indexerKind = BaseNode.UtilityType.WriteOnly;
            else
            {
                AddSourceError(new ErrorIndexerMissingBody(node));
                Success = false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IIndexerFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType TypeToResolve = (IObjectType)node.EntityType;

            BaseNode.UtilityType IndexerKind = ((Tuple<BaseNode.UtilityType, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>>)data).Item1;
            ISealableDictionary<string, IScopeAttributeFeature> CheckedScope = ((Tuple<BaseNode.UtilityType, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>>)data).Item2;
            ISealableDictionary<string, IScopeAttributeFeature> CheckedGetScope = ((Tuple<BaseNode.UtilityType, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>>)data).Item3;
            ISealableDictionary<string, IScopeAttributeFeature> CheckedSetScope = ((Tuple<BaseNode.UtilityType, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>, ISealableDictionary<string, IScopeAttributeFeature>>)data).Item4;

            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            ITypeName EntityTypeName = TypeToResolve.ResolvedTypeName.Item;
            ICompiledType EntityType = TypeToResolve.ResolvedType.Item;

            List<IEntityDeclaration> IndexParameterList = new List<IEntityDeclaration>();
            foreach (IEntityDeclaration Item in node.IndexParameterList)
                IndexParameterList.Add(Item);

            BaseNode.ParameterEndStatus ParameterEnd = node.ParameterEnd;

            IList<IAssertion> GetRequireList = new List<IAssertion>();
            IList<IAssertion> GetEnsureList = new List<IAssertion>();
            IList<IIdentifier> GetExceptionIdentifierList = new List<IIdentifier>();
            if (node.GetterBody.IsAssigned)
            {
                IBody GetterBody = (IBody)node.GetterBody.Item;
                foreach (IAssertion Item in GetterBody.RequireList)
                    GetRequireList.Add(Item);
                foreach (IAssertion Item in GetterBody.EnsureList)
                    GetEnsureList.Add(Item);
                foreach (IIdentifier Item in GetterBody.ExceptionIdentifierList)
                    GetExceptionIdentifierList.Add(Item);
            }

            IList<IAssertion> SetRequireList = new List<IAssertion>();
            IList<IAssertion> SetEnsureList = new List<IAssertion>();
            IList<IIdentifier> SetExceptionIdentifierList = new List<IIdentifier>();
            if (node.SetterBody.IsAssigned)
            {
                IBody SetterBody = (IBody)node.SetterBody.Item;
                foreach (IAssertion Item in SetterBody.RequireList)
                    SetRequireList.Add(Item);
                foreach (IAssertion Item in SetterBody.EnsureList)
                    SetEnsureList.Add(Item);
                foreach (IIdentifier Item in SetterBody.ExceptionIdentifierList)
                    SetExceptionIdentifierList.Add(Item);
            }

            IndexerType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, EntityTypeName, EntityType, IndexerKind, IndexParameterList, ParameterEnd, GetRequireList, GetEnsureList, GetExceptionIdentifierList, SetRequireList, SetEnsureList, SetExceptionIdentifierList, out ITypeName ResolvedIndexerTypeName, out ICompiledType ResolvedIndexerType);

            node.ResolvedEntityTypeName.Item = EntityTypeName;
            node.ResolvedEntityType.Item = EntityType;
            node.ResolvedAgentTypeName.Item = ResolvedIndexerTypeName;
            node.ResolvedAgentType.Item = ResolvedIndexerType;
            node.ResolvedEffectiveTypeName.Item = EntityTypeName;
            node.ResolvedEffectiveType.Item = EntityType;

            foreach (KeyValuePair<string, IScopeAttributeFeature> Entry in CheckedScope)
                node.ParameterTable.Add(new Parameter(Entry.Key, Entry.Value));
            node.ParameterTable.Seal();

            if (node.GetterBody.IsAssigned)
                EmbeddingClass.BodyList.Add((IBody)node.GetterBody.Item);

            if (node.SetterBody.IsAssigned)
                EmbeddingClass.BodyList.Add((IBody)node.SetterBody.Item);

            node.LocalGetScope.Merge(CheckedGetScope);
            node.LocalGetScope.Seal();
            node.FullGetScope.Merge(node.LocalGetScope);

            node.LocalSetScope.Merge(CheckedSetScope);
            node.LocalSetScope.Seal();
            node.FullSetScope.Merge(node.LocalSetScope);

            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullGetScope, node.InnerGetScopes);
            ScopeHolder.RecursiveAdd(node.FullSetScope, node.InnerSetScopes);

            node.ResolvedFeature.Item = node;

#if COVERAGE
            string TypeString = ResolvedIndexerType.ToString();
#endif
        }
        #endregion
    }
}
