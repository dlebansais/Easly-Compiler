namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IThrowInstruction"/>.
    /// </summary>
    public interface IThrowInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IThrowInstruction"/>.
    /// </summary>
    public class ThrowInstructionComputationRuleTemplate : RuleTemplate<IThrowInstruction, ThrowInstructionComputationRuleTemplate>, IThrowInstructionComputationRuleTemplate
    {
        #region Init
        static ThrowInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IThrowInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<IThrowInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<IThrowInstruction, IArgument, IResultException>(nameof(IThrowInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IThrowInstruction, IResultException>(nameof(IThrowInstruction.ResolvedException)),
                new UnsealedListDestinationTemplate<IThrowInstruction, IParameter>(nameof(IThrowInstruction.SelectedParameterList)),
            };
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks for errors before applying a rule.
        /// </summary>
        /// <param name="node">The node instance to check.</param>
        /// <param name="dataList">Optional data collected during inspection of sources.</param>
        /// <param name="data">Private data to give to Apply() upon return.</param>
        /// <returns>True if an error occured.</returns>
        public override bool CheckConsistency(IThrowInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IObjectType ThrowExceptionType = (IObjectType)node.ExceptionType;
            ICompiledType ResolvedType = ThrowExceptionType.ResolvedType.Item;
            IIdentifier CreationRoutine = (IIdentifier)node.CreationRoutine;
            string ValidText = CreationRoutine.ValidText.Item;
            IClass EmbeddingClass = node.EmbeddingClass;

            if (!Expression.IsLanguageTypeAvailable(LanguageClasses.Exception.Guid, node, out ITypeName ExceptionTypeName, out ICompiledType ExceptionType))
            {
                AddSourceError(new ErrorExceptionTypeMissing(node));
                return false;
            }

            IHashtableEx<ICompiledType, ICompiledType> SubstitutionTypeTable = new HashtableEx<ICompiledType, ICompiledType>();

            if (!ObjectType.TypeConformToBase(ResolvedType, ExceptionType, SubstitutionTypeTable))
            {
                AddSourceError(new ErrorExceptionTypeRequired(node));
                return false;
            }

            if (!FeatureName.TableContain(ResolvedType.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Value))
            {
                AddSourceError(new ErrorUnknownIdentifier(CreationRoutine, ValidText));
                return false;
            }

            if (Value.Feature.Item is CreationFeature AsCreationFeature)
            {
                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, ErrorList))
                    return false;

                IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
                ICompiledType FinalType = AsCreationFeature.ResolvedFeatureType.Item;
                IProcedureType AsProcedureType = FinalType as IProcedureType;
                Debug.Assert(AsProcedureType != null);

                foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                    ParameterTableList.Add(Overload.ParameterTable);

                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out int SelectedIndex))
                    return false;

                ICommandOverloadType SelectedOverload = AsProcedureType.OverloadList[SelectedIndex];

                IResultException ResolvedException = new ResultException();
                ResolvedException.Add(ValidText);

                Debug.Assert(ResolvedException.At(0).Text == ValidText);
                Debug.Assert(ResolvedException.At(0).ValidText.IsAssigned);
                Debug.Assert(ResolvedException.At(0).ValidText.Item == ValidText);

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                ResultException.Merge(ResolvedException, SelectedOverload.ExceptionIdentifierList);

                ListTableEx<IParameter> SelectedParameterList = SelectedOverload.ParameterTable;

                data = new Tuple<IResultException, ListTableEx<IParameter>>(ResolvedException, SelectedParameterList);
            }
            else
            {
                AddSourceError(new ErrorCreationFeatureRequired(CreationRoutine, ValidText));
                return false;
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IThrowInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item1;
            ListTableEx<IParameter> SelectedParameterList = ((Tuple<IResultException, ListTableEx<IParameter>>)data).Item2;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedParameterList.AddRange(SelectedParameterList);
            node.SelectedParameterList.Seal();
        }
        #endregion
    }
}
