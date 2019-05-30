namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// An interface to write to a C# file.
    /// </summary>
    public interface ICSharpWriter : IDisposable
    {
        /// <summary>
        /// Current indentation level.
        /// </summary>
        int IndentLevel { get; }

        /// <summary>
        /// Writes an empty line.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a line using the current indentation.
        /// </summary>
        /// <param name="line">The line to write.</param>
        void WriteIndentedLine(string line);

        /// <summary>
        /// Writes the documentation associated to a node.
        /// </summary>
        /// <param name="node">The documented node.</param>
        void WriteDocumentation(BaseNode.INode node);

        /// <summary>
        /// Increased the current indentation level by 1.
        /// </summary>
        void IncreaseIndent();

        /// <summary>
        /// Decreased the current indentation level by 1.
        /// </summary>
        void DecreaseIndent();
    }

    /// <summary>
    /// A class to write to a C# file.
    /// </summary>
    public class CSharpWriter : StreamWriter, ICSharpWriter
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public CSharpWriter(Stream stream)
            : base(stream, Encoding.Unicode)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Current indentation level.
        /// </summary>
        public int IndentLevel { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes a line using the current indentation.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteIndentedLine(string line)
        {
            for (int i = 0; i < IndentLevel; i++)
                Write("    ");

            WriteLine(line);
        }

        /// <summary>
        /// Writes the documentation associated to a node.
        /// </summary>
        /// <param name="node">The documented node.</param>
        public void WriteDocumentation(BaseNode.INode node)
        {
            string Comment = node.Documentation.Comment;

            if (!string.IsNullOrEmpty(Comment))
                WriteIndentedLine($"// {Comment}");
        }

        /// <summary>
        /// Increased the current indentation level by 1.
        /// </summary>
        public void IncreaseIndent()
        {
            IndentLevel++;

            Debug.Assert(IndentLevel > 0);
        }

        /// <summary>
        /// Decreased the current indentation level by 1.
        /// </summary>
        public void DecreaseIndent()
        {
            Debug.Assert(IndentLevel > 0);

            IndentLevel--;
        }
        #endregion
    }
}
