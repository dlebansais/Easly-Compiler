namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A C# command overload node.
    /// </summary>
    public interface ICSharpCommandOverload : ICSharpSource<ICommandOverload>, ICSharpOverload
    {
        /// <summary>
        /// The overload body.
        /// </summary>
        ICSharpBody Body { get; }
    }

    /// <summary>
    /// A C# command overload node.
    /// </summary>
    public class CSharpCommandOverload : CSharpSource<ICommandOverload>, ICSharpCommandOverload
    {
        #region Init
        /// <summary>
        /// Create a new C# overload.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        public static ICSharpCommandOverload Create(ICSharpContext context, ICommandOverload source, ICSharpFeature parentFeature, ICSharpClass owner)
        {
            return new CSharpCommandOverload(context, source, parentFeature, owner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCommandOverload"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="owner">The class where the overload is declared.</param>
        protected CSharpCommandOverload(ICSharpContext context, ICommandOverload source, ICSharpFeature parentFeature, ICSharpClass owner)
            : base(source)
        {
            ParentFeature = parentFeature;

            foreach (IParameter Parameter in source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Parameter, owner);
                ParameterList.Add(NewParameter);
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
        /// The overload body.
        /// </summary>
        public ICSharpBody Body { get; }

        /// <summary>
        /// The precursor overload. Can be null.
        /// </summary>
        public ICSharpOverload Precursor { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# overload of a feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="isOverride">True if the feature is an override.</param>
        /// <param name="nameString">The composed feature name.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isConstructor">True if the feature is a constructor.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpFeatureTextTypes featureTextType, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsHandled = false;

            switch (featureTextType)
            {
                case CSharpFeatureTextTypes.Implementation:
                    WriteCSharpImplementation(writer, outputNamespace, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case CSharpFeatureTextTypes.Interface:
                    WriteCSharpInterface(writer, outputNamespace, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsHandled = false;

            switch (Body)
            {
                case ICSharpDeferredBody AsDeferredBody:
                    WriteCSharpImplementationDeferred(writer, outputNamespace, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case ICSharpEffectiveBody AsEffectiveBody:
                    WriteCSharpImplementationEffective(writer, outputNamespace, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case ICSharpPrecursorBody AsPrecursorBody:
                    WriteCSharpImplementationPrecursor(writer, outputNamespace, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementationDeferred(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(ParameterList, outputNamespace, out string ParameterEntityList, out string ParameterNameList);

            //Assertion.WriteContract(sw, AsDeferredBody.RequireList, AsDeferredBody.EnsureList, ContractLocations.Other, true, ref IsFirstFeature, ref IsMultiline);

            string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
            writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList});");

            isMultiline = false;
        }

        private void WriteCSharpImplementationEffective(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(ParameterList, outputNamespace, out string ParameterEntityList, out string ParameterNameList);

            //Assertion.WriteContract(sw, AsEffectiveBody.RequireList, AsEffectiveBody.EnsureList, ContractLocations.Other, true, ref IsFirstFeature, ref IsMultiline);

            bool SkipFirstInstruction = false;

            if (isConstructor)
                WriteCSharpImplementationConstructor(writer, outputNamespace, isOverride, nameString, exportStatus, ref isFirstFeature, ref isMultiline, ref SkipFirstInstruction);
            else
            {
                string ExportStatusText = CSharpNames.ComposedExportStatus(isOverride, false, false, exportStatus);

                writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList})");
            }

            ICSharpEffectiveBody AsEffectiveBody = Body as ICSharpEffectiveBody;
            AsEffectiveBody.WriteCSharp(writer, outputNamespace, CSharpBodyFlags.MandatoryCurlyBrackets, string.Empty, SkipFirstInstruction, new List<string>());

            isMultiline = true;
        }

        private void WriteCSharpImplementationConstructor(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, ref bool isFirstFeature, ref bool isMultiline, ref bool skipFirstInstruction)
        {
            CSharpArgument.BuildParameterList(ParameterList, outputNamespace, out string ParameterEntityList, out string ParameterNameList);

            string ExportStatusText = CSharpNames.ComposedExportStatus(isOverride, false, true, exportStatus);

            writer.WriteIndentedLine($"{ExportStatusText} {nameString}({ParameterEntityList})");

            ICSharpEffectiveBody AsEffectiveBody = Body as ICSharpEffectiveBody;
            ICSharpClass Owner = ParentFeature.Owner;

            if (Owner.BaseClass != null)
            {
                ICSharpClass ParentClass = Owner.BaseClass;
                if (ParentClass.ClassConstructorType == CSharpConstructorTypes.OneConstructor)
                {
                    if (AsEffectiveBody.BodyInstructionList.Count > 0)
                    {
                        /*TODO
                        ICommandInstruction AsCommandInstruction;
                        if ((AsCommandInstruction = AsEffectiveBody.BodyInstructionList[0] as ICommandInstruction) != null)
                        {
                            if (AsCommandInstruction.SelectedFeature.IsAssigned)
                            {
                                ICreationFeature AsCreationFeature;
                                if ((AsCreationFeature = AsCommandInstruction.SelectedFeature.Item as ICreationFeature) != null)
                                {
                                    ICommandOverloadType SelectedOverload = AsCommandInstruction.SelectedOverload.Item;
                                    ListTableEx<Parameter> SelectedParameterList = SelectedOverload.ParameterTable;
                                    string ArgumentListString = CSharpRootOutput.CSharpArgumentList(Context, SelectedParameterList, AsCommandInstruction.ArgumentList, new List<IQualifiedName>());

                                    SkipFirstInstruction = true;
                                    CSharpRootOutput.IncreaseIndent();
                                    writer.WriteIndentedLine(":" + " " + "base" + "(" + ArgumentListString + ")");
                                    CSharpRootOutput.DecreaseIndent();
                                }
                            }
                        }
                        */
                    }
                }
            }
        }

        private void WriteCSharpImplementationPrecursor(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(ParameterList, outputNamespace, out string ParameterEntityList, out string ParameterNameList);

            if (isMultiline)
                writer.WriteLine();

            string ExportStatusText = CSharpNames.ComposedExportStatus(true, false, false, exportStatus);
            writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList})");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine($"base.{nameString}({ParameterNameList});");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            isMultiline = true;
        }

        private void WriteCSharpInterface(ICSharpWriter writer, string outputNamespace, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(ParameterList, outputNamespace, out string ParameterEntityList, out string ParameterNameList);

            if (!isConstructor)
            {
                writer.WriteIndentedLine($"void {nameString}({ParameterEntityList});");
                isMultiline = false;
            }
        }
        #endregion
    }
}
