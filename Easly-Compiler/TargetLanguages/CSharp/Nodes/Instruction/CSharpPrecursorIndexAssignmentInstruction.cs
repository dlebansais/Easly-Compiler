﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpPrecursorIndexAssignmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IPrecursorIndexAssignmentInstruction Source { get; }

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
    public class CSharpPrecursorIndexAssignmentInstruction : CSharpInstruction, ICSharpPrecursorIndexAssignmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpPrecursorIndexAssignmentInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IPrecursorIndexAssignmentInstruction source)
        {
            return new CSharpPrecursorIndexAssignmentInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPrecursorIndexAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpPrecursorIndexAssignmentInstruction(ICSharpContext context, ICSharpFeature parentFeature, IPrecursorIndexAssignmentInstruction source)
            : base(context, parentFeature, source)
        {
            FeatureCall = new CSharpFeatureCall(context, source.FeatureCall.Item);
            SourceExpression = CSharpExpression.Create(context, (IExpression)source.Source);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IPrecursorIndexAssignmentInstruction Source { get { return (IPrecursorIndexAssignmentInstruction)base.Source; } }

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
            Debug.Assert(WriteDown);

            ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
            SourceExpression.WriteCSharp(writer, SourceExpressionContext, -1);

            string SourceString = SourceExpressionContext.ReturnValue;

            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
            string ArgumentListString = CSharpArgument.CSharpArgumentList(writer, ExpressionContext, FeatureCall);

            writer.WriteIndentedLine($"base[{ArgumentListString}] = {SourceString};");
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

            SourceExpression.SetWriteDown();
            FeatureCall.SetWriteDown();
        }
        #endregion
    }
}
