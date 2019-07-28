namespace EaslyCompiler
{
    /// <summary>
    /// Nodes that can produce C# source code.
    /// </summary>
    public interface ICSharpOutputNode
    {
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        bool WriteDown { get; }

        /// <summary>
        /// Sets the <see cref="WriteDown"/> flag.
        /// </summary>
        void SetWriteDown();
    }

    /// <summary>
    /// Nodes that can produce C# source code.
    /// </summary>
    public class CSharpOutputNode : ICSharpOutputNode
    {
        #region Properties
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        public bool WriteDown { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the <see cref="WriteDown"/> flag.
        /// </summary>
        public void SetWriteDown()
        {
            WriteDown = true;
        }
        #endregion
    }
}
