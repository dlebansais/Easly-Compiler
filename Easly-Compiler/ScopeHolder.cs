namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Information about entities, where they are declared or used.
    /// </summary>
    public interface IScopeHolder
    {
        /// <summary>
        /// Entities local to a scope.
        /// </summary>
        IHashtableEx<string, IScopeAttributeFeature> LocalScope { get; }

        /// <summary>
        /// List of scopes containing the current instance.
        /// </summary>
        IList<IScopeHolder> InnerScopes { get; }

        /// <summary>
        /// All reachable entities.
        /// </summary>
        IHashtableEx<string, IScopeAttributeFeature> FullScope { get; }
    }

    /// <summary>
    /// Helper class for scopes.
    /// </summary>
    public static class ScopeHolder
    {
        /// <summary>
        /// Adds the content of a local scope to a full scope and all inner scopes inside.
        /// </summary>
        /// <param name="source">The local scope.</param>
        /// <param name="innerScopeList">The list of inner scopes.</param>
        public static void RecursiveAdd(IHashtableEx<string, IScopeAttributeFeature> source, IList<IScopeHolder> innerScopeList)
        {
            foreach (IScopeHolder Item in innerScopeList)
            {
                Item.FullScope.Merge(source);
                RecursiveAdd(source, Item.InnerScopes);
            }
        }

        /// <summary>
        /// Finds all names that in conflict with others already defined in embedding scopes.
        /// </summary>
        /// <param name="source">The scope where the check is performed.</param>
        /// <param name="innerScopeList">The list of inner scopes.</param>
        /// <param name="conflictList">The list of conflicting names.</param>
        public static void RecursiveCheck(IHashtableEx<string, IScopeAttributeFeature> source, IList<IScopeHolder> innerScopeList, IList<string> conflictList)
        {
            foreach (IScopeHolder Item in innerScopeList)
            {
                foreach (KeyValuePair<string, IScopeAttributeFeature> ScopeNameItem in source)
                    if (Item.FullScope.ContainsKey(ScopeNameItem.Key))
                        if (!conflictList.Contains(ScopeNameItem.Key))
                            conflictList.Add(ScopeNameItem.Key);

                RecursiveCheck(source, Item.InnerScopes, conflictList);
            }
        }

        /// <summary>
        /// Gets the scope embedding the provided one.
        /// </summary>
        /// <param name="source">The scope.</param>
        public static IList<IScopeHolder> EmbeddingScope(ISource source)
        {
            ISource ChildSource = source;
            ISource ParentSource = source.ParentSource;
            IList<IScopeHolder> Result = null;

            do
            {
                bool IsHandled = false;

                switch (ParentSource)
                {
                    case IScope AsScope:
                        Result = AsScope.InnerScopes;
                        IsHandled = true;
                        break;

                    case IContinuation AsContinuation:
                        Result = AsContinuation.InnerScopes;
                        IsHandled = true;
                        break;

                    case IConditional AsConditional:
                        Result = AsConditional.InnerScopes;
                        IsHandled = true;
                        break;

                    case IAttachment AsAttachment:
                        Result = AsAttachment.InnerScopes;
                        IsHandled = true;
                        break;

                    case IInstruction AsInstruction:
                        Result = AsInstruction.InnerScopes;
                        IsHandled = true;
                        break;

                    case IEffectiveBody AsEffectiveBody:
                        Result = AsEffectiveBody.InnerScopes;
                        IsHandled = true;
                        break;

                    case ICommandOverload AsCommandOverload:
                        Result = AsCommandOverload.InnerScopes;
                        IsHandled = true;
                        break;

                    case IQueryOverload AsQueryOverload:
                        Result = AsQueryOverload.InnerScopes;
                        IsHandled = true;
                        break;

                    case IPropertyFeature AsPropertyFeature:
                        if (AsPropertyFeature.GetterBody.IsAssigned && AsPropertyFeature.GetterBody.Item == ChildSource)
                        {
                            Result = AsPropertyFeature.InnerGetScopes;
                            IsHandled = true;
                        }
                        else if (AsPropertyFeature.SetterBody.IsAssigned && AsPropertyFeature.SetterBody.Item == ChildSource)
                        {
                            Result = AsPropertyFeature.InnerSetScopes;
                            IsHandled = true;
                        }
                        break;

                    case IIndexerFeature AsIndexerFeature:
                        if (AsIndexerFeature.GetterBody.IsAssigned && AsIndexerFeature.GetterBody.Item == ChildSource)
                        {
                            Result = AsIndexerFeature.InnerGetScopes;
                            IsHandled = true;
                        }
                        else if (AsIndexerFeature.SetterBody.IsAssigned && AsIndexerFeature.SetterBody.Item == ChildSource)
                        {
                            Result = AsIndexerFeature.InnerSetScopes;
                            IsHandled = true;
                        }
                        break;

                    default:
                        ParentSource = ParentSource.ParentSource;
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }
            while (ParentSource != null && Result == null);

            return Result;
        }
    }
}
