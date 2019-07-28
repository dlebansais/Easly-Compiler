namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using FormattedNumber;

    /// <summary>
    /// A C# case node.
    /// </summary>
    public interface ICSharpWith : ICSharpSource<IWith>, ICSharpOutputNode
    {
        /// <summary>
        /// The parent feature.
        /// </summary>
        ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of case constants.
        /// </summary>
        IList<ILanguageConstant> ConstantList { get; }

        /// <summary>
        /// The case instructions.
        /// </summary>
        ICSharpScope Instructions { get; }

        /// <summary>
        /// Writes down the C# conditional instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharp(ICSharpWriter writer);
    }

    /// <summary>
    /// A C# case node.
    /// </summary>
    public class CSharpWith : CSharpSource<IWith>, ICSharpWith
    {
        #region Init
        /// <summary>
        /// Create a new C# conditional.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpWith Create(ICSharpContext context, ICSharpFeature parentFeature, IWith source)
        {
            return new CSharpWith(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpWith"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpWith(ICSharpContext context, ICSharpFeature parentFeature, IWith source)
            : base(source)
        {
            ParentFeature = parentFeature;

            foreach (IRange Range in source.RangeList)
                AddValueToList(Range.ResolvedRange.Item);

            Instructions = CSharpScope.Create(context, parentFeature, (IScope)source.Instructions);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent feature.
        /// </summary>
        public ICSharpFeature ParentFeature { get; }

        /// <summary>
        /// The list of case constants.
        /// </summary>
        public IList<ILanguageConstant> ConstantList { get; } = new List<ILanguageConstant>();

        /// <summary>
        /// The conditional instructions.
        /// </summary>
        public ICSharpScope Instructions { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# conditional instructions.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public virtual void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            foreach (ILanguageConstant Constant in ConstantList)
            {
                bool IsHandled = false;

                switch (Constant)
                {
                    case INumberLanguageConstant AsManifestConstant:
                        CanonicalNumber AsNumber = AsManifestConstant.Value;
                        if (AsNumber.TryParseInt(out int IntValue))
                        {
                            writer.WriteIndentedLine($"case {IntValue}:");
                            IsHandled = true;
                        }
                        break;

                    case IDiscreteLanguageConstant AsDiscreteConstant:
                        IDiscrete DiscreteItem = AsDiscreteConstant.Discrete;
                        IName ClassEntityName = (IName)DiscreteItem.EmbeddingClass.EntityName;
                        string ClassName = CSharpNames.ToCSharpIdentifier(ClassEntityName.ValidText.Item);
                        IFeatureName DiscreteFeatureName = DiscreteItem.ValidDiscreteName.Item;
                        string DiscreteName = CSharpNames.ToCSharpIdentifier(DiscreteFeatureName.Name);

                        writer.WriteIndentedLine($"case {ClassName}.{DiscreteName}:");
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            Instructions.WriteCSharp(writer, CSharpCurlyBracketsInsertions.Indifferent, true);
        }
        #endregion

        #region Implementation
        private void AddValueToList(IConstantRange range)
        {
            if (LanguageConstant.TryParseInt(range.Minimum, out int IntMinimum) && LanguageConstant.TryParseInt(range.Maximum, out int IntMaximum))
            {
                ConstantList.Add(range.Minimum);

                if (IntMinimum < IntMaximum)
                {
                    for (int i = IntMinimum + 1; i < IntMaximum; i++)
                        ConstantList.Add(new NumberLanguageConstant(new CanonicalNumber(i)));

                    ConstantList.Add(range.Maximum);
                }
            }

            else if (range.Minimum is DiscreteLanguageConstant AsDiscreteMinimum && AsDiscreteMinimum.IsValueKnown && range.Maximum is DiscreteLanguageConstant AsDiscreteMaximum && AsDiscreteMaximum.IsValueKnown)
            {
                ConstantList.Add(AsDiscreteMinimum);

                if (AsDiscreteMaximum.IsConstantGreater(AsDiscreteMinimum))
                {
                    IClass EmbeddingClass = AsDiscreteMinimum.Discrete.EmbeddingClass;

                    int DiscreteMinimumIndex = EmbeddingClass.DiscreteList.IndexOf(AsDiscreteMinimum.Discrete);
                    int DiscreteMaximumIndex = EmbeddingClass.DiscreteList.IndexOf(AsDiscreteMaximum.Discrete);

                    if (DiscreteMinimumIndex >= 0 && DiscreteMaximumIndex >= 0)
                    {
                        for (int Index = DiscreteMinimumIndex + 1; Index < DiscreteMaximumIndex; Index++)
                        {
                            IDiscrete MiddleDiscrete = EmbeddingClass.DiscreteList[Index];
                            DiscreteLanguageConstant MiddleConstant = new DiscreteLanguageConstant(MiddleDiscrete);
                            ConstantList.Add(MiddleConstant);
                        }
                    }
                }
            }
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// True if the node should be produced.
        /// </summary>
        public bool WriteDown { get; private set; }

        /// <summary>
        /// Sets the <see cref="WriteDown"/> flag.
        /// </summary>
        public void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            Instructions.SetWriteDown();
        }
        #endregion
    }
}
