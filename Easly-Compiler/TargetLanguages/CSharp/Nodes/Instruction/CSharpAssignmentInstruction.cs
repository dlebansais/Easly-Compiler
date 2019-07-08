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
            List<ICSharpVariableContext> DestinationNameList = new List<ICSharpVariableContext>();
            IDictionary<string, ICSharpQualifiedName> DestinationTable = new Dictionary<string, ICSharpQualifiedName>();

            foreach (ICSharpQualifiedName Destination in DestinationList)
            {
                ICSharpVariableContext DestinationContext;

                if (Destination.IsSimple)
                {
                    string DestinationName = CSharpNames.ToCSharpIdentifier(Destination.SimpleName);
                    if (writer.AttachmentMap.ContainsKey(DestinationName))
                        DestinationName = writer.AttachmentMap[DestinationName];

                    DestinationContext = new CSharpVariableContext(DestinationName);
                }
                else
                {
                    string DestinationName = writer.GetTemporaryName();
                    DestinationContext = new CSharpVariableContext(DestinationName, isDeclared: false);
                    DestinationTable.Add(DestinationName, Destination);
                }

                DestinationNameList.Add(DestinationContext);
            }

            ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext(DestinationNameList);

            SourceExpression.WriteCSharp(writer, ExpressionContext, false, -1);

            IDictionary<string, string> FilledDestinationTable = ExpressionContext.FilledDestinationTable;

            for (int i = 0; i < DestinationList.Count; i++)
            {
                ICSharpQualifiedName Destination = DestinationList[i];
                string Name = DestinationNameList[i].Name;

                Debug.Assert(FilledDestinationTable.ContainsKey(Name));
                string ResultText = FilledDestinationTable[Name];

                if (ResultText == null)
                    ResultText = ExpressionContext.ReturnValue;

                if (Destination.IsAttributeWithContract)
                {
                    string SetterText = Destination.CSharpSetter(writer);
                    writer.WriteIndentedLine($"{SetterText}({ResultText});");
                }
                else if (DestinationTable.ContainsKey(Name))
                {
                    string DestinationName = DestinationTable[Name].CSharpText(writer, 0);
                    writer.WriteIndentedLine($"{DestinationName} = {ResultText};");
                }
                else if (ResultText != Name)
                    writer.WriteIndentedLine($"{Name} = {ResultText};");
            }
        }
        #endregion
    }
}
