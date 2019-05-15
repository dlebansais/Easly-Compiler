namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BaseNodeHelper;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllEntitiesWithDefaultRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllEntitiesWithDefaultRuleTemplate : RuleTemplate<IClass, AllEntitiesWithDefaultRuleTemplate>, IAllEntitiesWithDefaultRuleTemplate
    {
        #region Init
        static AllEntitiesWithDefaultRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<IClass, IExpression, ILanguageConstant>(nameof(IClass.NodeWithDefaultList), nameof(IExpression.ExpressionConstant)),
                new OnceReferenceCollectionSourceTemplate<IClass, IExpression, ILanguageConstant>(nameof(IClass.NodeWithNumberConstantList), nameof(IExpression.ExpressionConstant)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClass, IList<IExpression>>(nameof(IClass.ResolvedNodeWithDefaultList)),
                new OnceReferenceDestinationTemplate<IClass, IList<IExpression>>(nameof(IClass.ResolvedNodeWithNumberConstantList)),
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
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            foreach (IExpression ExpressionItem in node.NodeWithDefaultList)
            {
                IList<IExpressionType> ResolvedResult = ExpressionItem.ResolvedResult.Item;
                if (ResolvedResult.Count != 1)
                {
                    ErrorList.AddError(new ErrorInvalidExpression(ExpressionItem));
                    Success = false;
                }
                else if (!ExpressionItem.ExpressionConstant.IsAssigned) // TODO: verify implementations
                {
                    ErrorList.AddError(new ErrorInvalidExpression(ExpressionItem));
                    Success = false;
                }
                else if (node.NodeWithNumberConstantList.Contains(ExpressionItem) && !(ExpressionItem.ExpressionConstant.Item is INumberLanguageConstant))
                {
                    ErrorList.AddError(new ErrorNumberConstantExpected(ExpressionItem));
                    Success = false;
                }
            }

            if (Success)
            {
                Dictionary<ICanonicalNumber, IDiscrete> CombinedDiscreteNumericValueList = new Dictionary<ICanonicalNumber, IDiscrete>();

                foreach (KeyValuePair<IFeatureName, IDiscrete> Entry in node.DiscreteTable)
                {
                    IDiscrete DiscreteItem = Entry.Value;

                    if (DiscreteItem.NumericValue.IsAssigned)
                    {
                        ICanonicalNumber NumberConstant = null;
                        IDiscrete CurrentDiscreteItem = DiscreteItem;
                        bool IsFound = false;

                        while (Success && !IsFound)
                        {
                            Debug.Assert(CurrentDiscreteItem.NumericValue.IsAssigned);
                            IExpression NumericValue = (IExpression)CurrentDiscreteItem.NumericValue.Item;

                            Debug.Assert(NumericValue.ExpressionConstant.IsAssigned);
                            ILanguageConstant ExpressionConstant = NumericValue.ExpressionConstant.Item;

                            bool IsHandled = false;

                            switch (ExpressionConstant)
                            {
                                case INumberLanguageConstant AsNumberLanguageConstant:
                                    NumberConstant = AsNumberLanguageConstant.Value;
                                    IsHandled = true;
                                    IsFound = true;
                                    break;

                                case IDiscreteLanguageConstant AsDiscreteLanguageConstant:
                                    CurrentDiscreteItem = AsDiscreteLanguageConstant.Discrete;
                                    if (!CurrentDiscreteItem.NumericValue.IsAssigned)
                                    {
                                        AddSourceError(new ErrorInvalidExpression(NumericValue));
                                        Success = false;
                                    }

                                    IsHandled = true;
                                    break;
                            }

                            Debug.Assert(IsHandled);
                        }

                        Debug.Assert(NumberConstant != null);

                        CombinedDiscreteNumericValueList.Add(NumberConstant, CurrentDiscreteItem);
                    }
                }

                IList<ICanonicalNumber> ErroneousConstantList = new List<ICanonicalNumber>();

                foreach (KeyValuePair<ICanonicalNumber, IDiscrete> Entry1 in CombinedDiscreteNumericValueList)
                {
                    foreach (KeyValuePair<ICanonicalNumber, IDiscrete> Entry2 in CombinedDiscreteNumericValueList)
                    {
                        ICanonicalNumber Number1 = Entry1.Key;
                        ICanonicalNumber Number2 = Entry2.Key;

                        if (Number1 != Number2 && !ErroneousConstantList.Contains(Number1) && !ErroneousConstantList.Contains(Number2) && Number1.IsEqual(Number2))
                        {
                            IDiscrete Discrete1 = Entry1.Value;
                            IDiscrete Discrete2 = Entry1.Value;

                            int Index1 = node.DiscreteList.IndexOf(Discrete1);
                            int Index2 = node.DiscreteList.IndexOf(Discrete2);

                            ISource MostAccurateSource;
                            if (Index1 < 0 && Index2 < 0)
                                MostAccurateSource = node;
                            else if (Index2 >= 0)
                                MostAccurateSource = Discrete2;
                            else
                                MostAccurateSource = Discrete1;

                            // Prevents having the same error twice.
                            ErroneousConstantList.Add(Number1);
                            ErroneousConstantList.Add(Number2);

                            AddSourceError(new ErrorMultipleIdenticalDiscrete(MostAccurateSource, Number1));
                            Success = false;
                        }
                    }
                }
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            node.ResolvedNodeWithDefaultList.Item = node.NodeWithDefaultList;
            node.ResolvedNodeWithNumberConstantList.Item = node.NodeWithNumberConstantList;
        }
        #endregion
    }
}
