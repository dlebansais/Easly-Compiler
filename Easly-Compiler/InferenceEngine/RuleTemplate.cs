namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process an Easly node.
    /// </summary>
    public interface IRuleTemplate
    {
        /// <summary>
        /// Type on which a rule operates.
        /// </summary>
        Type NodeType { get; }

        /// <summary>
        /// List of errors found when applying this rule.
        /// </summary>
        IList<IError> ErrorList { get; }

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        bool IsNoDestinationSet(object node);

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        bool AreAllSourcesReady(object node);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(object node);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        void Apply(object node);
    }

    /// <summary>
    /// A rule to process an Easly node.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRule">The rule type, used to ensure unicity of the static constructor.</typeparam>
    public interface IRuleTemplate<TNode, TRule>
        where TNode : INode
        where TRule : IRuleTemplate
    {
        /// <summary>
        /// Type on which a rule operates.
        /// </summary>
        Type NodeType { get; }

        /// <summary>
        /// List of errors found when applying this rule.
        /// </summary>
        IList<IError> ErrorList { get; }

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        bool IsNoDestinationSet(TNode node);

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        bool AreAllSourcesReady(TNode node);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(TNode node);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        void Apply(TNode node);
    }

    /// <summary>
    /// A rule to process an Easly node.
    /// </summary>
    /// <typeparam name="TNode">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRule">The rule type, used to ensure unicity of the static constructor.</typeparam>
    public abstract class RuleTemplate<TNode, TRule> : IRuleTemplate<TNode, TRule>, IRuleTemplate
        where TNode : INode
        where TRule : IRuleTemplate
    {
        static RuleTemplate()
        {
        }

        /// <summary>
        /// Sources this rule is watching.
        /// </summary>
        public static List<ISourceTemplate> SourceTemplateList { get; protected set; }

        /// <summary>
        /// Destinations this rule applies to.
        /// </summary>
        public static List<IDestinationTemplate> DestinationTemplateList { get; protected set; }

        /// <summary>
        /// Type on which a rule operates.
        /// </summary>
        public Type NodeType { get { return typeof(TNode); } }

        /// <summary>
        /// List of errors found when applying this rule.
        /// </summary>
        public IList<IError> ErrorList { get; } = new List<IError>();

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        public virtual bool IsNoDestinationSet(TNode node)
        {
            return !DestinationTemplateList.Exists((IDestinationTemplate destinationTemplate) => { return destinationTemplate.IsSet(node); });
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsNoDestinationSet(object node) { return IsNoDestinationSet((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        public virtual bool AreAllSourcesReady(TNode node)
        {
            return SourceTemplateList.TrueForAll((ISourceTemplate sourceTemplate) => { return sourceTemplate.IsReady(node); });
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool AreAllSourcesReady(object node) { return AreAllSourcesReady((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <returns>True if an error occured.</returns>
        public abstract bool CheckConsistency(TNode node);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool CheckConsistency(object node) { return CheckConsistency((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        public abstract void Apply(TNode node);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Apply(object node) { Apply((TNode)node); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
