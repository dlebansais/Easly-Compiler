namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# query overload node.
    /// </summary>
    public interface ICSharpQueryOverload : ICSharpSource<IQueryOverload>, ICSharpOverload
    {
        /// <summary>
        /// The precursor overload. Can be null.
        /// </summary>
        new ICSharpQueryOverload Precursor { get; }

        /// <summary>
        /// The list of results.
        /// </summary>
        IList<ICSharpParameter> ResultList { get; }

        /// <summary>
        /// The overload body.
        /// </summary>
        ICSharpBody Body { get; }

        /// <summary>
        /// Sets the precursor.
        /// </summary>
        /// <param name="precursor">The precursor.</param>
        void SetPrecursor(ICSharpQueryOverload precursor);
    }

    /// <summary>
    /// A C# query overload node.
    /// </summary>
    public class CSharpQueryOverload : CSharpSource<IQueryOverload>, ICSharpQueryOverload
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpQueryOverload Create(ICSharpContext context, IQueryOverload source, ICSharpFeature parentFeature, ICSharpClass owner)
        {
            return new CSharpQueryOverload(context, source, parentFeature, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQueryOverload"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpQueryOverload(ICSharpContext context, IQueryOverload source, ICSharpFeature parentFeature, ICSharpClass owner)
            : base(source)
        {
            ParentFeature = parentFeature;

            foreach (IParameter Parameter in source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Parameter, owner);
                ParameterList.Add(NewParameter);
            }

            foreach (IParameter Result in source.ResultTable)
            {
                ICSharpParameter NewResult = CSharpParameter.Create(context, Result, owner);
                ResultList.Add(NewResult);
            }

            Body = CSharpBody.Create(context, parentFeature, source.ResolvedBody.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of parameters.
        /// </summary>
        public IList<ICSharpParameter> ParameterList { get; } = new List<ICSharpParameter>();

        /// <summary>
        /// The list of results.
        /// </summary>
        public IList<ICSharpParameter> ResultList { get; } = new List<ICSharpParameter>();

        /// <summary>
        /// The overload body.
        /// </summary>
        public ICSharpBody Body { get; }

        /// <summary>
        /// The precursor overload. Can be null.
        /// </summary>
        public ICSharpQueryOverload Precursor { get; private set; }
        ICSharpOverload ICSharpOverload.Precursor { get { return Precursor; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the precursor.
        /// </summary>
        /// <param name="precursor">The precursor.</param>
        public virtual void SetPrecursor(ICSharpQueryOverload precursor)
        {
            Debug.Assert(precursor != null);
            Debug.Assert(Precursor == null);

            Precursor = precursor;
        }

        /// <summary>
        /// Writes down the C# overload of a feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="isOverride">True if the feature is an override.</param>
        /// <param name="nameString">The composed feature name.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isConstructor">True if the feature is a constructor.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            IList<ICSharpParameter> SelectedParameterList = ParameterList;
            IList<ICSharpParameter> SelectedResultList = ResultList;

            if (isOverride && Precursor != null)
            {
                SelectedParameterList = Precursor.ParameterList;
                SelectedResultList = Precursor.ResultList;
            }

            CSharpArgument.BuildParameterList(writer, SelectedParameterList, SelectedResultList, featureTextType, out string ArgumentEntityList, out string ArgumentNameList, out string ResultType);
            string ExportStatusText;

            if (featureTextType == CSharpFeatureTextTypes.Implementation)
            {
                bool IsHandled = false;

                switch (Body)
                {
                    case ICSharpDeferredBody AsDeferredBody:
                        CSharpAssertion.WriteContract(writer, AsDeferredBody.RequireList, AsDeferredBody.EnsureList, CSharpContractLocations.Other, true, ref isFirstFeature, ref isMultiline);
                        ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                        writer.WriteIndentedLine($"{ExportStatusText} {ResultType} {nameString}({ArgumentEntityList});");
                        isMultiline = false;
                        IsHandled = true;
                        break;

                    case ICSharpEffectiveBody AsEffectiveBody:
                        CSharpAssertion.WriteContract(writer, AsEffectiveBody.RequireList, AsEffectiveBody.EnsureList, CSharpContractLocations.Other, true, ref isFirstFeature, ref isMultiline);

                        CSharpBodyFlags Flags = CSharpBodyFlags.MandatoryCurlyBrackets;
                        string ResultString = string.Empty;
                        List<string> InitialisationStringList = new List<string>();

                        if (ResultList.Count == 1)
                        {
                            Flags |= CSharpBodyFlags.HasResult;
                            ICSharpParameter Result = ResultList[0];
                            ResultString = Result.Feature.Type.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                        }
                        else
                        {
                            foreach (ICSharpParameter Item in ResultList)
                            {
                                string InitValueString;

                                ICSharpType ResultEntityType = Item.Feature.Type;

                                if (ResultEntityType is ICSharpClassType AsClassType)
                                {
                                    // TODO: when the type inherit from Enumeration
                                    if (AsClassType.Class.Source.ClassGuid == LanguageClasses.AnyOptionalReference.Guid)
                                        InitValueString = "new OptionalReference<>(null)"; // TODO

                                    else if (AsClassType.Class.Source.ClassGuid == LanguageClasses.String.Guid)
                                        InitValueString = "\"\"";

                                    else if (AsClassType.Class.Source.ClassGuid == LanguageClasses.Boolean.Guid)
                                        InitValueString = "false";

                                    else if (AsClassType.Class.Source.ClassGuid == LanguageClasses.Character.Guid)
                                        InitValueString = "'\0'";

                                    else if (AsClassType.Class.Source.ClassGuid == LanguageClasses.Number.Guid)
                                        InitValueString = "0";

                                    else
                                        InitValueString = "null";
                                }

                                else
                                    InitValueString = "null"; // TODO : tuples

                                string InitNameString = CSharpNames.ToCSharpIdentifier(Item.Name);

                                InitialisationStringList.Add($"{InitNameString} = {InitValueString};");
                            }
                        }

                        ExportStatusText = CSharpNames.ComposedExportStatus(isOverride, false, false, exportStatus);
                        writer.WriteIndentedLine($"{ExportStatusText} {ResultType} {nameString}({ArgumentEntityList})");

                        AsEffectiveBody.WriteCSharp(writer, Flags, ResultString, false, InitialisationStringList);
                        isMultiline = true;
                        IsHandled = true;
                        break;

                    case ICSharpPrecursorBody AsPrecursorBody:
                        if (isMultiline)
                            writer.WriteEmptyLine();

                        ExportStatusText = CSharpNames.ComposedExportStatus(true, false, false, exportStatus);
                        writer.WriteIndentedLine($"{ExportStatusText} {ResultType} {nameString}({ArgumentEntityList})");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"return base.{nameString}({ArgumentNameList});");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        isMultiline = true;
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }
            else
            {
                writer.WriteIndentedLine($"{ResultType} {nameString}({ArgumentEntityList});");
                isMultiline = false;
            }
        }
        #endregion
    }
}
