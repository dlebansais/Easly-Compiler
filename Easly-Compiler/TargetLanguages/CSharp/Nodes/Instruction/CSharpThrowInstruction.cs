namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpThrowInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IThrowInstruction Source { get; }

        /// <summary>
        /// The exception type.
        /// </summary>
        ICSharpType ExceptionType { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpThrowInstruction : CSharpInstruction, ICSharpThrowInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpThrowInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IThrowInstruction source)
        {
            return new CSharpThrowInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpThrowInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpThrowInstruction(ICSharpContext context, ICSharpFeature parentFeature, IThrowInstruction source)
            : base(context, parentFeature, source)
        {
            ExceptionType = CSharpType.Create(context, source.ResolvedType.Item);
            FeatureCall = new CSharpFeatureCall(context, source.SelectedParameterList, source.ArgumentList, source.ArgumentStyle);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IThrowInstruction Source { get { return (IThrowInstruction)base.Source; } }

        /// <summary>
        /// The exception type.
        /// </summary>
        public ICSharpType ExceptionType { get; }

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
            string ExceptionTypeString = ExceptionType.Type2CSharpString(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string ArgumentListString = CSharpArgument.CSharpArgumentList(outputNamespace, FeatureCall, new List<ICSharpQualifiedName>());

            // TODO: CreationRoutine

            writer.WriteIndentedLine($"throw new {ExceptionTypeString}({ArgumentListString});");
        }
        #endregion
    }
}
