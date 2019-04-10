namespace EaslyCompiler
{
    using System.Collections.Generic;

    /// <summary>
    /// Context used when walking the node tree to build a list of sources for a <see cref="InferenceEngine"/>.
    /// </summary>
    public class BuildInferenceSourceList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildInferenceSourceList"/> class.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that will be used during inference.</param>
        public BuildInferenceSourceList(IList<IRuleTemplate> ruleTemplateList)
        {
            RuleTemplateList = ruleTemplateList;
        }

        /// <summary>
        /// The list of rule templates that will be used during inference.
        /// </summary>
        public IList<IRuleTemplate> RuleTemplateList { get; }

        /// <summary>
        /// The resulting source list.
        /// </summary>
        public List<ISource> SourceList { get; } = new List<ISource>();
    }
}
