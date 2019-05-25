namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// An instance of a feature in a class (direct or inherited).
    /// </summary>
    public interface IFeatureInstance
    {
        /// <summary>
        /// The class with the instance. Can be different than the class that defines the feature.
        /// </summary>
        OnceReference<IClass> Owner { get; }

        /// <summary>
        /// The feature.
        /// </summary>
        OnceReference<ICompiledFeature> Feature { get; }

        /// <summary>
        /// Inherited with a forget clause.
        /// </summary>
        bool IsForgotten { get; }

        /// <summary>
        /// Inherited with a keep clause.
        /// </summary>
        bool IsKept { get; }

        /// <summary>
        /// Inherited with a discontinue clause.
        /// </summary>
        bool IsDiscontinued { get; }

        /// <summary>
        /// Inherited from an effective body.
        /// </summary>
        bool InheritBySideAttribute { get; }

        /// <summary>
        /// List of precursors.
        /// </summary>
        IList<IPrecursorInstance> PrecursorList { get; }

        /// <summary>
        /// The first precursor in the inheritance tree.
        /// </summary>
        OnceReference<IPrecursorInstance> OriginalPrecursor { get; }

        /// <summary>
        /// Sets the <see cref="IsForgotten"/> flag.
        /// </summary>
        /// <param name="isForgotten">The new value.</param>
        void SetIsForgotten(bool isForgotten);

        /// <summary>
        /// Sets the <see cref="IsKept"/> flag.
        /// </summary>
        /// <param name="isKept">The new value.</param>
        void SetIsKept(bool isKept);

        /// <summary>
        /// Sets the <see cref="IsDiscontinued"/> flag.
        /// </summary>
        /// <param name="isDiscontinued">The new value.</param>
        void SetIsDiscontinued(bool isDiscontinued);

        /// <summary>
        /// Sets the <see cref="InheritBySideAttribute"/> flag.
        /// </summary>
        /// <param name="inheritBySideAttribute">The new value.</param>
        void SetInheritBySideAttribute(bool inheritBySideAttribute);

        /// <summary>
        /// Clones this instance using the specified ancestor.
        /// </summary>
        /// <param name="ancestor">The ancestor.</param>
        IFeatureInstance Clone(IClassType ancestor);

        /// <summary>
        /// Find the precursor, either the only one or the selected one.
        /// </summary>
        /// <param name="ancestorType">The optionally selected precursor.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="source">The source to use when reporting errors.</param>
        /// <param name="selectedPrecursor">The selected precursor upon return if successful.</param>
        bool FindPrecursor(IOptionalReference<BaseNode.IObjectType> ancestorType, IErrorList errorList, ISource source, out IFeatureInstance selectedPrecursor);
    }

    /// <summary>
    /// An instance of a feature in a class (direct or inherited).
    /// </summary>
    public class FeatureInstance : IFeatureInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInstance"/> class.
        /// </summary>
        /// <param name="owner">The class with the instance. Can be different than the class that defines the feature.</param>
        /// <param name="feature">The feature.</param>
        public FeatureInstance(IClass owner, ICompiledFeature feature)
            : this(owner, feature, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInstance"/> class.
        /// </summary>
        /// <param name="owner">The class with the instance. Can be different than the class that defines the feature.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="isKept">Inherited with a keep clause.</param>
        /// <param name="isDiscontinued">Inherited with a discontinue clause.</param>
        public FeatureInstance(IClass owner, ICompiledFeature feature, bool isKept, bool isDiscontinued)
        {
            // TODO: check if oncereference is really needed.
            Owner.Item = owner;
            Feature.Item = feature;
            IsForgotten = feature.IsDeferredFeature;
            IsKept = isKept;
            IsDiscontinued = isDiscontinued;
            InheritBySideAttribute = false;
        }

        /// <summary>
        /// The class with the instance. Can be different than the class that defines the feature.
        /// </summary>
        public OnceReference<IClass> Owner { get; } = new OnceReference<IClass>();

        /// <summary>
        /// The feature.
        /// </summary>
        public OnceReference<ICompiledFeature> Feature { get; } = new OnceReference<ICompiledFeature>();

        /// <summary>
        /// Inherited with a forget clause.
        /// </summary>
        public bool IsForgotten { get; private set; }

        /// <summary>
        /// Inherited with a keep clause.
        /// </summary>
        public bool IsKept { get; private set; }

        /// <summary>
        /// Inherited with a discontinue clause.
        /// </summary>
        public bool IsDiscontinued { get; private set; }

        /// <summary>
        /// Inherited from an effective body.
        /// </summary>
        public bool InheritBySideAttribute { get; private set; }

        /// <summary>
        /// List of precursors.
        /// </summary>
        public IList<IPrecursorInstance> PrecursorList { get; } = new List<IPrecursorInstance>();

        /// <summary>
        /// The first precursor in the inheritance tree.
        /// </summary>
        public OnceReference<IPrecursorInstance> OriginalPrecursor { get; private set; } = new OnceReference<IPrecursorInstance>();

        #region Client Interface
        /// <summary>
        /// Sets the <see cref="IsForgotten"/> flag.
        /// </summary>
        /// <param name="isForgotten">The new value.</param>
        public virtual void SetIsForgotten(bool isForgotten)
        {
            IsForgotten = isForgotten;
        }

        /// <summary>
        /// Sets the <see cref="IsKept"/> flag.
        /// </summary>
        /// <param name="isKept">The new value.</param>
        public virtual void SetIsKept(bool isKept)
        {
            IsKept = isKept;
        }

        /// <summary>
        /// Sets the <see cref="IsDiscontinued"/> flag.
        /// </summary>
        /// <param name="isDiscontinued">The new value.</param>
        public virtual void SetIsDiscontinued(bool isDiscontinued)
        {
            IsDiscontinued = isDiscontinued;
        }

        /// <summary>
        /// Sets the <see cref="InheritBySideAttribute"/> flag.
        /// </summary>
        /// <param name="inheritBySideAttribute">The new value.</param>
        public virtual void SetInheritBySideAttribute(bool inheritBySideAttribute)
        {
            InheritBySideAttribute = inheritBySideAttribute;
        }

        /// <summary>
        /// Clones this instance using the specified ancestor.
        /// </summary>
        /// <param name="ancestor">The ancestor.</param>
        public virtual IFeatureInstance Clone(IClassType ancestor)
        {
            IPrecursorInstance NewPrecursor = new PrecursorInstance(ancestor, this);
            Debug.Assert(NewPrecursor.Ancestor == ancestor);

            Debug.Assert(Feature.IsAssigned);
            IFeatureInstance ClonedObject = new FeatureInstance(Owner.Item, Feature.Item);

            foreach (IPrecursorInstance PrecursorInstance in PrecursorList)
                ClonedObject.PrecursorList.Add(PrecursorInstance);
            ClonedObject.PrecursorList.Add(NewPrecursor);
            ClonedObject.SetIsForgotten(IsForgotten);
            ClonedObject.SetIsKept(IsKept);
            ClonedObject.SetIsDiscontinued(IsDiscontinued);
            ClonedObject.SetInheritBySideAttribute(InheritBySideAttribute);

            if (OriginalPrecursor.IsAssigned)
                ClonedObject.OriginalPrecursor.Item = OriginalPrecursor.Item;

            return ClonedObject;
        }

        /// <summary>
        /// Find the precursor, either the only one or the selected one.
        /// </summary>
        /// <param name="ancestorType">The optionally selected precursor.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="source">The source to use when reporting errors.</param>
        /// <param name="selectedPrecursor">The selected precursor upon return if successful.</param>
        public bool FindPrecursor(IOptionalReference<BaseNode.IObjectType> ancestorType, IErrorList errorList, ISource source, out IFeatureInstance selectedPrecursor)
        {
            selectedPrecursor = null;

            if (ancestorType.IsAssigned)
            {
                IObjectType AssignedAncestorType = (IObjectType)ancestorType.Item;
                IClassType Ancestor = AssignedAncestorType.ResolvedType.Item as IClassType;
                Debug.Assert(Ancestor != null);

                foreach (IPrecursorInstance PrecursorItem in PrecursorList)
                    if (PrecursorItem.Ancestor.BaseClass == Ancestor.BaseClass)
                    {
                        selectedPrecursor = PrecursorItem.Precursor;
                        break;
                    }

                if (selectedPrecursor == null)
                    errorList.AddError(new ErrorInvalidPrecursor(AssignedAncestorType));
            }
            else if (PrecursorList.Count == 0)
                errorList.AddError(new ErrorNoPrecursor(source));
            else if (PrecursorList.Count > 1)
                errorList.AddError(new ErrorInvalidPrecursor(source));
            else
                selectedPrecursor = PrecursorList[0].Precursor;

            return selectedPrecursor != null;
        }
        #endregion
    }
}
