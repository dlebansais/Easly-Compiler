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
                new OnceReferenceDestinationTemplate<IThrowInstruction, ICompiledType>(nameof(IThrowInstruction.ResolvedType)),
                new OnceReferenceDestinationTemplate<IThrowInstruction, IResultException>(nameof(IThrowInstruction.ResolvedException)),
                new OnceReferenceDestinationTemplate<IThrowInstruction, IFeatureCall>(nameof(IThrowInstruction.FeatureCall)),
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

            if (!ObjectType.TypeConformToBase(ResolvedType, ExceptionType, isConversionAllowed: true))
            {
                AddSourceError(new ErrorExceptionTypeRequired(node));
                return false;
            }

            if (!FeatureName.TableContain(ResolvedType.FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Value))
            {
                AddSourceError(new ErrorUnknownIdentifier(CreationRoutine, ValidText));
                return false;
            }

            if (Value.Feature is CreationFeature AsCreationFeature)
            {
                List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
                if (!Argument.Validate(node.ArgumentList, MergedArgumentList, out TypeArgumentStyles TypeArgumentStyle, ErrorList))
                    return false;

                IList<ISealableList<IParameter>> ParameterTableList = new List<ISealableList<IParameter>>();
                ICompiledType FinalType = AsCreationFeature.ResolvedFeatureType2.Item;
                IProcedureType AsProcedureType = FinalType as IProcedureType;
                Debug.Assert(AsProcedureType != null);

                foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                    ParameterTableList.Add(Overload.ParameterTable);

                if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, TypeArgumentStyle, ErrorList, node, out int SelectedIndex))
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

                IFeatureCall FeatureCall = new FeatureCall(SelectedOverload.ParameterTable, node.ArgumentList, MergedArgumentList, TypeArgumentStyle);

                data = new Tuple<ICompiledType, IResultException, IFeatureCall>(ResolvedType, ResolvedException, FeatureCall);
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
            ICompiledType ResolvedType = ((Tuple<ICompiledType, IResultException, IFeatureCall>)data).Item1;
            IResultException ResolvedException = ((Tuple<ICompiledType, IResultException, IFeatureCall>)data).Item2;
            IFeatureCall FeatureCall = ((Tuple<ICompiledType, IResultException, IFeatureCall>)data).Item3;

            node.ResolvedType.Item = ResolvedType;
            node.ResolvedException.Item = ResolvedException;
            node.FeatureCall.Item = FeatureCall;
        }
        #endregion
    }
}
