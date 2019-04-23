namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public interface ISimpleTypeClassRuleTemplate : ISimpleTypeRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public class SimpleTypeClassRuleTemplate : SimpleTypeRuleTemplate<IClass>, ISimpleTypeClassRuleTemplate
    {
        #region Init
        static SimpleTypeClassRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedTableSourceTemplate<ISimpleType, string, IImportedClass>(nameof(IClass.ImportedClassTable), TemplateClassStart<ISimpleType>.Default),
                new SealedTableSourceTemplate<ISimpleType, string, ICompiledType>(nameof(IClass.LocalGenericTable), TemplateClassStart<ISimpleType>.Default),
                new SealedTableSourceTemplate<ISimpleType, IFeatureName, IHashtableEx<string, IClass>>(nameof(IClass.LocalExportTable), TemplateClassStart<ISimpleType>.Default),
                new SealedTableSourceTemplate<ISimpleType, IFeatureName, ITypedefType>(nameof(IClass.LocalTypedefTable), TemplateClassStart<ISimpleType>.Default),
                new SealedTableSourceTemplate<ISimpleType, IFeatureName, IDiscrete>(nameof(IClass.LocalDiscreteTable), TemplateClassStart<ISimpleType>.Default),
                new SealedTableSourceTemplate<ISimpleType, IFeatureName, IFeatureInstance>(nameof(IClass.LocalFeatureTable), TemplateClassStart<ISimpleType>.Default),
                new OnceReferenceSourceTemplate<ISimpleType, IClassType>(nameof(IClass.ResolvedClassType), TemplateClassStart<ISimpleType>.Default),
            };
        }
        #endregion
    }
}
