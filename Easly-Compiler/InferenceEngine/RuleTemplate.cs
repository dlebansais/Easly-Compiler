namespace EaslyCompiler
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
        /// Checks that all sources are ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data returned by each source.</param>
        bool AreAllSourcesReady(ISource source, out IDictionary<ISourceTemplate, object> dataList);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(ISource source, IDictionary<ISourceTemplate, object> dataList, out object data);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="source">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        void Apply(ISource source, object data);

        /// <summary>
        /// Checks that all destination values have been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        bool AreAllDestinationsSet(ISource source);

        /// <summary>
        /// Gets all sources that are not ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        IList<ISourceTemplate> GetAllSourceTemplatesNotReady(ISource source);

        /// <summary>
        /// Gets all destinations that are not set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        IList<IDestinationTemplate> GetAllDestinationTemplatesNotSet(ISource source);
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
        /// Checks that all sources are ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data returned by each source.</param>
        bool AreAllSourcesReady(TSource source, out IDictionary<ISourceTemplate, object> dataList);

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        bool CheckConsistency(TSource source, IDictionary<ISourceTemplate, object> dataList, out object data);

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="source">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        void Apply(TSource source, object data);

        /// <summary>
        /// Checks that all destination values have been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        bool AreAllDestinationsSet(TSource source);

        /// <summary>
        /// Gets all sources that are not ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        IList<ISourceTemplate> GetAllSourceTemplatesNotReady(TSource source);

        /// <summary>
        /// Gets all destinations that are not set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        IList<IDestinationTemplate> GetAllDestinationTemplatesNotSet(TSource source);
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
        #region Constant
        /// <summary>
        /// The dot separator for property path.
        /// </summary>
        public static readonly string Dot = InferenceEngine.Dot;
        #endregion

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
        /// Checks that all sources are ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data returned by each source.</param>
        public virtual bool AreAllSourcesReady(TSource source, out IDictionary<ISourceTemplate, object> dataList)
        {
            dataList = new Dictionary<ISourceTemplate, object>();
            bool IsReady = true;

            foreach (ISourceTemplate SourceTemplate in SourceTemplateList)
            {
                IsReady &= SourceTemplate.IsReady(source, out object data);
                dataList.Add(SourceTemplate, data);
            }

            return IsReady;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool AreAllSourcesReady(ISource source, out IDictionary<ISourceTemplate, object> dataList) { return AreAllSourcesReady((TSource)source, out dataList); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public abstract bool CheckConsistency(TSource source, IDictionary<ISourceTemplate, object> dataList, out object data);
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool CheckConsistency(ISource source, IDictionary<ISourceTemplate, object> dataList, out object data) { return CheckConsistency((TSource)source, dataList, out data); }
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

        /// <summary>
        /// Checks that all destinations values have been set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        public virtual bool AreAllDestinationsSet(TSource source)
        {
            bool IsSet = true;

            foreach (IDestinationTemplate DestinationTemplate in DestinationTemplateList)
                IsSet &= DestinationTemplate.IsSet(source);

            return IsSet;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool AreAllDestinationsSet(ISource source) { return AreAllDestinationsSet((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets all sources that are not ready.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        public virtual IList<ISourceTemplate> GetAllSourceTemplatesNotReady(TSource source)
        {
            IList<ISourceTemplate> Result = new List<ISourceTemplate>();

            foreach (ISourceTemplate SourceTemplate in SourceTemplateList)
                if (!SourceTemplate.IsReady(source, out object data))
                    Result.Add(SourceTemplate);

            return Result;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public IList<ISourceTemplate> GetAllSourceTemplatesNotReady(ISource source) { return GetAllSourceTemplatesNotReady((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets all destinations that are not set.
        /// </summary>
        /// <param name="source">The node instance to check.</param>
        public virtual IList<IDestinationTemplate> GetAllDestinationTemplatesNotSet(TSource source)
        {
            IList<IDestinationTemplate> Result = new List<IDestinationTemplate>();

            foreach (IDestinationTemplate DestinationTemplate in DestinationTemplateList)
                if (!DestinationTemplate.IsSet(source))
                    Result.Add(DestinationTemplate);

            return Result;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public IList<IDestinationTemplate> GetAllDestinationTemplatesNotSet(ISource source) { return GetAllDestinationTemplatesNotSet((TSource)source); }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #region Implementation
        /// <summary>
        /// Adds an error.
        /// </summary>
        /// <param name="error">The error to add.</param>
        protected virtual void AddSourceError(IError error)
        {
            ErrorList.Add(error);
        }

        /// <summary>
        /// Adds several errors.
        /// </summary>
        /// <param name="sourceErrorList">The list of errors to add.</param>
        protected virtual void AddSourceErrorList(IList<IError> sourceErrorList)
        {
            foreach (IError Error in sourceErrorList)
                ErrorList.Add(Error);
        }
        #endregion
    }
}
