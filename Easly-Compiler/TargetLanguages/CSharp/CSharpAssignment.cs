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
        /// The list of assignment destinations.
        /// </summary>
        IList<ICSharpQualifiedName> DestinationList { get; }

        /// <summary>
        /// The expression source of the assignment.
        /// </summary>
        ICSharpExpression SourceExpression { get; }

        /// <summary>
        /// Writes down the C# instruction.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="isOutputAssigned">True if temporary 'out' variable should be assigned to their destination.</param>
        /// <param name="destinationEntityList">The list of destinations assigned upon return.</param>
        void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, bool isOutputAssigned, out IList<string> destinationEntityList);
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
        /// <param name="destinationList">The list of assignment destinations.</param>
        /// <param name="sourceExpression">The expression source of the assignment.</param>
        public CSharpAssignment(IList<ICSharpQualifiedName> destinationList, ICSharpExpression sourceExpression)
        {
            DestinationList = destinationList;
            SourceExpression = sourceExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="destinationNameList">The list of destination names.</param>
        /// <param name="tempPrefix">The prefix to add to destination names.</param>
        /// <param name="sourceExpression">The expression source of the assignment.</param>
        public CSharpAssignment(ICSharpContext context, IList<IName> destinationNameList, string tempPrefix, ICSharpExpression sourceExpression)
        {
            DestinationList = new List<ICSharpQualifiedName>();
            IResultType ResolvedResult = sourceExpression.Source.ResolvedResult.Item;

            for (int i = 0; i < destinationNameList.Count; i++)
            {
                IName Name = destinationNameList[i];

                string Text = Name.ValidText.Item;
                BaseNode.IQualifiedName BaseNodeDestination = BaseNodeHelper.NodeHelper.CreateSimpleQualifiedName(Text);
                IExpressionType DestinationType = ResolvedResult.At(i);

                IQualifiedName Destination = new QualifiedName(BaseNodeDestination, DestinationType);

                IScopeAttributeFeature DestinationAttributeFeature = new ScopeAttributeFeature(Name, Text, DestinationType.ValueTypeName, DestinationType.ValueType);
                ICSharpScopeAttributeFeature DestinationFeature = CSharpScopeAttributeFeature.Create(context, null, DestinationAttributeFeature);

                ICSharpQualifiedName CSharpDestination = CSharpQualifiedName.Create(context, Destination, DestinationFeature, null, false);
                DestinationList.Add(CSharpDestination);
            }

            SourceExpression = sourceExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpAssignment"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="tempPrefix">The prefix to add to destination names.</param>
        /// <param name="sourceExpression">The expression source of the assignment.</param>
        public CSharpAssignment(ICSharpContext context, string tempPrefix, ICSharpExpression sourceExpression)
        {
            DestinationList = new List<ICSharpQualifiedName>();
            IResultType ResolvedResult = sourceExpression.Source.ResolvedResult.Item;

            for (int i = 0; i < ResolvedResult.Count; i++)
            {
                IExpressionType Result = ResolvedResult.At(i);

                string Text = Result.Name;
                BaseNode.IQualifiedName BaseNodeDestination = BaseNodeHelper.NodeHelper.CreateSimpleQualifiedName(Text);
                IExpressionType DestinationType = ResolvedResult.At(i);

                IQualifiedName Destination = new QualifiedName(BaseNodeDestination, DestinationType);

                IScopeAttributeFeature DestinationAttributeFeature = new ScopeAttributeFeature(sourceExpression.Source, Text, DestinationType.ValueTypeName, DestinationType.ValueType);
                ICSharpScopeAttributeFeature DestinationFeature = CSharpScopeAttributeFeature.Create(context, null, DestinationAttributeFeature);

                ICSharpQualifiedName CSharpDestination = CSharpQualifiedName.Create(context, Destination, DestinationFeature, null, false);
                DestinationList.Add(CSharpDestination);
            }

            SourceExpression = sourceExpression;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of assignment destinations.
        /// </summary>
        public IList<ICSharpQualifiedName> DestinationList { get; }

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
        /// <param name="isNeverSimple">True if the assignment must not consider an 'out' variable as simple.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="isOutputAssigned">True if temporary 'out' variable should be assigned to their destination.</param>
        /// <param name="destinationEntityList">The list of destinations assigned upon return.</param>
        public virtual void WriteCSharp(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, bool isOutputAssigned, out IList<string> destinationEntityList)
        {
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
        }

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
            string AssignementString = SourceExpression.CSharpText(writer, isNeverSimple, isDeclaredInPlace, DestinationList, -1);
            writer.WriteIndentedLine($"{AssignementString};");
        }

        private void WriteCSharpMultipleWithResult(ICSharpWriter writer, bool isNeverSimple, bool isDeclaredInPlace, int resultNameIndex)
        {
            Debug.Assert(resultNameIndex < DestinationList.Count);

            ICSharpQualifiedName Destination = DestinationList[resultNameIndex];
            IList<IIdentifier> ValidPath = Destination.Source.ValidPath.Item;
            Debug.Assert(ValidPath.Count == 1);

            string ResultDestinationName = ValidPath[0].ValidText.Item;

            string AssignementString = SourceExpression.CSharpText(writer, isNeverSimple, isDeclaredInPlace, DestinationList, resultNameIndex);

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
        #endregion
    }
}
