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
    public interface IInitializedObjectExpression : BaseNode.IInitializedObjectExpression, INodeWithReplicatedBlocks, IExpression, IComparableExpression
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
        OnceReference<IClassType> ResolvedClassType { get; }

        /// <summary>
        /// The list of features assigned in the resolved type.
        /// </summary>
        ISealableDictionary<string, ICompiledFeature> AssignedFeatureTable { get; }
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
                ResolvedClassTypeName = new OnceReference<ITypeName>();
                ResolvedClassType = new OnceReference<IClassType>();
                AssignedFeatureTable = new SealableDictionary<string, ICompiledFeature>();
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
                IsResolved = ExpressionConstant.IsAssigned;

                Debug.Assert(ResolvedResult.IsAssigned || !IsResolved);
                Debug.Assert(ResolvedClassTypeName.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(ResolvedClassType.IsAssigned == ResolvedResult.IsAssigned);
                Debug.Assert(AssignedFeatureTable.IsSealed == ResolvedResult.IsAssigned);

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
        /// The resolved class type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedClassTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved class type.
        /// </summary>
        public OnceReference<IClassType> ResolvedClassType { get; private set; } = new OnceReference<IClassType>();

        /// <summary>
        /// The list of features assigned in the resolved type.
        /// </summary>
        public ISealableDictionary<string, ICompiledFeature> AssignedFeatureTable { get; private set; } = new SealableDictionary<string, ICompiledFeature>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IInitializedObjectExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IInitializedObjectExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= AssignmentList.Count == other.AssignmentList.Count;

            for (int i = 0; i < AssignmentList.Count && i < other.AssignmentList.Count; i++)
            {
                IAssignmentArgument InitializationAssignment1 = AssignmentList[i];
                IAssignmentArgument InitializationAssignment2 = other.AssignmentList[i];

                Result &= AssignmentArgument.IsAssignmentArgumentEqual(InitializationAssignment1, InitializationAssignment2);
            }

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IInitializedObjectExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="initializedObjectTypeName">The initialized object type name upon return.</param>
        /// <param name="initializedObjectType">The initialized object type upon return.</param>
        /// <param name="assignedFeatureTable">The table of assigned values upon return.</param>
        public static bool ResolveCompilerReferences(IInitializedObjectExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ITypeName initializedObjectTypeName, out IClassType initializedObjectType, out ISealableDictionary<string, ICompiledFeature> assignedFeatureTable)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            initializedObjectTypeName = null;
            initializedObjectType = null;
            assignedFeatureTable = new SealableDictionary<string, ICompiledFeature>();

            IIdentifier ClassIdentifier = (IIdentifier)node.ClassIdentifier;
            IList<IAssignmentArgument> AssignmentList = node.AssignmentList;
            IClass EmbeddingClass = node.EmbeddingClass;
            string ValidText = ClassIdentifier.ValidText.Item;

            if (!EmbeddingClass.ImportedClassTable.ContainsKey(ValidText))
            {
                errorList.AddError(new ErrorUnknownIdentifier(ClassIdentifier, ValidText));
                return false;
            }

            IClass BaseClass = EmbeddingClass.ImportedClassTable[ValidText].Item;

            Debug.Assert(BaseClass.ResolvedClassTypeName.IsAssigned);
            Debug.Assert(BaseClass.ResolvedClassType.IsAssigned);
            initializedObjectTypeName = BaseClass.ResolvedClassTypeName.Item;
            initializedObjectType = BaseClass.ResolvedClassType.Item;

            if (!CheckAssignemntList(node, errorList, BaseClass.FeatureTable, constantSourceList, assignedFeatureTable))
                return false;

            resolvedResult = new ResultType(initializedObjectTypeName, initializedObjectType, string.Empty);

            if (!ResolveObjectInitialization(node, errorList, assignedFeatureTable))
                return false;

            resolvedException = new ResultException();

            return true;
        }

        private static bool CheckAssignemntList(IInitializedObjectExpression node, IErrorList errorList, ISealableDictionary<IFeatureName, IFeatureInstance> featureTable, ISealableList<IExpression> constantSourceList, ISealableDictionary<string, ICompiledFeature> assignedFeatureTable)
        {
            bool Success = true;
            IList<IAssignmentArgument> AssignmentList = node.AssignmentList;

            foreach (IAssignmentArgument AssignmentItem in AssignmentList)
            {
                constantSourceList.Add((IExpression)AssignmentItem.Source);

                IResultType ExpressionResult = AssignmentItem.ResolvedResult.Item;
                if (ExpressionResult.Count < AssignmentItem.ParameterList.Count)
                {
                    errorList.AddError(new ErrorInvalidInstruction(AssignmentItem));
                    Success = false;
                }

                foreach (IIdentifier IdentifierItem in AssignmentItem.ParameterList)
                    Success &= CheckAssignemntIdentifier(errorList, featureTable, assignedFeatureTable, IdentifierItem);
            }

            return Success;
        }

        private static bool CheckAssignemntIdentifier(IErrorList errorList, ISealableDictionary<IFeatureName, IFeatureInstance> featureTable, ISealableDictionary<string, ICompiledFeature> assignedFeatureTable, IIdentifier identifierItem)
        {
            bool Success = true;
            string ValidIdentifierText = identifierItem.ValidText.Item;

            if (assignedFeatureTable.ContainsKey(ValidIdentifierText))
            {
                errorList.AddError(new ErrorIdentifierAlreadyListed(identifierItem, ValidIdentifierText));
                Success = false;
            }
            else
            {
                if (FeatureName.TableContain(featureTable, ValidIdentifierText, out IFeatureName Key, out IFeatureInstance FeatureItem))
                {
                    bool ValidFeature = false;

                    if (FeatureItem.Feature is AttributeFeature AsAttributeFeature)
                        ValidFeature = true;
                    else if (FeatureItem.Feature is IPropertyFeature AsPropertyFeature)
                    {
                        bool IsHandled = false;
                        switch (AsPropertyFeature.PropertyKind)
                        {
                            case BaseNode.UtilityType.ReadOnly:
                                ValidFeature = !AsPropertyFeature.GetterBody.IsAssigned;
                                IsHandled = true;
                                break;

                            case BaseNode.UtilityType.ReadWrite:
                                ValidFeature = !(AsPropertyFeature.GetterBody.IsAssigned && !AsPropertyFeature.SetterBody.IsAssigned);
                                IsHandled = true;
                                break;

                            case BaseNode.UtilityType.WriteOnly:
                                ValidFeature = true;
                                IsHandled = true;
                                break;
                        }

                        Debug.Assert(IsHandled);
                    }

                    if (ValidFeature)
                        assignedFeatureTable.Add(ValidIdentifierText, FeatureItem.Feature);
                    else
                    {
                        errorList.AddError(new ErrorAttributeOrPropertyRequired(identifierItem, ValidIdentifierText));
                        Success = false;
                    }
                }
                else
                {
                    errorList.AddError(new ErrorUnknownIdentifier(identifierItem, ValidIdentifierText));
                    Success = false;
                }
            }

            return Success;
        }

        private static bool ResolveObjectInitialization(IInitializedObjectExpression node, IErrorList errorList, ISealableDictionary<string, ICompiledFeature> assignedFeatureTable)
        {
            IList<IAssignmentArgument> AssignmentList = node.AssignmentList;

            foreach (IAssignmentArgument AssignmentItem in AssignmentList)
            {
                IResultType ExpressionResult = AssignmentItem.ResolvedResult.Item;

                for (int i = 0; i < AssignmentItem.ParameterList.Count; i++)
                {
                    IIdentifier IdentifierItem = (IIdentifier)AssignmentItem.ParameterList[i];
                    string ValidIdentifierText = IdentifierItem.ValidText.Item;
                    ICompiledFeature TargetFeature = assignedFeatureTable[ValidIdentifierText];

                    ICompiledType SourceType = ExpressionResult.At(i).ValueType;
                    ICompiledType DestinationType = null;

                    if (TargetFeature is IAttributeFeature AsAttributeFeature)
                        DestinationType = AsAttributeFeature.ResolvedEntityType.Item;
                    else if (TargetFeature is IPropertyFeature AsPropertyFeature)
                        DestinationType = AsPropertyFeature.ResolvedEntityType.Item;

                    Debug.Assert(DestinationType != null);

                    if (!ObjectType.TypeConformToBase(SourceType, DestinationType, errorList, IdentifierItem, isConversionAllowed: true))
                    {
                        errorList.AddError(new ErrorAssignmentMismatch(IdentifierItem));
                        return false;
                    }
                }
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
