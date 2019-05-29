namespace EaslyCompiler
{
    /// <summary>
    /// A C# node.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public interface ICSharpSource<T>
        where T : class
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        T Source { get; }
    }

    /// <summary>
    /// A C# node.
    /// </summary>
    /// <typeparam name="T">The corresponding compiler node.</typeparam>
    public class CSharpSource<T> : ICSharpSource<T>
        where T : class
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpSource{T}"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpSource(T source)
        {
            Source = source;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        public T Source { get; }
        #endregion
    }
}
