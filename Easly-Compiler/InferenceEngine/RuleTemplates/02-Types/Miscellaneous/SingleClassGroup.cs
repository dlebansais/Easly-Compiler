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
            GroupClassList = new List<IClass>();
            GroupClassList.Add(firstClass);
        }

        /// <summary>
        /// Locks the group if it contains more than one class.
        /// </summary>
        public void SetAssigned()
        {
            if (GroupClassList.Count > 1)
                IsAssigned = true;
        }

        /// <summary>
        /// List of classes in the group.
        /// </summary>
        public IList<IClass> GroupClassList { get; }

        /// <summary>
        /// Indicates that the group doesn't accept more classes.
        /// </summary>
        public bool IsAssigned { get; private set; }
    }
}
