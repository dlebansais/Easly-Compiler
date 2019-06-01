namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A C# body.
    /// </summary>
    public interface ICSharpEffectiveBody : ICSharpBody
    {
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        new IEffectiveBody Source { get; }

        /// <summary>
        /// The list of instructions in the body.
        /// </summary>
        IList<ICSharpInstruction> BodyInstructionList { get; }

        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList);
    }

    /// <summary>
    /// A C# body.
    /// </summary>
    public class CSharpEffectiveBody : CSharpBody, ICSharpEffectiveBody
    {
        #region Init
        /// <summary>
        /// Creates a new C# body.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        public static ICSharpEffectiveBody Create(ICSharpContext context, IEffectiveBody source)
        {
            return new CSharpEffectiveBody(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpEffectiveBody"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly body from which the C# body is created.</param>
        protected CSharpEffectiveBody(ICSharpContext context, IEffectiveBody source)
            : base(context, source)
        {
            foreach (IInstruction Instruction in source.BodyInstructionList)
            {
                ICSharpInstruction NewInstruction = CSharpInstruction.Create(context, Instruction);
                BodyInstructionList.Add(NewInstruction);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly body from which the C# body is created.
        /// </summary>
        public new IEffectiveBody Source { get { return (IEffectiveBody)base.Source; } }

        /// <summary>
        /// The list of instructions in the body.
        /// </summary>
        public IList<ICSharpInstruction> BodyInstructionList { get; } = new List<ICSharpInstruction>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the body source code.
        /// </summary>
        /// <param name="writer">The stream on which to write down.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        /// <param name="flags">Some flags.</param>
        /// <param name="resultType">Type of the result, if any.</param>
        /// <param name="skipFirstInstruction">Skip the first instruction.</param>
        /// <param name="initialisationStringList">List of initializations.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, string outputNamespace, CSharpBodyFlags flags, string resultType, bool skipFirstInstruction, IList<string> initialisationStringList)
        {
            //TODO
        }
        #endregion
    }
}
