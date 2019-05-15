namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="INewExpression"/>.
    /// </summary>
    public interface INewExpressionRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="INewExpression"/>.
    /// </summary>
    public class NewExpressionRuleTemplate : RuleTemplate<INewExpression, NewExpressionRuleTemplate>, INewExpressionRuleTemplate
    {
        #region Init
        static NewExpressionRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<INewExpression, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateScopeStart<INewExpression>.Default),
                new OnceReferenceTableSourceTemplate<INewExpression, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateScopeStart<INewExpression>.Default),
                new OnceReferenceSourceTemplate<INewExpression, IList<IExpressionType>>(nameof(INewExpression.Object) + Dot + nameof(IQualifiedName.ValidResultTypePath)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<INewExpression, IList<IExpressionType>>(nameof(INewExpression.ResolvedResult)),
                new UnsealedListDestinationTemplate<INewExpression, IExpression>(nameof(INewExpression.ConstantSourceList)),
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
        public override bool CheckConsistency(INewExpression node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            Success &= NewExpressionRuleTemplate.ResolveCompilerReferences(node, ErrorList, out IList<IExpressionType> ResolvedResult, out IList<IIdentifier> ResolvedExceptions, out ListTableEx<IExpression> ConstantSourceList, out ILanguageConstant ExpressionConstant, out ICompiledFeature ResolvedFinalFeature);

            if (Success)
                data = new Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>(ResolvedResult, ResolvedExceptions, ConstantSourceList, ExpressionConstant, ResolvedFinalFeature);

            return Success;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="INewExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedExceptions">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The matching feature upon return.</param>
        public static bool ResolveCompilerReferences(INewExpression node, IErrorList errorList, out IList<IExpressionType> resolvedResult, out IList<IIdentifier> resolvedExceptions, out ListTableEx<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature)
        {
            resolvedResult = null;
            resolvedExceptions = null;
            constantSourceList = new ListTableEx<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;

            IClass EmbeddingClass = node.EmbeddingClass;
            IQualifiedName Object = (IQualifiedName)node.Object;
            IList<IIdentifier> ValidPath = Object.ValidPath.Item;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Boolean.Guid, node, out ITypeName BooleanTypeName, out ICompiledType BooleanType))
            {
                errorList.AddError(new ErrorBooleanTypeMissing(node));
                return false;
            }

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            if (FinalFeature == null)
            {
                errorList.AddError(new ErrorConstantNewExpression(node));
                return false;
            }

            ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Object.ValidResultTypePath.Item);

            resolvedResult = new List<IExpressionType>()
            {
                new ExpressionType(BooleanTypeName, BooleanType, string.Empty)
            };

            resolvedExceptions = new List<IIdentifier>();
            resolvedFinalFeature = FinalFeature;

            return true;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(INewExpression node, object data)
        {
            IList<IExpressionType> ResolvedResult = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item1;
            IList<IIdentifier> ResolvedExceptions = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item2;
            ListTableEx<IExpression> ConstantSourceList = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item3;
            ILanguageConstant ExpressionConstant = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item4;
            ICompiledFeature ResolvedFinalFeature = ((Tuple<IList<IExpressionType>, IList<IIdentifier>, ListTableEx<IExpression>, ILanguageConstant, ICompiledFeature>)data).Item5;

            node.ResolvedResult.Item = ResolvedResult;
            node.ResolvedExceptions.Item = ResolvedExceptions;
            node.ConstantSourceList.AddRange(ConstantSourceList);
            node.ConstantSourceList.Seal();
            node.ExpressionConstant.Item = ExpressionConstant;
        }
        #endregion
    }
}
