namespace EaslyCompiler
{
    /// <summary>
    /// The mode to use to write a C# contract.
    /// </summary>
    public enum CSharpContractLocations
    {
        /// <summary>
        /// The contract is a of a getter.
        /// </summary>
        Getter,

        /// <summary>
        /// The contract is a of a setter.
        /// </summary>
        Setter,

        /// <summary>
        /// The contract is neither of a getter nor a setter.
        /// </summary>
        Other
    }
}
