namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ISimpleType.
    /// </summary>
    public interface ISimpleType : BaseNode.ISimpleType, IObjectType
    {
        /// <summary>
        /// Type name of the source.
        /// </summary>
        OnceReference<ITypeName> TypeNameSource { get; }

        /// <summary>
        /// Type of the source.
        /// </summary>
        OnceReference<ICompiledType> TypeSource { get; }

        /// <summary>
        /// Name of the source type.
        /// </summary>
        OnceReference<string> ValidTypeSource { get; }

        /// <summary>
        /// Name of the source type when a formal generic.
        /// </summary>
        OnceReference<ITypeName> FormalGenericNameSource { get; }

        /// <summary>
        /// Source type when a formal generic.
        /// </summary>
        OnceReference<ICompiledType> FormalGenericSource { get; }
    }

    /// <summary>
    /// Compiler ISimpleType.
    /// </summary>
    public class SimpleType : BaseNode.SimpleType, ISimpleType
    {
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
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                TypeNameSource = new OnceReference<ITypeName>();
                TypeSource = new OnceReference<ICompiledType>();
                ValidTypeSource = new OnceReference<string>();
                FormalGenericNameSource = new OnceReference<ITypeName>();
                FormalGenericSource = new OnceReference<ICompiledType>();
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
                Debug.Assert(ResolvedTypeName.IsAssigned == ResolvedType.IsAssigned);
                IsResolved = ResolvedType.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IObjectType
        /// <summary>
        /// The resolved type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Compiler
        /// <summary>
        /// Type name of the source.
        /// </summary>
        public OnceReference<ITypeName> TypeNameSource { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Type of the source.
        /// </summary>
        public OnceReference<ICompiledType> TypeSource { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// Name of the source type.
        /// </summary>
        public OnceReference<string> ValidTypeSource { get; private set; } = new OnceReference<string>();

        /// <summary>
        /// Name of the source type when a formal generic.
        /// </summary>
        public OnceReference<ITypeName> FormalGenericNameSource { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Source type when a formal generic.
        /// </summary>
        public OnceReference<ICompiledType> FormalGenericSource { get; private set; } = new OnceReference<ICompiledType>();
        #endregion
    }
}
