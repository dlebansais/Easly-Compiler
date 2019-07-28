namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# attribute.
    /// </summary>
    public interface ICSharpAttributeFeature : ICSharpFeature<IAttributeFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IAttributeFeature Source { get; }

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
        /// The attribute type.
        /// </summary>
        ICSharpType Type { get; }

        /// <summary>
        /// The list of ensure C# assertions.
        /// </summary>
        IList<ICSharpAssertion> EnsureList { get; }
    }

    /// <summary>
    /// A C# attribute.
    /// </summary>
    public class CSharpAttributeFeature : CSharpFeature<IAttributeFeature>, ICSharpAttributeFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# attribute.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpAttributeFeature Create(ICSharpClass owner, IFeatureInstance instance, IAttributeFeature source)
        {
            return new CSharpAttributeFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAttributeFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpAttributeFeature(ICSharpClass owner, IFeatureInstance instance, IAttributeFeature source)
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
        /// The attribute type.
        /// </summary>
        public ICSharpType Type { get; private set; }

        /// <summary>
        /// The list of ensure C# assertions.
        /// </summary>
        public IList<ICSharpAssertion> EnsureList { get; } = new List<ICSharpAssertion>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            Type = CSharpType.Create(context, Source.ResolvedEntityType.Item);

            foreach (IAssertion Assertion in Source.EnsureList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Assertion);
                EnsureList.Add(NewAssertion);
            }
        }

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitHierarchy(ICSharpContext context)
        {
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

            isMultiline = false;

            if (featureTextType == CSharpFeatureTextTypes.Implementation)
                WriteCSharpImplementation(writer, exportStatus, ref isFirstFeature, ref isMultiline);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, CSharpExports exportStatus, ref bool isFirstFeature, ref bool isMultiline)
        {
            bool IsEvent = false;

            if (Type is ICSharpClassType AsClassType)
            {
                ICSharpClass Class = AsClassType.Class;
                if (Class.InheritFromDotNetEvent)
                    IsEvent = true;
            }

            writer.WriteDocumentation(Source);

            CSharpAssertion.WriteContract(writer, new List<ICSharpAssertion>(), EnsureList, CSharpContractLocations.Other, false, ref isFirstFeature, ref isMultiline);

            string TypeString = Type.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string AttributeString = CSharpNames.ToCSharpIdentifier(Name);
            string ExportString = CSharpNames.ComposedExportStatus(false, false, true, exportStatus);

            if (IsEvent)
                writer.WriteIndentedLine($"{ExportString} event {TypeString} {AttributeString};");
            else if (Type.GetSingletonString(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None, out string SingletonString))
                writer.WriteIndentedLine($"{ExportString} {TypeString} {AttributeString} {{ get {{ return {SingletonString}; }} }}");
            else if (EnsureList.Count > 0)
            {
                writer.WriteIndentedLine($"{ExportString} {TypeString} {AttributeString} {{ get; private set; }}");
                writer.WriteIndentedLine($"protected void Set_{AttributeString}({TypeString} value)");
                writer.WriteIndentedLine("{");
                writer.IncreaseIndent();

                writer.WriteIndentedLine($"{AttributeString} = value;");

                writer.WriteEmptyLine();
                foreach (ICSharpAssertion Assertion in EnsureList)
                    Assertion.WriteCSharp(writer);

                writer.DecreaseIndent();
                writer.WriteIndentedLine("}");
            }
            else
                writer.WriteIndentedLine($"{ExportString} {TypeString} {AttributeString} {{ get; protected set; }}");
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

            foreach (ICSharpAssertion Assertion in EnsureList)
                Assertion.SetWriteDown();
        }
        #endregion
    }
}
