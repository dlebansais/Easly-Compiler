namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ICommandInstruction"/>.
    /// </summary>
    public interface ICommandInstructionComputationRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ICommandInstruction"/>.
    /// </summary>
    public class CommandInstructionComputationRuleTemplate : RuleTemplate<ICommandInstruction, CommandInstructionComputationRuleTemplate>, ICommandInstructionComputationRuleTemplate
    {
        #region Init
        static CommandInstructionComputationRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTableSourceTemplate<ICommandInstruction, string, IScopeAttributeFeature, ITypeName>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureTypeName), TemplateNodeStart<ICommandInstruction>.Default),
                new OnceReferenceTableSourceTemplate<ICommandInstruction, string, IScopeAttributeFeature, ICompiledType>(nameof(IScopeHolder.FullScope), nameof(IScopeAttributeFeature.ResolvedFeatureType), TemplateNodeStart<ICommandInstruction>.Default),
                new SealedTableSourceTemplate<ICommandInstruction, string, IScopeAttributeFeature>(nameof(IScopeHolder.LocalScope), TemplateScopeStart<ICommandInstruction>.Default),
                new OnceReferenceCollectionSourceTemplate<ICommandInstruction, IArgument, IResultException>(nameof(ICommandInstruction.ArgumentList), nameof(IArgument.ResolvedException)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<ICommandInstruction, IResultException>(nameof(ICommandInstruction.ResolvedException)),
                new OnceReferenceDestinationTemplate<ICommandInstruction, ICompiledFeature>(nameof(ICommandInstruction.SelectedFeature)),
                new OnceReferenceDestinationTemplate<ICommandInstruction, ICommandOverloadType>(nameof(ICommandInstruction.SelectedOverload)),
                new OnceReferenceDestinationTemplate<ICommandInstruction, IFeatureCall>(nameof(ICommandInstruction.FeatureCall)),
                new OnceReferenceDestinationTemplate<ICommandInstruction, IProcedureType>(nameof(ICommandInstruction.CommandFinalType)),
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
        public override bool CheckConsistency(ICommandInstruction node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            data = null;
            bool Success = true;

            IQualifiedName Command = (IQualifiedName)node.Command;
            IList<IIdentifier> ValidPath = Command.ValidPath.Item;
            IClass EmbeddingClass = node.EmbeddingClass;
            IClassType BaseType = EmbeddingClass.ResolvedClassType.Item;

            IHashtableEx<string, IScopeAttributeFeature> LocalScope = Scope.CurrentScope(node);

            if (!ObjectType.GetQualifiedPathFinalType(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, ErrorList, out ICompiledFeature FinalFeature, out IDiscrete FinalDiscrete, out ITypeName FinalTypeName, out ICompiledType FinalType, out bool InheritBySideAttribute))
                return false;

            Debug.Assert(FinalFeature != null);

            IList<IArgument> ArgumentList = node.ArgumentList;
            List<IExpressionType> MergedArgumentList = new List<IExpressionType>();
            if (!Argument.Validate(ArgumentList, MergedArgumentList, out TypeArgumentStyles ArgumentStyle, ErrorList))
                return false;

            IList<ListTableEx<IParameter>> ParameterTableList = new List<ListTableEx<IParameter>>();
            ICommandOverloadType SelectedOverload = null;
            ListTableEx<IParameter> SelectedParameterList = null;
            IProcedureType CommandFinalType = null;
            bool IsHandled = false;

            switch (FinalType)
            {
                case IFunctionType AsFunctionType:
                case IPropertyType AsPropertyType:
                    AddSourceError(new ErrorInvalidInstruction(node));
                    Success = false;
                    IsHandled = true;
                    break;

                case IProcedureType AsProcedureType:
                    foreach (ICommandOverloadType Overload in AsProcedureType.OverloadList)
                        ParameterTableList.Add(Overload.ParameterTable);

                    int SelectedIndex;
                    if (!Argument.ArgumentsConformToParameters(ParameterTableList, MergedArgumentList, ArgumentStyle, ErrorList, node, out SelectedIndex))
                        Success = false;
                    else
                    {
                        SelectedOverload = AsProcedureType.OverloadList[SelectedIndex];
                        SelectedParameterList = ParameterTableList[SelectedIndex];
                        CommandFinalType = AsProcedureType;
                    }
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (Success)
            {
                ObjectType.FillResultPath(EmbeddingClass, BaseType, LocalScope, ValidPath, 0, Command.ValidResultTypePath.Item);

                IResultException ResolvedException = new ResultException();

                foreach (IArgument Item in node.ArgumentList)
                    ResultException.Merge(ResolvedException, Item.ResolvedException.Item);

                IList<IIdentifier> CommandInstructionException = SelectedOverload.ExceptionIdentifierList;
                ResultException.Merge(ResolvedException, CommandInstructionException);

                IFeatureCall FeatureCall = new FeatureCall(SelectedParameterList, ArgumentList, MergedArgumentList, ArgumentStyle);

                data = new Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>(ResolvedException, FinalFeature, SelectedOverload, FeatureCall, CommandFinalType);
            }

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(ICommandInstruction node, object data)
        {
            IResultException ResolvedException = ((Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>)data).Item1;
            ICompiledFeature SelectedFeature = ((Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>)data).Item2;
            ICommandOverloadType SelectedOverload = ((Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>)data).Item3;
            IFeatureCall FeatureCall = ((Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>)data).Item4;
            IProcedureType CommandFinalType = ((Tuple<IResultException, ICompiledFeature, ICommandOverloadType, IFeatureCall, IProcedureType>)data).Item5;

            node.ResolvedException.Item = ResolvedException;
            node.SelectedFeature.Item = SelectedFeature;
            node.SelectedOverload.Item = SelectedOverload;
            node.FeatureCall.Item = FeatureCall;
            node.CommandFinalType.Item = CommandFinalType;
        }
        #endregion
    }
}
