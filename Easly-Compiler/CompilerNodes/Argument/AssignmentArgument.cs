namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IAssignmentArgument.
    /// </summary>
    public interface IAssignmentArgument : BaseNode.IAssignmentArgument, IArgument, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.AssignmentArgument.ParameterBlocks"/>.
        /// </summary>
        IList<IIdentifier> ParameterList { get; }
    }

    /// <summary>
    /// Compiler IAssignmentArgument.
    /// </summary>
    public class AssignmentArgument : BaseNode.AssignmentArgument, IAssignmentArgument
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.AssignmentArgument.ParameterBlocks"/>.
        /// </summary>
        public IList<IIdentifier> ParameterList { get; } = new List<IIdentifier>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyName">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyName, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyName)
            {
                case nameof(ParameterBlocks):
                    TargetList = (IList)ParameterList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
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
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new SealableList<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IArgument
        /// <summary>
        /// Types of expression results for the argument.
        /// </summary>
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the argument can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ISealableList<IExpression> ConstantSourceList { get; private set; } = new SealableList<IExpression>();

        /// <summary>
        /// The constant expression, if assigned.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// Compares two assignment arguments.
        /// </summary>
        /// <param name="argument1">The first argument.</param>
        /// <param name="argument2">The second argument.</param>
        public static bool IsAssignmentArgumentEqual(IAssignmentArgument argument1, IAssignmentArgument argument2)
        {
            bool Result = true;

            Result &= argument1.ParameterList.Count != argument2.ParameterList.Count;

            for (int i = 0; i < argument1.ParameterList.Count && i < argument2.ParameterList.Count; i++)
            {
                IIdentifier Parameter1 = argument1.ParameterList[i];
                IIdentifier Parameter2 = argument2.ParameterList[i];

                Result &= Parameter1.ValidText.Item == Parameter2.ValidText.Item;
            }

            Result &= Expression.IsExpressionEqual((IExpression)argument1.Source, (IExpression)argument2.Source);

            return Result;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the argument.
        /// </summary>
        public string ArgumentToString
        {
            get
            {
                return $"{Identifier.IdentifierListToString(ParameterList)} ← {((IExpression)Source).ExpressionToString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Assignment Argument '{ArgumentToString}'";
        }
        #endregion
    }
}
