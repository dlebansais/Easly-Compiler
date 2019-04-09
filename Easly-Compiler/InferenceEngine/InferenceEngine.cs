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
        /// The list of classes on which rules are checked and applied.
        /// </summary>
        IList<IClass> SourceList { get; }

        /// <summary>
        /// Checks if a node is fully resolved.
        /// </summary>
        Func<INode, bool> IsResolvedHandler { get; }

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
        /// <param name="sourceList">The list of classes on which rules are checked and applied.</param>
        /// <param name="isResolvedHandler">Checks if a node is fully resolved.</param>
        /// <param name="isCycleErrorChecked">True if the engine should check for cyclic dependencies errors.</param>
        public InferenceEngine(IList<IRuleTemplate> ruleTemplateList, IList<IClass> sourceList, Func<INode, bool> isResolvedHandler, bool isCycleErrorChecked)
        {
            RuleTemplateList = ruleTemplateList;
            SourceList = sourceList;
            IsCycleErrorChecked = isCycleErrorChecked;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The set of rules to execute.
        /// </summary>
        public IList<IRuleTemplate> RuleTemplateList { get; }

        /// <summary>
        /// The list of classes on which rules are checked and applied.
        /// </summary>
        public IList<IClass> SourceList { get; }

        /// <summary>
        /// Checks if a node is fully resolved.
        /// </summary>
        public Func<INode, bool> IsResolvedHandler { get; }

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
            IList<IClass> UnresolvedClassList = new List<IClass>(SourceList);
            bool Result = true;

            bool Exit = false;
            while (UnresolvedClassList.Count > 0 && errorList.Count == 0 && !Exit)
            {
                Exit = true;
                InferTemplates(ref Exit, errorList);

                MoveResolvedClasses(ref Exit, UnresolvedClassList, ResolvedClassList);
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
        protected virtual void InferTemplates(ref bool exit, IList<IError> errorList)
        {
            foreach (IRuleTemplate Rule in RuleTemplateList)
            {
                foreach (INode Node in SourceList)
                {
                    if (Node.GetType() != Rule.NodeType)
                        continue;

                    if (Rule.IsNoDestinationSet(Node))
                        if (Rule.AreAllSourcesReady(Node) && Rule.ErrorList.Count == 0)
                        {
                            if (Rule.CheckConsistency(Node))
                            {
                                Debug.Assert(Rule.ErrorList.Count == 0);
                                Rule.Apply(Node);
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
        protected virtual void MoveResolvedClasses(ref bool exit, IList<IClass> unresolvedClassList, IList<IClass> resolvedClassList)
        {
            IList<IClass> ToMove = new List<IClass>();

            foreach (IClass Class in unresolvedClassList)
                if (IsResolvedHandler(Class))
                    ToMove.Add(Class);

            if (ToMove.Count > 0)
            {
                foreach (IClass ClassItem in ToMove)
                {
                    unresolvedClassList.Remove(ClassItem);
                    resolvedClassList.Add(ClassItem);
                }

                exit = false;
            }
        }
        #endregion
    }
}
