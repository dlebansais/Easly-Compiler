namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# procedure.
    /// </summary>
    public interface ICSharpProcedureFeature : ICSharpFeature<IProcedureFeature>, ICSharpRoutineFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IProcedureFeature Source { get; }

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
        /// The precursor if any. Can be null.
        /// </summary>
        ICSharpProcedureFeature OriginalPrecursor { get; }
    }

    /// <summary>
    /// A C# procedure.
    /// </summary>
    public class CSharpProcedureFeature : CSharpFeature<IProcedureFeature>, ICSharpProcedureFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# procedure.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpProcedureFeature Create(ICSharpClass owner, IFeatureInstance instance, IProcedureFeature source)
        {
            return new CSharpProcedureFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpProcedureFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpProcedureFeature(ICSharpClass owner, IFeatureInstance instance, IProcedureFeature source)
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
        /// The list of overloads.
        /// </summary>
        public IList<ICSharpOverload> OverloadList { get; } = new List<ICSharpOverload>();

        /// <summary>
        /// The precursor if any. Can be null.
        /// </summary>
        public ICSharpProcedureFeature OriginalPrecursor { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            foreach (ICommandOverload Overload in Source.OverloadList)
            {
                ICSharpCommandOverload NewOverload = CSharpCommandOverload.Create(context, Overload, this, Owner);
                OverloadList.Add(NewOverload);
            }
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

                OriginalPrecursor = context.GetFeature(PrecursorFeature) as ICSharpProcedureFeature;
                Debug.Assert(OriginalPrecursor != null);

                IList<ICSharpOverload> PrecursorOverloadList = OriginalPrecursor.OverloadList;

                foreach (ICSharpCommandOverload Overload in OverloadList)
                {
                    ICommandOverloadType ResolvedAssociatedType = Overload.Source.ResolvedAssociatedType.Item;
                    ICSharpCommandOverload ParentPrecursorOverload = null;

                    foreach (ICSharpCommandOverload PrecursorOverload in PrecursorOverloadList)
                    {
                        ICommandOverloadType PrecursorResolvedAssociatedType = PrecursorOverload.Source.ResolvedAssociatedType.Item;

                        if (ObjectType.CommandOverloadConformToBase(ResolvedAssociatedType, PrecursorResolvedAssociatedType, ErrorList.Ignored, ErrorList.NoLocation))
                        {
                            Debug.Assert(ParentPrecursorOverload == null);
                            ParentPrecursorOverload = PrecursorOverload;
                        }
                    }

                    Debug.Assert(ParentPrecursorOverload != null);
                    Overload.SetPrecursor(ParentPrecursorOverload);
                }
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

            outgoingParameterCount = 0;
            returnValueIndex = -1;
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

            writer.WriteDocumentation(Source);

            string NameString = CSharpNames.ToCSharpIdentifier(Name);

            foreach (ICSharpCommandOverload Overload in OverloadList)
                Overload.WriteCSharp(writer, featureTextType, IsOverride, NameString, exportStatus, false, ref isFirstFeature, ref isMultiline);
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

            foreach (ICSharpOverload Overload in OverloadList)
                Overload.SetWriteDown();
        }
        #endregion
    }
}
