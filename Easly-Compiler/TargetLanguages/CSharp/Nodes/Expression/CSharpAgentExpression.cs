﻿namespace EaslyCompiler
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
                BaseType = CSharpType.Create(context, ((IObjectType)source.BaseType.Item).ResolvedType.Item) as ICSharpTypeWithFeature;
                Debug.Assert(BaseType != null);

                foreach (ICSharpClassType ClassType in BaseType.ConformingClassTypeList)
                {
                    foreach (ICSharpFeature Feature in ClassType.Class.FeatureList)
                    {
                        if (Feature is ICSharpFeatureWithName AsFeatureWithName)
                        {
                            if (AsFeatureWithName.Name == DelegatedName)
                            {
                                Debug.Assert(Delegated == null);
                                Delegated = AsFeatureWithName;
                                Delegated = context.GetFeature(source.ResolvedFeature.Item) as ICSharpFeatureWithName;
                            }
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
        /// The feature referenced.
        /// </summary>
        public ICSharpFeatureWithName Delegated { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpText(cSharpNamespace, new List<ICSharpQualifiedName>());
        }

        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="destinationList">The list of destinations.</param>
        public override string CSharpText(string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result = null;

            if (BaseType != null)
            {
                switch (Delegated)
                {
                    case ICSharpProcedureFeature AsProcedureFeature:
                        Result = CSharpTextProcedure(AsProcedureFeature, cSharpNamespace, destinationList);
                        break;

                    case ICSharpFunctionFeature AsFunctionFeature:
                        Result = CSharpTextFunction(AsFunctionFeature, cSharpNamespace, destinationList);
                        break;

                    case ICSharpPropertyFeature AsPropertyFeature:
                        Result = CSharpTextProperty(AsPropertyFeature, cSharpNamespace, destinationList);
                        break;
                }
            }
            else if (Delegated is ICSharpPropertyFeature AsPropertyFeature)
            {
                string EmbeddingClassName = Delegated.Owner.ValidClassName;
                Result = $"({EmbeddingClassName} agentBase) => {{ agentBase.{Delegated.Name}; }}";
            }
            else
                Result = CSharpNames.ToCSharpIdentifier(Delegated.Name);

            Debug.Assert(Result != null);

            return Result;
        }

        private string CSharpTextProcedure(ICSharpProcedureFeature feature, string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result;

            // TODO handle several overloads.

            Debug.Assert(feature.OverloadList.Count > 0);
            ICSharpCommandOverload Overload = feature.OverloadList[0] as ICSharpCommandOverload;
            Debug.Assert(Overload != null);

            string BaseTypeText = BaseType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.OneWord);
            string AgentParameters;
            string ParameterNameListText;

            if (Overload.ParameterList.Count > 0)
            {
                CSharpArgument.BuildParameterList(Overload.ParameterList, cSharpNamespace, out string ParameterListText, out ParameterNameListText);
                AgentParameters = $"({BaseTypeText} agentBase, {ParameterListText})";
            }
            else
            {
                AgentParameters = $"({BaseTypeText} agentBase)";
                ParameterNameListText = string.Empty;
            }

            Result = $"{AgentParameters} => {{ agentBase.{Delegated.Name}({ParameterNameListText}); }}";

            return Result;
        }

        private string CSharpTextFunction(ICSharpFunctionFeature feature, string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result;

            // TODO handle several overloads.

            Debug.Assert(feature.OverloadList.Count > 0);
            ICSharpQueryOverload Overload = feature.OverloadList[0] as ICSharpQueryOverload;
            Debug.Assert(Overload != null);

            string BaseTypeText = BaseType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.OneWord);
            string AgentParameters;
            string ParameterNameListText;

            if (Overload.ParameterList.Count > 0)
            {
                CSharpArgument.BuildParameterList(Overload.ParameterList, cSharpNamespace, out string ParameterListText, out ParameterNameListText);
                AgentParameters = $"({BaseTypeText} agentBase, {ParameterListText})";
            }
            else
            {
                AgentParameters = $"({BaseTypeText} agentBase)";
                ParameterNameListText = string.Empty;
            }

            Result = $"{AgentParameters} => {{ return agentBase.{Delegated.Name}({ParameterNameListText}); }}";

            return Result;
        }

        private string CSharpTextProperty(ICSharpPropertyFeature feature, string cSharpNamespace, IList<ICSharpQualifiedName> destinationList)
        {
            string Result;

            // TODO handle several overloads.

            string BaseTypeText = BaseType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.OneWord);

            Result = $"({BaseTypeText} agentBase) => {{ agentBase.{Delegated.Name}; }}";

            return Result;
        }
        #endregion
    }
}
