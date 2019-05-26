namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IIndexQueryExpression.
    /// </summary>
    public interface IIndexQueryExpression : BaseNode.IIndexQueryExpression, INodeWithReplicatedBlocks, IExpression, IComparableExpression
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexQueryExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// List of parameters from the selected overload.
        /// </summary>
        ListTableEx<IParameter> SelectedParameterList { get; }

        /// <summary>
        /// Resolved arguments of the call.
        /// </summary>
        OnceReference<IList<IExpressionType>> ResolvedArgumentList { get; }
    }

    /// <summary>
    /// Compiler IIndexQueryExpression.
    /// </summary>
    public class IndexQueryExpression : BaseNode.IndexQueryExpression, IIndexQueryExpression
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.IndexQueryExpression.ArgumentBlocks"/>.
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ResolvedResult = new OnceReference<IResultType>();
                ConstantSourceList = new ListTableEx<IExpression>();
                ExpressionConstant = new OnceReference<ILanguageConstant>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                ResolvedException = new OnceReference<IResultException>();
                SelectedParameterList = new ListTableEx<IParameter>();
                ResolvedArgumentList = new OnceReference<IList<IExpressionType>>();
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

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Types)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;

                Debug.Assert(SelectedParameterList.IsSealed || !IsResolved);
                Debug.Assert(ResolvedArgumentList.IsAssigned || !IsResolved);

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
        public OnceReference<IResultType> ResolvedResult { get; private set; } = new OnceReference<IResultType>();

        /// <summary>
        /// List of exceptions the expression can throw.
        /// </summary>
        public OnceReference<IResultException> ResolvedException { get; private set; } = new OnceReference<IResultException>();

        /// <summary>
        /// The list of sources for a constant, if any.
        /// </summary>
        public ListTableEx<IExpression> ConstantSourceList { get; private set; } = new ListTableEx<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// List of parameters from the selected overload.
        /// </summary>
        public ListTableEx<IParameter> SelectedParameterList { get; private set; } = new ListTableEx<IParameter>();

        /// <summary>
        /// Resolved arguments of the call.
        /// </summary>
        public OnceReference<IList<IExpressionType>> ResolvedArgumentList { get; private set; } = new OnceReference<IList<IExpressionType>>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IIndexQueryExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IIndexQueryExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= Expression.IsExpressionEqual((IExpression)IndexedExpression, (IExpression)other.IndexedExpression);
            Result &= Argument.IsArgumentListEqual(ArgumentList, other.ArgumentList);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IIndexQueryExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="selectedParameterList">The selected parameters.</param>
        /// <param name="resolvedArgumentList">The list of arguments corresponding to selected parameters.</param>
        public static bool ResolveCompilerReferences(IIndexQueryExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ListTableEx<IParameter> selectedParameterList, out List<IExpressionType> resolvedArgumentList)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            selectedParameterList = null;
            resolvedArgumentList = null;

            IExpression IndexedExpression = (IExpression)node.IndexedExpression;
            IList<IArgument> ArgumentList = (IList<IArgument>)node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IResultType ResolvedIndexerResult = IndexedExpression.ResolvedResult.Item;

            OnceReference<ICompiledType> IndexedExpressionType = new OnceReference<ICompiledType>();
            foreach (IExpressionType Item in ResolvedIndexerResult)
                if (Item.Name == nameof(BaseNode.Keyword.Result) || ResolvedIndexerResult.Count == 1)
                {
                    IndexedExpressionType.Item = Item.ValueType;
                    break;
                }

            if (!IndexedExpressionType.IsAssigned)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            if (IndexedExpressionType.Item is IClassType AsClassType)
            {
                IClass IndexedBaseClass = AsClassType.BaseClass;
                IHashtableEx<IFeatureName, IFeatureInstance> IndexedFeatureTable = IndexedBaseClass.FeatureTable;

                if (!IndexedFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    errorList.AddError(new ErrorMissingIndexer(node));
                    return false;
                }

                IFeatureInstance IndexerInstance = IndexedFeatureTable[FeatureName.IndexerFeatureName];
                IIndexerFeature Indexer = (IndexerFeature)IndexerInstance.Feature.Item;
                IIndexerType AsIndexerType = (IndexerType)Indexer.ResolvedFeatureType.Item;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                TypeArgumentStyles ArgumentStyle;
                if (!Argument.Validate(ArgumentList, MergedArgumentList, out ArgumentStyle, errorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                ParameterTableList.Add(AsIndexerType.ParameterTable);

                int SelectedIndex;
                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, errorList, node, out SelectedIndex))
                    return false;

                resolvedResult = new ResultType(AsIndexerType.ResolvedEntityTypeName.Item, AsIndexerType.ResolvedEntityType.Item, string.Empty);
                resolvedException = new ResultException(AsIndexerType.GetExceptionIdentifierList);
                selectedParameterList = ParameterTableList[SelectedIndex];
                resolvedArgumentList = MergedArgumentList;

                // TODO: check if the result is a constant number
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

            return true;
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
                string Arguments = Argument.ArgumentListToString(ArgumentList);
                return $"({((IExpression)IndexedExpression).ExpressionToString})[{Arguments}]";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Index Query Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
