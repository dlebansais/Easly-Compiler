namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# inheritance node.
    /// </summary>
    public interface ICSharpInheritance : ICSharpSource<IInheritance>
    {
        /// <summary>
        /// The class associated to this inheritance clause.
        /// </summary>
        ICSharpClass AncestorClass { get; }
    }

    /// <summary>
    /// A C# inheritance node.
    /// </summary>
    public class CSharpInheritance : CSharpSource<IInheritance>, ICSharpInheritance
    {
        #region Init
        /// <summary>
        /// Create a new C# inheritance.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="ancestorClass">The ancestor class.</param>
        public static ICSharpInheritance Create(IInheritance source, ICSharpClass ancestorClass)
        {
            return new CSharpInheritance(source, ancestorClass);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInheritance"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="ancestorClass">The ancestor class.</param>
        protected CSharpInheritance(IInheritance source, ICSharpClass ancestorClass)
            : base(source)
        {
            Debug.Assert(ancestorClass != null);

            AncestorClass = ancestorClass;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class associated to this inheritance clause.
        /// </summary>
        public ICSharpClass AncestorClass { get; private set; }
        #endregion
    }
}
