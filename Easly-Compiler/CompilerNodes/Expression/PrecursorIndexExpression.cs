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
    public interface IPrecursorIndexExpression : IExpression, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Gets or sets the type where to get the precursor from.
        /// </summary>
        IOptionalReference<BaseNode.ObjectType> AncestorType { get; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        BaseNode.IBlockList<BaseNode.Argument> ArgumentBlocks { get; }

        /// <summary>
        /// Replicated list from <see cref="BaseNode.PrecursorIndexExpression.ArgumentBlocks"/>.
        /// </summary>
        IList<IArgument> ArgumentList { get; }

        /// <summary>
        /// The resolved precursor.
        /// </summary>
        OnceReference<IFeatureInstance> ResolvedPrecursor { get; }

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
        public void FillReplicatedList(string propertyName, List<BaseNode.Node> nodeList)
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

            foreach (BaseNode.Node Node in nodeList)
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
                ResolvedPrecursor = new OnceReference<IFeatureInstance>();
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
                Debug.Assert(ResolvedPrecursor.IsAssigned || !IsResolved);

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
        /// The resolved precursor.
        /// </summary>
        public OnceReference<IFeatureInstance> ResolvedPrecursor { get; private set; } = new OnceReference<IFeatureInstance>();

        /// <summary>
        /// The resolved indexer.
        /// </summary>
        public OnceReference<IIndexerFeature> ResolvedIndexer { get; private set; } = new OnceReference<IIndexerFeature>();

        /// <summary>
        /// Details of the feature call.
        /// </summary>
        public OnceReference<IFeatureCall> FeatureCall { get; private set; } = new OnceReference<IFeatureCall>();

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

        /// <summary>
        /// Finds the matching nodes of a <see cref="IPrecursorIndexExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedExpression">The result of the search.</param>
        public static bool ResolveCompilerReferences(IPrecursorIndexExpression node, IErrorList errorList, out ResolvedExpression resolvedExpression)
        {
            resolvedExpression = new ResolvedExpression();

            IOptionalReference<BaseNode.ObjectType> AncestorType = node.AncestorType;
            IList<IArgument> ArgumentList = node.ArgumentList;
            IClass EmbeddingClass = node.EmbeddingClass;

            ISealableDictionary<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = EmbeddingClass.FeatureTable;
            IFeature InnerFeature = node.EmbeddingFeature;

            if (InnerFeature is IIndexerFeature AsIndexerFeature)
            {
                IFeatureInstance Instance = FeatureTable[FeatureName.IndexerFeatureName];

                if (!Instance.FindPrecursor(node.AncestorType, errorList, node, out IFeatureInstance SelectedPrecursor))
                    return false;

                resolvedExpression.SelectedPrecursor = SelectedPrecursor;

                if (!ResolveSelectedPrecursor(node, SelectedPrecursor, errorList, ref resolvedExpression))
                    return false;
            }
            else
            {
                errorList.AddError(new ErrorIndexPrecursorNotAllowedOutsideIndexer(node));
                return false;
            }

#if COVERAGE
            Debug.Assert(!node.IsComplex);
#endif

            return true;
        }

        private static bool ResolveSelectedPrecursor(IPrecursorIndexExpression node, IFeatureInstance selectedPrecursor, IErrorList errorList, ref ResolvedExpression resolvedExpression)
        {
            IList<IArgument> ArgumentList = node.ArgumentList;

            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, errorList))
                return false;

            IIndexerFeature OperatorFeature = selectedPrecursor.Feature as IIndexerFeature;
            Debug.Assert(OperatorFeature != null);
            IIndexerType OperatorType = OperatorFeature.ResolvedAgentType.Item as IIndexerType;
            Debug.Assert(OperatorType != null);

            IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
            ParameterTableList.Add(OperatorType.ParameterTable);

            IList<ISealableList<IParameter>> ResultTableList = new List<ISealableList<IParameter>>();
            ResultTableList.Add(new SealableList<IParameter>());

            int SelectedIndex;
            if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, errorList, node, out SelectedIndex))
                return false;

            resolvedExpression.ResolvedResult = new ResultType(OperatorType.ResolvedEntityTypeName.Item, OperatorType.ResolvedEntityType.Item, string.Empty);

            resolvedExpression.ResolvedException = new ResultException(OperatorType.GetExceptionIdentifierList);
            resolvedExpression.FeatureCall = new FeatureCall(ParameterTableList[SelectedIndex], ResultTableList[SelectedIndex], ArgumentList, MergedArgumentList, TypeArgumentStyle);
            resolvedExpression.ResolvedFinalFeature = OperatorFeature;

            Argument.AddConstantArguments(ArgumentList, resolvedExpression.ConstantSourceList);

            return true;
        }
        #endregion

        #region Numbers
        /// <summary>
        /// The number kind if the constant type is a number.
        /// </summary>
        public NumberKinds NumberKind { get { return ResolvedResult.Item.NumberKind; } }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
            foreach (IArgument Argument in ArgumentList)
                Argument.RestartNumberType(ref isChanged);
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
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
