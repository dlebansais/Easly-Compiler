namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        IInstruction Source { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public abstract class CSharpInstruction : ICSharpInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpInstruction Create(ICSharpContext context, IInstruction source)
        {
            ICSharpInstruction Result = null;

            switch (source)
            {
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpInstruction(ICSharpContext context, IInstruction source)
        {
            Debug.Assert(source != null);

            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public IInstruction Source { get; }
        #endregion

        #region Client Interface
        #endregion
    }
}
