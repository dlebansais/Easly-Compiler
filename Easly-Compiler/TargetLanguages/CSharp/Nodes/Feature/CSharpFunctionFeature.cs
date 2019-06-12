﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# function.
    /// </summary>
    public interface ICSharpFunctionFeature : ICSharpFeature<IFunctionFeature>, ICSharpRoutineFeature
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IFunctionFeature Source { get; }

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
    }

    /// <summary>
    /// A C# function.
    /// </summary>
    public class CSharpFunctionFeature : CSharpFeature<IFunctionFeature>, ICSharpFunctionFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# function.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpFunctionFeature Create(ICSharpClass owner, IFeatureInstance instance, IFunctionFeature source)
        {
            return new CSharpFunctionFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFunctionFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpFunctionFeature(ICSharpClass owner, IFeatureInstance instance, IFunctionFeature source)
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
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void Init(ICSharpContext context)
        {
            foreach (IQueryOverload Overload in Source.OverloadList)
            {
                ICSharpQueryOverload NewOverload = CSharpQueryOverload.Create(context, Overload, this, Owner);
                OverloadList.Add(NewOverload);
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
            writer.WriteDocumentation(Source);

            string NameString = CSharpNames.ToCSharpIdentifier(Name);

            foreach (ICSharpQueryOverload Overload in OverloadList)
                Overload.WriteCSharp(writer, featureTextType, IsOverride, NameString, exportStatus, false, ref isFirstFeature, ref isMultiline);
        }
        #endregion
    }
}
