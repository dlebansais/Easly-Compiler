namespace CompilerNode
{
    using EaslyCompiler;

    /// <summary>
    /// Compiler IDocument.
    /// </summary>
    public interface IDocument : BaseNode.IDocument
    {
    }

    /// <summary>
    /// Compiler IDocument.
    /// </summary>
    public class Document : BaseNode.Document, IDocument
    {
    }
}
