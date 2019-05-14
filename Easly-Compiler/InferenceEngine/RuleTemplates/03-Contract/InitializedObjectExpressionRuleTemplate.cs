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

            Success &= InitializedObjectExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out ITypeName InitializedObjectTypeName, out ICompiledType InitializedObjectType, out IHashtableEx<string, ICompiledFeature> AssignedFeatureTable, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions);

            if (Success)
                data = new Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>(InitializedObjectTypeName, InitializedObjectType, AssignedFeatureTable, ResolvedResult, ResolvedExceptions);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IInitializedObjectExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="initializedObjectTypeName">The initialized object type name upon return.</param>
        /// <param name="initializedObjectType">The initialized object type upon return.</param>
        /// <param name="assignedFeatureTable">The table of assigned values upon return.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        public static bool ResolveCompilerReferences(IInitializedObjectExpression node, IErrorList errorList, out ITypeName initializedObjectTypeName, out ICompiledType initializedObjectType, out IHashtableEx<string, ICompiledFeature> assignedFeatureTable, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions)
        {
            initializedObjectTypeName = null;
            initializedObjectType = null;
            assignedFeatureTable = null;
            resolvedResult = null;
            resolvedExceptions = null;

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
            initializedObjectTypeName = BaseClass.ResolvedClassTypeName.Item;
            initializedObjectType = BaseClass.ResolvedClassType.Item;

            IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable = BaseClass.FeatureTable;

            int ExpressionErrorCount = 0;
            foreach (IAssignmentArgument AssignmentItem in AssignmentList)
            {
                if (!AssignmentItem.IsConstant)
                {
                    errorList.AddError(new ErrorConstantExpected(AssignmentItem));
                    ExpressionErrorCount++;
                }

                IList<IExpressionType> ExpressionResult = AssignmentItem.ResolvedResult.Item;
                if (ExpressionResult.Count < AssignmentItem.ParameterList.Count)
                {
                    errorList.AddError(new ErrorInvalidInstruction(AssignmentItem));
                    ExpressionErrorCount++;
                }

                foreach (IIdentifier IdentifierItem in AssignmentItem.ParameterList)
                {
                    string ValidIdentifierText = IdentifierItem.ValidText.Item;

                    if (assignedFeatureTable.ContainsKey(ValidIdentifierText))
                    {
                        errorList.AddError(new ErrorIdentifierAlreadyListed(IdentifierItem, ValidIdentifierText));
                        ExpressionErrorCount++;
                    }
                    else
                    {
                        if (FeatureName.TableContain(FeatureTable, ValidIdentifierText, out IFeatureName Key, out IFeatureInstance FeatureItem))
                        {
                            bool ValidFeature;

                            if (FeatureItem.Feature.Item is AttributeFeature AsAttributeFeature)
                                ValidFeature = true;

                            else if (FeatureItem.Feature.Item is IPropertyFeature AsPropertyFeature)
                            {
                                switch (AsPropertyFeature.PropertyKind)
                                {
                                    case BaseNode.UtilityType.ReadOnly:
                                        if (AsPropertyFeature.GetterBody.IsAssigned)
                                            ValidFeature = false;
                                        else
                                            ValidFeature = true;
                                        break;

                                    case BaseNode.UtilityType.ReadWrite:
                                        if (AsPropertyFeature.GetterBody.IsAssigned && !AsPropertyFeature.SetterBody.IsAssigned)
                                            ValidFeature = false;
                                        else
                                            ValidFeature = true;
                                        break;

                                    case BaseNode.UtilityType.WriteOnly:
                                        ValidFeature = true;
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                                ValidFeature = false;

                            if (ValidFeature)
                                assignedFeatureTable.Add(ValidIdentifierText, FeatureItem.Feature.Item);
                            else
                            {
                                errorList.AddError(new ErrorAttributeOrPropertyRequired(IdentifierItem, ValidIdentifierText));
                                ExpressionErrorCount++;
                            }

                        }
                        else
                        {
                            errorList.AddError(new ErrorUnknownIdentifier(IdentifierItem, ValidIdentifierText));
                            ExpressionErrorCount++;
                        }
                    }
                }
            }

            if (ExpressionErrorCount > 0)
                return false;

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(initializedObjectTypeName, initializedObjectType, string.Empty)
            };

            if (node.ResolvedClassType.IsAssigned)
            {
                if (node.ResolvedClassType.Item is IClassType AsClassType)
                {
                    IClass InitializedClass = AsClassType.BaseClass;

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
                }
                else
                {
                    //TODO: tuples
                }

                resolvedExceptions = new List<IIdentifier>();
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
            ITypeName InitializedObjectTypeName = ((Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>)data).Item1;
            ICompiledType InitializedObjectType = ((Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>)data).Item2;
            IHashtableEx<string, ICompiledFeature> AssignedFeatureTable = ((Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>)data).Item3;
            IList<IExpressionType> ResolvedResult = ((Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>)data).Item4;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<ITypeName, ICompiledType, IHashtableEx<string, ICompiledFeature>, IList<IExpressionType>, IList<IIdentifier>>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedClassTypeName.Item = InitializedObjectTypeName;
            node.ResolvedClassType.Item = InitializedObjectType;

            Debug.Assert(node.AssignedFeatureTable.Count == 0);
            node.AssignedFeatureTable.Merge(AssignedFeatureTable);
            node.AssignedFeatureTable.Seal();
        }
        #endregion
    }
}
