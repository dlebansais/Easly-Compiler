namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpCreateInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new ICreateInstruction Source { get; }

        /// <summary>
        /// The created object type.
        /// </summary>
        ICSharpType EntityType { get; }

        /// <summary>
        /// The created object name.
        /// </summary>
        string CreatedObjectName { get; }

        /// <summary>
        /// The creation routine name.
        /// </summary>
        string CreationRoutineName { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpCreateInstruction : CSharpInstruction, ICSharpCreateInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpCreateInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, ICreateInstruction source)
        {
            return new CSharpCreateInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCreateInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpCreateInstruction(ICSharpContext context, ICSharpFeature parentFeature, ICreateInstruction source)
            : base(context, parentFeature, source)
        {
            EntityType = CSharpType.Create(context, source.ResolvedEntityType.Item);
            CreatedObjectName = ((IIdentifier)source.EntityIdentifier).ValidText.Item;
            CreationRoutineName = ((IIdentifier)source.CreationRoutineIdentifier).ValidText.Item;
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new ICreateInstruction Source { get { return (ICreateInstruction)base.Source; } }

        /// <summary>
        /// The created object type.
        /// </summary>
        public ICSharpType EntityType { get; }

        /// <summary>
        /// The created object name.
        /// </summary>
        public string CreatedObjectName { get; }

        /// <summary>
        /// The creation routine name.
        /// </summary>
        public string CreationRoutineName { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public override void WriteCSharp(ICSharpWriter writer, string outputNamespace)
        {
            string EntityString = CSharpNames.ToCSharpIdentifier(CreatedObjectName);
            string EntityTypeString = EntityType.Type2CSharpString(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

            bool IsAnchoredToCreationType = false;

            if (EntityType.Source is IAnchoredType AsAnchoredType)
                if (AsAnchoredType.AnchorKind == BaseNode.AnchorKinds.Creation)
                    IsAnchoredToCreationType = true;

            string CreationRoutineString = CSharpNames.ToCSharpIdentifier(CreationRoutineName);
            string ArgumentListText = CSharpArgument.CSharpArgumentList(outputNamespace, FeatureCall, new List<ICSharpQualifiedName>());

            CSharpConstructorTypes ClassConstructorType = CSharpConstructorTypes.OneConstructor;

            if (EntityType is ICSharpClassType AsClassType)
            {
                ClassConstructorType = AsClassType.Class.ClassConstructorType;

                if (AsClassType.Class.Source.ClassGuid == LanguageClasses.List.Guid && CreationRoutineString == "MakeEmpty")
                    ClassConstructorType = CSharpConstructorTypes.NoConstructor;
            }

            bool IsHandled = false;

            switch (ClassConstructorType)
            {
                case CSharpConstructorTypes.NoConstructor:
                case CSharpConstructorTypes.OneConstructor:
                    if (IsAnchoredToCreationType)
                        writer.WriteIndentedLine($"{EntityString} = Activator.CreateInstance(typeof({EntityTypeString}).Assembly.FullName, typeof({EntityTypeString}).FullName, {ArgumentListText}).Unwrap() as {EntityTypeString};");
                    else
                        writer.WriteIndentedLine($"{EntityString} = new {EntityTypeString}({ArgumentListText});");
                    IsHandled = true;
                    break;

                case CSharpConstructorTypes.ManyConstructors:
                    if (IsAnchoredToCreationType)
                    {
                        writer.WriteIndentedLine($"{EntityString} = Activator.CreateInstance(typeof({EntityTypeString}).Assembly.FullName, typeof({EntityTypeString}).FullName).Unwrap() as {EntityTypeString};");
                        writer.WriteIndentedLine($"{EntityString}.{CreationRoutineString}({ArgumentListText});");
                    }
                    else
                    {
                        writer.WriteIndentedLine($"{EntityString} = new {EntityTypeString}();");
                        writer.WriteIndentedLine($"{EntityString}.{CreationRoutineString}({ArgumentListText});");
                    }
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}
