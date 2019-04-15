namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IExceptionHandler.
    /// </summary>
    public interface IExceptionHandler : BaseNode.IExceptionHandler, INode, ISource
    {
    }

    /// <summary>
    /// Compiler IExceptionHandler.
    /// </summary>
    public class ExceptionHandler : BaseNode.ExceptionHandler, IExceptionHandler
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
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two lists of exception handler identifiers.
        /// </summary>
        /// <param name="exceptionIdentifierList1">The first list.</param>
        /// <param name="exceptionIdentifierList2">The second list.</param>
        public static bool IdenticalExceptionSignature(IList<IIdentifier> exceptionIdentifierList1, IList<IIdentifier> exceptionIdentifierList2)
        {
            if (exceptionIdentifierList1.Count != exceptionIdentifierList2.Count)
                return false;

            for (int i = 0; i < exceptionIdentifierList1.Count; i++)
            {
                IIdentifier DerivedIdentifier = exceptionIdentifierList1[i];

                bool Found = false;
                for (int j = 0; j < exceptionIdentifierList2.Count; j++)
                {
                    IIdentifier BaseIdentifier = exceptionIdentifierList2[j];

                    if (DerivedIdentifier.ValidText == BaseIdentifier.ValidText)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
