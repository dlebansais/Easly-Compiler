namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public interface IAllInheritancesGroupRuleTemplate : IRuleTemplate
    {
    }

    /// <summary>
    /// A rule to process <see cref="IClass"/>.
    /// </summary>
    public class AllInheritancesGroupRuleTemplate : RuleTemplate<IClass, AllInheritancesGroupRuleTemplate>, IAllInheritancesGroupRuleTemplate
    {
        #region Init
        static AllInheritancesGroupRuleTemplate()
        {
            SourceTemplateList = new List<ISourceTemplate>()
            {
                new SealedListSourceTemplate<IClass, IClass>(nameof(IClass.ClassGroupList2)),
                new SealedListCollectionSourceTemplate<IClass, IClass, IClass>(nameof(IClass.ClassGroupList2), nameof(IClass.ClassGroupList2)),
            };

            DestinationTemplateList = new List<IDestinationTemplate>()
            {
                new OnceReferenceDestinationTemplate<IClass, SingleClassGroup>(nameof(IClass.ClassGroup2)),
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

            IList<IClass> SingleClassList = new List<IClass>();

            foreach (IClass InheritedClass in node.ClassGroupList2)
            {
                Debug.Assert(InheritedClass.ClassGroupList2.IsSealed);

                if (InheritedClass.Cloneable == BaseNode.CloneableStatus.Single)
                {
                    SingleClassList.Add(InheritedClass);

                    foreach (IClass Class in InheritedClass.ClassGroupList2)
                        SingleClassList.Add(Class);
                }
            }

            bool IsHandled = false;
            switch (node.Cloneable)
            {
                case BaseNode.CloneableStatus.Single:
                    SingleClassList.Add(node);
                    IsHandled = true;
                    break;

                case BaseNode.CloneableStatus.Cloneable:
                    if (SingleClassList.Count > 0)
                    {
                        AddSourceError(new ErrorCloneableClass(node));
                        Success = false;
                    }
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if (Success)
                data = SingleClassList;

            return Success;
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="node">The node instance to modify.</param>
        /// <param name="data">Private data from CheckConsistency().</param>
        public override void Apply(IClass node, object data)
        {
            IList<IClass> SingleClassList = (IList<IClass>)data;

            node.ClassGroup2.Item = new SingleClassGroup(node);
            node.UpdateClassGroup(SingleClassList);
        }
        #endregion
    }
}
