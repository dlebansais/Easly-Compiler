namespace EaslyCompiler
{
    using System;

    /// <summary>
    /// A pair of name and GUID.
    /// </summary>
    public struct NameGuidPair
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The GUID.
        /// </summary>
        public Guid Guid { get; set; }
    }
}
