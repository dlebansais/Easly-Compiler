namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// List of classes that belong to the same group.
    /// </summary>
    public class SingleClassGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleClassGroup"/> class.
        /// </summary>
        /// <param name="firstClass">First class of the group.</param>
        public SingleClassGroup(IClass firstClass)
        {
            ClassList = new List<IClass>();
            ClassList.Add(firstClass);
        }

        /// <summary>
        /// Locks the group if it contains more than one class.
        /// </summary>
        public void SetAssigned()
        {
            if (ClassList.Count > 1)
                IsAssigned = true;
        }

        /// <summary>
        /// List of classes in the group.
        /// </summary>
        public IList<IClass> ClassList { get; }

        /// <summary>
        /// Indicates that the group doesn't accept more classes.
        /// </summary>
        public bool IsAssigned { get; private set; }
    }
}
