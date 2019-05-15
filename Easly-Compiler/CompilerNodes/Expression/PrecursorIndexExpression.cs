namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IPrecursorIndexExpression.
    /// </summary>
    public interface IPrecursorIndexExpression : BaseNode.IPrecursorIndexExpression, IExpression, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorIndexExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }
    }

    /// <summary>
    /// Compiler IPrecursorIndexExpression.
    /// </summary>
    public class PrecursorIndexExpression : BaseNode.PrecursorIndexExpression, IPrecursorIndexExpression
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorIndexExpression.ArgumentBlocks"/>.
        /// </summary>
        public IList<IArgument> ArgumentList { get; } = new List<IArgument>();

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
                case nameof(ArgumentBlocks):
                    TargetList = (IList)ArgumentList;
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
                ResolvedExceptions = new OnceReference<IList<IIdentifier>>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
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

                Debug.Assert(!ExpressionConstant.IsAssigned || IsResolved);

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
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ResolvedExceptions { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();

        /// <summary>
        /// Sets the <see cref="IExpression.ExpressionConstant"/> property.
        /// </summary>
        /// <param name="expressionConstant">The expression constant.</param>
        public void SetExpressionConstant(ILanguageConstant expressionConstant)
        {
            Debug.Assert(!ExpressionConstant.IsAssigned);

            if (expressionConstant != null)
                ExpressionConstant.Item = expressionConstant;
        }
        #endregion

        #region Compiler
        /*
         * Two precursor index expressions cannot be compared because it happens only when comparing different features, and there can be only one indexer.
        public static bool IsExpressionEqual(IPrecursorIndexExpression expression1, IPrecursorIndexExpression expression2)
        {
            bool Result = true;

            if (expression1.AncestorType.IsAssigned && expression2.AncestorType.IsAssigned)
            {
                IObjectType AncestorType1 = (IObjectType)expression1.AncestorType;
                IObjectType AncestorType2 = (IObjectType)expression2.AncestorType;

                Debug.Assert(AncestorType1.ResolvedType.IsAssigned);
                Debug.Assert(AncestorType2.ResolvedType.IsAssigned);

                Result &= AncestorType1.ResolvedType.Item == AncestorType2.ResolvedType.Item;
            }

            Result &= expression1.AncestorType.IsAssigned == expression2.AncestorType.IsAssigned;
            Result &= Argument.IsArgumentListEqual(expression1.ArgumentList, expression2.ArgumentList);

            return Result;
        }
        */
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString
        {
            get
            {
                string AncestorString = AncestorType.IsAssigned ? $" {{{((IObjectType)AncestorType.Item).TypeToString}}} " : string.Empty;
                return $"precursor{AncestorString}[{Argument.ArgumentListToString(ArgumentList)}]";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Precursor Index Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
