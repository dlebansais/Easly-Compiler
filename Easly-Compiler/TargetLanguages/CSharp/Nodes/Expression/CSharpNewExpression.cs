﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpNewExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new INewExpression Source { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpNewExpression : CSharpExpression, ICSharpNewExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpNewExpression Create(ICSharpContext context, INewExpression source)
        {
            return new CSharpNewExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpNewExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpNewExpression(ICSharpContext context, INewExpression source)
            : base(context, source)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new INewExpression Source { get { return (INewExpression)base.Source; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, int skippedIndex)
        {
            Debug.Assert(WriteDown);

            expressionContext.SetSingleReturnValue(string.Empty);//TODO
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            WriteDown = true;
        }
        #endregion
    }
}
