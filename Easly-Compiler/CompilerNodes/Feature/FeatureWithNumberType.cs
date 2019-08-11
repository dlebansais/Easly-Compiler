namespace CompilerNode
{
    using EaslyCompiler;

    /// <summary>
    /// Compiler IFeature if it can be of a number type.
    /// </summary>
    public interface IFeatureWithNumberType
    {
        /// <summary>
        /// The number kind if the type is a number.
        /// </summary>
        NumberKinds NumberKind { get; }
    }
}
