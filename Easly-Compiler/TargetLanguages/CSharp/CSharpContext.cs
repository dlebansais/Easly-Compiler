namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A context for creating C# nodes.
    /// </summary>
    public interface ICSharpContext
    {
        /// <summary>
        /// The table of all classes.
        /// </summary>
        IDictionary<IClass, ICSharpClass> ClassTable { get; }

        /// <summary>
        /// The table of all known features.
        /// </summary>
        IDictionary<ICompiledFeature, ICSharpFeature> FeatureTable { get; }

        /// <summary>
        /// Gets the C# class from the source class.
        /// </summary>
        /// <param name="sourceClass">The source class.</param>
        ICSharpClass GetClass(IClass sourceClass);

        /// <summary>
        /// Gets the C# feature from the source feature.
        /// </summary>
        /// <param name="sourceFeature">The source feature.</param>
        ICSharpFeature GetFeature(ICompiledFeature sourceFeature);
    }

    /// <summary>
    /// A context for creating C# nodes.
    /// </summary>
    public class CSharpContext : ICSharpContext
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpContext"/> class.
        /// </summary>
        /// <param name="classTable">The table of all classes.</param>
        /// <param name="featureTable">The table of all known features.</param>
        public CSharpContext(IDictionary<IClass, ICSharpClass> classTable, IDictionary<ICompiledFeature, ICSharpFeature> featureTable)
        {
            ClassTable = classTable;
            FeatureTable = featureTable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The table of all classes.
        /// </summary>
        public IDictionary<IClass, ICSharpClass> ClassTable { get; }

        /// <summary>
        /// The table of all known features.
        /// </summary>
        public IDictionary<ICompiledFeature, ICSharpFeature> FeatureTable { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the C# class from the source class.
        /// </summary>
        /// <param name="sourceClass">The source class.</param>
        public ICSharpClass GetClass(IClass sourceClass)
        {
            Debug.Assert(ClassTable.ContainsKey(sourceClass));

            return ClassTable[sourceClass];
        }

        /// <summary>
        /// Gets the C# feature from the source feature.
        /// </summary>
        /// <param name="sourceFeature">The source feature.</param>
        public ICSharpFeature GetFeature(ICompiledFeature sourceFeature)
        {
            Debug.Assert(FeatureTable.ContainsKey(sourceFeature));

            return FeatureTable[sourceFeature];
        }
        #endregion
    }
}
