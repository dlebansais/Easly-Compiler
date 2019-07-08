namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpAgentExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IAgentExpression Source { get; }

        /// <summary>
        /// The base type. Can be null.
        /// </summary>
        ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The effective base type if different than <see cref="BaseType"/>. Can be null.
        /// </summary>
        ICSharpTypeWithFeature EffectiveBaseType { get; }

        /// <summary>
        /// The feature referenced.
        /// </summary>
        ICSharpFeatureWithName Delegated { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpAgentExpression : CSharpExpression, ICSharpAgentExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpAgentExpression Create(ICSharpContext context, IAgentExpression source)
        {
            return new CSharpAgentExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAgentExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpAgentExpression(ICSharpContext context, IAgentExpression source)
            : base(context, source)
        {
            string DelegatedName = ((IIdentifier)Source.Delegated).ValidText.Item;

            if (source.BaseType.IsAssigned)
            {
                IObjectType SourceBaseType = (IObjectType)source.BaseType.Item;
                ICompiledType ResolvedBaseType = SourceBaseType.ResolvedType.Item;
                BaseType = CSharpType.Create(context, ResolvedBaseType) as ICSharpTypeWithFeature;
                Debug.Assert(BaseType != null);

                EffectiveBaseType = BaseType;

                foreach (ICSharpClassType ClassType in BaseType.ConformingClassTypeList)
                {
                    foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in ClassType.Source.FeatureTable)
                    {
                        if (Entry.Key.Name == DelegatedName)
                        {
                            Debug.Assert(Delegated == null);

                            ICompiledFeature Feature = Entry.Value.Feature;
                            Delegated = context.GetFeature(Feature) as ICSharpFeatureWithName;

                            if (ResolvedBaseType is IFormalGenericType)
                                EffectiveBaseType = ClassType;
                        }
                    }
                }
            }
            else
            {
                Delegated = context.GetFeature(source.ResolvedFeature.Item) as ICSharpFeatureWithName;
            }

            Debug.Assert(Delegated != null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IAgentExpression Source { get { return (IAgentExpression)base.Source; } }

        /// <summary>
        /// The base type. Can be null.
        /// </summary>
        public ICSharpTypeWithFeature BaseType { get; }

        /// <summary>
        /// The effective base type if different than <see cref="BaseType"/>. Can be null.
        /// </summary>
        public ICSharpTypeWithFeature EffectiveBaseType { get; }

        /// <summary>
        /// The feature referenced.
        /// </summary>
        public ICSharpFeatureWithName Delegated { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isDeclaredInPlace, int skippedIndex)
        {
            string Result = null;

            switch (Delegated)
            {
                case ICSharpAttributeFeature AsAttributeFeature:
                    Result = CSharpTextAttribute(writer, AsAttributeFeature);
                    break;

                case ICSharpConstantFeature AsConstantFeature:
                    Result = CSharpTextConstant(writer, AsConstantFeature);
                    break;

                case ICSharpProcedureFeature AsProcedureFeature:
                    Result = CSharpTextProcedure(writer, AsProcedureFeature);
                    break;

                case ICSharpFunctionFeature AsFunctionFeature:
                    Result = CSharpTextFunction(writer, AsFunctionFeature);
                    break;

                case ICSharpPropertyFeature AsPropertyFeature:
                    Result = CSharpTextProperty(writer, AsPropertyFeature);
                    break;
            }

            Debug.Assert(Result != null);

            expressionContext.SetSingleReturnValue(Result);
        }

        private string CSharpTextAttribute(ICSharpUsingCollection usingCollection, ICSharpAttributeFeature feature)
        {
            string Result;

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            Result = $"({BaseTypeText} agentBase) => {{ agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}; }}";

            return Result;
        }

        private string CSharpTextConstant(ICSharpUsingCollection usingCollection, ICSharpConstantFeature feature)
        {
            string Result;

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            Result = $"({BaseTypeText} agentBase) => {{ agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}; }}";

            return Result;
        }

        private string CSharpTextProcedure(ICSharpUsingCollection usingCollection, ICSharpProcedureFeature feature)
        {
            string Result;

            // TODO handle several overloads.

            Debug.Assert(feature.OverloadList.Count > 0);
            ICSharpCommandOverload Overload = feature.OverloadList[0] as ICSharpCommandOverload;
            Debug.Assert(Overload != null);

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            string AgentParameters;
            string ParameterNameListText;

            if (Overload.ParameterList.Count > 0)
            {
                CSharpArgument.BuildParameterList(usingCollection, Overload.ParameterList, out string ParameterListText, out ParameterNameListText);
                AgentParameters = $"({BaseTypeText} agentBase, {ParameterListText})";
            }
            else
            {
                AgentParameters = $"({BaseTypeText} agentBase)";
                ParameterNameListText = string.Empty;
            }

            Result = $"{AgentParameters} => {{ agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}({ParameterNameListText}); }}";

            return Result;
        }

        private string CSharpTextFunction(ICSharpUsingCollection usingCollection, ICSharpFunctionFeature feature)
        {
            string Result;

            // TODO handle several overloads.

            Debug.Assert(feature.OverloadList.Count > 0);
            ICSharpQueryOverload Overload = feature.OverloadList[0] as ICSharpQueryOverload;
            Debug.Assert(Overload != null);

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            string AgentParameters;
            string ParameterNameListText;

            if (Overload.ParameterList.Count > 0)
            {
                CSharpArgument.BuildParameterList(usingCollection, Overload.ParameterList, out string ParameterListText, out ParameterNameListText);
                AgentParameters = $"({BaseTypeText} agentBase, {ParameterListText})";
            }
            else
            {
                AgentParameters = $"({BaseTypeText} agentBase)";
                ParameterNameListText = string.Empty;
            }

            Result = $"{AgentParameters} => {{ return agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}({ParameterNameListText}); }}";

            return Result;
        }

        private string CSharpTextProperty(ICSharpUsingCollection usingCollection, ICSharpPropertyFeature feature)
        {
            string Result;

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            Result = $"({BaseTypeText} agentBase) => {{ return agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}; }}";

            return Result;
        }
        #endregion
    }
}
