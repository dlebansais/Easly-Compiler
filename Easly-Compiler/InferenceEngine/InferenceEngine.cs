namespace EaslyCompiler
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
        IList<IRuleTemplate> RuleTemplateList { get; }

        /// <summary>
        /// The list of nodes on which rules are checked and applied.
        /// </summary>
        IList<ISource> SourceList { get; }

        /// <summary>
        /// The list of classes to resolve.
        /// </summary>
        IList<IClass> ClassList { get; }

        /// <summary>
        /// Checks if a class is fully resolved.
        /// </summary>
        Func<IList<ISource>, IClass, bool> IsResolvedHandler { get; }

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
        /// <returns>True if there is no source left to process.</returns>
        bool Solve(IList<IError> errorList);
    }

    /// <summary>
    /// Inference engine to process rules over a set of Easly nodes.
    /// </summary>
    public class InferenceEngine : IInferenceEngine
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceEngine"/> class.
        /// </summary>
        /// <param name="ruleTemplateList">The set of rules to execute.</param>
        /// <param name="sourceList">The list of nodes on which rules are checked and applied.</param>
        /// <param name="classList">The list of classes to resolve.</param>
        /// <param name="isResolvedHandler">Checks if a class is fully resolved.</param>
        /// <param name="isCycleErrorChecked">True if the engine should check for cyclic dependencies errors.</param>
        /// <param name="retries">Number of retries (debug only).</param>
        public InferenceEngine(IList<IRuleTemplate> ruleTemplateList, IList<ISource> sourceList, IList<IClass> classList, Func<IList<ISource>, IClass, bool> isResolvedHandler, bool isCycleErrorChecked, int retries)
        {
            RuleTemplateList = ruleTemplateList;
            SourceList = sourceList;
            ClassList = classList;
            IsResolvedHandler = isResolvedHandler;
            IsCycleErrorChecked = isCycleErrorChecked;
            Retries = retries;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The set of rules to execute.
        /// </summary>
        public IList<IRuleTemplate> RuleTemplateList { get; }

        /// <summary>
        /// The list of nodes on which rules are checked and applied.
        /// </summary>
        public IList<ISource> SourceList { get; }

        /// <summary>
        /// The list of classes to resolve.
        /// </summary>
        public IList<IClass> ClassList { get; }

        /// <summary>
        /// Checks if a class is fully resolved.
        /// </summary>
        public Func<IList<ISource>, IClass, bool> IsResolvedHandler { get; }

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
        /// <returns>True if there is no source left to process.</returns>
        public virtual bool Solve(IList<IError> errorList)
        {
            bool Result;
            bool? LastTryResult = null;

            foreach (IRuleTemplate Rule in RuleTemplateList)
                Rule.ErrorList.Clear();

            for (;;)
            {
                List<IError> TryErrorList = new List<IError>();
                bool TryResult = SolveWithRetry(TryErrorList);
                if (Retries == 0)
                {
                    foreach (IError Error in TryErrorList)
                        errorList.Add(Error);
                    Result = TryResult;
                    break;
                }

                // Errors can differ, but success or failure must not.
                if (!LastTryResult.HasValue)
                    LastTryResult = TryResult;

                Debug.Assert(TryResult == LastTryResult.Value);

                // Reset sources so we restart inference from a fresh state.
                ResetSources();
                ShuffleRules(RuleTemplateList);

                if (Retries <= 0)
                    throw new InvalidOperationException("Invalid inference retries count.");

                Retries--;
            }

            return Result;
        }

        /// <summary>
        /// Execute all rules until the source list is exhausted.
        /// </summary>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if there is no source left to process.</returns>
        protected virtual bool SolveWithRetry(IList<IError> errorList)
        {
            IList<IClass> ResolvedClassList = new List<IClass>();
            IList<IClass> UnresolvedClassList = new List<IClass>(ClassList);
            IList<ISource> UnresolvedSourceList = new List<ISource>(SourceList);
            bool Result = true;

            Debug.Assert(IsNoDestinationSet());

            bool Exit = false;
            while (UnresolvedClassList.Count > 0 && errorList.Count == 0 && !Exit)
            {
                Exit = true;
                InferTemplates(ref Exit, UnresolvedSourceList, errorList);

                MoveResolvedClasses(ref Exit, UnresolvedClassList, ResolvedClassList, UnresolvedSourceList);
            }

            if (IsCycleErrorChecked && UnresolvedClassList.Count > 0 && errorList.Count == 0)
            {
                IList<string> NameList = new List<string>();
                foreach (IClass Class in UnresolvedClassList)
                    NameList.Add(Class.ValidClassName);

                errorList.Add(new ErrorCyclicDependency(NameList));
                Result = false;
            }

            return Result;
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
        protected virtual void InferTemplates(ref bool exit, IList<ISource> unresolvedSourceList, IList<IError> errorList)
        {
            foreach (IRuleTemplate Rule in RuleTemplateList)
            {
                foreach (ISource Source in unresolvedSourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Source.GetType()))
                        continue;

                    bool IsNoDestinationSet = Rule.IsNoDestinationSet(Source);

                    if (IsNoDestinationSet)
                    {
                        bool AreAllSourcesReady = Rule.AreAllSourcesReady(Source);
                        bool NoError = Rule.ErrorList.Count == 0;

                        if (AreAllSourcesReady && NoError)
                        {
                            if (Rule.CheckConsistency(Source, out object data))
                            {
                                Debug.Assert(Rule.ErrorList.Count == 0);
                                Rule.Apply(Source, data);
                                exit = false;
                            }
                            else
                            {
                                Debug.Assert(Rule.ErrorList.Count > 0);
                                foreach (IError Error in Rule.ErrorList)
                                    errorList.Add(Error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary></summary>
        protected virtual void MoveResolvedClasses(ref bool exit, IList<IClass> unresolvedClassList, IList<IClass> resolvedClassList, IList<ISource> unresolvedSourceList)
        {
            IList<IClass> ToMove = new List<IClass>();

            foreach (IClass Class in unresolvedClassList)
                if (IsResolvedHandler(SourceList, Class))
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

        private static void ShuffleRules(IList<IRuleTemplate> ruleTemplateList)
        {
            Random Rng = new Random();
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
        #endregion
    }
}
