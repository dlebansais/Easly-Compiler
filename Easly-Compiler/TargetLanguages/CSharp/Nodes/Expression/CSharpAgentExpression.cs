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
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

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
        /// <param name="usingCollection">The collection of using directives.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection)
        {
            return CSharpText(usingCollection, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(ICSharpUsingCollection usingCollection, IList<ICSharpQualifiedName> destinationList)
        {
            string Result = null;

            switch (Delegated)
            {
                case ICSharpProcedureFeature AsProcedureFeature:
                    Result = CSharpTextProcedure(usingCollection, AsProcedureFeature, destinationList);
                    break;

                case ICSharpFunctionFeature AsFunctionFeature:
                    Result = CSharpTextFunction(usingCollection, AsFunctionFeature, destinationList);
                    break;

                case ICSharpPropertyFeature AsPropertyFeature:
                    Result = CSharpTextProperty(usingCollection, AsPropertyFeature, destinationList);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        private string CSharpTextProcedure(ICSharpUsingCollection usingCollection, ICSharpProcedureFeature feature, IList<ICSharpQualifiedName> destinationList)
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

        private string CSharpTextFunction(ICSharpUsingCollection usingCollection, ICSharpFunctionFeature feature, IList<ICSharpQualifiedName> destinationList)
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

        private string CSharpTextProperty(ICSharpUsingCollection usingCollection, ICSharpPropertyFeature feature, IList<ICSharpQualifiedName> destinationList)
        {
            string Result;

            // TODO handle several overloads.

            string BaseTypeText;

            if (BaseType != null)
                BaseTypeText = EffectiveBaseType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.OneWord);
            else
                BaseTypeText = $"I{CSharpNames.ToCSharpIdentifier(Delegated.Owner.ValidClassName)}";

            Result = $"({BaseTypeText} agentBase) => {{ agentBase.{CSharpNames.ToCSharpIdentifier(Delegated.Name)}; }}";

            return Result;
        }
        #endregion
    }
}
