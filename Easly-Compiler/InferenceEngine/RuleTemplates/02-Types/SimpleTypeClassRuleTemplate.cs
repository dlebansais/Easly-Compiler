namespace EaslyCompiler
{
    using System.Collections.Generic;
    using CompilerNode;

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
                new OnceReferenceTypeSourceTemplate<ISimpleType, ICompiledType>(),
            };
        }
        #endregion
    }
}
