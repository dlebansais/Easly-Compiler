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
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="featureCall">Details of the call.</param>
        public static string CSharpArgumentList(ICSharpUsingCollection usingCollection, ICSharpFeatureCall featureCall)
        {
            return CSharpArgumentList(usingCollection, false, false, featureCall, new List<ICSharpQualifiedName>(), -1);
        }

        /// <summary>
        /// Gets the source code of arguments of a feature call.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="featureCall">Details of the call.</param>
        /// <param name="destinationList">List of destination features.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public static string CSharpArgumentList(ICSharpUsingCollection usingCollection, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            if (featureCall.Count == 0 && destinationList.Count == 0)
                return string.Empty;

            string Result = null;

            switch (featureCall.ArgumentStyle)
            {
                case TypeArgumentStyles.None:
                case TypeArgumentStyles.Positional:
                    Result = CSharpPositionalArgumentList(usingCollection, isNeverSimple, isDeclaredInPlace, featureCall, destinationList, skippedIndex);
                    break;

                case TypeArgumentStyles.Assignment:
                    Result = CSharpAssignmentArgumentList(usingCollection, isNeverSimple, isDeclaredInPlace, featureCall, destinationList, skippedIndex);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        private static string CSharpPositionalArgumentList(ICSharpUsingCollection usingCollection, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            IList<ICSharpParameter> ParameterList = featureCall.ParameterList;
            IList<ICSharpParameter> ResultList = featureCall.ResultList;
            IList<ICSharpArgument> ArgumentList = featureCall.ArgumentList;

            int i;
            string Result = string.Empty;

            for (i = 0; i < ArgumentList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

                ICSharpPositionalArgument Argument = ArgumentList[i] as ICSharpPositionalArgument;
                Debug.Assert(Argument != null);

                ICSharpExpression SourceExpression = Argument.SourceExpression;

                Result += SourceExpression.CSharpText(usingCollection);
            }

            for (; i < ParameterList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

                ICSharpParameter Parameter = ParameterList[i];
                ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                ICSharpExpression DefaultValue = Feature.DefaultValue;

                Debug.Assert(DefaultValue != null);

                Result += DefaultValue.CSharpText(usingCollection);
            }

            for (i = 0; i < destinationList.Count; i++)
            {
                ICSharpQualifiedName Destination = destinationList[i];
                if (i == skippedIndex)
                    continue;

                if (Result.Length > 0)
                    Result += ", ";

                if (!Destination.IsSimple || isNeverSimple)
                {
                    Debug.Assert(i < ResultList.Count);
                    ICSharpParameter ResultParameter = ResultList[i];

                    string TempTypeText = ResultParameter.Feature.Type.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                    string TempText = Destination.CSharpText(usingCollection, 0).Replace('.', '_');
                    Result += $"out {TempTypeText} Temp_{TempText}";
                }
                else
                {
                    string DestinationText = Destination.CSharpText(usingCollection, 0);
                    Result += $"out {DestinationText}";
                }
            }

            return Result;
        }

        private static string CSharpAssignmentArgumentList(ICSharpUsingCollection usingCollection, bool isNeverSimple, bool isDeclaredInPlace, ICSharpFeatureCall featureCall, IList<ICSharpQualifiedName> destinationList, int skippedIndex)
        {
            IList<ICSharpParameter> ParameterList = featureCall.ParameterList;
            IList<ICSharpParameter> ResultList = featureCall.ResultList;
            IList<ICSharpArgument> ArgumentList = featureCall.ArgumentList;

            string Result = string.Empty;
            int i;

            for (i = 0; i < ParameterList.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ", ";

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
                    Result += SourceExpression.CSharpText(usingCollection);
                else
                {
                    ICSharpScopeAttributeFeature Feature = Parameter.Feature;
                    ICSharpExpression DefaultValue = Feature.DefaultValue;

                    Debug.Assert(DefaultValue != null);

                    Result += DefaultValue.CSharpText(usingCollection);
                }
            }

            for (i = 0; i < destinationList.Count; i++)
            {
                ICSharpQualifiedName Destination = destinationList[i];
                if (i == skippedIndex)
                    continue;

                if (Result.Length > 0)
                    Result += ", ";

                if (!Destination.IsSimple)
                {
                    string TempText = Destination.CSharpText(usingCollection, 0).Replace('.', '_');
                    Result += $"out Temp_{TempText}";
                }
                else
                {
                    string DestinationText = Destination.CSharpText(usingCollection, 0);
                    Result += $"out {DestinationText}";
                }
            }

            return Result;
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
