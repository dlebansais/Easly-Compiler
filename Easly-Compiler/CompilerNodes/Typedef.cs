namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ITypedef.
    /// </summary>
    public interface ITypedef : BaseNode.ITypedef, INode, ISource
    {
        /// <summary>
        /// The valid typedef name.
        /// </summary>
        OnceReference<IFeatureName> ValidTypedefName { get; }

        /// <summary>
        /// Resolved type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedDefinedTypeName { get; }

        /// <summary>
        /// Resolved type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedDefinedType { get; }
    }

    /// <summary>
    /// Compiler ITypedef.
    /// </summary>
    public class Typedef : BaseNode.Typedef, ITypedef
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                ValidTypedefName = new OnceReference<IFeatureName>();
                ResolvedDefinedTypeName = new OnceReference<ITypeName>();
                ResolvedDefinedType = new OnceReference<ICompiledType>();
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
                IsResolved = ValidTypedefName.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
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
        /// The valid typedef name.
        /// </summary>
        public OnceReference<IFeatureName> ValidTypedefName { get; private set; } = new OnceReference<IFeatureName>();

        /// <summary>
        /// Resolved type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedDefinedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedDefinedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion
    }
}
