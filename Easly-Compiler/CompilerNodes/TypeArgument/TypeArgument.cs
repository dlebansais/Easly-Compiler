namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler ITypeArgument.
    /// </summary>
    public interface ITypeArgument : BaseNode.ITypeArgument, INode, ISource
    {
        /// <summary>
        /// Name of the resolved source type.
        /// </summary>
        OnceReference<ITypeName> ResolvedSourceTypeName { get; }

        /// <summary>
        /// The resolved source type.
        /// </summary>
        OnceReference<ICompiledType> ResolvedSourceType { get; }
    }
}
