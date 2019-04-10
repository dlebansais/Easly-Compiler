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
        public InferenceEngine(IList<IRuleTemplate> ruleTemplateList, IList<ISource> sourceList, IList<IClass> classList, Func<IList<ISource>, IClass, bool> isResolvedHandler, bool isCycleErrorChecked)
        {
            RuleTemplateList = ruleTemplateList;
            SourceList = sourceList;
            ClassList = classList;
            IsResolvedHandler = isResolvedHandler;
            IsCycleErrorChecked = isCycleErrorChecked;
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
        #endregion

        #region Client Interface
        /// <summary>
        /// Execute all rules until the source list is exhausted.
        /// </summary>
        /// <param name="errorList">List of errors found.</param>
        /// <returns>True if there is no source left to process.</returns>
        public virtual bool Solve(IList<IError> errorList)
        {
            IList<IClass> ResolvedClassList = new List<IClass>();
            IList<IClass> UnresolvedClassList = new List<IClass>(ClassList);
            IList<ISource> UnresolvedSourceList = new List<ISource>(SourceList);
            bool Result = true;

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

                errorList.Add(new ErrorCyclicDependency(UnresolvedClassList[0], NameList));
                Result = false;
            }

            return Result;
        }

        /// <summary></summary>
        protected virtual void InferTemplates(ref bool exit, IList<ISource> unresolvedSourceList, IList<IError> errorList)
        {
            foreach (IRuleTemplate Rule in RuleTemplateList)
            {
                foreach (ISource Node in unresolvedSourceList)
                {
                    if (!Rule.NodeType.IsAssignableFrom(Node.GetType()))
                        continue;

                    if (Rule.IsNoDestinationSet(Node))
                        if (Rule.AreAllSourcesReady(Node) && Rule.ErrorList.Count == 0)
                        {
                            if (Rule.CheckConsistency(Node, out object data))
                            {
                                Debug.Assert(Rule.ErrorList.Count == 0);
                                Rule.Apply(Node, data);
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
        #endregion
    }
}
