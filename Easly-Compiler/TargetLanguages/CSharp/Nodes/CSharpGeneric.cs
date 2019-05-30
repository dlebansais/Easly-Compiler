namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# generic node.
    /// </summary>
    public interface ICSharpGeneric : ICSharpSource<IGeneric>
    {
        /// <summary>
        /// The generic name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The corresponding type.
        /// </summary>
        ICSharpFormalGenericType Type { get; }

        /// <summary>
        /// True if the generic is used to create at least one object.
        /// </summary>
        bool IsUsedToCreate { get; }

        /// <summary>
        /// The list of constraints.
        /// </summary>
        IList<ICSharpConstraint> ConstraintList { get; }
    }

    /// <summary>
    /// A C# generic node.
    /// </summary>
    public class CSharpGeneric : CSharpSource<IGeneric>, ICSharpGeneric
    {
        #region Init
        /// <summary>
        /// Create a new C# generic.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpGeneric Create(IGeneric source)
        {
            return new CSharpGeneric(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpGeneric"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpGeneric(IGeneric source)
            : base(source)
        {
            Name = ((IName)source.EntityName).ValidText.Item;
            Type = CSharpFormalGenericType.Create(source.ResolvedGenericType.Item);
            Type.SetGeneric(this);

            foreach (IConstraint Constraint in source.ConstraintList)
            {
                ICSharpConstraint NewConstraint = CSharpConstraint.Create(Constraint);
                ConstraintList.Add(NewConstraint);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The generic name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The corresponding type.
        /// </summary>
        public ICSharpFormalGenericType Type { get; }

        /// <summary>
        /// True if the generic is used to create at least one object.
        /// </summary>
        public bool IsUsedToCreate { get { return Type.Source.IsUsedToCreate; } }

        /// <summary>
        /// The list of constraints.
        /// </summary>
        public IList<ICSharpConstraint> ConstraintList { get; } = new List<ICSharpConstraint>();
        #endregion
    }
}
