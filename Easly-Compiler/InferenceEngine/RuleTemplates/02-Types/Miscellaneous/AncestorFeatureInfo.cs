namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    internal class AncestorFeatureInfo
    {
        /// <summary>
        /// The ancestor/
        /// </summary>
        public IClassType Ancestor { get; set; }

        /// <summary>
        /// The ancestor features.
        /// </summary>
        public IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable { get; set; }

        /// <summary>
        /// The location for error reports.
        /// </summary>
        public OnceReference<IInheritance> Location { get; set; }
    }
}
