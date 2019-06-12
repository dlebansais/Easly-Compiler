namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpIndexAssignmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IIndexAssignmentInstruction Source { get; }

        /// <summary>
        /// The assignment destination.
        /// </summary>
        ICSharpQualifiedName Destination { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// The assignment source.
        /// </summary>
        ICSharpExpression SourceExpression { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpIndexAssignmentInstruction : CSharpInstruction, ICSharpIndexAssignmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpIndexAssignmentInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IIndexAssignmentInstruction source)
        {
            return new CSharpIndexAssignmentInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpIndexAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpIndexAssignmentInstruction(ICSharpContext context, ICSharpFeature parentFeature, IIndexAssignmentInstruction source)
            : base(context, parentFeature, source)
        {
            Destination = CSharpQualifiedName.Create(context, (IQualifiedName)source.Destination, parentFeature, null, false);
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
            SourceExpression = CSharpExpression.Create(context, (IExpression)source.Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IIndexAssignmentInstruction Source { get { return (IIndexAssignmentInstruction)base.Source; } }

        /// <summary>
        /// The assignment destination.
        /// </summary>
        public ICSharpQualifiedName Destination { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        public ICSharpFeatureCall FeatureCall { get; }

        /// <summary>
        /// The assignment source.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public override void WriteCSharp(ICSharpWriter writer)
        {
            string DestinationString = Destination.CSharpText(writer, 0);
            string SourceString = SourceExpression.CSharpText(writer);
            string ArgumentListText = CSharpArgument.CSharpArgumentList(writer, FeatureCall, new List<ICSharpQualifiedName>());

            writer.WriteIndentedLine($"{DestinationString}[{ArgumentListText}] = {SourceString};");
        }
        #endregion
    }
}
