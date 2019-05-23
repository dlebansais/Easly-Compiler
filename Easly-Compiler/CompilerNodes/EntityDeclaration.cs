namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IEntityDeclaration.
    /// </summary>
    public interface IEntityDeclaration : BaseNode.IEntityDeclaration, INode, ISource
    {
        /// <summary>
        /// The resolved type name of this declaration.
        /// </summary>
        OnceReference<ITypeName> ResolvedEntityTypeName { get; }

        /// <summary>
        /// The resolved type of this declaration.
        /// </summary>
        OnceReference<ICompiledType> ResolvedEntityType { get; }

        /// <summary>
        /// The entity name as an atribute feature name.
        /// </summary>
        OnceReference<IFeatureName> ValidEntityName { get; }

        /// <summary>
        /// The entity instance as an atribute feature instance.
        /// </summary>
        OnceReference<IFeatureInstance> ValidEntityInstance { get; }

        /// <summary>
        /// The entity as an atribute feature.
        /// </summary>
        OnceReference<IScopeAttributeFeature> ValidEntity { get; }

        /// <summary>
        /// Gets a string representation of the entity declaration.
        /// </summary>
        string EntityDeclarationToString { get; }
    }

    /// <summary>
    /// Compiler IEntityDeclaration.
    /// </summary>
    public class EntityDeclaration : BaseNode.EntityDeclaration, IEntityDeclaration
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDeclaration"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public EntityDeclaration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDeclaration"/> class.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="resolvedTypeName">The resolved entity type name.</param>
        /// <param name="resolvedType">The resolved entity type.</param>
        public EntityDeclaration(IEntityDeclaration source, ITypeName resolvedTypeName, ICompiledType resolvedType)
        {
            Documentation = source.Documentation;
            EntityName = source.EntityName;
            EntityType = source.EntityType;
            DefaultValue = source.DefaultValue;

            ResolvedEntityTypeName.Item = resolvedTypeName;
            ResolvedEntityType.Item = resolvedType;
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedEntityTypeName = new OnceReference<ITypeName>();
                ResolvedEntityType = new OnceReference<ICompiledType>();
                ValidEntityName = new OnceReference<IFeatureName>();
                ValidEntityInstance = new OnceReference<IFeatureInstance>();
                ValidEntity = new OnceReference<IScopeAttributeFeature>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = ValidEntity.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved type name of this declaration.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEntityTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type of this declaration.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEntityType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The entity name as an atribute feature name.
        /// </summary>
        public OnceReference<IFeatureName> ValidEntityName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// The entity instance as an atribute feature instance.
        /// </summary>
        public OnceReference<IFeatureInstance> ValidEntityInstance { get; private set; } = new OnceReference<IFeatureInstance>();

        /// <summary>
        /// The entity as an atribute feature.
        /// </summary>
        public OnceReference<IScopeAttributeFeature> ValidEntity { get; private set; } = new OnceReference<IScopeAttributeFeature>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of a list of entity declarations.
        /// </summary>
        /// <param name="entityDeclarationList">The list of entity declarations.</param>
        public static string EntityDeclarationListToString(IEnumerable entityDeclarationList)
        {
            string Result = string.Empty;

            foreach (IEntityDeclaration EntityDeclaration in entityDeclarationList)
            {
                if (Result.Length > 0)
                    Result += ", ";
                Result += EntityDeclaration.EntityDeclarationToString;
            }

            return Result;
        }

        /// <summary>
        /// Gets a string representation of the entity declaration.
        /// </summary>
        public string EntityDeclarationToString
        {
            get
            {
                string DefaultString = DefaultValue.IsAssigned ? $" = {((IExpression)DefaultValue.Item).ExpressionToString}" : string.Empty;
                return $"{EntityName.Text}: {((IObjectType)EntityType).TypeToString}{DefaultString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Entity Declaration '{EntityDeclarationToString}'";
        }
        #endregion
    }
}
