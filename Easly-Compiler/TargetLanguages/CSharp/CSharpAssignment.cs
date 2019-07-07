namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// Assignment operation in C#
    /// </summary>
    public interface ICSharpAssignment
    {
        /// <summary>
        /// The expression source of the assignment.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of names for each assigned resulst.
        /// </summary>
        IList<string> DestinationNameList { get; }

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace);
    }

    /// <summary>
    /// Assignment operation in C#
    /// </summary>
    public class CSharpAssignment : ICSharpAssignment
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="destinationList">The list of assignment destinations.</param>
        /// <param name="sourceExpression">The expression source of the assignment.</param>
        public CSharpAssignment(ICSharpContext context, IList<ICSharpQualifiedName> destinationList, ICSharpExpression sourceExpression)
        {
            DestinationNameList = new List<string>();
            DestinationTable = new Dictionary<string, ICSharpQualifiedName>();
/*
            foreach (ICSharpQualifiedName Destination in destinationList)
            {
                string DestinationName;

                if (Destination.IsSimple)
                    DestinationName = Destination.SimpleName;
                else
                {
                    DestinationName = context.GetTemporaryName();
                    DestinationTable.Add(DestinationName, Destination);
                }

                DestinationNameList.Add(DestinationName);
            }
            */
            ExpressionContext = new CSharpExpressionContext(DestinationNameList);
            SourceExpression = sourceExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="destinationList">The list of assignment destinations.</param>
        /// <param name="sourceExpression">The expression source of the assignment.</param>
        public CSharpAssignment(ICSharpContext context, IList<IName> destinationList, ICSharpExpression sourceExpression)
        {
            DestinationNameList = new List<string>();
            DestinationTable = new Dictionary<string, ICSharpQualifiedName>();

            foreach (IName Destination in destinationList)
            {
                string DestinationName = Destination.ValidText.Item;
                DestinationNameList.Add(DestinationName);
            }

            ExpressionContext = new CSharpExpressionContext(DestinationNameList);
            SourceExpression = sourceExpression;
        }

        private ICSharpExpressionContext ExpressionContext;
        private IDictionary<string, ICSharpQualifiedName> DestinationTable;
        #endregion

        #region Properties
        /// <summary>
        /// The expression source of the assignment.
        /// </summary>
        public ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// The list of names for each assigned resulst.
        /// </summary>
        public IList<string> DestinationNameList { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace)
        {
            SourceExpression.WriteCSharp(writer, ExpressionContext, isNeverSimple, isDeclaredInPlace, -1);

            foreach (KeyValuePair<string, ICSharpQualifiedName> Entry in DestinationTable)
            {
                string ResultName = Entry.Key;
                string DestinationName = Entry.Value.CSharpText(writer, 0);

                writer.WriteIndentedLine($"{DestinationName} = {ResultName};");
            }

#if REMOVED
            if (DestinationList.Count > 1)
            {
                bool IsHandled = false;
                switch (SourceExpression)
                {
                    case ICSharpQueryExpression AsQueryExpression:
                    case ICSharpBinaryOperatorExpression AsBinaryOperatorExpression:
                    case ICSharpPrecursorExpression AsPrecursorExpression:
                        WriteCSharpMultiple(writer, isNeverSimple, isDeclaredInPlace, isOutputAssigned, out destinationEntityList);
                        IsHandled = true;
                        break;

                    default:
                        /*
                        string AssignementString = SourceExpression.CSharpText(writer, DestinationList, -1);
                        writer.WriteIndentedLine($"{AssignementString};");
                        */
                        destinationEntityList = new List<string>();
                        break;
                }

                Debug.Assert(IsHandled);
            }
            else
                WriteCSharpSingle(writer, isDeclaredInPlace, out destinationEntityList);
#endif
        }

#if REMOVED
        private void WriteCSharpSingle(ICSharpWriter writer, bool isDeclaredInPlace, out IList<string> destinationEntityList)
        {
            Debug.Assert(DestinationList.Count == 1);

            ICSharpQualifiedName Destination = DestinationList[0];
            ICSharpFeature Feature = Destination.Feature;
            string DestinationText = Destination.DecoratedCSharpText(writer, 0);

            if (Destination.IsAttributeWithContract)
            {
                string SetterText = Destination.CSharpSetter(writer);
                string SourceText = SourceExpression.CSharpText(writer);

                writer.WriteIndentedLine($"{SetterText}({SourceText});");
            }
            else
            {
                string SourceText = SourceExpression.CSharpText(writer);

                if (isDeclaredInPlace)
                {
                    ICSharpScopeAttributeFeature DestinationFeature = Destination.Feature as ICSharpScopeAttributeFeature;
                    Debug.Assert(DestinationFeature != null);
                    ICSharpType DestinationType = DestinationFeature.Type;

                    string DestinationTypeText = DestinationType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                    writer.WriteIndentedLine($"{DestinationTypeText} {DestinationText} = {SourceText};");
                }
                else
                    writer.WriteIndentedLine($"{DestinationText} = {SourceText};");
            }

            destinationEntityList = new List<string>() { DestinationText };
        }

        private void WriteCSharpMultiple(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, bool isOutputAssigned, out IList<string> destinationEntityList)
        {
            IResultType SourceResult = SourceExpression.Source.ResolvedResult.Item;
            Debug.Assert(SourceResult.Count >= DestinationList.Count);

            int ResultNameIndex = SourceExpression.Source.ResolvedResult.Item.ResultNameIndex;

            if (ResultNameIndex < 0)
                WriteCSharpMultipleNoResult(writer, isNeverSimple, isDeclaredInPlace);
            else
                WriteCSharpMultipleWithResult(writer, isNeverSimple, isDeclaredInPlace, ResultNameIndex);

            CopyComplexPaths(writer, isNeverSimple, isOutputAssigned, ResultNameIndex, out destinationEntityList);
        }

        private void WriteCSharpMultipleNoResult(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace)
        {
            SourceExpression.WriteCSharp(writer, ExpressionContext, isNeverSimple, isDeclaredInPlace, DestinationList, -1, out string AssignementString);
            writer.WriteIndentedLine($"{AssignementString};");
        }

        private void WriteCSharpMultipleWithResult(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, int resultNameIndex)
        {
            Debug.Assert(resultNameIndex < DestinationList.Count);

            ICSharpQualifiedName Destination = DestinationList[resultNameIndex];
            IList<IIdentifier> ValidPath = Destination.Source.ValidPath.Item;
            Debug.Assert(ValidPath.Count == 1);

            string ResultDestinationName = ValidPath[0].ValidText.Item;

            SourceExpression.WriteCSharp(writer, ExpressionContext, isNeverSimple, isDeclaredInPlace, DestinationList, resultNameIndex, out string AssignementString);

            if (isDeclaredInPlace)
            {
                ICSharpScopeAttributeFeature DestinationFeature = Destination.Feature as ICSharpScopeAttributeFeature;
                Debug.Assert(DestinationFeature != null);
                ICSharpType DestinationType = DestinationFeature.Type;

                string DestinationTypeText = DestinationType.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

                writer.WriteIndentedLine($"{DestinationTypeText} {ResultDestinationName} = {AssignementString};");
            }
            else
                writer.WriteIndentedLine($"{ResultDestinationName} = {AssignementString};");
        }

        private void CopyComplexPaths(ICSharpWriter writer, bool isNeverSimple, bool isOutputAssigned, int resultNameIndex, out IList<string> destinationEntityList)
        {
            destinationEntityList = new List<string>();

            for (int i = 0; i < DestinationList.Count; i++)
            {
                if (i == resultNameIndex)
                {
                    IList<IIdentifier> ValidPath = DestinationList[resultNameIndex].Source.ValidPath.Item;
                    Debug.Assert(ValidPath.Count == 1);

                    string ResultDestinationName = ValidPath[0].ValidText.Item;
                    destinationEntityList.Add(ResultDestinationName);
                }
                else
                {
                    ICSharpQualifiedName Destination = DestinationList[i];
                    string DestinationText = Destination.CSharpText(writer, 0);

                    if ((!Destination.IsSimple || isNeverSimple) && isOutputAssigned)
                    {
                        string TempText = DestinationText.Replace('.', '_');

                        writer.WriteIndentedLine($"{DestinationText} = Temp_{TempText};");
                    }

                    destinationEntityList.Add(DestinationText);
                }
            }
        }
#endif
        #endregion
    }
}
