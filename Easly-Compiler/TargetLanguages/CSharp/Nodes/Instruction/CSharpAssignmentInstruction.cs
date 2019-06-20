namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public interface ICSharpAssignmentInstruction : ICSharpInstruction
    {
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        new IAssignmentInstruction Source { get; }

        /// <summary>
        /// The list of assignment destinations.
        /// </summary>
        IList<ICSharpQualifiedName> DestinationList { get; }

        /// <summary>
        /// The expression source of the assignment.
        /// </summary>
        ICSharpExpression SourceExpression { get; }
    }

    /// <summary>
    /// A C# instruction.
    /// </summary>
    public class CSharpAssignmentInstruction : CSharpInstruction, ICSharpAssignmentInstruction
    {
        #region Init
        /// <summary>
        /// Creates a new C# instruction.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        public static ICSharpAssignmentInstruction Create(ICSharpContext context, ICSharpFeature parentFeature, IAssignmentInstruction source)
        {
            return new CSharpAssignmentInstruction(context, parentFeature, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignmentInstruction"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="parentFeature">The parent feature.</param>
        /// <param name="source">The Easly instruction from which the C# instruction is created.</param>
        protected CSharpAssignmentInstruction(ICSharpContext context, ICSharpFeature parentFeature, IAssignmentInstruction source)
            : base(context, parentFeature, source)
        {
            SourceExpression = CSharpExpression.Create(context, (IExpression)Source.Source);

            foreach (IQualifiedName Destination in source.DestinationList)
            {
                ICompiledFeature SourceFeature = source.FinalFeatureTable[Destination];

                ICSharpFeature FinalFeature;

                if (SourceFeature is IScopeAttributeFeature AsScopeAttributeFeature)
                    FinalFeature = CSharpScopeAttributeFeature.Create(null, AsScopeAttributeFeature);
                else
                    FinalFeature = context.GetFeature(SourceFeature);

                ICSharpQualifiedName NewDestination = CSharpQualifiedName.Create(context, Destination, FinalFeature, null, false);
                DestinationList.Add(NewDestination);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly instruction from which the C# instruction is created.
        /// </summary>
        public new IAssignmentInstruction Source { get { return (IAssignmentInstruction)base.Source; } }

        /// <summary>
        /// The list of assignment destinations.
        /// </summary>
        public IList<ICSharpQualifiedName> DestinationList { get; } = new List<ICSharpQualifiedName>();

        /// <summary>
        /// The expression source of the assignment.
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
            if (DestinationList.Count > 1)
            {
                bool IsHandled = false;
                switch (SourceExpression)
                {
                    case ICSharpQueryExpression AsQueryExpression:
                    case ICSharpBinaryOperatorExpression AsBinaryOperatorExpression:
                    case ICSharpPrecursorExpression AsPrecursorExpression:
                        WriteCSharpMultiple(writer);
                        IsHandled = true;
                        break;

                    default:
                        /*
                        string AssignementString = SourceExpression.CSharpText(writer, DestinationList, -1);
                        writer.WriteIndentedLine($"{AssignementString};");
                        */
                        break;
                }

                Debug.Assert(IsHandled);
            }
            else
                WriteCSharpSingle(writer);
        }

        private void WriteCSharpSingle(ICSharpWriter writer)
        {
            Debug.Assert(DestinationList.Count == 1);

            ICSharpQualifiedName Destination = DestinationList[0];
            ICSharpFeature Feature = Destination.Feature;

            if (Destination.IsAttributeWithContract)
            {
                string SetterText = Destination.CSharpSetter(writer);
                string SourceText = SourceExpression.CSharpText(writer);

                writer.WriteIndentedLine($"{SetterText}({SourceText});");
            }
            else
            {
                string DestinationText = Destination.DecoratedCSharpText(writer, 0);
                string SourceText = SourceExpression.CSharpText(writer);

                writer.WriteIndentedLine($"{DestinationText} = {SourceText};");
            }
        }

        private void WriteCSharpMultiple(ICSharpWriter writer)
        {
            IResultType SourceResult = SourceExpression.Source.ResolvedResult.Item;
            Debug.Assert(SourceResult.Count >= DestinationList.Count);

            int ResultNameIndex = SourceExpression.Source.ResolvedResult.Item.ResultNameIndex;

            if (ResultNameIndex < 0)
                WriteCSharpMultipleNoResult(writer);
            else
                WriteCSharpMultipleWithResult(writer, ResultNameIndex);

            CopyComplexPaths(writer, ResultNameIndex);
        }

        private void WriteCSharpMultipleNoResult(ICSharpWriter writer)
        {
            string AssignementString = SourceExpression.CSharpText(writer, DestinationList, -1);
            writer.WriteIndentedLine($"{AssignementString};");
        }

        private void WriteCSharpMultipleWithResult(ICSharpWriter writer, int resultNameIndex)
        {
            Debug.Assert(resultNameIndex < DestinationList.Count);
            IList<IIdentifier> ValidPath = DestinationList[resultNameIndex].Source.ValidPath.Item;
            Debug.Assert(ValidPath.Count == 1);

            string ResultDestinationName = ValidPath[0].ValidText.Item;

            string AssignementString = SourceExpression.CSharpText(writer, DestinationList, resultNameIndex);
            writer.WriteIndentedLine($"{ResultDestinationName} = {AssignementString};");
        }

        private void CopyComplexPaths(ICSharpWriter writer, int resultNameIndex)
        {
            for (int i = 0; i < DestinationList.Count; i++)
            {
                if (i == resultNameIndex)
                    continue;

                ICSharpQualifiedName Destination = DestinationList[i];
                if (!Destination.IsSimple)
                {
                    string DestinationText = Destination.CSharpText(writer, 0);
                    string TempText = DestinationText.Replace('.', '_');

                    writer.WriteIndentedLine($"{DestinationText} = Temp_{TempText};");
                }
            }
        }
        #endregion
    }
}
