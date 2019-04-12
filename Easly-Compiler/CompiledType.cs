namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A type, from a <see cref="BaseNode.ObjectType"/> or specific to the compiler.
    /// </summary>
    public interface ICompiledType
    {
        /// <summary>
        /// Discretes available in this type.
        /// </summary>
        IHashtableEx<IFeatureName, IDiscrete> DiscreteTable { get; }

        /// <summary>
        /// Features available in this type.
        /// </summary>
        IHashtableEx<IFeatureName, IFeatureInstance> FeatureTable { get; }

        /*
        /// <summary>
        /// Type friendly name, unique.
        /// </summary>
        string TypeFriendlyName { get; }

        /// <summary>
        /// True if the type is a reference type.
        /// </summary>
        bool IsReference { get; }

        /// <summary>
        /// True if the type is a value type.
        /// </summary>
        bool IsValue { get; }
        */
    }
}
