﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpPrecursorInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IPrecursorInstruction Source { get; }

        /// <summary>
        /// The parent feature.
        /// </summary>
        new ICSharpFeatureWithName ParentFeature { get; }

        /// <summary>
        /// The feature call.
        /// </summary>
        ICSharpFeatureCall FeatureCall { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpPrecursorInstruction : CSharpInstruction, ICSharpPrecursorInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpPrecursorInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IPrecursorInstruction source)
        {
            return new CSharpPrecursorInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpPrecursorInstruction(ICSharpContext context, ICSharpFeature parentFeature, IPrecursorInstruction source)
            : base(context, parentFeature, source)
        {
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IPrecursorInstruction Source { get { return (IPrecursorInstruction)base.Source; } }

        /// <summary>
        /// The parent feature.
        /// </summary>
        public new ICSharpFeatureWithName ParentFeature { get { return (ICSharpFeatureWithName)base.ParentFeature; } }

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
        public override void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            string CoexistingPrecursorName = string.Empty;
            string CoexistingPrecursorRootName = ParentFeature.CoexistingPrecursorName;

            if (!string.IsNullOrEmpty(CoexistingPrecursorRootName))
                CoexistingPrecursorName = CSharpNames.ToCSharpIdentifier(CoexistingPrecursorRootName + " " + "Base");

            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
            string ArgumentListString = CSharpArgument.CSharpArgumentList(writer, ExpressionContext, FeatureCall);

            if (CoexistingPrecursorName.Length > 0)
                writer.WriteIndentedLine($"{CoexistingPrecursorName}({ArgumentListString});");
            else
            {
                string ProcedureName = CSharpNames.ToCSharpIdentifier(ParentFeature.Name);
                writer.WriteIndentedLine($"base.{ProcedureName}({ArgumentListString});");
            }
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            foreach (ICSharpArgument Argument in FeatureCall.ArgumentList)
                Argument.SetWriteDown();
        }
        #endregion
    }
}
