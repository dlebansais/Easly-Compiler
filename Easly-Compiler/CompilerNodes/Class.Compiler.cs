namespace CompilerNode
{
    using Easly;
    using EaslyCompiler;
    using System.Collections.Generic;
    /// <summary>
    /// Compiler IClass.
    /// </summary>
    public partial class Class : BaseNode.Class, IClass
    {
        /// <summary>
        /// The class path with replication info.
        /// </summary>
        public string FullClassPath { get; private set; }

        /// <summary>
        /// Initializes the class path.
        /// </summary>
        public void SetFullClassPath()
        {
            FullClassPath = ClassPath;
        }

        /// <summary>
        /// Initializes the class path with replication info.
        /// </summary>
        /// <param name="replicationPattern">The replication pattern used.</param>
        /// <param name="source">The source text.</param>
        public void SetFullClassPath(string replicationPattern, string source)
        {
            FullClassPath = $"{ClassPath};{replicationPattern}={source}";
        }

        /// <summary>
        /// The class-specific counter, for the <see cref="BaseNode.PreprocessorMacro.Counter"/> macro.
        /// </summary>
        public int ClassCounter { get; private set; }

        /// <summary>
        /// Increments <see cref="ClassCounter"/>.
        /// </summary>
        public virtual void IncrementClassCounter()
        {
            ClassCounter++;
        }

        /// <summary>
        /// True if the class is an enumeration (inherits from one of the enumeration language classes).
        /// </summary>
        public bool IsEnumeration
        {
            get
            {
                bool IsInheritingEnumeration = false;

                if (GenericTable.Count == 0 && ExportTable.Count == 0 && TypedefTable.Count == 0 && DiscreteTable.Count > 0)
                {
                    ISealableDictionary<ITypeName, ICompiledType> ConformanceTable = ResolvedClassType.Item.ConformanceTable;
                    foreach (KeyValuePair<ITypeName, ICompiledType> Entry in ConformanceTable)
                        if (Entry.Value is IClassType AsClassType)
                            if (AsClassType.BaseClass.ClassGuid == LanguageClasses.Enumeration.Guid)
                                IsInheritingEnumeration |= ConformanceTable.Count == 1;
                }

                return IsInheritingEnumeration;
            }
        }
    }
}
