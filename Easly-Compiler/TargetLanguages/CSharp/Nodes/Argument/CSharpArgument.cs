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
        /// Gets the source code of arguments of a feature call.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureCall">Details of the call.</param>
        public static string CSharpArgumentList(ICSharpWriter writer, ICSharpFeatureCall featureCall)
        {
            CSharpArgumentList(writer, false, false, featureCall, new List<ICSharpQualifiedName>(), -1, out string callText, out string resultText);
            return callText;
        }

        /// <summary>
        /// Gets the source code of arguments of a feature call.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="featureCall">Details of the call.</param>
        /// <param name="destinationList">List of destination features.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        /// <param name="callText">The string to use for a call upon return.</param>
        /// <param name="resultText">The string to use for a nested call with the result upon return.</param>
        public static void CSharpArgumentList(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string callText, out string resultText)
        {
            if (featureCall.Count == 0 && destinationList.Count == 0)
            {
                callText = string.Empty;
                resultText = string.Empty;
            }
            else
            {
                callText = null;
                resultText = null;

                switch (featureCall.ArgumentStyle)
                {
                    case TypeArgumentStyles.None:
                    case TypeArgumentStyles.Positional:
                        CSharpPositionalArgumentList(writer, isNeverSimple, isDeclaredInPlace, featureCall, destinationList, skippedIndex, out callText, out resultText);
                        break;

                    case TypeArgumentStyles.Assignment:
                        CSharpAssignmentArgumentList(writer, isNeverSimple, isDeclaredInPlace, featureCall, destinationList, skippedIndex, out callText, out resultText);
                        break;
                }

                Debug.Assert(callText != null);
                Debug.Assert(resultText != null);
            }
        }

        private static void CSharpPositionalArgumentList(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string callText, out string resultText)
        {
            IList<ICSharpParameter> ParameterList = featureCall.ParameterList;
            IList<ICSharpParameter> ResultList = featureCall.ResultList;
            IList<ICSharpArgument> ArgumentList = featureCall.ArgumentList;

            int i;
            callText = string.Empty;

            for (i = 0; i < ArgumentList.Count; i++)
            {
                if (callText.Length > 0)
                    callText += ", ";

                ICSharpPositionalArgument Argument = ArgumentList[i] as ICSharpPositionalArgument;
                Debug.Assert(Argument != null);

                ICSharpExpression SourceExpression = Argument.SourceExpression;

                callText += SourceExpression.CSharpText(writer);
            }

            for (; i < ParameterList.Count; i++)
            {
                if (callText.Length > 0)
                    callText += ", ";

                ICSharpParameter Parameter = ParameterList[i];
                ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                ICSharpExpression DefaultValue = Feature.DefaultValue;

                Debug.Assert(DefaultValue != null);

                callText += DefaultValue.CSharpText(writer);
            }

            CSharpAssignmentResultList(writer, isNeverSimple, featureCall, destinationList, skippedIndex, ref callText, out resultText);
        }

        private static void CSharpAssignmentArgumentList(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex, out string callText, out string resultText)
        {
            IList<ICSharpParameter> ParameterList = featureCall.ParameterList;
            IList<ICSharpParameter> ResultList = featureCall.ResultList;
            IList<ICSharpArgument> ArgumentList = featureCall.ArgumentList;

            int i;
            callText = string.Empty;

            for (i = 0; i < ParameterList.Count; i++)
            {
                if (callText.Length > 0)
                    callText += ", ";

                ICSharpParameter Parameter = ParameterList[i];
                string ParameterName = Parameter.Name;

                ICSharpExpression SourceExpression = null;

                foreach (ICSharpAssignmentArgument Argument in ArgumentList)
                    foreach (string Name in Argument.ParameterNameList)
                        if (ParameterName == Name)
                        {
                            SourceExpression = Argument.SourceExpression;
                            break;
                        }

                if (SourceExpression != null)
                    callText += SourceExpression.CSharpText(writer);
                else
                {
                    ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                    ICSharpExpression DefaultValue = Feature.DefaultValue;

                    Debug.Assert(DefaultValue != null);

                    callText += DefaultValue.CSharpText(writer);
                }
            }

            CSharpAssignmentResultList(writer, isNeverSimple, featureCall, destinationList, skippedIndex, ref callText, out resultText);
        }

        private static void CSharpAssignmentResultList(ICSharpWriter writer, bool isNeverSimple, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex, ref string callText, out string resultText)
        {
            IList<ICSharpParameter> ResultList = featureCall.ResultList;

            resultText = string.Empty;

            for (int i = 0; i < ResultList.Count; i++)
            {
                if (i == skippedIndex)
                    continue;

                if (callText.Length > 0)
                    callText += ", ";

                if (resultText.Length > 0)
                    resultText += ", ";

                ICSharpQualifiedName Destination = i < destinationList.Count ? destinationList[i] : null;

                if (Destination == null || !Destination.IsSimple || isNeverSimple)
                {
                    Debug.Assert(i < ResultList.Count);
                    ICSharpParameter callTextParameter = ResultList[i];

                    string TempTypeText = callTextParameter.Feature.Type.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                    string TempText;
                    if (Destination != null)
                    {
                        TempText = Destination.CSharpText(writer, 0).Replace('.', '_');
                        TempText = Destination.CSharpText(writer, 0).Replace('.', '_');
                    }
                    else
                        TempText = ResultList[i].Name;

                    callText += $"out {TempTypeText} Temp_{TempText}";
                    resultText += $"Temp_{TempText}";
                }
                else
                {
                    string DestinationText = Destination.CSharpText(writer, 0);
                    callText += $"out {DestinationText}";
                    resultText += DestinationText;
                }
            }
        }

        /// <summary>
        /// Builds a list of parameters, with and without their type.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="parameterListText">The list of parameters with type upon return.</param>
        /// <param name="parameterNameListText">The list of parameters without type upon return.</param>
        public static void BuildParameterList(ICSharpUsingCollection usingCollection, IList<ICSharpParameter> parameterList, out string parameterListText, out string parameterNameListText)
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

                string ParameterText = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);
                string ParameterNameText = CSharpNames.ToCSharpIdentifier(ParameterName);

                parameterListText += $"{ParameterText} {ParameterNameText}";
                parameterNameListText += ParameterNameText;
            }
        }

        /// <summary>
        /// Builds a list of parameters, with and without their type.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="parameterList">The list of parameters.</param>
        /// <param name="resultList">The list of results.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="parameterListText">The list of parameters with type upon return.</param>
        /// <param name="parameterNameListText">The list of parameters without type upon return.</param>
        /// <param name="resultTypeText">The type text upon return.</param>
        public static void BuildParameterList(ICSharpUsingCollection usingCollection, IList<ICSharpParameter> parameterList, IList<ICSharpParameter> resultList, CSharpFeatureTextTypes featureTextType, out string parameterListText, out string parameterNameListText, out string resultTypeText)
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

                string ParameterText = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);
                string ParameterNameText = CSharpNames.ToCSharpIdentifier(ParameterName);

                parameterListText += $"{ParameterText} {ParameterNameText}";
                parameterNameListText += ParameterNameText;
            }

            if (resultList.Count == 1)
                BuildResultListSingle(usingCollection, resultList, out resultTypeText);
            else
            {
                int ResultIndex = -1;
                for (int i = 0; i < resultList.Count; i++)
                {
                    ICSharpParameter Result = resultList[i];
                    if (Result.Name == nameof(BaseNode.Keyword.Result))
                    {
                        ResultIndex = i;
                        break;
                    }
                }

                if (ResultIndex < 0)
                    BuildResultListNoResult(usingCollection, resultList, ref parameterListText, ref parameterNameListText, out resultTypeText);
                else
                    BuildResultListWithResult(usingCollection, resultList, ResultIndex, ref parameterListText, ref parameterNameListText, out resultTypeText);
            }
        }

        private static void BuildResultListSingle(ICSharpUsingCollection usingCollection, IList<ICSharpParameter> resultList, out string resultTypeText)
        {
            Debug.Assert(resultList.Count == 1);

            ICSharpParameter Result = resultList[0];
            ICSharpScopeAttributeFeature ResultAttribute = Result.Feature;

            /*if (FeatureTextType == FeatureTextTypes.Interface)
                ResultType = CSharpTypes.Type2CSharpString(ResultAttribute.ResolvedFeatureType.Item, Context, CSharpTypeFormats.AsInterface);
            else
                ResultType = CSharpTypes.Type2CSharpString(ResultAttribute.ResolvedFeatureType.Item, Context, CSharpTypeFormats.None);*/
            resultTypeText = ResultAttribute.Type.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
        }

        private static void BuildResultListNoResult(ICSharpUsingCollection usingCollection, IList<ICSharpParameter> resultList, ref string parameterListText, ref string parameterNameListText, out string resultTypeText)
        {
            resultTypeText = "void";

            foreach (ICSharpParameter Result in resultList)
            {
                ICSharpScopeAttributeFeature ResultAttribute = Result.Feature;
                ICSharpType ParameterType = ResultAttribute.Type;
                CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;

                string TypeString = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);
                string AttributeString = CSharpNames.ToCSharpIdentifier(Result.Name);

                if (parameterListText.Length > 0)
                    parameterListText += ", ";
                if (parameterNameListText.Length > 0)
                    parameterNameListText += ", ";

                parameterListText += $"out {TypeString} {AttributeString}";
                parameterNameListText += $"out {AttributeString}";
            }
        }

        private static void BuildResultListWithResult(ICSharpUsingCollection usingCollection, IList<ICSharpParameter> resultList, int resultIndex, ref string parameterListText, ref string parameterNameListText, out string resultTypeText)
        {
            resultTypeText = null;

            for (int i = 0; i < resultList.Count; i++)
            {
                ICSharpParameter Result = resultList[i];
                ICSharpScopeAttributeFeature ResultAttribute = Result.Feature;
                ICSharpType ParameterType = ResultAttribute.Type;
                CSharpTypeFormats ParameterFormat = ParameterType.HasInterfaceText ? CSharpTypeFormats.AsInterface : CSharpTypeFormats.Normal;

                string TypeString = ParameterType.Type2CSharpString(usingCollection, ParameterFormat, CSharpNamespaceFormats.None);
                string AttributeString = CSharpNames.ToCSharpIdentifier(Result.Name);

                if (i == resultIndex)
                    resultTypeText = TypeString;
                else
                {
                    if (parameterListText.Length > 0)
                        parameterListText += ", ";
                    if (parameterNameListText.Length > 0)
                        parameterNameListText += ", ";

                    parameterListText += $"out {TypeString} {AttributeString}";
                    parameterNameListText += $"out {AttributeString}";
                }
            }

            Debug.Assert(resultTypeText != null);
        }
        #endregion
    }
}
