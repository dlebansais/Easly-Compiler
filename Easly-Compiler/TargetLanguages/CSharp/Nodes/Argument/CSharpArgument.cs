namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# argument.
    /// </summary>
    public interface ICSharpArgument
    {
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        IArgument Source { get; }

        /// <summary>
        /// The source expression.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// Gets the source code corresponding to the argument.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        string CSharpText(string cSharpNamespace);
    }

    /// <summary>
    /// A C# argument.
    /// </summary>
    public abstract class CSharpArgument : ICSharpArgument
    {
        #region Init
        /// <summary>
        /// Creates a new C# argument.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        public static ICSharpArgument Create(ICSharpContext context, IArgument source)
        {
            ICSharpArgument Result = null;

            switch (source)
            {
                case IPositionalArgument AsPositionalArgument:
                    Result = CSharpPositionalArgument.Create(context, AsPositionalArgument);
                    break;

                case IAssignmentArgument AsAssignmentArgument:
                    Result = CSharpAssignmentArgument.Create(context, AsAssignmentArgument);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpArgument"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        protected CSharpArgument(ICSharpContext context, IArgument source)
        {
            Debug.Assert(source != null);

            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        public IArgument Source { get; }

        /// <summary>
        /// The source expression.
        /// </summary>
        public ICSharpExpression SourceExpression { get; protected set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the argument.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public abstract string CSharpText(string cSharpNamespace);

        /// <summary>
        /// Gets the source code of arguments of a feature call.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="featureCall">Details of the call.</param>
        /// <param name="destinationList">List of destination features.</param>
        public static string CSharpArgumentList(string cSharpNamespace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList)
        {
            if (featureCall.Count == 0)
                return string.Empty;

            string Result = null;

            switch (featureCall.ArgumentStyle)
            {
                case TypeArgumentStyles.None:
                case TypeArgumentStyles.Positional:
                    Result = CSharpPositionalArgumentList(cSharpNamespace, featureCall.ParameterList, featureCall.ArgumentList, destinationList);
                    break;

                case TypeArgumentStyles.Assignment:
                    Result = CSharpAssignmentArgumentList(cSharpNamespace, featureCall.ParameterList, featureCall.ArgumentList, destinationList);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        private static string CSharpPositionalArgumentList(string cSharpNamespace, IList<ICSharpParameter> parameterList, IList<ICSharpArgument> argumentList, IList<ICSharpQualifiedName> destinationList)
        {
            int i;
            string Result = string.Empty;

            for (i = 0; i < argumentList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

                ICSharpPositionalArgument Argument = argumentList[i] as ICSharpPositionalArgument;
                Debug.Assert(Argument != null);

                ICSharpExpression SourceExpression = Argument.SourceExpression;

                Result += SourceExpression.CSharpText(cSharpNamespace);
            }

            for (; i < parameterList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

                ICSharpParameter Parameter = parameterList[i];
                ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                ICSharpExpression DefaultValue = Feature.DefaultValue;

                Debug.Assert(DefaultValue != null);

                Result += DefaultValue.CSharpText(cSharpNamespace);
            }

            foreach (ICSharpQualifiedName Destination in destinationList)
            {
                if (Result.Length > 0)
                    Result += ", ";

                string DestinationText = Destination.CSharpText(cSharpNamespace, 0);

                Result += $"out {DestinationText }";
            }

            return Result;
        }

        private static string CSharpAssignmentArgumentList(string cSharpNamespace, IList<ICSharpParameter> parameterList, IList<ICSharpArgument> argumentList, IList<ICSharpQualifiedName> destinationList)
        {
            string Result = string.Empty;

            for (int i = 0; i < parameterList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

                ICSharpParameter Parameter = parameterList[i];
                string ParameterName = Parameter.Name;

                ICSharpExpression SourceExpression = null;

                foreach (ICSharpAssignmentArgument Argument in argumentList)
                    foreach (string Name in Argument.ParameterNameList)
                        if (ParameterName == Name)
                        {
                            SourceExpression = Argument.SourceExpression;
                            break;
                        }

                if (SourceExpression != null)
                    Result += SourceExpression.CSharpText(cSharpNamespace);
                else
                {
                    ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                    ICSharpExpression DefaultValue = Feature.DefaultValue;

                    Debug.Assert(DefaultValue != null);

                    Result += DefaultValue.CSharpText(cSharpNamespace);
                }
            }

            foreach (ICSharpQualifiedName Destination in destinationList)
            {
                if (Result.Length > 0)
                    Result += ", ";

                string DestinationText = Destination.CSharpText(cSharpNamespace, 0);

                Result += $"out {DestinationText }";
            }

            return Result;
        }

        /// <summary>
        /// Builds a list of parameters, with and without their type.
        /// </summary>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="outputNamespace">The current namespace.</param>
        /// <param name="parameterListText">The list of parameters with type upon return.</param>
        /// <param name="parameterNameListText">The list of parameters without type upon return.</param>
        public static void BuildParameterList(IList<ICSharpParameter> parameterList, string outputNamespace, out string parameterListText, out string parameterNameListText)
        {
            parameterListText = string.Empty;
            parameterNameListText = string.Empty;

            foreach (ICSharpParameter Parameter in parameterList)
            {
                if (parameterListText.Length > 0)
                    parameterListText += ", ";
                if (parameterNameListText.Length > 0)
                    parameterNameListText += ", ";

                string ParameterName = Parameter.Name;
                ICSharpType ParameterType = Parameter.Feature.Type;

                CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;

                string ParameterText = ParameterType.Type2CSharpString(outputNamespace, ParameterFormat, CSharpNamespaceFormats.None);
                string ParameterNameText = CSharpNames.ToCSharpIdentifier(ParameterName);

                parameterListText += $"{ParameterText} {ParameterNameText}";
                parameterNameListText += ParameterNameText;
            }
        }

        /// <summary>
        /// Builds a list of parameters, with and without their type.
        /// </summary>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="resultList">The list of results.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="outputNamespace">The current namespace.</param>
        /// <param name="parameterListText">The list of parameters with type upon return.</param>
        /// <param name="parameterNameListText">The list of parameters without type upon return.</param>
        /// <param name="resultTypeText">The type text upon return.</param>
        public static void BuildParameterList(IList<ICSharpParameter> parameterList, IList<ICSharpParameter> resultList, CSharpFeatureTextTypes featureTextType, string outputNamespace, out string parameterListText, out string parameterNameListText, out string resultTypeText)
        {
            parameterListText = string.Empty;
            parameterNameListText = string.Empty;

            foreach (ICSharpParameter Parameter in parameterList)
            {
                if (parameterListText.Length > 0)
                    parameterListText += ", ";
                if (parameterNameListText.Length > 0)
                    parameterNameListText += ", ";

                string ParameterName = Parameter.Name;
                ICSharpType ParameterType = Parameter.Feature.Type;

                CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;

                string ParameterText = ParameterType.Type2CSharpString(outputNamespace, ParameterFormat, CSharpNamespaceFormats.None);
                string ParameterNameText = CSharpNames.ToCSharpIdentifier(ParameterName);

                parameterListText += $"{ParameterText} {ParameterNameText}";
                parameterNameListText += ParameterNameText;
            }

            if (resultList.Count == 1)
            {
                ICSharpParameter Result = resultList[0];
                ICSharpScopeAttributeFeature ResultAttribute = Result.Feature;

                /*if (FeatureTextType == FeatureTextTypes.Interface)
                    ResultType = CSharpTypes.Type2CSharpString(ResultAttribute.ResolvedFeatureType.Item, Context, CSharpTypeFormats.AsInterface);
                else
                    ResultType = CSharpTypes.Type2CSharpString(ResultAttribute.ResolvedFeatureType.Item, Context, CSharpTypeFormats.None);*/
                resultTypeText = ResultAttribute.Type.Type2CSharpString(outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            }

            else
            {
                resultTypeText = "void";

                foreach (ICSharpParameter Result in resultList)
                {
                    ICSharpScopeAttributeFeature ResultAttribute = Result.Feature;
                    ICSharpType ParameterType = ResultAttribute.Type;

                    if (parameterListText.Length > 0)
                        parameterListText += ", ";
                    if (parameterNameListText.Length > 0)
                        parameterNameListText += ", ";

                    CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;

                    string TypeString = ParameterType.Type2CSharpString(outputNamespace, ParameterFormat, CSharpNamespaceFormats.None);
                    string AttributeString = CSharpNames.ToCSharpIdentifier(Result.Name);

                    parameterListText += $"out {TypeString} {AttributeString}";
                    parameterNameListText += $"out {AttributeString}";
                }
            }
        }
        #endregion
    }
}
