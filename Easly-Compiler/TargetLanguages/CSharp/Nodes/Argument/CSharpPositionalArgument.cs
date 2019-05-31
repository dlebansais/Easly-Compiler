namespace EaslyCompiler
{
    using CompilerNode;

    /// <summary>
    /// A C# argument.
    /// </summary>
    public interface ICSharpPositionalArgument : ICSharpArgument
    {
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        new IPositionalArgument Source { get; }
    }

    /// <summary>
    /// A C# argument.
    /// </summary>
    public class CSharpPositionalArgument : CSharpArgument, ICSharpPositionalArgument
    {
        #region Init
        /// <summary>
        /// Creates a new C# argument.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        public static ICSharpPositionalArgument Create(ICSharpContext context, IPositionalArgument source)
        {
            return new CSharpPositionalArgument(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpPositionalArgument"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        protected CSharpPositionalArgument(ICSharpContext context, IPositionalArgument source)
            : base(context, source)
        {
            IExpression ArgumentSource = (IExpression)source.Source;
            SourceExpression = CSharpExpression.Create(context, ArgumentSource);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        public new IPositionalArgument Source { get { return (IPositionalArgument)base.Source; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the argument.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        public override string CSharpText(string cSharpNamespace)
        {
            //TODO
            return null;
        }
        #endregion
    }
}
