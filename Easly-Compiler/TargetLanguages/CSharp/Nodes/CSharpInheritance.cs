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

        /// <summary>
        /// Sets the <see cref="AncestorClass"/> property.
        /// </summary>
        /// <param name="ancestorClass">The class associated to this inheritance clause.</param>
        void SetAncestorClass(ICSharpClass ancestorClass);
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
        public static ICSharpInheritance Create(IInheritance source)
        {
            return new CSharpInheritance(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInheritance"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpInheritance(IInheritance source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The class associated to this inheritance clause.
        /// </summary>
        public ICSharpClass AncestorClass { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the <see cref="AncestorClass"/> property.
        /// </summary>
        /// <param name="ancestorClass">The class associated to this inheritance clause.</param>
        public void SetAncestorClass(ICSharpClass ancestorClass)
        {
            Debug.Assert(ancestorClass != null);
            Debug.Assert(AncestorClass == null);

            AncestorClass = ancestorClass;
        }
        #endregion
    }
}
