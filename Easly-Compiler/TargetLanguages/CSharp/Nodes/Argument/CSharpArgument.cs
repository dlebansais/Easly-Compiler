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
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpArgument Create(IArgument source, ICSharpContext context)
        {
            ICSharpArgument Result = null;

            switch (source)
            {
                case IPositionalArgument AsPositionalArgument:
                    Result = CSharpPositionalArgument.Create(AsPositionalArgument, context);
                    break;

                case IAssignmentArgument AsAssignmentArgument:
                    Result = CSharpAssignmentArgument.Create(AsAssignmentArgument, context);
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpArgument"/> class.
        /// </summary>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpArgument(IArgument source, ICSharpContext context)
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
        #endregion
    }
}
