namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IScopeAttributeFeature.
    /// </summary>
    public interface IScopeAttributeFeature : IFeatureWithName, ICompiledFeature
    {
        /// <summary>
        /// The default value, if any.
        /// </summary>
        IOptionalReference<IExpression> DefaultValue { get; }
    }

    /// <summary>
    /// Compiler IScopeAttributeFeature.
    /// </summary>
    public class ScopeAttributeFeature : IScopeAttributeFeature
    {
        #region Init
        /// <summary>
        /// Creates a <see cref="IScopeAttributeFeature"/>.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        public static IScopeAttributeFeature Create(ISource location, string attributeName)
        {
            return new ScopeAttributeFeature(location, attributeName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        private ScopeAttributeFeature(ISource location, string attributeName)
            : this(location, attributeName, null, null, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="IScopeAttributeFeature"/>.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeTypeName">Resolved type name of the attribute.</param>
        /// <param name="attributeType">Resolved type of the attribute.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <param name="feature">The created feature if successful.</param>
        public static bool Create(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType, IList<IError> errorList, out IScopeAttributeFeature feature)
        {
            feature = null;
            bool Result = false;

            if (CheckGroupAssigned(attributeType, attributeName, true, location, errorList))
            {
                feature = new ScopeAttributeFeature(location, attributeName, attributeTypeName, attributeType);
                Result = true;
            }

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeTypeName">Resolved type name of the attribute.</param>
        /// <param name="attributeType">Resolved type of the attribute.</param>
        public ScopeAttributeFeature(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType)
            : this(location, attributeName, attributeTypeName, attributeType, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="IScopeAttributeFeature"/>.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeTypeName">Resolved type name of the attribute.</param>
        /// <param name="attributeType">Resolved type of the attribute.</param>
        /// <param name="initialDefaultValue">Default value, if any.</param>
        /// <param name="errorList">List of errors found.</param>
        /// <param name="feature">The created feature if successful.</param>
        public static bool Create(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType, IExpression initialDefaultValue, IList<IError> errorList, out IScopeAttributeFeature feature)
        {
            Debug.Assert(initialDefaultValue != null);

            feature = null;
            bool Result = false;

            if (CheckGroupAssigned(attributeType, attributeName, false, location, errorList))
            {
                feature = new ScopeAttributeFeature(location, attributeName, attributeTypeName, attributeType, initialDefaultValue);
                Result = true;
            }

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeTypeName">Resolved type name of the attribute.</param>
        /// <param name="attributeType">Resolved type of the attribute.</param>
        /// <param name="initialDefaultValue">Default value, if any.</param>
        public ScopeAttributeFeature(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType, IExpression initialDefaultValue)
        {
            EmbeddingClass = location.EmbeddingClass;
            EmbeddingFeature = location.EmbeddingFeature;
            EmbeddingOverload = location.EmbeddingOverload;
            EmbeddingBody = location.EmbeddingBody;
            EmbeddingAssertion = location.EmbeddingAssertion;
            ParentSource = location.ParentSource;

            EntityName = new Name(location, attributeName);

            ResolvedFeature = new OnceReference<ICompiledFeature>();
            ResolvedFeature.Item = this;
            ExportIdentifier = new ExportIdentifier();
            Export = BaseNode.ExportStatus.Exported;
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();

            ValidFeatureName = new OnceReference<IFeatureName>();
            ValidFeatureName.Item = new FeatureName(attributeName);

            ResolvedFeatureTypeName = new OnceReference<ITypeName>();
            if (attributeTypeName != null)
                ResolvedFeatureTypeName.Item = attributeTypeName;

            ResolvedFeatureType = new OnceReference<ICompiledType>();
            if (attributeType != null)
                ResolvedFeatureType.Item = attributeType;

            if (initialDefaultValue != null)
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper<IExpression>.CreateReference(initialDefaultValue);
            else
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper<IExpression>.CreateReference(new ManifestNumberExpression());
            DefaultValue.Unassign();
        }

        private static bool CheckGroupAssigned(ICompiledType attributeType, string attributeName, bool isSingleClassAllowed, ISource location, IList<IError> errorList)
        {
            bool Result = true;

            if (attributeType is IClassType AsClassType)
            {
                SingleClassGroup Group = AsClassType.BaseClass.ClassGroup.Item;

                if (Group.IsAssigned)
                {
                    errorList.Add(new ErrorSingleInstanceConflict(location, attributeName));
                    Result = false;
                }
                else if (!isSingleClassAllowed && AsClassType.BaseClass.Cloneable == BaseNode.CloneableStatus.Single)
                {
                    errorList.Add(new ErrorSingleTypeNotAllowed(location, attributeName));
                    Result = false;
                }
                else
                    Group.SetAssigned();
            }

            return Result;
        }

        /// <summary>
        /// Creates a feature called 'Result' with the provided type.
        /// </summary>
        /// <param name="resultType">The feature type.</param>
        /// <param name="embeddingClass">The class where the feature is created.</param>
        /// <param name="location">The location to use when reporting errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="feature">The created feature upon return is successful.</param>
        public static bool CreateResultFeature(IObjectType resultType, IClass embeddingClass, ISource location, IList<IError> errorList, out IScopeAttributeFeature feature)
        {
            return Create(location, BaseNode.Keyword.Result.ToString(), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item, errorList, out feature);
        }

        /// <summary>
        /// Creates a feature called 'Value' with the provided type.
        /// </summary>
        /// <param name="resultType">The feature type.</param>
        /// <param name="embeddingClass">The class where the feature is created.</param>
        /// <param name="location">The location to use when reporting errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="feature">The created feature upon return is successful.</param>
        public static bool CreateValueFeature(IObjectType resultType, IClass embeddingClass, ISource location, IList<IError> errorList, out IScopeAttributeFeature feature)
        {
            return Create(location, BaseNode.Keyword.Value.ToString(), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item, errorList, out feature);
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
        public IQueryOverload EmbeddingOverload { get; private set; }

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
            EmbeddingOverload = parentSource is IQueryOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ResolvedFeatureTypeName = new OnceReference<ITypeName>();
                ResolvedFeatureType = new OnceReference<ICompiledType>();
                ValidFeatureName = new OnceReference<IFeatureName>();
                ResolvedFeature = new OnceReference<ICompiledFeature>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Fake export identifier.
        /// </summary>
        public BaseNode.IIdentifier ExportIdentifier { get; }

        /// <summary>
        /// Fake export specification.
        /// </summary>
        public BaseNode.ExportStatus Export { get; }

        /// <summary>
        /// Fake documentation.
        /// </summary>
        public BaseNode.IDocument Documentation { get; }

        /// <summary>
        /// The generated attribute name.
        /// </summary>
        public BaseNode.IName EntityName { get; }

        /// <summary>
        /// The default value, if any.
        /// </summary>
        public IOptionalReference<IExpression> DefaultValue { get; }
        #endregion

        #region Implementation of ICompiledFeature
        /// <summary>
        /// Indicates if the feature is deferred in another class.
        /// </summary>
        public bool IsDeferredFeature { get { return false; } }

        /// <summary>
        /// True if the feature contains extern bodies in its overloads.
        /// </summary>
        public bool HasExternBody { get { return false; } }

        /// <summary>
        /// True if the feature contains precursor bodies in its overloads.
        /// </summary>
        public bool HasPrecursorBody { get { return false; } }

        /// <summary>
        /// Name of the associated type.
        /// </summary>
        public OnceReference<ITypeName> ResolvedFeatureTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Associated type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedFeatureType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Implementation of IFeature
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        public OnceReference<IFeatureName> ValidFeatureName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFeature { get; private set; } = new OnceReference<ICompiledFeature>();
        #endregion
    }
}
