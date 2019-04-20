namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IPropertyFeature"/>.
    /// </summary>
    public interface IPropertyFeatureRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IPropertyFeature"/>.
    /// </summary>
    public class PropertyFeatureRuleTemplate : RuleTemplate<IPropertyFeature, PropertyFeatureRuleTemplate>, IPropertyFeatureRuleTemplate
    {
        #region Init
        static PropertyFeatureRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceSourceTemplate<IPropertyFeature, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<IPropertyFeature>.Default),
                new OnceReferenceSourceTemplate<IPropertyFeature, ITypeName>(nameof(IPropertyFeature.EntityType) + Dot + nameof(IObjectType.ResolvedTypeName)),
                new OnceReferenceSourceTemplate<IPropertyFeature, ICompiledType>(nameof(IPropertyFeature.EntityType) + Dot + nameof(IObjectType.ResolvedType)),
                // TODO getter and setter bodies local scope
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IPropertyFeature, ITypeName>(nameof(IPropertyFeature.ResolvedFeatureTypeName)),
                new OnceReferenceDestinationTemplate<IPropertyFeature, ICompiledType>(nameof(IPropertyFeature.ResolvedFeatureType)),
                new OnceReferenceDestinationTemplate<IPropertyFeature, ICompiledFeature>(nameof(IPropertyFeature.ResolvedFeature)),
                new UnsealedTableDestinationTemplate<IPropertyFeature, string, IScopeAttributeFeature>(nameof(IPropertyFeature.LocalScope)),
                new UnsealedTableDestinationTemplate<IPropertyFeature, string, IScopeAttributeFeature>(nameof(IPropertyFeature.LocalGetScope)),
                new UnsealedTableDestinationTemplate<IPropertyFeature, string, IScopeAttributeFeature>(nameof(IPropertyFeature.LocalSetScope)),
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
        public override bool CheckConsistency(IPropertyFeature node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType TypeToResolve = (IObjectType)node.EntityType;

            Success &= ScopeAttributeFeature.CreateResultFeature(TypeToResolve, EmbeddingClass, node, ErrorList, out IScopeAttributeFeature Result);
            Success &= ScopeAttributeFeature.CreateValueFeature(TypeToResolve, EmbeddingClass, node, ErrorList, out IScopeAttributeFeature Value);

            if (Success)
            {
                string NameResult = Result.ValidFeatureName.Item.Name;
                string NameValue = Value.ValidFeatureName.Item.Name;

                IHashtableEx<string, IScopeAttributeFeature> CheckedGetScope = new HashtableEx<string, IScopeAttributeFeature>();
                CheckedGetScope.Add(NameResult, Result);
                IHashtableEx<string, IScopeAttributeFeature> CheckedSetScope = new HashtableEx<string, IScopeAttributeFeature>();
                CheckedSetScope.Add(NameValue, Value);

                IList<string> ConflictList = new List<string>();
                ScopeHolder.RecursiveCheck(CheckedGetScope, node.InnerScopes, ConflictList);
                ScopeHolder.RecursiveCheck(CheckedSetScope, node.InnerScopes, ConflictList);

                if (ConflictList.Contains(NameResult))
                {
                    AddSourceError(new ErrorNameResultNotAllowed(node));
                    Success = false;
                }

                if (ConflictList.Contains(NameValue))
                {
                    AddSourceError(new ErrorNameValueNotAllowed(node));
                    Success = false;
                }

                IName PropertyName = (IName)((IFeatureWithName)node).EntityName;

                if (node.GetterBody.IsAssigned && node.SetterBody.IsAssigned)
                {
                    ICompiledBody AsCompiledGetter = (ICompiledBody)node.GetterBody.Item;
                    ICompiledBody AsCompiledSetter = (ICompiledBody)node.SetterBody.Item;

                    if (AsCompiledGetter.IsDeferredBody != AsCompiledSetter.IsDeferredBody)
                    {
                        AddSourceError(new ErrorBodyTypeMismatch(node, PropertyName.ValidText.Item));
                        Success = false;
                    }

                    if (node.PropertyKind != BaseNode.UtilityType.ReadWrite)
                    {
                        AddSourceError(new ErrorBodyTypeMismatch(node, PropertyName.ValidText.Item));
                        Success = false;
                    }
                }
                else if (node.GetterBody.IsAssigned)
                {
                    if (node.PropertyKind == BaseNode.UtilityType.WriteOnly)
                    {
                        AddSourceError(new ErrorBodyTypeMismatch(node, PropertyName.ValidText.Item));
                        Success = false;
                    }
                }
                else if (node.SetterBody.IsAssigned)
                {
                    if (node.PropertyKind == BaseNode.UtilityType.ReadOnly)
                    {
                        AddSourceError(new ErrorBodyTypeMismatch(node, PropertyName.ValidText.Item));
                        Success = false;
                    }
                }
            }

            if (Success)
                data = new Tuple<IScopeAttributeFeature, IScopeAttributeFeature>(Result, Value);

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IPropertyFeature node, object data)
        {
            IClass EmbeddingClass = node.EmbeddingClass;
            IObjectType TypeToResolve = (IObjectType)node.EntityType;

            ITypeName BaseTypeName = EmbeddingClass.ResolvedClassTypeName.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;
            ITypeName EntityTypeName = TypeToResolve.ResolvedTypeName.Item;
            ICompiledType EntityType = TypeToResolve.ResolvedType.Item;

            IList<IAssertion> GetEnsureList = new List<IAssertion>();
            IList<IIdentifier> GetExceptionIdentifierList = new List<IIdentifier>();
            if (node.GetterBody.IsAssigned)
            {
                IBody GetterBody = (IBody)node.GetterBody.Item;
                foreach (IAssertion Item in GetterBody.EnsureList)
                    GetEnsureList.Add(Item);
                foreach (IIdentifier Item in GetterBody.ExceptionIdentifierList)
                    GetExceptionIdentifierList.Add(Item);
            }

            IList<IAssertion> SetRequireList = new List<IAssertion>();
            IList<IIdentifier> SetExceptionIdentifierList = new List<IIdentifier>();
            if (node.SetterBody.IsAssigned)
            {
                IBody SetterBody = (IBody)node.SetterBody.Item;
                foreach (IAssertion Item in SetterBody.RequireList)
                    SetRequireList.Add(Item);
                foreach (IIdentifier Item in SetterBody.ExceptionIdentifierList)
                    SetExceptionIdentifierList.Add(Item);
            }

            PropertyType.ResolveType(EmbeddingClass.TypeTable, BaseTypeName, BaseType, EntityTypeName, EntityType, node.PropertyKind, GetEnsureList, GetExceptionIdentifierList, SetRequireList, SetExceptionIdentifierList, out ITypeName ResolvedPropertyTypeName, out ICompiledType ResolvedPropertyType);

            node.ResolvedEntityTypeName.Item = EntityTypeName;
            node.ResolvedEntityType.Item = EntityType;
            node.ResolvedFeatureTypeName.Item = ResolvedPropertyTypeName;
            node.ResolvedFeatureType.Item = ResolvedPropertyType;

            if (node.GetterBody.IsAssigned)
                EmbeddingClass.BodyList.Add((IBody)node.GetterBody.Item);

            if (node.SetterBody.IsAssigned)
                EmbeddingClass.BodyList.Add((IBody)node.SetterBody.Item);

            IScopeAttributeFeature Result = ((Tuple<IScopeAttributeFeature, IScopeAttributeFeature>)data).Item1;
            IScopeAttributeFeature Value = ((Tuple<IScopeAttributeFeature, IScopeAttributeFeature>)data).Item2;

            node.LocalGetScope.Add(Result.ValidFeatureName.Item.Name, Result);
            node.LocalGetScope.Seal();
            node.FullGetScope.Merge(node.LocalGetScope);

            node.LocalSetScope.Add(Value.ValidFeatureName.Item.Name, Value);
            node.LocalSetScope.Seal();
            node.FullSetScope.Merge(node.LocalSetScope);

            node.LocalScope.Seal();
            node.FullScope.Merge(node.LocalScope);

            ScopeHolder.RecursiveAdd(node.FullGetScope, node.InnerGetScopes);
            ScopeHolder.RecursiveAdd(node.FullSetScope, node.InnerSetScopes);

            node.ResolvedFeature.Item = node;
        }
        #endregion
    }
}
