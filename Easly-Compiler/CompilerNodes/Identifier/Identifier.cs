namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IIdentifier.
    /// </summary>
    public interface IIdentifier : BaseNode.IIdentifier, INode, ISource
    {
        /// <summary>
        /// The valid value of <see cref="BaseNode.IIdentifier.Text"/>.
        /// </summary>
        OnceReference<string> ValidText { get; }
    }

    /// <summary>
    /// Compiler IIdentifier.
    /// </summary>
    public class Identifier : BaseNode.Identifier, IIdentifier
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public Identifier()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier"/> class.
        /// </summary>
        /// <param name="text">The identifier text.</param>
        public Identifier(string text)
        {
            Text = text;
            ValidText = new OnceReference<string>();
            ValidText.Item = Text;
        }
        #endregion

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

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                ValidText = new OnceReference<string>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
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
                IsResolved = ValidText.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
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
        /// The valid value of <see cref="BaseNode.IIdentifier.Text"/>.
        /// </summary>
        public OnceReference<string> ValidText { get; private set; } = new OnceReference<string>();
        #endregion

        #region Debugging

        /// <summary>
        /// Gets a string representation of a list of identifiers.
        /// </summary>
        /// <param name="identifierList">The list of identifiers.</param>
        public static string IdentifierListToString(IList<IIdentifier> identifierList)
        {
            string Result = string.Empty;

            for (int i = 0; i < identifierList.Count; i++)
            {
                if (i > 0)
                    Result += ", ";

                Result += identifierList[i].Text;
            }

            return Result;
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Identifier '{Text}'";
        }
        #endregion
    }
}
