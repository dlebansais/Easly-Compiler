namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IScopeAttributeFeature.
    /// </summary>
    public interface IScopeAttributeFeature : IFeatureWithName, ICompiledFeature, IFeatureWithEntity, IFeatureWithNumberType
    {
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        OnceReference<IFeatureName> ValidFeatureName { get; }

        /// <summary>
        /// The default value, if any.
        /// </summary>
        IOptionalReference<IExpression> DefaultValue { get; }

        /// <summary>
        /// Checks if this attribute conflicts with another from the same class group.
        /// </summary>
        /// <param name="assignedSingleClassList">The list of already single class attributes.</param>
        /// <param name="source">The location where to report errors.</param>
        /// <param name="errorList">The list of errors found.</param>
        bool IsGroupAssigned(IList<IClass> assignedSingleClassList, ISource source, IErrorList errorList);

        /// <summary>
        /// Sets the associated type.
        /// </summary>
        /// <param name="attributeTypeName">The associated type name.</param>
        /// <param name="attributeType">The associated type.</param>
        void FixFeatureType(ITypeName attributeTypeName, ICompiledType attributeType);
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
            : this(location, attributeName, null, null, null, null, null)
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
            : this(location, attributeName, attributeTypeName, attributeType, attributeTypeName, attributeType, null)
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
                feature = new ScopeAttributeFeature(location, attributeName, attributeTypeName, attributeType, attributeTypeName, attributeType, initialDefaultValue);
                Result = true;
            }

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="location">The location associated to this attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="agentTypeName">Resolved agent type name of the attribute.</param>
        /// <param name="agentType">Resolved agent type of the attribute.</param>
        /// <param name="attributeTypeName">Resolved effective type name of the attribute.</param>
        /// <param name="attributeType">Resolved effective type of the attribute.</param>
        /// <param name="initialDefaultValue">Default value, if any.</param>
        public ScopeAttributeFeature(ISource location, string attributeName, ITypeName agentTypeName, ICompiledType agentType, ITypeName attributeTypeName, ICompiledType attributeType, IExpression initialDefaultValue)
        {
            Location = location;

            EntityName = new Name(Location, attributeName);
            ValidFeatureName = new OnceReference<IFeatureName>();
            ValidFeatureName.Item = new FeatureName(EntityName.Text);

            if (attributeTypeName != null)
            {
                ResolvedAgentTypeName.Item = agentTypeName;
                ResolvedEffectiveTypeName.Item = attributeTypeName;
            }

            if (attributeType != null)
            {
                ResolvedAgentType.Item = agentType;
                ResolvedEffectiveType.Item = attributeType;
            }

            if (initialDefaultValue != null)
            {
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper.CreateReference<IExpression>(initialDefaultValue);
                DefaultValue.Assign();
            }
            else
                DefaultValue = BaseNodeHelper.OptionalReferenceHelper.CreateEmptyReference<IExpression>();

            Debug.Assert(!IsDeferredFeature);
            Debug.Assert(!HasExternBody);
            Debug.Assert(!HasPrecursorBody);

            if (attributeType is ICompiledNumberType AsNumberType)
                NumberKind = AsNumberType.GetDefaultNumberKind();
            else
                NumberKind = NumberKinds.NotApplicable;
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

            if (ResolvedEffectiveType.IsAssigned && ResolvedEffectiveType.Item is IClassType AsClassType)
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
            return Create(location, nameof(BaseNode.Keyword.Result), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item);
        }

        /// <summary>
        /// Creates a feature called 'Value' with the provided type.
        /// </summary>
        /// <param name="resultType">The feature type.</param>
        /// <param name="embeddingClass">The class where the feature is created.</param>
        /// <param name="location">The location to use when reporting errors.</param>
        public static IScopeAttributeFeature CreateValueFeature(IObjectType resultType, IClass embeddingClass, ISource location)
        {
            return Create(location, nameof(BaseNode.Keyword.Value), resultType.ResolvedTypeName.Item, resultType.ResolvedType.Item);
        }

        /// <summary>
        /// Sets the associated type.
        /// </summary>
        /// <param name="attributeTypeName">The associated type name.</param>
        /// <param name="attributeType">The associated type.</param>
        public void FixFeatureType(ITypeName attributeTypeName, ICompiledType attributeType)
        {
            if (ResolvedEffectiveTypeName.IsAssigned || ResolvedEffectiveType.IsAssigned)
            {
                Debug.Assert(ResolvedAgentTypeName.IsAssigned);
                Debug.Assert(ResolvedAgentType.IsAssigned);
                Debug.Assert(ResolvedEffectiveTypeName.IsAssigned);
                Debug.Assert(ResolvedEffectiveType.IsAssigned);

                Debug.Assert(ResolvedAgentTypeName.Item == attributeTypeName);
                Debug.Assert(ResolvedAgentType.Item == attributeType);
                Debug.Assert(ResolvedEffectiveTypeName.Item == attributeTypeName);
                Debug.Assert(ResolvedEffectiveType.Item == attributeType);
            }
            else
            {
                ResolvedAgentTypeName.Item = attributeTypeName;
                ResolvedAgentType.Item = attributeType;
                ResolvedEffectiveTypeName.Item = attributeTypeName;
                ResolvedEffectiveType.Item = attributeType;
            }
        }
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
        /// Name of the agent type associated to the feature.
        /// </summary>
        public OnceReference<ITypeName> ResolvedAgentTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The agent type associated to the feature.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedAgentType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The name of the type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        public OnceReference<ITypeName> ResolvedEffectiveTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The type to use, as source or destination, for the purpose of path searching, assignment and query.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedEffectiveType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Implementation of IFeatureWithEntity
        /// <summary>
        /// Guid of the language type corresponding to the entity object for an instance of this class.
        /// </summary>
        public Guid EntityGuid { get { return LanguageClasses.LocalEntity.Guid; } }

        /// <summary>
        /// The source node associated to this instance.
        /// </summary>
        public ISource Location { get; }
        #endregion

        #region Properties
        /// <summary>
        /// The resolved feature name.
        /// </summary>
        public OnceReference<IFeatureName> ValidFeatureName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// The default value, if any.
        /// </summary>
        public IOptionalReference<IExpression> DefaultValue { get; }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind { get; private set; }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
        }
        #endregion

        #region Implementation of IFeatureWithName
        /// <summary>
        /// The generated attribute name.
        /// </summary>
        public BaseNode.IName EntityName { get; }
        #endregion
    }
}
