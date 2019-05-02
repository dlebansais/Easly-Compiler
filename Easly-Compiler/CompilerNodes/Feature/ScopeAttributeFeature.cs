namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IScopeAttributeFeature.
    /// </summary>
    public interface IScopeAttributeFeature : IFeatureWithName // ,ICompiledFeature
    {
        /// <summary>
        /// The source used to create this attribute.
        /// </summary>
        ISource Location { get; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        IOverload EmbeddingOverload { get; }

        /// <summary>
        /// The resolved feature name.
        /// </summary>
        OnceReference<IFeatureName> ValidFeatureName { get; }

        /*
        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFeature { get; }*/

        /// <summary>
        /// The default value, if any.
        /// </summary>
        IOptionalReference<IExpression> DefaultValue { get; }

        /// <summary>
        /// Name of the associated type.
        /// </summary>
        OnceReference<ITypeName> ResolvedFeatureTypeName { get; }

        /// <summary>
        /// Associated type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedFeatureType { get; }

        /// <summary>
        /// Checks if this attribute conflicts with another from the same class group.
        /// </summary>
        /// <param name="assignedSingleClassList">The list of already single class attributes.</param>
        /// <param name="source">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        bool IsGroupAssigned(IList<IClass> assignedSingleClassList, ISource source, IErrorList errorList);
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
        public static IScopeAttributeFeature Create(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType)
        {
            return new ScopeAttributeFeature(location, attributeName, attributeTypeName, attributeType);
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
        public static bool Create(ISource location, string attributeName, ITypeName attributeTypeName, ICompiledType attributeType, IExpression initialDefaultValue, IErrorList errorList, out IScopeAttributeFeature feature)
        {
            Debug.Assert(initialDefaultValue != null);

            feature = null;
            bool Result = false;

            if (attributeType is IClassType AsClassType && AsClassType.BaseClass.Cloneable == BaseNode.CloneableStatus.Single)
                errorList.AddError(new ErrorSingleTypeNotAllowed(location, attributeName));
            else
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
            Location = location;
            EmbeddingOverload = location.EmbeddingOverload;

            EntityName = new Name(Location, attributeName);

            /*
            ResolvedFeature = new OnceReference<ICompiledFeature>();
            ResolvedFeature.Item = this;

            ExportIdentifier = new ExportIdentifier();
            Export = BaseNode.ExportStatus.Exported;
            Documentation = BaseNodeHelper.NodeHelper.CreateEmptyDocumentation();
            */
            ValidFeatureName = new OnceReference<IFeatureName>();
            ValidFeatureName.Item = new FeatureName(EntityName.Text);

            ResolvedFeatureTypeName = new OnceReference<ITypeName>();
            if (attributeTypeName != null)
                ResolvedFeatureTypeName.Item = attributeTypeName;

            ResolvedFeatureType = new OnceReference<ICompiledType>();
            if (attributeType != null)
                ResolvedFeatureType.Item = attributeType;

            if (initialDefaultValue != null)
            {
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper<IExpression>.CreateReference(initialDefaultValue);
                DefaultValue.Assign();
            }
            else
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper<IExpression>.CreateEmptyReference();
        }

        /// <summary>
        /// Checks if this attribute conflicts with another from the same class group.
        /// </summary>
        /// <param name="assignedSingleClassList">The list of already single class attributes.</param>
        /// <param name="source">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        public bool IsGroupAssigned(IList<IClass> assignedSingleClassList, ISource source, IErrorList errorList)
        {
            bool IsAssigned = false;

            if (ResolvedFeatureType.IsAssigned && ResolvedFeatureType.Item is IClassType AsClassType)
            {
                IClass BaseClass = AsClassType.BaseClass;
                Debug.Assert(BaseClass.ClassGroup.IsAssigned);

                if (BaseClass.Cloneable == BaseNode.CloneableStatus.Single)
                {
                    SingleClassGroup Group = AsClassType.BaseClass.ClassGroup.Item;
                    foreach (IClass GroupClass in Group.GroupClassList)
                        if (assignedSingleClassList.Contains(GroupClass))
                        {
                            IName EntityName = (IName)BaseClass.EntityName;
                            errorList.AddError(new ErrorSingleInstanceConflict(source, EntityName.ValidText.Item));
                            IsAssigned = true;
                            break;
                        }

                    if (!IsAssigned)
                        assignedSingleClassList.Add(BaseClass);
                }
            }

            return IsAssigned;
        }

        /// <summary>
        /// Creates a feature called 'Result' with the provided type.
        /// </summary>
        /// <param name="resultType">The feature type.</param>
        /// <param name="embeddingClass">The class where the feature is created.</param>
        /// <param name="location">The location to use when reporting errors.</param>
        public static IScopeAttributeFeature CreateResultFeature(IObjectType resultType, IClass embeddingClass, ISource location)
        {
            return Create(location, BaseNode.Keyword.Result.ToString(), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item);
        }

        /// <summary>
        /// Creates a feature called 'Value' with the provided type.
        /// </summary>
        /// <param name="resultType">The feature type.</param>
        /// <param name="embeddingClass">The class where the feature is created.</param>
        /// <param name="location">The location to use when reporting errors.</param>
        public static IScopeAttributeFeature CreateValueFeature(IObjectType resultType, IClass embeddingClass, ISource location)
        {
            return Create(location, BaseNode.Keyword.Value.ToString(), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The source used to create this name.
        /// </summary>
        public ISource Location { get; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IOverload EmbeddingOverload { get; }
/*
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
*/
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
/*
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
*/
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

        /*
        /// <summary>
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFeature { get; private set; } = new OnceReference<ICompiledFeature>();*/
        #endregion
    }
}
