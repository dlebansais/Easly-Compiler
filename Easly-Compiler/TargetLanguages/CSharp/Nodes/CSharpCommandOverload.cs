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
        /// The precursor overload. Can be null.
        /// </summary>
        new ICSharpCommandOverload Precursor { get; }

        /// <summary>
        /// The overload body.
        /// </summary>
        ICSharpBody Body { get; }

        /// <summary>
        /// Sets the precursor.
        /// </summary>
        /// <param name="precursor">The precursor.</param>
        void SetPrecursor(ICSharpCommandOverload precursor);
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
        public ICSharpCommandOverload Precursor { get; private set; }
        ICSharpOverload ICSharpOverload.Precursor { get { return Precursor; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the precursor.
        /// </summary>
        /// <param name="precursor">The precursor.</param>
        public virtual void SetPrecursor(ICSharpCommandOverload precursor)
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
            Debug.Assert(WriteDown);

            bool IsHandled = false;

            switch (featureTextType)
            {
                case CSharpFeatureTextTypes.Implementation:
                    WriteCSharpImplementation(writer, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case CSharpFeatureTextTypes.Interface:
                    WriteCSharpInterface(writer, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsHandled = false;

            switch (Body)
            {
                case ICSharpDeferredBody AsDeferredBody:
                    WriteCSharpImplementationDeferred(writer, AsDeferredBody, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case ICSharpEffectiveBody AsEffectiveBody:
                    WriteCSharpImplementationEffective(writer, AsEffectiveBody, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case ICSharpPrecursorBody AsPrecursorBody:
                    WriteCSharpImplementationPrecursor(writer, AsPrecursorBody, isOverride, nameString, exportStatus, isConstructor, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementationDeferred(ICSharpWriter writer, ICSharpDeferredBody deferredBody, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(writer, ParameterList, out string ParameterEntityList, out string ParameterNameList);

            CSharpAssertion.WriteContract(writer, deferredBody.RequireList, deferredBody.EnsureList, CSharpContractLocations.Other, true, ref isFirstFeature, ref isMultiline);

            string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
            writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList});");

            isMultiline = false;
        }

        private void WriteCSharpImplementationEffective(ICSharpWriter writer, ICSharpEffectiveBody effectiveBody, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(writer, ParameterList, out string ParameterEntityList, out string ParameterNameList);

            CSharpAssertion.WriteContract(writer, effectiveBody.RequireList, effectiveBody.EnsureList, CSharpContractLocations.Other, true, ref isFirstFeature, ref isMultiline);

            bool SkipFirstInstruction = false;

            if (isConstructor)
                WriteCSharpImplementationConstructor(writer, isOverride, nameString, exportStatus, ref isFirstFeature, ref isMultiline, ref SkipFirstInstruction);
            else
            {
                string ExportStatusText = CSharpNames.ComposedExportStatus(isOverride, false, false, exportStatus);

                writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList})");
            }

            ICSharpEffectiveBody AsEffectiveBody = Body as ICSharpEffectiveBody;
            AsEffectiveBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets, string.Empty, SkipFirstInstruction, new List<string>());

            isMultiline = true;
        }

        private void WriteCSharpImplementationConstructor(ICSharpWriter writer, bool isOverride, string nameString, CSharpExports exportStatus, ref bool isFirstFeature, ref bool isMultiline, ref bool skipFirstInstruction)
        {
            CSharpArgument.BuildParameterList(writer, ParameterList, out string ParameterEntityList, out string ParameterNameList);

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
                                    ISealableList<Parameter> SelectedParameterList = SelectedOverload.ParameterTable;
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

        private void WriteCSharpImplementationPrecursor(ICSharpWriter writer, ICSharpPrecursorBody precursorBody, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(writer, ParameterList, out string ParameterEntityList, out string ParameterNameList);

            if (isMultiline)
                writer.WriteEmptyLine();

            string ExportStatusText = CSharpNames.ComposedExportStatus(true, false, false, exportStatus);
            writer.WriteIndentedLine($"{ExportStatusText} void {nameString}({ParameterEntityList})");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine($"base.{nameString}({ParameterNameList});");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            isMultiline = true;
        }

        private void WriteCSharpInterface(ICSharpWriter writer, bool isOverride, string nameString, CSharpExports exportStatus, bool isConstructor, ref bool isFirstFeature, ref bool isMultiline)
        {
            CSharpArgument.BuildParameterList(writer, ParameterList, out string ParameterEntityList, out string ParameterNameList);

            if (!isConstructor)
            {
                writer.WriteIndentedLine($"void {nameString}({ParameterEntityList});");
                isMultiline = false;
            }
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        public bool WriteDown { get; private set; }

        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            foreach (ICSharpParameter Parameter in ParameterList)
                if (Parameter.Feature.DefaultValue != null)
                    Parameter.Feature.DefaultValue.SetWriteDown();

            if (Body is ICSharpEffectiveBody AsEffectiveBody)
                AsEffectiveBody.SetWriteDown();
        }
        #endregion
    }
}
