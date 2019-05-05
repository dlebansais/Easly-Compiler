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
            _GroupClassList = new List<IClass>();
            _GroupClassList.Add(firstClass);
        }

        /// <summary>
        /// Adds a class belonging to the same group.
        /// </summary>
        /// <param name="groupClass">The class added.</param>
        /// <param name="isUpdated">True upon return if the class list changed.</param>
        public void AddClass(IClass groupClass, ref bool isUpdated)
        {
            // Don't add if inherited several times (diamond inheritance).
            if (!_GroupClassList.Contains(groupClass))
            {
                _GroupClassList.Add(groupClass);
                isUpdated = true;
            }
        }

        /// <summary>
        /// List of classes in the group.
        /// </summary>
        public IReadOnlyList<IClass> GroupClassList { get { return _GroupClassList; } }
        private List<IClass> _GroupClassList;
    }
}
