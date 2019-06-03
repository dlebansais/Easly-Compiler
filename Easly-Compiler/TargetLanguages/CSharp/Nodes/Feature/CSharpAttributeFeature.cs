namespace EaslyCompiler
{
    using System.Collections.Generic;
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
        /// Initializes the feature.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Init(ICSharpContext context)
        {
            Type = CSharpType.Create(context, Source.ResolvedEntityType.Item);

            foreach (IAssertion Assertion in Source.EnsureList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Assertion);
                EnsureList.Add(NewAssertion);
            }
        }

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            isMultiline = false;

            if (featureTextType == CSharpFeatureTextTypes.Implementation)
                WriteCSharpImplementation(writer, outputNamespace, exportStatus, ref isFirstFeature, ref isMultiline);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, string outputNamespace, CSharpExports exportStatus, ref bool isFirstFeature, ref bool isMultiline)
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

            string TypeString = Type.Type2CSharpString(outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string AttributeString = CSharpNames.ToCSharpIdentifier(Name);
            string ExportString = CSharpNames.ComposedExportStatus(false, false, true, exportStatus);

            if (IsEvent)
                writer.WriteIndentedLine($"{ExportString} event {TypeString} {AttributeString};");
            else if (Type.GetSingletonString(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None, out string SingletonString))
                writer.WriteIndentedLine($"{ExportString} {TypeString} {AttributeString} {{ get {{ return {SingletonString}; }} }}");
            else
                writer.WriteIndentedLine($"{ExportString} {TypeString} {AttributeString} {{ get; private set; }}");
        }
        #endregion
    }
}
