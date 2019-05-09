namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public interface ISimpleTypeInheritanceRuleTemplate : ISimpleTypeRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="ISimpleType"/>.
    /// </summary>
    public class SimpleTypeInheritanceRuleTemplate : SimpleTypeRuleTemplate<IInheritance>, ISimpleTypeInheritanceRuleTemplate
    {
        #region Init
        static SimpleTypeInheritanceRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new OnceReferenceTypeSourceTemplate<ISimpleType, ICompiledType>(),
                new SealedTableSourceTemplate<ISimpleType, ITypeName, ICompiledType>(nameof(IClass.InheritanceTable), TemplateClassStart<ISimpleType>.Default),
            };
        }
        #endregion
    }
}
