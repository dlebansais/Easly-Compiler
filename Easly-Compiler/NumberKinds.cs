namespace EaslyCompiler
{
    /// <summary>
    /// The specific kind of a number type.
    /// </summary>
    public enum NumberKinds
    {
        /// <summary>
        /// This number kind hasn't been checked yet.
        /// </summary>
        NotChecked,

        /// <summary>
        /// Not applicable (not a number).
        /// </summary>
        NotApplicable,

        /// <summary>
        /// Not known yet.
        /// </summary>
        Unknown,

        /// <summary>
        /// An integer number.
        /// </summary>
        Integer,

        /// <summary>
        /// A real number.
        /// </summary>
        Real,
    }
}
