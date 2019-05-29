namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# argument.
    /// </summary>
    public interface ICSharpAssignmentArgument : ICSharpArgument
    {
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        new IAssignmentArgument Source { get; }

        /// <summary>
        /// List of assigned parameters.
        /// </summary>
        IList<string> ParameterNameList { get; }
    }

    /// <summary>
    /// A C# argument.
    /// </summary>
    public class CSharpAssignmentArgument : CSharpArgument, ICSharpAssignmentArgument
    {
        #region Init
        /// <summary>
        /// Creates a new C# argument.
        /// </summary>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        /// <param name="context">The creation context.</param>
        public static ICSharpAssignmentArgument Create(IAssignmentArgument source, ICSharpContext context)
        {
            return new CSharpAssignmentArgument(source, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignmentArgument"/> class.
        /// </summary>
        /// <param name="source">The Easly argument from which the C# argument is created.</param>
        /// <param name="context">The creation context.</param>
        protected CSharpAssignmentArgument(IAssignmentArgument source, ICSharpContext context)
            : base(source, context)
        {
            IExpression ArgumentSource = (IExpression)source.Source;
            SourceExpression = CSharpExpression.Create(ArgumentSource, context);

            foreach (IIdentifier Item in source.ParameterList)
                ParameterNameList.Add(Item.ValidText.Item);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly argument from which the C# argument is created.
        /// </summary>
        public new IAssignmentArgument Source { get { return (IAssignmentArgument)base.Source; } }

        /// <summary>
        /// List of assigned parameters.
        /// </summary>
        public IList<string> ParameterNameList { get; } = new List<string>();
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
