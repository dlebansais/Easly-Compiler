namespace EaslyCompiler
{
    /// <summary>
    /// Name of a unique type.
    /// </summary>
    public interface ITypeName
    {
        /// <summary>
        /// The unique type name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Name of a unique type.
    /// </summary>
    public class TypeName : ITypeName
    {
        #region Init
        static TypeName()
        {
            Count = 0;
        }

        private static int Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeName"/> class.
        /// </summary>
        /// <param name="name">The type name, not unique.</param>
        public TypeName(string name)
        {
            // Create the unique name.
            Name = $"{name}.{Count}";

            Count++;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The unique type name.
        /// </summary>
        public string Name { get; }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
