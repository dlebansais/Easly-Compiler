namespace EaslyCompiler
{
    /// <summary>
    /// Context for a variable used in expressions.
    /// </summary>
    public interface ICSharpVariableContext
    {
        /// <summary>
        /// The variable name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if the variable is already declared.
        /// </summary>
        bool IsDeclared { get; }
    }

    /// <summary>
    /// Context for a variable used in expressions.
    /// </summary>
    public class CSharpVariableContext : ICSharpVariableContext
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpVariableContext"/> class.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="isDeclared">True if the variable is already declared.</param>
        public CSharpVariableContext(string name, bool isDeclared = true)
        {
            Name = name;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The variable name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// True if the variable is already declared.
        /// </summary>
        public bool IsDeclared { get; }
        #endregion
    }
}
