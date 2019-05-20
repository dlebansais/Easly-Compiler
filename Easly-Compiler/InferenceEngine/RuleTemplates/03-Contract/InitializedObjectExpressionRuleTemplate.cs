namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public interface IInitializedObjectExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IInitializedObjectExpression"/>.
    /// </summary>
    public class InitializedObjectExpressionRuleTemplate : RuleTemplate<IInitializedObjectExpression, InitializedObjectExpressionRuleTemplate>, IInitializedObjectExpressionRuleTemplate
    {
        #region Init
        static InitializedObjectExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IInitializedObjectExpression, IAssignmentArgument, IList<IExpressionType>>(nameof(IInitializedObjectExpression.AssignmentList), nameof(IAssignmentArgument.ResolvedResult)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IInitializedObjectExpression, IList<IExpressionType>>(nameof(IInitializedObjectExpression.ResolvedResult)),
                new OnceReferenceDestinationTemplate<IInitializedObjectExpression, IList<IIdentifier>>(nameof(IInitializedObjectExpression.ResolvedExceptions)),
                new UnsealedListDestinationTemplate<IInitializedObjectExpression, IExpression>(nameof(IInitializedObjectExpression.ConstantSourceList)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IInitializedObjectExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= InitializedObjectExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ITypeName InitializedObjectTypeName, out ICompiledType InitializedObjectType, out IHashtableEx<string, ICompiledFeature> AssignedFeatureTable);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, InitializedObjectTypeName, InitializedObjectType, AssignedFeatureTable);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IInitializedObjectExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="initializedObjectTypeName">The initialized object type name upon return.</param>
        /// <param name="initializedObjectType">The initialized object type upon return.</param>
        /// <param name="assignedFeatureTable">The table of assigned values upon return.</param>
        public static bool ResolveCompilerReferences(IInitializedObjectExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ITypeName initializedObjectTypeName, out ICompiledType initializedObjectType, out IHashtableEx<string, ICompiledFeature> assignedFeatureTable)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            initializedObjectTypeName = null;
            initializedObjectType = null;
            assignedFeatureTable = new HashtableEx<string, ICompiledFeature>();

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

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(initializedObjectTypeName, initializedObjectType, string.Empty)
            };

            if (!ResolveObjectInitialization(node, errorList, assignedFeatureTable))
                return false;

            resolvedExceptions = new List<IIdentifier>();

            return true;
        }

        private static bool CheckAssignemntList(IInitializedObjectExpression node, IErrorList errorList, IHashtableEx<IFeatureName, IFeatureInstance> featureTable, ListTableEx<IExpression> constantSourceList, IHashtableEx<string, ICompiledFeature> assignedFeatureTable)
        {
            bool Success = true;
            IList<IAssignmentArgument> AssignmentList = node.AssignmentList;

            foreach (IAssignmentArgument AssignmentItem in AssignmentList)
            {
                constantSourceList.Add((IExpression)AssignmentItem.Source);

                IList<IExpressionType> ExpressionResult = AssignmentItem.ResolvedResult.Item;
                if (ExpressionResult.Count < AssignmentItem.ParameterList.Count)
                {
                    errorList.AddError(new ErrorInvalidInstruction(AssignmentItem));
                    Success = false;
                }

                foreach (IIdentifier IdentifierItem in AssignmentItem.ParameterList)
                {
                    string ValidIdentifierText = IdentifierItem.ValidText.Item;

                    if (assignedFeatureTable.ContainsKey(ValidIdentifierText))
                    {
                        errorList.AddError(new ErrorIdentifierAlreadyListed(IdentifierItem, ValidIdentifierText));
                        Success = false;
                    }
                    else
                    {
                        if (FeatureName.TableContain(featureTable, ValidIdentifierText, out IFeatureName Key, out IFeatureInstance FeatureItem))
                        {
                            bool ValidFeature = false;

                            if (FeatureItem.Feature.Item is AttributeFeature AsAttributeFeature)
                                ValidFeature = true;
                            else if (FeatureItem.Feature.Item is IPropertyFeature AsPropertyFeature)
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
                                assignedFeatureTable.Add(ValidIdentifierText, FeatureItem.Feature.Item);
                            else
                            {
                                errorList.AddError(new ErrorAttributeOrPropertyRequired(IdentifierItem, ValidIdentifierText));
                                Success = false;
                            }
                        }
                        else
                        {
                            errorList.AddError(new ErrorUnknownIdentifier(IdentifierItem, ValidIdentifierText));
                            Success = false;
                        }
                    }
                }
            }

            return Success;
        }

        private static bool ResolveObjectInitialization(IInitializedObjectExpression node, IErrorList errorList, IHashtableEx<string, ICompiledFeature> assignedFeatureTable)
        {
            IList<IAssignmentArgument> AssignmentList = node.AssignmentList;

            foreach (IAssignmentArgument AssignmentItem in AssignmentList)
            {
                IList<IExpressionType> ExpressionResult = AssignmentItem.ResolvedResult.Item;

                for (int i = 0; i < AssignmentItem.ParameterList.Count; i++)
                {
                    IIdentifier IdentifierItem = (IIdentifier)AssignmentItem.ParameterList[i];
                    string ValidIdentifierText = IdentifierItem.ValidText.Item;
                    ICompiledFeature TargetFeature = assignedFeatureTable[ValidIdentifierText];

                    ICompiledType SourceType = ExpressionResult[i].ValueType;
                    ICompiledType DestinationType = null;

                    if (TargetFeature is IAttributeFeature AsAttributeFeature)
                        DestinationType = AsAttributeFeature.ResolvedEntityType.Item;
                    else if (TargetFeature is IPropertyFeature AsPropertyFeature)
                        DestinationType = AsPropertyFeature.ResolvedEntityType.Item;

                    Debug.Assert(DestinationType != null);

                    IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();
                    if (!ObjectType.TypeConformToBase(SourceType, DestinationType, SubstitutionTypeTable, errorList, IdentifierItem))
                    {
                        errorList.AddError(new ErrorAssignmentMismatch(IdentifierItem));
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IInitializedObjectExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item4;
            ITypeName InitializedObjectTypeName = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item5;
            ICompiledType InitializedObjectType = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item6;
            IHashtableEx<string, ICompiledFeature> AssignedFeatureTable = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>>)data).Item7;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();

            node.ResolvedClassTypeName.Item = InitializedObjectTypeName;
            node.ResolvedClassType.Item = InitializedObjectType;

            Debug.Assert(node.AssignedFeatureTable.Count == 0);
            node.AssignedFeatureTable.Merge(AssignedFeatureTable);
            node.AssignedFeatureTable.Seal();
        }
        #endregion
    }
}
