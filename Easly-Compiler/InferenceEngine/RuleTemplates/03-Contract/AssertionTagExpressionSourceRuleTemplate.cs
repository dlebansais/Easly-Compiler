namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public interface IAssertionTagExpressionSourceRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IAssertionTagExpression"/>.
    /// </summary>
    public class AssertionTagExpressionSourceRuleTemplate : RuleTemplate<IAssertionTagExpression, AssertionTagExpressionSourceRuleTemplate>, IAssertionTagExpressionSourceRuleTemplate
    {
        #region Init
        static AssertionTagExpressionSourceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IAssertionTagExpression, IClassType, IList<IBody>>(nameof(IClass.InheritedBodyTagListTable), TemplateClassStart<IAssertionTagExpression>.Default),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IAssertionTagExpression, IExpression>(nameof(IAssertionTagExpression.ResolvedBooleanExpression)),
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
        public override bool CheckConsistency(IAssertionTagExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IClass EmbeddingClass = node.EmbeddingClass;

            Debug.Assert(node.ResolvedAssertion.IsAssigned);
            IAssertion InnerAssertion = node.ResolvedAssertion.Item;

            IList<IAssertion> MatchingAssertionList = new List<IAssertion>();
            IIdentifier TagIdentifier = (IIdentifier)node.TagIdentifier;
            string ExpectedTag = TagIdentifier.ValidText.Item;
            IFeature Feature = node.EmbeddingFeature;
            IFeatureName FeatureName = Feature.ValidFeatureName.Item;

            Debug.Assert(EmbeddingClass.FeatureTable.ContainsKey(FeatureName));
            IFeatureInstance FeatureInstance = EmbeddingClass.FeatureTable[FeatureName];
            if (FeatureInstance.OriginalPrecursor.IsAssigned)
                FeatureInstance = FeatureInstance.OriginalPrecursor.Item.Precursor;

            IList<IPrecursorInstance> PrecursorList = FeatureInstance.PrecursorList;

            foreach (KeyValuePair<IClassType, IList<IBody>> Entry in EmbeddingClass.InheritedBodyTagListTable)
            {
                IClass InheritedClass = Entry.Key.BaseClass;

                foreach (IBody InheritedBody in Entry.Value)
                {
                    IFeature InheritedFeature = InheritedBody.EmbeddingFeature;
                    IFeatureName InheritedFeatureName = InheritedFeature.ValidFeatureName.Item;

                    Debug.Assert(InheritedClass.FeatureTable.ContainsKey(InheritedFeatureName));
                    IFeatureInstance InheritedFeatureInstance = InheritedClass.FeatureTable[InheritedFeatureName];

                    foreach (IPrecursorInstance PrecursorInstance in PrecursorList)
                        if (PrecursorInstance.Precursor == InheritedFeatureInstance)
                        {
                            FindMatchingAssertions(InheritedBody.RequireList, InnerAssertion, ExpectedTag, MatchingAssertionList);
                            FindMatchingAssertions(InheritedBody.EnsureList, InnerAssertion, ExpectedTag, MatchingAssertionList);
                        }
                }
            }

            if (InnerAssertion.EmbeddingBody != null)
            {
                IBody ResolvedBody = InnerAssertion.EmbeddingBody;
                Debug.Assert(ResolvedBody.RequireList.Count > 0 || ResolvedBody.EnsureList.Count > 0);

                FindMatchingAssertions(ResolvedBody.RequireList, InnerAssertion, ExpectedTag, MatchingAssertionList);
                FindMatchingAssertions(ResolvedBody.EnsureList, InnerAssertion, ExpectedTag, MatchingAssertionList);
            }
            else
            {
                Debug.Assert(EmbeddingClass.InvariantList.Count > 0);
                FindMatchingAssertions(EmbeddingClass.InvariantList, InnerAssertion, ExpectedTag, MatchingAssertionList);
            }

            if (MatchingAssertionList.Count == 0)
            {
                AddSourceError(new ErrorInvalidExpression(node));
                Success = false;
            }
            else if (MatchingAssertionList.Count > 1)
            {
                AddSourceError(new ErrorInvalidExpression(node));
                Success = false;
            }
            else
            {
                Debug.Assert(MatchingAssertionList.Count == 1);
                IAssertion MatchingAssertion = MatchingAssertionList[0];

                data = MatchingAssertion.BooleanExpression;
            }

            return Success;
        }

        private void FindMatchingAssertions(IList<IAssertion> assertionList, IAssertion excludedAssertion, string expectedTag, IList<IAssertion> matchinggAssertionList)
        {
            foreach (IAssertion Assertion in assertionList)
                if (Assertion != excludedAssertion && Assertion.Tag.IsAssigned)
                {
                    IName TagName = (IName)Assertion.Tag.Item;
                    string Tag = TagName.ValidText.Item;

                    if (Tag == expectedTag)
                        matchinggAssertionList.Add(Assertion);
                }
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IAssertionTagExpression node, object data)
        {
            IExpression ResolvedBooleanExpression = (IExpression)data;

            node.ResolvedBooleanExpression.Item = ResolvedBooleanExpression;
        }
        #endregion
    }
}
