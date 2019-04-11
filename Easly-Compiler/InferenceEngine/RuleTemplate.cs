﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;

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
        /// <param name="source">The node instance to check.</param>
        bool IsNoDestinationSet(ISource source);

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        bool AreAllSourcesReady(ISource source);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(ISource source, out object data);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="source">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        void Apply(ISource source, object data);
    }

    /// <summary>
    /// A rule to process an Easly node.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRule">The rule type, used to ensure unicity of the static constructor.</typeparam>
    public interface IRuleTemplate<TSource, TRule>
        where TSource : ISource
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
        /// <param name="source">The node instance to check.</param>
        bool IsNoDestinationSet(TSource source);

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        bool AreAllSourcesReady(TSource source);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(TSource source, out object data);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="source">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        void Apply(TSource source, object data);
    }

    /// <summary>
    /// A rule to process an Easly node.
    /// </summary>
    /// <typeparam name="TSource">The node type on which the rule applies.</typeparam>
    /// <typeparam name="TRule">The rule type, used to ensure unicity of the static constructor.</typeparam>
    public abstract class RuleTemplate<TSource, TRule> : IRuleTemplate<TSource, TRule>, IRuleTemplate
        where TSource : ISource
        where TRule : IRuleTemplate
    {
        #region Init
        /// <summary>
        /// Sources this rule is watching.
        /// </summary>
        public static List<ISourceTemplate> SourceTemplateList { get; protected set; }

        /// <summary>
        /// Destinations this rule applies to.
        /// </summary>
        public static List<IDestinationTemplate> DestinationTemplateList { get; protected set; }
        #endregion

        #region Properties
        /// <summary>
        /// Type on which a rule operates.
        /// </summary>
        public Type NodeType { get { return typeof(TSource); } }

        /// <summary>
        /// List of errors found when applying this rule.
        /// </summary>
        public IList<IError> ErrorList { get; } = new List<IError>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        public virtual bool IsNoDestinationSet(TSource source)
        {
            return !DestinationTemplateList.Exists((IDestinationTemplate destinationTemplate) => { return destinationTemplate.IsSet(source); });
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool IsNoDestinationSet(ISource source) { return IsNoDestinationSet((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Checks that no destination value has been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        public virtual bool AreAllSourcesReady(TSource source)
        {
            return SourceTemplateList.TrueForAll((ISourceTemplate sourceTemplate) => { return sourceTemplate.IsReady(source); });
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool AreAllSourcesReady(ISource source) { return AreAllSourcesReady((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public abstract bool CheckConsistency(TSource source, out object data);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool CheckConsistency(ISource source, out object data) { return CheckConsistency((TSource)source, out data); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="source">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public abstract void Apply(TSource source, object data);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void Apply(ISource source, object data) { Apply((TSource)source, data); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Implementation
        /// <summary>
        /// Adds an error.
        /// </summary>
        /// <param name="error">The error to add.</param>
        protected virtual void AddSourceError(IError error)
        {
            ErrorList.Add(error);
        }
        #endregion
    }
}