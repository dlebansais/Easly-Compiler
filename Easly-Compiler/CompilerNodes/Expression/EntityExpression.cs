namespace CompilerNode
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IEntityExpression.
    /// </summary>
    public interface IEntityExpression : IExpression, IComparableExpression
    {
        /// <summary>
        /// Gets or sets the feature to get the entity from.
        /// </summary>
        BaseNode.QualifiedName Query { get; }

        /// <summary>
        /// The resolved feature.
        /// </summary>
        OnceReference<ICompiledFeature> ResolvedFinalFeature { get; }

        /// <summary>
        /// The resolved discrete.
        /// </summary>
        OnceReference<IDiscrete> ResolvedFinalDiscrete { get; }
    }

    /// <summary>
    /// Compiler IEntityExpression.
    /// </summary>
    public class EntityExpression : BaseNode.EntityExpression, IEntityExpression
    {
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
                ResolvedFinalFeature = new OnceReference<ICompiledFeature>();
                ResolvedFinalDiscrete = new OnceReference<IDiscrete>();
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
                Debug.Assert(ResolvedFinalFeature.IsAssigned || ResolvedFinalDiscrete.IsAssigned || !IsResolved);
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
        /// The resolved feature.
        /// </summary>
        public OnceReference<ICompiledFeature> ResolvedFinalFeature { get; private set; } = new OnceReference<ICompiledFeature>();

        /// <summary>
        /// The resolved discrete.
        /// </summary>
        public OnceReference<IDiscrete> ResolvedFinalDiscrete { get; private set; } = new OnceReference<IDiscrete>();

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        public bool IsExpressionEqual(IComparableExpression other)
        {
            return IsExpressionEqual(other as IEntityExpression);
        }

        /// <summary>
        /// Compares two expressions.
        /// </summary>
        /// <param name="other">The other expression.</param>
        protected bool IsExpressionEqual(IEntityExpression other)
        {
            Debug.Assert(other != null);

            bool Result = true;

            Result &= QualifiedName.IsQualifiedNameEqual((IQualifiedName)Query, (IQualifiedName)other.Query);

            return Result;
        }

        /// <summary>
        /// Finds the matching nodes of a <see cref="IEntityExpression"/>.
        /// </summary>
        /// <param name="node">The agent expression to check.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="resolvedResult">The expression result types upon return.</param>
        /// <param name="resolvedException">Exceptions the expression can throw upon return.</param>
        /// <param name="constantSourceList">Sources of the constant expression upon return, if any.</param>
        /// <param name="expressionConstant">The expression constant upon return.</param>
        /// <param name="resolvedFinalFeature">The feature if the end of the path is a feature.</param>
        /// <param name="resolvedFinalDiscrete">The discrete if the end of the path is a discrete.</param>
        public static bool ResolveCompilerReferences(IEntityExpression node, IErrorList errorList, out IResultType resolvedResult, out IResultException resolvedException, out ISealableList<IExpression> constantSourceList, out ILanguageConstant expressionConstant, out ICompiledFeature resolvedFinalFeature, out IDiscrete resolvedFinalDiscrete)
        {
            resolvedResult = null;
            resolvedException = null;
            constantSourceList = new SealableList<IExpression>();
            expressionConstant = NeutralLanguageConstant.NotConstant;
            resolvedFinalFeature = null;
            resolvedFinalDiscrete = null;

            IQualifiedName Query = (IQualifiedName)node.Query;

            IClass EmbeddingClass = (Class)node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Entity.Guid, node, out ITypeName ResultTypeName, out ICompiledType ResultType))
            {
                errorList.AddError(new ErrorEntityTypeMissing(node));
                return false;
            }

            IList<IIdentifier> ValidPath = Query.ValidPath.Item;
            IIdentifier LastIdentifier = ValidPath[ValidPath.Count - 1];
            string ValidText = LastIdentifier.ValidText.Item;

            ISealableDictionary<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, errorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Guid EntityGuid;

            if (FinalFeature is IFeatureWithEntity AsFeatureWithEntity)
            {
                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Query.ValidResultTypePath.Item);
                resolvedFinalFeature = FinalFeature;
                EntityGuid = AsFeatureWithEntity.EntityGuid;

                expressionConstant = new EntityLanguageConstant(AsFeatureWithEntity);
            }
            else
            {
                Debug.Assert(FinalDiscrete != null);

                resolvedFinalDiscrete = FinalDiscrete;
                EntityGuid = LanguageClasses.NamedFeatureEntity.Guid;

                expressionConstant = new EntityLanguageConstant(resolvedFinalDiscrete);
            }

            ITypeName EntityTypeName = EmbeddingClass.ImportedLanguageTypeTable[EntityGuid].Item1;
            ICompiledType EntityType = EmbeddingClass.ImportedLanguageTypeTable[EntityGuid].Item2;

            resolvedResult = new ResultType(EntityTypeName, EntityType, ValidText);
            resolvedException = new ResultException();

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
        public NumberKinds NumberKind { get { return NumberKinds.NotApplicable; } }

        /// <summary>
        /// Restarts a check of number types.
        /// </summary>
        public void RestartNumberType(ref bool isChanged)
        {
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public void CheckNumberType(ref bool isChanged)
        {
            Debug.Assert(ResolvedResult.Item.NumberKind == NumberKinds.NotApplicable);
        }

        /// <summary>
        /// Validates number types. If not valid, adds an error.
        /// </summary>
        /// <param name="errorList">The list of errors found.</param>
        public void ValidateNumberType(IErrorList errorList)
        {
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string ExpressionToString { get { return $"entity {((IQualifiedName)Query).PathToString}"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Entity Expression '{ExpressionToString}'";
        }
        #endregion
    }
}
