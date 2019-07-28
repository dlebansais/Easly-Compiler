namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# property.
    /// </summary>
    public interface ICSharpPropertyFeature : ICSharpFeature<IPropertyFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IPropertyFeature Source { get; }

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
        /// The property should declare a side-by-side private field of the same type.
        /// </summary>
        bool HasSideBySideAttribute { get; }

        /// <summary>
        /// Type of the entity returned by the property.
        /// </summary>
        ICSharpType EntityType { get; }

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
        ICSharpPropertyFeature OriginalPrecursor { get; }

        /// <summary>
        /// Mark this feature as both read and write.
        /// </summary>
        void MarkAsForcedReadWrite();

        /// <summary>
        /// Checks if a property should declare a side-by-side private field of the same type.
        /// </summary>
        void CheckSideBySideAttribute();

        /// <summary>
        /// Checks if a property should declare a side-by-side private field of the same type due to inherited ancestors.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        void CheckInheritSideBySideAttribute(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable);
    }

    /// <summary>
    /// A C# property.
    /// </summary>
    public class CSharpPropertyFeature : CSharpFeature<IPropertyFeature>, ICSharpPropertyFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# property.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpPropertyFeature Create(ICSharpClass owner, IFeatureInstance instance, IPropertyFeature source)
        {
            return new CSharpPropertyFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPropertyFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpPropertyFeature(ICSharpClass owner, IFeatureInstance instance, IPropertyFeature source)
            : base(owner, instance, source)
        {
            Name = Source.ValidFeatureName.Item.Name;
        }
        #endregion

        #region Properties
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance; } }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// True if this feature must be both read and write.
        /// </summary>
        public bool IsForcedReadWrite { get; private set; }

        /// <summary>
        /// The property should declare a side-by-side private field of the same type.
        /// </summary>
        public bool HasSideBySideAttribute { get; private set; }

        /// <summary>
        /// Type of the entity returned by the property.
        /// </summary>
        public ICSharpType EntityType { get; private set; }

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
        public ICSharpPropertyFeature OriginalPrecursor { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            EntityType = CSharpType.Create(context, Source.ResolvedEntityType.Item);

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

                OriginalPrecursor = context.GetFeature(PrecursorFeature) as ICSharpPropertyFeature;
                Debug.Assert(OriginalPrecursor != null);
            }
        }

        /// <summary>
        /// Check number types.
        /// </summary>
        /// <param name="isChanged">True upon return if a number type was changed.</param>
        public override void CheckNumberType(ref bool isChanged)
        {
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
        /// Checks if a property should declare a side-by-side private field of the same type.
        /// </summary>
        public void CheckSideBySideAttribute()
        {
            if (IsForcedReadWrite)
                CheckSideBySideAttributeOnForcedReadWrite();
            else
                CheckSideBySideAttributeNormal();
        }

        private void CheckSideBySideAttributeOnForcedReadWrite()
        {
            if (Source.GetterBody.IsAssigned)
                CheckSideBySideAttributeFromBody((IBody)Source.GetterBody.Item);
            else if (Source.SetterBody.IsAssigned)
                CheckSideBySideAttributeFromBody((IBody)Source.SetterBody.Item);
            else
                HasSideBySideAttribute = true;
        }

        private void CheckSideBySideAttributeNormal()
        {
            bool IsHandled = false;

            switch (Source.PropertyKind)
            {
                case BaseNode.UtilityType.ReadOnly:
                    if (Source.GetterBody.IsAssigned)
                        CheckSideBySideAttributeFromBody((IBody)Source.GetterBody.Item);
                    else
                        HasSideBySideAttribute = false;

                    IsHandled = true;
                    break;

                case BaseNode.UtilityType.WriteOnly:
                    if (Source.SetterBody.IsAssigned)
                        CheckSideBySideAttributeFromBody((IBody)Source.SetterBody.Item);
                    else
                        HasSideBySideAttribute = false;

                    IsHandled = true;
                    break;

                case BaseNode.UtilityType.ReadWrite:
                    if (Source.GetterBody.IsAssigned)
                        CheckSideBySideAttributeFromBody((IBody)Source.GetterBody.Item);
                    else if (Source.SetterBody.IsAssigned)
                        CheckSideBySideAttributeFromBody((IBody)Source.SetterBody.Item);
                    else
                        HasSideBySideAttribute = false;

                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void CheckSideBySideAttributeFromBody(IBody body)
        {
            bool IsHandled = false;

            switch (body)
            {
                case IDeferredBody AsDeferredBody:
                case IExternBody AsExternBody:
                case IPrecursorBody AsPrecursorBody:
                    HasSideBySideAttribute = false;
                    IsHandled = true;
                    break;

                case IEffectiveBody AsEffectiveBody:
                    HasSideBySideAttribute = true;
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a property should declare a side-by-side private field of the same type due to inherited ancestors.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        public void CheckInheritSideBySideAttribute(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable)
        {
            if (HasSideBySideAttribute)
                foreach (IPrecursorInstance Item in Instance.PrecursorList)
                {
                    ICompiledFeature PrecursorFeature = Item.Precursor.Feature;

                    Debug.Assert(globalFeatureTable.ContainsKey(PrecursorFeature));
                    ICSharpPropertyFeature PrecursorPropertyFeature = globalFeatureTable[PrecursorFeature] as ICSharpPropertyFeature;
                    Debug.Assert(PrecursorPropertyFeature != null);

                    if (IsOverride && PrecursorPropertyFeature.HasSideBySideAttribute)
                    {
                        Instance.SetInheritBySideAttribute(true);
                        break;
                    }
                }
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
            bool IsHandled = false;

            switch (featureTextType)
            {
                case CSharpFeatureTextTypes.Implementation:
                    WriteCSharpImplementation(writer, featureTextType, exportStatus, isLocal, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;

                case CSharpFeatureTextTypes.Interface:
                    WriteCSharpInterface(writer, featureTextType, exportStatus, isLocal, ref isFirstFeature, ref isMultiline);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            writer.WriteDocumentation(Source);

            string ResultType = EntityType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string PropertyName = CSharpNames.ToCSharpIdentifier(Name);
            bool IsHandled = false;

            if (IsForcedReadWrite)
            {
                WriteCSharpForcedReadWriteProperty(writer, IsOverride, exportStatus, PropertyName, ResultType, ref isFirstFeature, ref isMultiline);
                IsHandled = true;
            }
            else
                switch (Source.PropertyKind)
                {
                    case BaseNode.UtilityType.ReadOnly:
                        WriteCSharpReadOnlyProperty(writer, IsOverride, exportStatus, PropertyName, ResultType, ref isFirstFeature, ref isMultiline);
                        IsHandled = true;
                        break;

                    case BaseNode.UtilityType.WriteOnly:
                        WriteCSharpWriteOnlyProperty(writer, IsOverride, exportStatus, PropertyName, ResultType, ref isFirstFeature, ref isMultiline);
                        IsHandled = true;
                        break;

                    case BaseNode.UtilityType.ReadWrite:
                        WriteCSharpReadWriteProperty(writer, IsOverride, exportStatus, PropertyName, ResultType, ref isFirstFeature, ref isMultiline);
                        IsHandled = true;
                        break;
                }

            Debug.Assert(IsHandled);

            if (HasSideBySideAttribute && !Instance.InheritBySideAttribute)
            {
                writer.WriteIndentedLine($"protected {ResultType} _{PropertyName};");

                isMultiline = true;
            }
        }

        private void WriteCSharpInterface(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            string ResultType = EntityType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string PropertyName = CSharpNames.ToCSharpIdentifier(Name);
            string Accessors = null;

            if (IsForcedReadWrite)
                Accessors = "{ get; set; }";
            else
                switch (Source.PropertyKind)
                {
                    case BaseNode.UtilityType.ReadOnly:
                        Accessors = "{ get; }";
                        break;

                    case BaseNode.UtilityType.WriteOnly:
                        Accessors = "{ set; }";
                        break;

                    case BaseNode.UtilityType.ReadWrite:
                        Accessors = "{ get; set; }";
                        break;
                }

            Debug.Assert(Accessors != null);

            writer.WriteIndentedLine($"{ResultType} {PropertyName} {Accessors}");

            isMultiline = false;
        }

        private void WriteCSharpReadOnlyProperty(ICSharpWriter writer, bool isOverride, CSharpExports exportStatus, string propertyName, string resultType, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsDeferred = false;
            bool IsPrecursor = false;

            isFirstFeature = false;

            if (GetterBody != null)
            {
                if (GetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (GetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (GetterBody != null)
            {
                if (IsDeferred)
                {
                    ICSharpDeferredBody DeferredGetterBody = GetterBody as ICSharpDeferredBody;
                    Debug.Assert(DeferredGetterBody != null);

                    CSharpAssertion.WriteContract(writer, DeferredGetterBody.RequireList, DeferredGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                    string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ get; protected set; }}");
                }
                else
                {
                    if (isMultiline)
                        writer.WriteEmptyLine();

                    string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName}");

                    if (IsPrecursor)
                    {
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"get {{ return base.{propertyName}; }}");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }

                    else
                    {
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();

                        ICSharpEffectiveBody EffectiveGetterBody = GetterBody as ICSharpEffectiveBody;
                        Debug.Assert(EffectiveGetterBody != null);

                        isMultiline = false;
                        CSharpAssertion.WriteContract(writer, EffectiveGetterBody.RequireList, EffectiveGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                        writer.WriteIndentedLine("get");
                        EffectiveGetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasResult, resultType, false, new List<string>());

                        writer.WriteIndentedLine($"protected set {{ _{propertyName} = value; }}");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }

                    isMultiline = true;
                }
            }

            else
            {
                if (isMultiline)
                    writer.WriteEmptyLine();

                string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ get; protected set; }}");
                isMultiline = false;
            }
        }

        private void WriteCSharpWriteOnlyProperty(ICSharpWriter writer, bool isOverride, CSharpExports exportStatus, string propertyName, string resultType, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsDeferred = false;
            bool IsPrecursor = false;

            isFirstFeature = false;

            if (SetterBody != null)
            {
                if (SetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (SetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (SetterBody != null)
            {
                if (IsDeferred)
                {
                    ICSharpDeferredBody DeferredSetterBody = SetterBody as ICSharpDeferredBody;
                    Debug.Assert(DeferredSetterBody != null);

                    CSharpAssertion.WriteContract(writer, DeferredSetterBody.RequireList, DeferredSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                    string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ protected get; set; }}");

                    isMultiline = false;
                }
                else
                {
                    if (isMultiline)
                        writer.WriteEmptyLine();

                    string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName}");

                    if (IsPrecursor)
                    {
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"set {{ base.{propertyName} = value; }}");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }

                    else
                    {
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine($"protected get {{ return _{propertyName}; }}");

                        ICSharpEffectiveBody EffectiveSetterBody = SetterBody as ICSharpEffectiveBody;
                        Debug.Assert(EffectiveSetterBody != null);

                        isMultiline = false;
                        CSharpAssertion.WriteContract(writer, EffectiveSetterBody.RequireList, EffectiveSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                        writer.WriteIndentedLine("set");
                        EffectiveSetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasValue, string.Empty, false, new List<string>());

                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                    }

                    isMultiline = true;
                }
            }

            else
            {
                if (isMultiline)
                    writer.WriteEmptyLine();

                string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ protected get; set; }}");

                isMultiline = false;
            }
        }

        private void WriteCSharpReadWriteProperty(ICSharpWriter writer, bool isOverride, CSharpExports exportStatus, string propertyName, string resultType, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsDeferred = false;
            bool IsPrecursor = false;

            isFirstFeature = false;

            if (GetterBody != null)
            {
                if (GetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (GetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (SetterBody != null)
            {
                if (SetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (SetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (GetterBody != null || SetterBody != null)
            {
                if (IsDeferred)
                {
                    bool IsGetterMultiline = isMultiline;
                    bool IsSetterMultiline = isMultiline;

                    if (GetterBody != null)
                    {
                        ICSharpDeferredBody DeferredGetterBody = GetterBody as ICSharpDeferredBody;
                        Debug.Assert(DeferredGetterBody != null);

                        CSharpAssertion.WriteContract(writer, DeferredGetterBody.RequireList, DeferredGetterBody.EnsureList, CSharpContractLocations.Getter, false, ref isFirstFeature, ref IsGetterMultiline);
                        IsSetterMultiline = false;
                    }

                    if (SetterBody != null)
                    {
                        ICSharpDeferredBody DeferredSetterBody = SetterBody as ICSharpDeferredBody;
                        Debug.Assert(DeferredSetterBody != null);

                        CSharpAssertion.WriteContract(writer, DeferredSetterBody.RequireList, DeferredSetterBody.EnsureList, CSharpContractLocations.Setter, false, ref isFirstFeature, ref IsSetterMultiline);
                    }

                    string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ get; set; }}");

                    isMultiline = IsGetterMultiline || IsSetterMultiline;
                }
                else
                {
                    if (isMultiline)
                        writer.WriteEmptyLine();

                    string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName}");
                    writer.WriteIndentedLine("{");
                    writer.IncreaseIndent();

                    if (IsPrecursor)
                    {
                        writer.WriteIndentedLine($"get {{ return base.{propertyName}; }}");
                        writer.WriteIndentedLine($"set {{ base.{propertyName} = value; }}");
                    }
                    else
                    {
                        if (GetterBody != null)
                        {
                            ICSharpEffectiveBody EffectiveGetterBody = GetterBody as ICSharpEffectiveBody;
                            Debug.Assert(EffectiveGetterBody != null);

                            isMultiline = false;
                            CSharpAssertion.WriteContract(writer, EffectiveGetterBody.RequireList, EffectiveGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                            writer.WriteIndentedLine("get");
                            EffectiveGetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasResult, resultType, false, new List<string>());
                        }
                        else
                            writer.WriteIndentedLine($"get {{ return _{propertyName}; }}");

                        if (SetterBody != null)
                        {
                            ICSharpEffectiveBody EffectiveSetterBody = SetterBody as ICSharpEffectiveBody;
                            Debug.Assert(EffectiveSetterBody != null);

                            isMultiline = false;
                            CSharpAssertion.WriteContract(writer, EffectiveSetterBody.RequireList, EffectiveSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                            writer.WriteIndentedLine("set");
                            EffectiveSetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasValue, string.Empty, false, new List<string>());
                        }
                        else
                            writer.WriteIndentedLine($"set {{ _{propertyName} = value; }}");
                    }

                    writer.DecreaseIndent();
                    writer.WriteIndentedLine("}");
                    isMultiline = true;
                }
            }

            else
            {
                if (isMultiline)
                    writer.WriteEmptyLine();

                string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ get; set; }}");

                isMultiline = false;
            }
        }

        private void WriteCSharpForcedReadWriteProperty(ICSharpWriter writer, bool isOverride, CSharpExports exportStatus, string propertyName, string resultType, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsDeferred = false;
            bool IsPrecursor = false;

            isFirstFeature = false;

            if (GetterBody != null)
            {
                if (GetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (GetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (SetterBody != null)
            {
                if (SetterBody is ICSharpDeferredBody)
                    IsDeferred = true;

                if (SetterBody is ICSharpPrecursorBody)
                    IsPrecursor = true;
            }

            if (GetterBody != null || SetterBody != null)
            {
                if (IsDeferred)
                {
                    bool IsGetterMultiline = isMultiline;
                    bool IsSetterMultiline = isMultiline;

                    if (GetterBody != null)
                    {
                        ICSharpDeferredBody DeferredGetterBody = GetterBody as ICSharpDeferredBody;
                        Debug.Assert(DeferredGetterBody != null);

                        CSharpAssertion.WriteContract(writer, DeferredGetterBody.RequireList, DeferredGetterBody.EnsureList, CSharpContractLocations.Getter, false, ref isFirstFeature, ref IsGetterMultiline);
                        IsSetterMultiline = false;
                    }

                    if (SetterBody != null)
                    {
                        ICSharpDeferredBody DeferredSetterBody = SetterBody as ICSharpDeferredBody;
                        Debug.Assert(DeferredSetterBody != null);

                        CSharpAssertion.WriteContract(writer, DeferredSetterBody.RequireList, DeferredSetterBody.EnsureList, CSharpContractLocations.Setter, false, ref isFirstFeature, ref IsSetterMultiline);
                    }

                    string ExportStatusText = CSharpNames.ComposedExportStatus(false, true, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName} {{ get; set; }}");

                    isMultiline = IsGetterMultiline || IsSetterMultiline;
                }
                else
                {
                    if (isMultiline)
                        writer.WriteEmptyLine();

                    string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                    writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName}");
                    writer.WriteIndentedLine("{");
                    writer.IncreaseIndent();

                    if (IsPrecursor)
                    {
                        if (Source.PropertyKind != BaseNode.UtilityType.WriteOnly)
                            writer.WriteIndentedLine($"get {{ return base.{propertyName}; }}");
                        else
                            writer.WriteIndentedLine($"get {{ throw new InvalidOperationException(); }}");

                        if (Source.PropertyKind != BaseNode.UtilityType.ReadOnly)
                            writer.WriteIndentedLine($"set {{ base.{propertyName} = value; }}");
                        else
                            writer.WriteIndentedLine($"set {{ throw new InvalidOperationException(); }}");
                    }
                    else
                    {
                        if (GetterBody != null)
                        {
                            ICSharpEffectiveBody EffectiveGetterBody = GetterBody as ICSharpEffectiveBody;
                            Debug.Assert(EffectiveGetterBody != null);

                            isMultiline = false;
                            CSharpAssertion.WriteContract(writer, EffectiveGetterBody.RequireList, EffectiveGetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                            writer.WriteIndentedLine("get");
                            EffectiveGetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasResult, resultType, false, new List<string>());
                        }
                        else
                            writer.WriteIndentedLine($"get {{ throw new InvalidOperationException(); }}");

                        if (SetterBody != null)
                        {
                            ICSharpEffectiveBody EffectiveSetterBody = SetterBody as ICSharpEffectiveBody;
                            Debug.Assert(EffectiveSetterBody != null);

                            isMultiline = false;
                            CSharpAssertion.WriteContract(writer, EffectiveSetterBody.RequireList, EffectiveSetterBody.EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

                            writer.WriteIndentedLine("set");
                            EffectiveSetterBody.WriteCSharp(writer, CSharpBodyFlags.MandatoryCurlyBrackets | CSharpBodyFlags.HasValue, string.Empty, false, new List<string>());
                        }
                        else
                            writer.WriteIndentedLine($"set {{ throw new InvalidOperationException(); }}");
                    }

                    writer.DecreaseIndent();
                    writer.WriteIndentedLine("}");
                    isMultiline = true;
                }
            }

            else
            {
                if (isMultiline)
                    writer.WriteEmptyLine();

                string ExportStatusText = CSharpNames.ComposedExportStatus(IsOverride, false, false, exportStatus);
                writer.WriteIndentedLine($"{ExportStatusText} {resultType} {propertyName}");
                writer.WriteIndentedLine("{");
                writer.IncreaseIndent();

                if (Source.PropertyKind != BaseNode.UtilityType.WriteOnly)
                    writer.WriteIndentedLine($"get {{ return _{propertyName}; }}");
                else
                    writer.WriteIndentedLine($"get {{ throw new InvalidOperationException(); }}");

                if (Source.PropertyKind != BaseNode.UtilityType.ReadOnly)
                    writer.WriteIndentedLine($"set {{ _{propertyName} = value; }}");
                else
                    writer.WriteIndentedLine($"set {{ throw new InvalidOperationException(); }}");

                writer.DecreaseIndent();
                writer.WriteIndentedLine("}");
                isMultiline = true;
            }
        }
        #endregion
    }
}
