namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A C# feature.
    /// </summary>
    public interface ICSharpFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        ICompiledFeature Source { get; }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        bool IsOverride { get; }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        string CoexistingPrecursorName { get; }

        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        void InitOverloadsAndBodies(ICSharpContext context);

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        void InitHierarchy(ICSharpContext context);

        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        void MarkAsOverride();

        /// <summary>
        /// Gets the export status of a feature in the class that implements it.
        /// </summary>
        /// <param name="sourceClass">The class implementing the feature.</param>
        CSharpExports GetExportStatus(ICSharpClass sourceClass);

        /// <summary>
        /// Sets the <see cref="CoexistingPrecursorName"/> property.
        /// </summary>
        /// <param name="coexistingPrecursorName">The name of the precursor.</param>
        void MarkPrecursorAsCoexisting(string coexistingPrecursorName);

        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="selectedOverloadType">The selected overload type.</param>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        void GetOutputFormat(ICSharpQueryOverloadType selectedOverloadType, out int outgoingParameterCount, out int returnValueIndex);

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline);
    }

    /// <summary>
    /// A C# feature.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public interface ICSharpFeature<T> : ICSharpSource<T>
        where T : class, ICompiledFeature
    {
        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        bool IsOverride { get; }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        string CoexistingPrecursorName { get; }

        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        void InitOverloadsAndBodies(ICSharpContext context);

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        void InitHierarchy(ICSharpContext context);

        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        void MarkAsOverride();

        /// <summary>
        /// Gets the export status of a feature in the class that implements it.
        /// </summary>
        /// <param name="sourceClass">The class implementing the feature.</param>
        CSharpExports GetExportStatus(ICSharpClass sourceClass);

        /// <summary>
        /// Sets the <see cref="CoexistingPrecursorName"/> property.
        /// </summary>
        /// <param name="coexistingPrecursorName">The name of the precursor.</param>
        void MarkPrecursorAsCoexisting(string coexistingPrecursorName);
    }

    /// <summary>
    /// A C# feature.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public abstract class CSharpFeature<T> : CSharpSource<T>, ICSharpFeature<T>, ICSharpFeature
        where T : class, ICompiledFeature
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFeature{T}"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpFeature(ICSharpClass owner, T source)
            : base(source)
        {
            Debug.Assert(source is IScopeAttributeFeature);

            Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFeature{T}"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpFeature(ICSharpClass owner, IFeatureInstance instance, T source)
            : base(source)
        {
            Owner = owner;
            Instance = instance;

            Debug.Assert(Instance.Feature == Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        public ICSharpClass Owner { get; }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        public IFeatureInstance Instance { get; }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance ; } }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        public bool IsOverride { get; private set; }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// The name of the precursor if it's implemented as a separate feature in the same class. Can be null.
        /// </summary>
        public string CoexistingPrecursorName { get; private set; }
        string ICSharpFeature.CoexistingPrecursorName { get { return CoexistingPrecursorName; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public abstract void InitOverloadsAndBodies(ICSharpContext context);

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public abstract void InitHierarchy(ICSharpContext context);

        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="selectedOverloadType">The selected overload type.</param>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        public abstract void GetOutputFormat(ICSharpQueryOverloadType selectedOverloadType, out int outgoingParameterCount, out int returnValueIndex);

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public abstract void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline);

        /// <summary>
        /// Mark this feature as an override of a virtual parent.
        /// </summary>
        public void MarkAsOverride()
        {
            IsOverride = true;
        }

        /// <summary>
        /// Sets the <see cref="CoexistingPrecursorName"/> property.
        /// </summary>
        /// <param name="coexistingPrecursorName">The name of the precursor.</param>
        public void MarkPrecursorAsCoexisting(string coexistingPrecursorName)
        {
            CoexistingPrecursorName = coexistingPrecursorName;
        }

        /// <summary>
        /// Gets the export status of a feature in the class that implements it.
        /// </summary>
        /// <param name="sourceClass">The class implementing the feature.</param>
        public virtual CSharpExports GetExportStatus(ICSharpClass sourceClass)
        {
            bool IsExportedToClient;

            IFeature AsFeature = Source as IFeature;
            Debug.Assert(AsFeature != null);

            IIdentifier Identifier = (IIdentifier)AsFeature.ExportIdentifier;
            string ExportIdentifier = Identifier.ValidText.Item;

            if (ExportIdentifier == "All")
                IsExportedToClient = true;

            else if (ExportIdentifier == "None" || ExportIdentifier == "Self")
                IsExportedToClient = false;

            else
            {
                FeatureName.TableContain(Owner.Source.ExportTable, ExportIdentifier, out IFeatureName ExportName, out ISealableDictionary<string, IClass> ExportList);
                Debug.Assert(ExportList.Count > 0);

                if (ExportList.Count > 1)
                    IsExportedToClient = true;
                else
                {
                    if (ExportList.ContainsKey(sourceClass.ValidClassName))
                        IsExportedToClient = false; // Export to self = self + descendant = protected
                    else
                        IsExportedToClient = true; // export to another = export to all = public
                }
            }

            if (IsExportedToClient)
                return CSharpExports.Public;

            else if (AsFeature.Export == BaseNode.ExportStatus.Exported)
                return CSharpExports.Protected;
            else
                return CSharpExports.Private;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return Source.ToString();
        }
        #endregion
    }
}
