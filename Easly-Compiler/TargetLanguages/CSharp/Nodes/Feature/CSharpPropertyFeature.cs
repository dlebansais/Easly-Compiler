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
        #endregion

        #region Client Interface
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
                    ICompiledFeature PrecursorFeature = Item.Precursor.Feature.Item;

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
        #endregion
    }
}
