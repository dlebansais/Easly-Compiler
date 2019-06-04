namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllConversionsRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllConversionsRuleTemplate : RuleTemplate<IClass, AllConversionsRuleTemplate>, IAllConversionsRuleTemplate
    {
        #region Init
        static AllConversionsRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<IClass, IFeatureName, IFeatureInstance>(nameof(IClass.FeatureTable)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, ICreationFeature>(nameof(IClass.ConversionFromTable)),
                new UnsealedTableDestinationTemplate<IClass, IFeatureName, IFunctionFeature>(nameof(IClass.ConversionToTable)),
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
        public override bool CheckConsistency(IClass node, IDictionary<ISourceTemplate, object> dataList, out object data)
        {
            bool Success = true;
            data = null;

            IList<IIdentifier> ConversionList = node.ConversionList;
            ISealableDictionary<IFeatureName, IFeatureInstance> FeatureTable = node.FeatureTable;
            ISealableDictionary<IFeatureName, ICreationFeature> ConversionFromTable = new SealableDictionary<IFeatureName, ICreationFeature>();
            ISealableDictionary<IFeatureName, IFunctionFeature> ConversionToTable = new SealableDictionary<IFeatureName, IFunctionFeature>();

            foreach (IIdentifier Identifier in ConversionList)
            {
                Debug.Assert(Identifier.ValidText.IsAssigned);
                string ValidText = Identifier.ValidText.Item;

                if (!FeatureName.TableContain(FeatureTable, ValidText, out IFeatureName Key, out IFeatureInstance Instance))
                {
                    AddSourceError(new ErrorUnknownIdentifier(Identifier, ValidText));
                    Success = false;
                }
                else
                {
                    bool IsValidFeature;

                    switch (Instance.Feature)
                    {
                        case ICreationFeature AsCreationFeature:
                            IsValidFeature = IsConversionFrom(Key, AsCreationFeature, ConversionFromTable);
                            break;

                        case IFunctionFeature AsFunctionFeature:
                            IsValidFeature = IsConversionTo(Key, AsFunctionFeature, ConversionToTable);
                            break;

                        default:
                            IsValidFeature = false;
                            break;
                    }

                    if (!IsValidFeature)
                    {
                        AddSourceError(new ErrorInvalidConversionFeature(Identifier));
                        Success = false;
                    }
                }
            }

            if (Success)
                data = new Tuple<ISealableDictionary<IFeatureName, ICreationFeature>, ISealableDictionary<IFeatureName, IFunctionFeature>>(ConversionFromTable, ConversionToTable);

            return Success;
        }

        private bool IsConversionFrom(IFeatureName featureName, ICreationFeature feature, ISealableDictionary<IFeatureName, ICreationFeature> conversionTable)
        {
            bool IsValidOverload = true;
            foreach (ICommandOverload Overload in feature.OverloadList)
                IsValidOverload &= Overload.ParameterList.Count == 1;

            if (IsValidOverload)
                conversionTable.Add(featureName, feature);

            return IsValidOverload;
        }

        private bool IsConversionTo(IFeatureName featureName, IFunctionFeature feature, ISealableDictionary<IFeatureName, IFunctionFeature> conversionTable)
        {
            bool IsValidOverload = true;
            foreach (IQueryOverload Overload in feature.OverloadList)
                IsValidOverload &= Overload.ParameterList.Count == 0 && Overload.ResultList.Count == 1;

            if (IsValidOverload)
                conversionTable.Add(featureName, feature);

            return IsValidOverload;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            ISealableDictionary<IFeatureName, ICreationFeature> ConversionFromTable = ((Tuple<ISealableDictionary<IFeatureName, ICreationFeature>, ISealableDictionary<IFeatureName, IFunctionFeature>>)data).Item1;
            ISealableDictionary<IFeatureName, IFunctionFeature> ConversionToTable = ((Tuple<ISealableDictionary<IFeatureName, ICreationFeature>, ISealableDictionary<IFeatureName, IFunctionFeature>>)data).Item2;

            node.ConversionFromTable.Merge(ConversionFromTable);
            node.ConversionFromTable.Seal();

            node.ConversionToTable.Merge(ConversionToTable);
            node.ConversionToTable.Seal();
        }
        #endregion
    }
}
