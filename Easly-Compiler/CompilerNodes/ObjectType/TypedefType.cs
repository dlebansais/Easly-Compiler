namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only ITypedefType.
    /// </summary>
    public interface ITypedefType : BaseNode.IObjectType, ISource
    {
        /// <summary>
        /// Resolved type name of the source.
        /// </summary>
        OnceReference<ITypeName> ReferencedTypeName { get; }

        /// <summary>
        /// Resolved type of the source.
        /// </summary>
        OnceReference<ICompiledType> ReferencedType { get; }
    }

    /// <summary>
    /// Compiler-only ITypedefType.
    /// </summary>
    public class TypedefType : BaseNode.ObjectType, ITypedefType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedefType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public TypedefType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedefType"/> class.
        /// </summary>
        /// <param name="typeName">Resolved type name of the source.</param>
        /// <param name="type">Resolved type of the source.</param>
        public TypedefType(ITypeName typeName, ICompiledType type)
        {
            ReferencedTypeName.Item = typeName;
            ReferencedType.Item = type;
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
                ReferencedTypeName = new OnceReference<ITypeName>();
                ReferencedType = new OnceReference<ICompiledType>();
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

        #region Compiler
        /// <summary>
        /// Resolved type name of the source.
        /// </summary>
        public OnceReference<ITypeName> ReferencedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type of the source.
        /// </summary>
        public OnceReference<ICompiledType> ReferencedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString
        {
            get
            {
                string TypeString = ReferencedType.Item is IObjectType AsObjectType ? $" = {AsObjectType.TypeToString}" : string.Empty;
                return $"typedef {ReferencedTypeName.Item.Name}{TypeString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Typedef Type '{TypeToString}'";
        }
        #endregion
    }
}
