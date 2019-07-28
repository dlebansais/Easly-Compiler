namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# indexer.
    /// </summary>
    public interface ICSharpIndexerFeature : ICSharpFeature<IIndexerFeature>, ICSharpFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IIndexerFeature Source { get; }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        new ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        new IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        new bool IsOverride { get; }

        /// <summary>
        /// True if this feature must be both read and write.
        /// </summary>
        bool IsForcedReadWrite { get; }

        /// <summary>
        /// Type of the entity returned by the indexer.
        /// </summary>
        ICSharpType EntityType { get; }

        /// <summary>
        /// The list of parameters.
        /// </summary>
        IList<ICSharpParameter> IndexParameterList { get; }

        /// <summary>
        /// Body of the getter. Can be null.
        /// </summary>
        ICSharpBody GetterBody { get; }

        /// <summary>
        /// Body of the setter. Can be null.
        /// </summary>
        ICSharpBody SetterBody { get; }

        /// <summary>
        /// The precursor if any. Can be null.
        /// </summary>
        ICSharpIndexerFeature OriginalPrecursor { get; }

        /// <summary>
        /// Mark this feature as both read and write.
        /// </summary>
        void MarkAsForcedReadWrite();
    }

    /// <summary>
    /// A C# indexer.
    /// </summary>
    public class CSharpIndexerFeature : CSharpFeature<IIndexerFeature>, ICSharpIndexerFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# indexer.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpIndexerFeature Create(ICSharpClass owner, IFeatureInstance instance, IIndexerFeature source)
        {
            return new CSharpIndexerFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexerFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpIndexerFeature(ICSharpClass owner, IFeatureInstance instance, IIndexerFeature source)
            : base(owner, instance, source)
        {
        }
        #endregion

        #region Properties
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance; } }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// True if this feature must be both read and write.
        /// </summary>
        public bool IsForcedReadWrite { get; private set; }

        /// <summary>
        /// Type of the entity returned by the indexer.
        /// </summary>
        public ICSharpType EntityType { get; private set; }

        /// <summary>
        /// The list of parameters.
        /// </summary>
        public IList<ICSharpParameter> IndexParameterList { get; } = new List<ICSharpParameter>();

        /// <summary>
        /// Body of the getter. Can be null.
        /// </summary>
        public ICSharpBody GetterBody { get; private set; }

        /// <summary>
        /// Body of the setter. Can be null.
        /// </summary>
        public ICSharpBody SetterBody { get; private set; }

        /// <summary>
        /// The precursor if any. Can be null.
        /// </summary>
        public ICSharpIndexerFeature OriginalPrecursor { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            EntityType = CSharpType.Create(context, Source.ResolvedEntityType.Item);

            foreach (IParameter Parameter in Source.ParameterTable)
            {
                ICSharpParameter NewParameter = CSharpParameter.Create(context, Parameter, Owner);
                IndexParameterList.Add(NewParameter);
            }

            if (Source.GetterBody.IsAssigned)
                GetterBody = CSharpBody.Create(context, this, (ICompiledBody)Source.GetterBody.Item);

            if (Source.SetterBody.IsAssigned)
                SetterBody = CSharpBody.Create(context, this, (ICompiledBody)Source.SetterBody.Item);
        }

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitHierarchy(ICSharpContext context)
        {
            if (Instance.OriginalPrecursor.IsAssigned)
            {
                IPrecursorInstance Item = Instance.OriginalPrecursor.Item;
                ICompiledFeature PrecursorFeature = Item.Precursor.Feature;

                OriginalPrecursor = context.GetFeature(PrecursorFeature) as ICSharpIndexerFeature;
                Debug.Assert(OriginalPrecursor != null);
            }
        }

        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="selectedOverloadType">The selected overload type.</param>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        public override void GetOutputFormat(ICSharpQueryOverloadType selectedOverloadType, out int outgoingParameterCount, out int returnValueIndex)
        {
            Debug.Assert(selectedOverloadType == null);

            outgoingParameterCount = 1;
            returnValueIndex = 0;
        }

        /// <summary>
        /// Mark this feature as both read and write.
        /// </summary>
        public void MarkAsForcedReadWrite()
        {
            Debug.Assert(!IsForcedReadWrite);

            IsForcedReadWrite = true;
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public override void CheckNumberType(ref bool isChanged)
        {
            //TODO
        }

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public override void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            if (!WriteDown)
                return;

            bool IsHandled = false;

            switch (featureTextType)
            {
                case CSharpFeatureTextTypes.Implementation:
                    WriteCSharpImplementation(writer, exportStatus, isLocal, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case CSharpFeatureTextTypes.Interface:
                    WriteCSharpInterface(writer, exportStatus, isLocal, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            writer.WriteDocumentation(Source);

            string ResultType = EntityType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            CSharpArgument.BuildParameterList(writer, IndexParameterList, out string ParameterEntityList, out string ParameterNameList);

            string Accessors;
            if (!IsForcedReadWrite && SetterBody == null)
                Accessors = "{ get; }";

            else if (!IsForcedReadWrite && GetterBody == null)
                Accessors = "{ set; }";

            else
                Accessors = "{ get; set; }";

            bool IsDeferred = false;
            if (GetterBody != null)
                if (GetterBody is ICSharpDeferredBody)
                    IsDeferred = true;
                else
                    IsDeferred = false;
            else if (SetterBody != null)
                if (SetterBody is ICSharpDeferredBody)
                    IsDeferred = true;
                else
                    IsDeferred = false;

            if (IsDeferred)
            {
                if (GetterBody is ICSharpDeferredBody AsDeferredGetterBody)
                {
                    CSharpAssertion.WriteContract(writer, AsDeferredGetterBody.RequireList, AsDeferredGetterBody.EnsureList, CSharpContractLocations.Getter, false, ref isFirstFeature, ref isMultiline);
                    isMultiline = false;
                }

                if (SetterBody is ICSharpDeferredBody AsDeferredSetterBody)
                {
                    CSharpAssertion.WriteContract(writer, AsDeferredSetterBody.RequireList, AsDeferredSetterBody.EnsureList, CSharpContractLocations.Setter, false, ref isFirstFeature, ref isMultiline);
                }

                string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                writer.WriteIndentedLine($"{ExportStatusText} {ResultType} this[{ParameterEntityList}] {Accessors}");

                isMultiline = false;
            }
            else
            {
                string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);

                writer.WriteIndentedLine($"{ExportStatusText} {ResultType} this[{ParameterEntityList}]");
                writer.WriteIndentedLine("{");
                writer.IncreaseIndent();

                bool IsPrecursor = false;
                if (GetterBody != null)
                    if (GetterBody is ICSharpPrecursorBody)
                        IsPrecursor = true;
                    else
                        IsPrecursor = false;
                else if (SetterBody != null)
                    if (GetterBody is ICSharpPrecursorBody)
                        IsPrecursor = true;
                    else
                        IsPrecursor = false;

                if (IsPrecursor)
                {
                    if (GetterBody is ICSharpPrecursorBody AsPrecursorGetterBody)
                    {
                        isMultiline = false;

                        CSharpAssertion.WriteContract(writer, AsPrecursorGetterBody.RequireList, AsPrecursorGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);
                        writer.WriteIndentedLine("get");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"return base[{ParameterEntityList}];");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }
                    else if (IsForcedReadWrite)
                        writer.WriteIndentedLine("get { throw new InvalidOperationException(); }");

                    if (SetterBody is ICSharpPrecursorBody AsPrecursorSetterBody)
                    {
                        isMultiline = false;

                        CSharpAssertion.WriteContract(writer, AsPrecursorSetterBody.RequireList, AsPrecursorSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);
                        writer.WriteIndentedLine("set");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"base[{ParameterEntityList}] = value;");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }

                    else if (IsForcedReadWrite)
                        writer.WriteIndentedLine("set { throw new InvalidOperationException(); }");
                }
                else
                {
                    if (GetterBody != null)
                    {
                        if (GetterBody is ICSharpEffectiveBody AsEffectiveGetterBody)
                        {
                            isMultiline = false;

                            CSharpAssertion.WriteContract(writer, AsEffectiveGetterBody.RequireList, AsEffectiveGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);
                            writer.WriteIndentedLine("get");
                            AsEffectiveGetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasResult, ResultType, false, new List<string>());
                        }
                    }
                    else if (IsForcedReadWrite)
                        writer.WriteIndentedLine("get { throw new InvalidOperationException(); }");

                    if (SetterBody != null)
                    {
                        if (SetterBody is ICSharpEffectiveBody AsEffectiveSetterBody)
                        {
                            isMultiline = false;

                            CSharpAssertion.WriteContract(writer, AsEffectiveSetterBody.RequireList, AsEffectiveSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);
                            writer.WriteIndentedLine("set");
                            AsEffectiveSetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasValue, string.Empty, false, new List<string>());
                        }
                    }
                    else if (IsForcedReadWrite)
                        writer.WriteIndentedLine("set { throw new InvalidOperationException(); }");
                }

                writer.DecreaseIndent();
                writer.WriteIndentedLine("}");

                isMultiline = true;
            }
        }

        private void WriteCSharpInterface(ICSharpWriter writer, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            isMultiline = false;

            string ResultType = EntityType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            CSharpArgument.BuildParameterList(writer, IndexParameterList, out string ParameterEntityList, out string ParameterNameList);

            string Accessors = "{ get; }";

            writer.WriteIndentedLine($"{ResultType} this[{ParameterEntityList}] {Accessors}");
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            if (GetterBody is ICSharpEffectiveBody AsEffectiveGetterBody)
                AsEffectiveGetterBody.SetWriteDown();

            if (SetterBody is ICSharpEffectiveBody AsEffectiveSetterBody)
                AsEffectiveSetterBody.SetWriteDown();
        }
        #endregion
    }
}
