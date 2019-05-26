namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// List of rule templates belonging to the same set.
    /// </summary>
    public interface IRuleTemplateList : IList<IRuleTemplate>
    {
    }

    /// <summary>
    /// List of rule templates belonging to the same set.
    /// </summary>
    public class RuleTemplateList : List<IRuleTemplate>, IRuleTemplateList
    {
    }
}
