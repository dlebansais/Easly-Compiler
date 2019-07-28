namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# formal generic type.
    /// </summary>
    public interface ICSharpFormalGenericType : ICSharpTypeWithFeature
    {
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        new IFormalGenericType Source { get; }

        /// <summary>
        /// The associated generic.
        /// </summary>
        ICSharpGeneric Generic { get; }
    }

    /// <summary>
    /// A C# formal generic type.
    /// </summary>
    public class CSharpFormalGenericType : CSharpType, ICSharpFormalGenericType
    {
        #region Init
        /// <summary>
        /// Create a new C# type.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        public static ICSharpFormalGenericType Create(ICSharpContext context, IFormalGenericType source)
        {
            return new CSharpFormalGenericType(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFormalGenericType"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly type from which the C# type is created.</param>
        protected CSharpFormalGenericType(ICSharpContext context, IFormalGenericType source)
            : base(context, source)
        {
            Generic = context.GetGeneric(Source.FormalGeneric);

            NumberType = CSharpNumberTypes.NotApplicable;

            foreach (ICSharpConstraint Constraint in Generic.ConstraintList)
                if (Constraint.TypeWithRename is ICSharpClassType AsClassType)
                    if (AsClassType.IsNumberType)
                    {
                        if (NumberType == CSharpNumberTypes.NotApplicable)
                            NumberType = CSharpNumberTypes.Unknown;

                        switch (AsClassType.NumberType)
                        {
                            case CSharpNumberTypes.Unknown:
                                if (NumberType == CSharpNumberTypes.NotApplicable)
                                    NumberType = CSharpNumberTypes.Unknown;
                                break;

                            case CSharpNumberTypes.Integer:
                                Debug.Assert(NumberType != CSharpNumberTypes.Real);
                                NumberType = CSharpNumberTypes.Integer;
                                break;

                            case CSharpNumberTypes.Real:
                                Debug.Assert(NumberType != CSharpNumberTypes.Integer);
                                NumberType = CSharpNumberTypes.Real;
                                break;
                        }
                    }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly type from which the C# type is created.
        /// </summary>
        public new IFormalGenericType Source { get { return (IFormalGenericType)base.Source; } }

        /// <summary>
        /// The associated generic.
        /// </summary>
        public ICSharpGeneric Generic { get; private set; }

        /// <summary>
        /// True if the type can be used in the interface 'I' text format.
        /// </summary>
        public override bool HasInterfaceText { get { return false; } }

        /// <summary>
        /// True if the type is a number.
        /// </summary>
        public override bool IsNumberType
        {
            get
            {
                bool Result = false;

                foreach (ICSharpConstraint Constraint in Generic.ConstraintList)
                    if (Constraint.TypeWithRename is ICSharpClassType AsClassType)
                        Result |= AsClassType.IsNumberType;

                return Result;
            }
        }

        /// <summary>
        /// The list of class types this type conforms to.
        /// </summary>
        public IList<ICSharpClassType> ConformingClassTypeList
        {
            get
            {
                IList<ICSharpClassType> Result = new List<ICSharpClassType>();

                foreach (ICSharpConstraint Constraint in Generic.ConstraintList)
                    if (Constraint.TypeWithRename is ICSharpClassType AsClassType)
                        Result.Add(AsClassType);

                return Result;
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Get the name of a type.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format.</param>
        public override string Type2CSharpString(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            SetUsedInCode();

            return ((cSharpTypeFormat == CSharpTypeFormats.AsInterface) ? "I" : string.Empty) + CSharpNames.ToCSharpIdentifier(Generic.Name);
        }
        #endregion
    }
}
