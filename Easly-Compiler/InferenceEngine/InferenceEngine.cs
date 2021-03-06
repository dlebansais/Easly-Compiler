﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Inference engine to process rules over a set of Easly nodes.
    /// </summary>
    public interface IInferenceEngine
    {
        /// <summary>
        /// The set of rules to execute.
        /// </summary>
        IRuleTemplateList RuleTemplateList { get; }

        /// <summary>
        /// The list of nodes on which rules are checked and applied.
        /// </summary>
        IList<ISource> SourceList { get; }

        /// <summary>
        /// The list of classes to resolve.
        /// </summary>
        IList<IClass> ClassList { get; }

        /// <summary>
        /// True if the engine should check for cyclic dependencies errors.
        /// </summary>
        bool IsCycleErrorChecked { get; }

        /// <summary>
        /// Number of retries (debug only).
        /// </summary>
        int Retries { get; }

        /// <summary>
        /// Execute all rules until the source list is exhausted.
        /// </summary>
        /// <param name="errorList">List of errors found.</param>
        /// <param name="passName">The pass name.</param>
        /// <returns>True if there is no source left to process.</returns>
        bool Solve(IErrorList errorList, string passName);
    }

    /// <summary>
    /// Inference engine to process rules over a set of Easly nodes.
    /// </summary>
    public class InferenceEngine : IInferenceEngine
    {
        #region Constant
        /// <summary>
        /// The dot separator for property path.
        /// </summary>
        public static readonly string Dot = ".";
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceEngine"/> class.
        /// </summary>
        /// <param name="ruleTemplateList">The set of rules to execute.</param>
        /// <param name="sourceList">The list of nodes on which rules are checked and applied.</param>
        /// <param name="classList">The list of classes to resolve.</param>
        /// <param name="isCycleErrorChecked">True if the engine should check for cyclic dependencies errors.</param>
        /// <param name="retries">Number of retries (debug only).</param>
        public InferenceEngine(IRuleTemplateList ruleTemplateList, IList<ISource> sourceList, IList<IClass> classList, bool isCycleErrorChecked, int retries)
        {
            RuleTemplateList = ruleTemplateList;
            SourceList = sourceList;
            ClassList = classList;
            IsCycleErrorChecked = isCycleErrorChecked;
            Retries = retries;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The set of rules to execute.
        /// </summary>
        public IRuleTemplateList RuleTemplateList { get; }

        /// <summary>
        /// The list of nodes on which rules are checked and applied.
        /// </summary>
        public IList<ISource> SourceList { get; }

        /// <summary>
        /// The list of classes to resolve.
        /// </summary>
        public IList<IClass> ClassList { get; }

        /// <summary>
        /// True if the engine should check for cyclic dependencies errors.
        /// </summary>
        public bool IsCycleErrorChecked { get; }

        /// <summary>
        /// Number of retries (debug only).
        /// </summary>
        public int Retries { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Execute all rules until the source list is exhausted.
        /// </summary>
        /// <param name="errorList">List of errors found.</param>
        /// <param name="passName">The pass name.</param>
        /// <returns>True if there is no source left to process.</returns>
        public virtual bool Solve(IErrorList errorList, string passName)
        {
            bool Success;
            bool? LastTryResult = null;

            for (;;)
            {
                foreach (IRuleTemplate Rule in RuleTemplateList)
                    Rule.Clear();

                IErrorList TryErrorList = new ErrorList();
                bool TryResult = SolveWithRetry(TryErrorList, passName);
                if (Retries == 0)
                {
                    errorList.AddErrors(TryErrorList);
                    Success = TryResult;
                    break;
                }

                // Errors can differ, but success or failure must not.
                if (!LastTryResult.HasValue)
                    LastTryResult = TryResult;

                Debug.Assert(TryResult == LastTryResult.Value);

                // Reset sources so we restart inference from a fresh state.
                ResetSources();
                ShuffleRules(RuleTemplateList, Retries);

                if (Retries <= 0)
                    throw new InvalidOperationException("Invalid inference retries count.");

                Retries--;
            }

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        /// <summary>
        /// Execute all rules until the source list is exhausted.
        /// </summary>
        /// <param name="errorList">List of errors found.</param>
        /// <param name="passName">The pass name.</param>
        /// <returns>True if there is no source left to process.</returns>
        protected virtual bool SolveWithRetry(IErrorList errorList, string passName)
        {
            IList<IClass> ResolvedClassList = new List<IClass>();
            IList<IClass> UnresolvedClassList = new List<IClass>(ClassList);
            IList<ISource> UnresolvedSourceList = new List<ISource>(SourceList);
            bool Success = true;

            Debug.Assert(IsNoDestinationSet());

            bool Exit = false;
            while (UnresolvedClassList.Count > 0 && Success && !Exit)
            {
                Exit = true;
                Success = InferTemplates(ref Exit, UnresolvedSourceList, errorList);
                if (Success)
                    MoveResolvedClasses(ref Exit, UnresolvedClassList, ResolvedClassList, UnresolvedSourceList);
            }

            if (Success && IsCycleErrorChecked && UnresolvedClassList.Count > 0)
            {
                ListDependencies(SourceList);

                IList<string> NameList = new List<string>();
                foreach (IClass Class in UnresolvedClassList)
                    NameList.Add(Class.ValidClassName);

                errorList.AddError(new ErrorCyclicDependency(NameList, passName));
                Success = false;
            }

            Debug.Assert(Success || !errorList.IsEmpty);
            return Success;
        }

        /// <summary></summary>
        protected virtual bool IsNoDestinationSet()
        {
            bool Result = true;

            foreach (IRuleTemplate Rule in RuleTemplateList)
            {
                foreach (ISource Source in SourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Source.GetType()))
                        continue;

                    Result &= Rule.IsNoDestinationSet(Source);
                }
            }

            return Result;
        }

        /// <summary></summary>
        protected virtual bool InferTemplates(ref bool exit, IList<ISource> unresolvedSourceList, IErrorList errorList)
        {
            bool Success = true;
            List<ISource> FailedSourceList = new List<ISource>();

            for (int i = 0; i < RuleTemplateList.Count; i++)
            {
                IRuleTemplate Rule = RuleTemplateList[i];

                /*if (Rule is IQueryOverloadTypeConformanceRuleTemplate AsRuleTemplate)
                {

                }*/

                foreach (ISource Source in unresolvedSourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Source.GetType()))
                        continue;

                    bool IsNoDestinationSet = Rule.IsNoDestinationSet(Source);

                    if (IsNoDestinationSet)
                    {
                        bool AreAllSourcesReady = Rule.AreAllSourcesReady(Source, out IDictionary<ISourceTemplate, object> DataList);
                        bool NoError = Rule.ErrorList.IsEmpty;

                        if (!AreAllSourcesReady && !FailedSourceList.Contains(Source))
                            FailedSourceList.Add(Source);

                        if (AreAllSourcesReady && NoError)
                        {
                            if (Rule.CheckConsistency(Source, DataList, out object data))
                            {
                                Debug.Assert(Rule.ErrorList.IsEmpty);
                                Rule.Apply(Source, data);
                                Debug.Assert(Rule.AreAllDestinationsSet(Source));

                                exit = false;
                            }
                            else
                            {
                                Debug.Assert(!Rule.ErrorList.IsEmpty);
                                errorList.AddErrors(Rule.ErrorList);

                                Success = false;
                            }
                        }
                    }
                }
            }

            return Success;
        }

        /// <summary></summary>
        protected virtual void MoveResolvedClasses(ref bool exit, IList<IClass> unresolvedClassList, IList<IClass> resolvedClassList, IList<ISource> unresolvedSourceList)
        {
            IList<IClass> ToMove = new List<IClass>();

            foreach (IClass Class in unresolvedClassList)
                if (CheckTypesResolved(Class))
                    ToMove.Add(Class);

            if (ToMove.Count > 0)
            {
                foreach (IClass Class in ToMove)
                {
                    unresolvedClassList.Remove(Class);
                    resolvedClassList.Add(Class);
                }

                exit = false;
            }
        }

        /// <summary></summary>
        protected virtual void ResetSources()
        {
            foreach (ISource Source in SourceList)
                Source.Reset(RuleTemplateList);
        }

        /// <summary></summary>
        protected virtual bool CheckTypesResolved(IClass item)
        {
            bool IsResolved = true;

            foreach (ISource Source in SourceList)
            {
                if (Source.EmbeddingClass == item || Source == item)
                    IsResolved = Source.IsResolved(RuleTemplateList);

                if (!IsResolved)
                    break;
            }

            return IsResolved;
        }

        private static void ShuffleRules(IList<IRuleTemplate> ruleTemplateList, int retries)
        {
            Random Rng = new Random(retries);
            int ShuffleCount = ruleTemplateList.Count * 4;
            int Count = ruleTemplateList.Count;

            for (int i = 0; i < ShuffleCount; i++)
            {
                int Index = Rng.Next(Count);
                IRuleTemplate RuleTemplate = ruleTemplateList[Index];
                ruleTemplateList.RemoveAt(Index);

                Index = Rng.Next(Count);
                ruleTemplateList.Insert(Index, RuleTemplate);
            }
        }

        private void ListDependencies(IList<ISource> unresolvedSourceList)
        {
            Debug.WriteLine("Performing cyclic dependencies analysis...");

            Dictionary<IRuleTemplate, IList<ISource>> Analysis = new Dictionary<IRuleTemplate, IList<ISource>>();

            foreach (IRuleTemplate Rule in RuleTemplateList)
            {
                /*if (Rule is IConstraintRuleTemplate AsRuleTemplate)
                {

                }*/

                foreach (ISource Source in unresolvedSourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Source.GetType()))
                        continue;

                    bool IsNoDestinationSet = Rule.IsNoDestinationSet(Source);
                    if (IsNoDestinationSet)
                    {
                        bool AreAllSourcesReady = Rule.AreAllSourcesReady(Source, out IDictionary<ISourceTemplate, object> DataList);
                        if (!AreAllSourcesReady)
                        {
                            IList<ISourceTemplate> SourceTemplateList = Rule.GetAllSourceTemplatesNotReady(Source);
                            Debug.Assert(SourceTemplateList.Count > 0);

                            if (!Analysis.ContainsKey(Rule))
                                Analysis.Add(Rule, new List<ISource>());
                            Analysis[Rule].Add(Source);
                        }
                    }
                }
            }

            bool Exit;
            do
            {
                ICollection<IRuleTemplate> Rules = Analysis.Keys;
                IList<IRuleTemplate> ToRemove = new List<IRuleTemplate>();

                foreach (KeyValuePair<IRuleTemplate, IList<ISource>> Entry in Analysis)
                {
                    IRuleTemplate Rule = Entry.Key;
                    IList<ISource> SourceList = Entry.Value;
                    Debug.Assert(SourceList.Count > 0);

                    foreach (ISource Source in SourceList)
                    {
                        IList<ISourceTemplate> SourceTemplateList = Rule.GetAllSourceTemplatesNotReady(Source);
                        Debug.Assert(SourceTemplateList.Count > 0);

                        foreach (ISourceTemplate SourceTemplate in SourceTemplateList)
                        {
                            string Path = SourceTemplate.Path;
                            Type SourceType = SourceTemplate.SourceType;
                            ITemplatePathStart StartingPoint = SourceTemplate.StartingPoint;

                            if (FindRuleWithDestination(Rules, unresolvedSourceList, Path, SourceType, StartingPoint, out IRuleTemplate MatchingRule))
                                if (!ToRemove.Contains(Rule))
                                    ToRemove.Add(Rule);
                        }
                    }
                }

                foreach (IRuleTemplate Rule in ToRemove)
                    Analysis.Remove(Rule);

                Exit = ToRemove.Count == 0;
            }
            while (!Exit);

            Debug.WriteLine($"{Analysis.Count} rule(s) are waiting on a source template:");
            foreach (KeyValuePair<IRuleTemplate, IList<ISource>> Entry in Analysis)
                Debug.WriteLine($"{Entry.Key} on {Entry.Value.Count} node(s)");
        }

        private bool FindRuleWithDestination(ICollection<IRuleTemplate> ruleTemplateList, IList<ISource> unresolvedSourceList, string path, Type type, ITemplatePathStart startingPoint, out IRuleTemplate matchingRule)
        {
            matchingRule = null;

            foreach (IRuleTemplate Rule in ruleTemplateList)
            {
                foreach (ISource Source in unresolvedSourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Source.GetType()))
                        continue;

                    IList<IDestinationTemplate> DestinationTemplateList = Rule.GetAllDestinationTemplatesNotSet(Source);
                    foreach (IDestinationTemplate DestinationTemplate in DestinationTemplateList)
                        if (DestinationTemplate.Path == path && DestinationTemplate.StartingPoint == startingPoint)
                            if (DestinationTemplate.DestinationType == type)
                            {
                                matchingRule = Rule;
                                return true;
                            }
                }
            }

            return false;
        }
        #endregion
    }
}
