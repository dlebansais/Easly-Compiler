namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    public interface IBodyRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public interface IBodyRuleTemplate<TBody> : IRuleTemplate<TBody, BodyRuleTemplate<TBody>>
        where TBody : IBody
    {
    }

    /// <summary>
    /// A rule to process <see cref="IBody"/>.
    /// </summary>
    /// <typeparam name="TBody">The body type.</typeparam>
    public class BodyRuleTemplate<TBody> : RuleTemplate<TBody, BodyRuleTemplate<TBody>>, IBodyRuleTemplate<TBody>, IBodyRuleTemplate
        where TBody : IBody
    {
        #region Init
        static BodyRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceCollectionSourceTemplate<TBody, IAssertion, ITaggedContract>(nameof(IBody.RequireList), nameof(IAssertion.ResolvedContract)),
                new OnceReferenceCollectionSourceTemplate<TBody, IAssertion, ITaggedContract>(nameof(IBody.EnsureList), nameof(IAssertion.ResolvedContract)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<TBody, IList<IAssertion>>(nameof(IBody.ResolvedRequireList)),
                new OnceReferenceDestinationTemplate<TBody, IList<IAssertion>>(nameof(IBody.ResolvedEnsureList)),
                new OnceReferenceDestinationTemplate<TBody, IList<IIdentifier>>(nameof(IBody.ResolvedExceptionIdentifierList)),
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
        public override bool CheckConsistency(TBody node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            ISealableDictionary<string, IExpression> ContractTable = new SealableDictionary<string, IExpression>();

            Success &= CheckTagConflict(ContractTable, node.RequireList);
            Success &= CheckTagConflict(ContractTable, node.EnsureList);

            if (Success)
                data = ContractTable;

            return Success;
        }

        private bool CheckTagConflict(ISealableDictionary<string, IExpression> contractTable, IList<IAssertion> assertionList)
        {
            bool Success = true;

            foreach (IAssertion Assertion in assertionList)
            {
                Debug.Assert(Assertion.ResolvedContract.IsAssigned);

                ITaggedContract Contract = Assertion.ResolvedContract.Item;
                if (Contract.HasTag)
                    if (contractTable.ContainsKey(Contract.Tag))
                    {
                        IName TagName = (IName)Assertion.Tag.Item;
                        AddSourceError(new ErrorDuplicateName(TagName, Contract.Tag));
                        Success = false;
                    }
                    else
                        contractTable.Add(Contract.Tag, Contract.Contract);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(TBody node, object data)
        {
            ISealableDictionary<string, IExpression> ContractTable = (ISealableDictionary<string, IExpression>)data;

            node.ResolvedTagTable.Merge(ContractTable);
            node.ResolvedTagTable.Seal();

            IList<IAssertion> ResolvedRequireList = new List<IAssertion>();
            foreach (IAssertion Assertion in node.RequireList)
                ResolvedRequireList.Add(Assertion);

            node.ResolvedRequireList.Item = ResolvedRequireList;

            IList<IAssertion> ResolvedEnsureList = new List<IAssertion>();
            foreach (IAssertion Assertion in node.EnsureList)
                ResolvedEnsureList.Add(Assertion);

            node.ResolvedEnsureList.Item = ResolvedEnsureList;

            IList<IIdentifier> ResolvedExceptionIdentifierList = new List<IIdentifier>();
            foreach (IIdentifier ExceptionIdentifier in node.ExceptionIdentifierList)
                ResolvedExceptionIdentifierList.Add(ExceptionIdentifier);

            node.ResolvedExceptionIdentifierList.Item = ResolvedExceptionIdentifierList;
        }
        #endregion
    }
}
