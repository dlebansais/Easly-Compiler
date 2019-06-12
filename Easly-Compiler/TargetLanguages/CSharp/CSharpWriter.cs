namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// An interface to write to a C# file.
    /// </summary>
    public interface ICSharpWriter : ICSharpUsingCollection, IDisposable
    {
        /// <summary>
        /// Current indentation level.
        /// </summary>
        int IndentLevel { get; }

        /// <summary>
        /// Writes an empty line.
        /// </summary>
        void WriteEmptyLine();

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

        /// <summary>
        /// Commits all accumulated lines.
        /// </summary>
        void CommitLines();
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
        /// <param name="defaultNamespace">The default namespace.</param>
        public CSharpWriter(Stream stream, string defaultNamespace)
            : base(stream, Encoding.Unicode)
        {
            DefaultNamespace = defaultNamespace;
            AutoFlush = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        public string DefaultNamespace { get; }

        /// <summary>
        /// Current indentation level.
        /// </summary>
        public int IndentLevel { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes an empty line.
        /// </summary>
        public void WriteEmptyLine()
        {
            LineList.Add(string.Empty);
        }

        /// <summary>
        /// Writes a line using the current indentation.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteIndentedLine(string line)
        {
            string Indentation = string.Empty;

            for (int i = 0; i < IndentLevel; i++)
                Indentation += "    ";

            LineList.Add(Indentation + line);
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

        /// <summary>
        /// Adds a using directive to write separately.
        /// </summary>
        public void AddUsing(string usingDirective)
        {
            UsingList.Add(usingDirective);
        }

        /// <summary>
        /// Commits all accumulated lines.
        /// </summary>
        public void CommitLines()
        {
            foreach (string UsingDirective in UsingList)
                WriteLine($"    using {UsingDirective};");

            if (UsingList.Count > 0)
                WriteLine(string.Empty);

            foreach (string Line in LineList)
                WriteLine(Line);

            if (UsingList.Count > 0 || LineList.Count > 0)
            {
                UsingList.Clear();
                LineList.Clear();
                Flush();
            }
        }

        private IList<string> UsingList { get; } = new List<string>();
        private IList<string> LineList { get; } = new List<string>();
        #endregion

        #region Implementation of IDisposable
        /// <summary></summary>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
                DisposeNow();
        }

        /// <summary></summary>
        protected virtual void DisposeNow()
        {
            CommitLines();
        }

        /// <summary></summary>
        public override void Close()
        {
            CommitLines();
            base.Close();
        }

        /// <summary></summary>
        public override void Flush()
        {
            CommitLines();
            base.Flush();
        }
        #endregion
    }
}
