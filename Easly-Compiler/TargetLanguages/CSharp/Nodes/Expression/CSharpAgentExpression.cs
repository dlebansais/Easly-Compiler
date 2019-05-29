﻿namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpAgentExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IAgentExpression Source { get; }

        /// <summary>
        /// The delegate name.
        /// </summary>
        string Delegated { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpAgentExpression : CSharpExpression, ICSharpAgentExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpAgentExpression Create(IAgentExpression source, ICSharpContext context)
        {
            return new CSharpAgentExpression(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAgentExpression"/> class.
        /// </summary>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpAgentExpression(IAgentExpression source, ICSharpContext context)
            : base(source, context)
        {
            Delegated = ((IIdentifier)Source.Delegated).ValidText.Item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IAgentExpression Source { get { return (IAgentExpression)base.Source; } }

        /// <summary>
        /// True if the expression is complex (and requires to be surrounded with parenthesis).
        /// </summary>
        public override bool IsComplex { get { return false; } }

        /// <summary>
        /// The delegate name.
        /// </summary>
        public string Delegated { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            return CSharpNames.ToCSharpIdentifier(Delegated);
        }
        #endregion
    }
}
