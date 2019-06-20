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
                switch (SourceExpression)
                {
                    case ICSharpQueryExpression AsQueryExpression:
                        WriteCSharpQueryExpression(writer, AsQueryExpression);
                        break;

                    default:
                        string AssignementString = SourceExpression.CSharpText(writer, DestinationList, -1);
                        writer.WriteIndentedLine($"{AssignementString};");
                        break;
                }
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

        private void WriteCSharpQueryExpression(ICSharpWriter writer, ICSharpQueryExpression expression)
        {
            IResultType SourceResult = expression.Source.ResolvedResult.Item;
            Debug.Assert(SourceResult.Count >= DestinationList.Count);

            bool IsSimple = true;

            foreach (ICSharpQualifiedName QualifiedName in DestinationList)
                if (!QualifiedName.IsSimple)
                    IsSimple = false;

            int ResultNameIndex = expression.Source.ResolvedResult.Item.ResultNameIndex;

            //if (IsSimple)
                if (ResultNameIndex < 0)
                    WriteCSharpMultipleNoResult(writer, expression);
                else
                    WriteCSharpMultipleWithResult(writer, expression, ResultNameIndex);
        }

        private void WriteCSharpMultipleNoResult(ICSharpWriter writer, ICSharpQueryExpression expression)
        {
            string AssignementString = expression.CSharpText(writer, DestinationList, -1);
            writer.WriteIndentedLine($"{AssignementString};");
        }

        private void WriteCSharpMultipleWithResult(ICSharpWriter writer, ICSharpQueryExpression expression, int resultNameIndex)
        {
            Debug.Assert(resultNameIndex < DestinationList.Count);
            IList<IIdentifier> ValidPath = DestinationList[resultNameIndex].Source.ValidPath.Item;
            Debug.Assert(ValidPath.Count == 1);

            string ResultDestinationName = ValidPath[0].ValidText.Item;

            string AssignementString = expression.CSharpText(writer, DestinationList, resultNameIndex);
            writer.WriteIndentedLine($"{ResultDestinationName} = {AssignementString};");
        }
        #endregion
    }
}
