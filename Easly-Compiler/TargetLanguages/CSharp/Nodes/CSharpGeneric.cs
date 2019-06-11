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

        /// <summary>
        /// Initializes the C# generic.
        /// </summary>
        /// <param name="context">The creation context.</param>
        void Init(ICSharpContext context);
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
        public ICSharpFormalGenericType Type { get; private set; }

        /// <summary>
        /// True if the generic is used to create at least one object.
        /// </summary>
        public bool IsUsedToCreate { get { return Type.Source.IsUsedToCreate; } }

        /// <summary>
        /// The list of constraints.
        /// </summary>
        public IList<ICSharpConstraint> ConstraintList { get; } = new List<ICSharpConstraint>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the C# generic.
        /// </summary>
        /// <param name="context">The creation context.</param>
        public void Init(ICSharpContext context)
        {
            foreach (IConstraint Constraint in Source.ConstraintList)
            {
                ICSharpConstraint NewConstraint = CSharpConstraint.Create(context, Constraint);
                ConstraintList.Add(NewConstraint);
            }

            // Create the type after constraints have been listed.
            Type = CSharpFormalGenericType.Create(context, Source.ResolvedGenericType.Item);
        }
        #endregion
    }
}
