namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// Indicates how to split the class inheritance tree.
    /// </summary>
    public interface IClassSplitting
    {
        /// <summary>
        /// These classes must be inherited ans can't be merged with the base class using them.
        /// </summary>
        IList<IClass> MustInherit { get; }

        /// <summary>
        /// These classes must have a corresponding interface and be used by the interface rather than directly.
        /// </summary>
        IList<IClass> MustInterface { get; }

        /// <summary>
        /// These classes can be used directly and merged with a base class.
        /// </summary>
        IList<IClass> OtherParents { get; }
    }

    /// <summary>
    /// Indicates how to split the class inheritance tree.
    /// </summary>
    public class ClassSplitting : IClassSplitting
    {
        #region Init
        /// <summary>
        /// Creates a <see cref="ClassSplitting"/> object.
        /// </summary>
        /// <param name="classList">The list of classes to split.</param>
        /// <param name="errorList">The list of errors found.</param>
        /// <param name="result">The created object upon return.</param>
        public static bool Create(IList<IClass> classList, IErrorList errorList, out IClassSplitting result)
        {
            result = new ClassSplitting();

            foreach (IClass ClassItem in classList)
                if (ClassItem.HasExternBody)
                    result.MustInherit.Add(ClassItem);

            foreach (IClass ClassItem in classList)
            {
                bool InheritanceForced = false;
                foreach (IInheritance InheritanceItem in ClassItem.InheritanceList)
                {
                    IClassType ClassParentType = InheritanceItem.ResolvedClassParentType.Item;
                    IClass BaseClass = ClassParentType.BaseClass;

                    if (result.MustInherit.Contains(BaseClass))
                        if (InheritanceForced)
                        {
                            errorList.AddError(new ErrorMultipleExternBody(ClassItem));
                            break;
                        }
                        else
                            InheritanceForced = true;
                }

                if (errorList.IsEmpty && InheritanceForced)
                    foreach (IInheritance InheritanceItem in ClassItem.InheritanceList)
                    {
                        IClassType ClassParentType = InheritanceItem.ResolvedClassParentType.Item;
                        IClass BaseClass = ClassParentType.BaseClass;

                        if (!result.MustInherit.Contains(BaseClass) && !result.MustInterface.Contains(BaseClass))
                            result.MustInterface.Add(BaseClass);
                    }
            }

            if (!errorList.IsEmpty)
                return false;

            foreach (IClass ClassItem in classList)
                if (!result.MustInherit.Contains(ClassItem) && !result.MustInterface.Contains(ClassItem))
                    result.OtherParents.Add(ClassItem);

            return true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// These classes must be inherited ans can't be merged with the base class using them.
        /// </summary>
        public IList<IClass> MustInherit { get; } = new List<IClass>();

        /// <summary>
        /// These classes must have a corresponding interface and be used by the interface rather than directly.
        /// </summary>
        public IList<IClass> MustInterface { get; } = new List<IClass>();

        /// <summary>
        /// These classes can be used directly and merged with a base class.
        /// </summary>
        public IList<IClass> OtherParents { get; } = new List<IClass>();
        #endregion
    }
}
