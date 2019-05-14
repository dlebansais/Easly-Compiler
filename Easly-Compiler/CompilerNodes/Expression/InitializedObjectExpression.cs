namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IInitializedObjectExpression.
    /// </summary>
    public interface IInitializedObjectExpression : BaseNode.IInitializedObjectExpression, IExpression, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.InitializedObjectExpression.AssignmentBlocks"/>.
        /// </summary>
        IList<IAssignmentArgument> AssignmentList { get; }

        /// <summary>
        /// The resolved class type name.
        /// </summary>
        OnceReference<ITypeName> ResolvedClassTypeName { get; }

        /// <summary>
        /// The resolved class type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedClassType { get; }

        /// <summary>
        /// The list of features assigned in the resolved type.
        /// </summary>
        IHashtableEx<string, ICompiledFeature> AssignedFeatureTable { get; }
    }

    /// <summary>
    /// Compiler IInitializedObjectExpression.
    /// </summary>
    public class InitializedObjectExpression : BaseNode.InitializedObjectExpression, IInitializedObjectExpression
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.InitializedObjectExpression.AssignmentBlocks"/>.
        /// </summary>
        public IList<IAssignmentArgument> AssignmentList { get; } = new List<IAssignmentArgument>();

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
                case nameof(AssignmentBlocks):
                    TargetList = (IList)AssignmentList;
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
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IList<IExpressionType>>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<ICompiledType>();
                AssignedFeatureTable = new HashtableEx<string, ICompiledFeature>();
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
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ResolvedResult.IsAssigned && ResolvedExceptions.IsAssigned;

                Debug.Assert(ExpressionConstant.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassTypeName.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassType.IsAssigned || !IsResolved);
                Debug.Assert(AssignedFeatureTable.IsSealed || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IExpression
        /// <summary>
        /// Types of expression results.
        /// </summary>
        public OnceReference<IList<IExpressionType>> ResolvedResult { get; private set; } = new OnceReference<IList<IExpressionType>>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// Sets the <see cref="IExpression.ExpressionConstant"/> property.
        /// </summary>
        /// <param name="expressionConstant">The expression constant.</param>
        public void SetExpressionConstant(ILanguageConstant expressionConstant)
        {
            Debug.Assert(!ExpressionConstant.IsAssigned);
            Debug.Assert(expressionConstant != null);

            ExpressionConstant.Item = expressionConstant;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved class type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved class type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedClassType { get; private set; } = new OnceReference<ICompiledType>();

        /// <summary>
        /// The list of features assigned in the resolved type.
        /// </summary>
        public IHashtableEx<string, ICompiledFeature> AssignedFeatureTable { get; private set; } = new HashtableEx<string, ICompiledFeature>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="expression1">The first expression.</param>
        /// <param name="expression2">The second expression.</param>
        public static bool IsExpressionEqual(IInitializedObjectExpression expression1, IInitializedObjectExpression expression2)
        {
            bool Result = true;

            Result &= expression1.AssignmentList.Count == expression2.AssignmentList.Count;

            for (int i = 0; i < expression1.AssignmentList.Count && i < expression2.AssignmentList.Count; i++)
            {
                IAssignmentArgument InitializationAssignment1 = expression1.AssignmentList[i];
                IAssignmentArgument InitializationAssignment2 = expression2.AssignmentList[i];

                Result &= AssignmentArgument.IsAssignmentArgumentEqual(InitializationAssignment1, InitializationAssignment2);
            }

            return Result;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString
        {
            get
            {
                string Arguments = Argument.ArgumentListToString(AssignmentList);
                return $"{ClassIdentifier.Text} {{{Arguments}}}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Initialized Object Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
