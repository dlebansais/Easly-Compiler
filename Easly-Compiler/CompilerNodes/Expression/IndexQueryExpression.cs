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
        /// The resolved indexer.
        /// </summary>
        OnceReference<IIndexerFeature> ResolvedIndexer { get; }

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        OnceReference<IFeatureCall> FeatureCall { get; }
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
                ResolvedIndexer = new OnceReference<IIndexerFeature>();
                FeatureCall = new OnceReference<IFeatureCall>();
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
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);

                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = ResolvedException.IsAssigned;

                Debug.Assert(ResolvedIndexer.IsAssigned || !IsResolved);
                Debug.Assert(FeatureCall.IsAssigned || !IsResolved);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IExpression
        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public bool IsComplex { get { return false; } }

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
        public ISealableList<IExpression> ConstantSourceList { get; private set; } = new SealableList<IExpression>();

        /// <summary>
        /// Specific constant number.
        /// </summary>
        public OnceReference<ILanguageConstant> ExpressionConstant { get; private set; } = new OnceReference<ILanguageConstant>();
        #endregion

        #region Compiler
        /// <summary>
        /// The resolved indexer.
        /// </summary>
        public OnceReference<IIndexerFeature> ResolvedIndexer { get; private set; } = new OnceReference<IIndexerFeature>();

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        public OnceReference<IFeatureCall> FeatureCall { get; private set; } = new OnceReference<IFeatureCall>();

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
        /// <param name="resolvedExpression">The result of the search.</param>
        public static bool ResolveCompilerReferences(IIndexQueryExpression node, IErrorList errorList, out ResolvedExpression resolvedExpression)
        {
            resolvedExpression = new ResolvedExpression();

            IExpression IndexedExpression = (IExpression)node.IndexedExpression;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            IResultType ResolvedIndexerResult = IndexedExpression.ResolvedResult.Item;
            IExpressionType PreferredIndexerResult = ResolvedIndexerResult.Preferred;
            ICompiledType IndexedExpressionType;

            if (PreferredIndexerResult == null)
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }
            else
                IndexedExpressionType = PreferredIndexerResult.ValueType;

            if (IndexedExpressionType is IClassType AsClassType)
            {
                IClass IndexedBaseClass = AsClassType.BaseClass;
                ISealableDictionary<IFeatureName, IFeatureInstance> IndexedFeatureTable = IndexedBaseClass.FeatureTable;

                if (!IndexedFeatureTable.ContainsKey(FeatureName.IndexerFeatureName))
                {
                    errorList.AddError(new ErrorMissingIndexer(node));
                    return false;
                }

                IFeatureInstance IndexerInstance = IndexedFeatureTable[FeatureName.IndexerFeatureName];
                IIndexerFeature Indexer = (IndexerFeature)IndexerInstance.Feature;
                IIndexerType AsIndexerType = (IndexerType)Indexer.ResolvedAgentType.Item;

                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList))
                    return false;

                IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
                ParameterTableList.Add(AsIndexerType.ParameterTable);

                IList<ISealableList<IParameter>> ResultTableList = new List<ISealableList<IParameter>>();
                ResultTableList.Add(new SealableList<IParameter>());

                int SelectedIndex;
                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, errorList, node, out SelectedIndex))
                    return false;

                resolvedExpression.ResolvedFinalFeature = Indexer;
                resolvedExpression.ResolvedResult = new ResultType(AsIndexerType.ResolvedEntityTypeName.Item, AsIndexerType.ResolvedEntityType.Item, string.Empty);
                resolvedExpression.ResolvedException = new ResultException(AsIndexerType.GetExceptionIdentifierList);
                resolvedExpression.FeatureCall = new FeatureCall(ParameterTableList[SelectedIndex], ResultTableList[SelectedIndex], ArgumentList, MergedArgumentList, TypeArgumentStyle);

                Argument.AddConstantArguments(ArgumentList, resolvedExpression.ConstantSourceList);
            }
            else
            {
                errorList.AddError(new ErrorInvalidExpression(node));
                return false;
            }

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind { get { return ResolvedIndexer.Item.NumberKind; } }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            IExpression Expression = (IExpression)IndexedExpression;
            Expression.RestartNumberType(ref isChanged);

            foreach (IArgument Argument in ArgumentList)
                Argument.RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            IExpression Expression = (IExpression)IndexedExpression;
            Expression.CheckNumberType(ref isChanged);

            foreach (IArgument Argument in ArgumentList)
                Argument.CheckNumberType(ref isChanged);

            Debug.Assert(ResolvedIndexer.IsAssigned);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
            IExpression Expression = (IExpression)IndexedExpression;
            Expression.ValidateNumberType(errorList);

            foreach (IArgument Argument in ArgumentList)
                Argument.ValidateNumberType(errorList);
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

                IExpression Expression = (IExpression)IndexedExpression;
                string IndexedExpressionString = Expression.IsComplex ? $"({Expression.ExpressionToString})" : Expression.ExpressionToString;

                return $"{IndexedExpressionString}[{Arguments}]";
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
