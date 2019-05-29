namespace EaslyCompiler
{
    /// <summary>
    /// A 'using' clause at the top of a namespace.
    /// </summary>
    public interface IUsingClause
    {
    }

    /// <summary>
    /// A 'using' clause at the top of a namespace.
    /// </summary>
    public class UsingClause : IUsingClause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingClause"/> class.
        /// </summary>
        /// <param name="name">The using clause name.</param>
        public UsingClause(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The using clause name.
        /// </summary>
        public string Name { get; }
    }
}
