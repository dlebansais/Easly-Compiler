namespace EaslyCompiler
{
    using System.Collections.Generic;
    using Easly;

    /// <summary>
    /// Name of a feature.
    /// </summary>
    public interface IFeatureName
    {
        /// <summary>
        /// The feature name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Name of a feature.
    /// </summary>
    public class FeatureName : IFeatureName
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureName"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        public FeatureName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The unique name for an indexer.
        /// </summary>
        public static IFeatureName IndexerFeatureName { get; } = new FeatureName("indexer");
        #endregion

        #region Properties
        /// <summary>
        /// The unique name.
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

        #region Client Interface
        /// <summary>
        /// Checks if a table of feature names contains a name, and if so returns the corresponding <see cref="IFeatureName"/> and associated value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value associated to feature names in the table.</typeparam>
        /// <param name="table">The table.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="key">The feature name found upon return.</param>
        /// <param name="value">The associated value.</param>
        public static bool TableContain<TValue>(IHashtableEx<IFeatureName, TValue> table, string name, out IFeatureName key, out TValue value)
        {
            key = null;
            value = default;
            bool Result = false;

            foreach (KeyValuePair<IFeatureName, TValue> Entry in table)
                if (Entry.Key.Name == name)
                {
                    key = Entry.Key;
                    value = Entry.Value;
                    Result = true;
                    break;
                }

            return Result;
        }
        #endregion
    }
}
