namespace EaslyCompiler
{
    using CompilerNode;
    using Easly;

    /// <summary>
    /// An instance of an overload parameter.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// The parameter name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The corresponding resolved attribute.
        /// </summary>
        IScopeAttributeFeature ResolvedParameter { get; }
    }

    /// <summary>
    /// An instance of an overload parameter.
    /// </summary>
    public class Parameter : IParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="resolvedParameter">The corresponding resolved attribute.</param>
        public Parameter(string name, IScopeAttributeFeature resolvedParameter)
        {
            Name = name;
            ResolvedParameter = resolvedParameter;

#if COVERAGE
            string DebugString = ToString();
#endif
        }

        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The corresponding resolved attribute.
        /// </summary>
        public IScopeAttributeFeature ResolvedParameter { get; }

        /// <summary>
        /// Checks if a table of parameters contains a parameter with the given name.
        /// </summary>
        /// <param name="parameterTable">The table of parameters.</param>
        /// <param name="name">The name to look for.</param>
        public static bool TableContainsName(ISealableList<IParameter> parameterTable, string name)
        {
            return parameterTable.Exists((IParameter item) => item.Name == name);
        }

        /// <summary></summary>
        public override string ToString()
        {
            string Value = ResolvedParameter.DefaultValue.IsAssigned ? $" = {ResolvedParameter.DefaultValue.Item.ExpressionToString}" : string.Empty;
            return $"Parameter '{Name}'{Value}";
        }
    }
}
