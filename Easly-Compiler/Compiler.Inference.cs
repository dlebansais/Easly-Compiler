namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using BaseNodeHelper;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public partial class Compiler : ICompiler
    {
        #region Source Initialization
        /// <summary></summary>
        protected virtual void InitializeSources(IRoot root)
        {
            Debug.Assert(ErrorList.IsEmpty);

            bool Success = NodeTreeWalk.Walk<object>((BaseNode.Root)root, new WalkCallbacks<object>() { HandlerNode = InitializeSource, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() }, null);
            Debug.Assert(Success);
        }

        /// <summary></summary>
        public static bool InitializeSource(BaseNode.Node node, BaseNode.Node parentNode, string propertyName, WalkCallbacks<object> callbacks, object context)
        {
            bool Result = true;

            if (node is ISource AsSource)
            {
                ISource ParentSource = parentNode as ISource;
                AsSource.InitializeSource(ParentSource);
            }

#if DEBUG
            string DebugString = node.ToString();

            if (node is ICompiledFeature AsFeature)
            {
                bool IsDeferredFeature = AsFeature.IsDeferredFeature;
                bool HasExternBody = AsFeature.HasExternBody;
                bool HasPrecursorBody = AsFeature.HasPrecursorBody;
            }
#endif
            return Result;
        }
        #endregion

        #region Classes and Libraries name collision check
        /// <summary></summary>
        protected virtual bool CheckClassAndLibraryNames(IRoot root)
        {
            ClassTable.Clear();
            LibraryTable.Clear();

            bool IsClassNamesValid = CheckClassNames(root);
            bool IsLibraryNamesValid = CheckLibraryNames(root);
            if (!IsClassNamesValid || !IsLibraryNamesValid)
                return false;

            if (!InitializeLibraries(root))
                return false;

            if (!CheckLibrariesConsistency(root))
                return false;

            if (!CheckClassesConsistency(root))
                return false;

            Debug.Assert(ErrorList.IsEmpty);
            return true;
        }

        /// <summary></summary>
        protected virtual bool CheckClassNames(IRoot root)
        {
            IList<IClass> ValidatedClassList = new List<IClass>();
            bool IsClassNamesValid = true;

            // Basic name checks.
            foreach (IClass Class in root.ClassList)
                IsClassNamesValid &= Class.CheckClassNames(ClassTable, ValidatedClassList, ErrorList);

            foreach (KeyValuePair<string, ISealableDictionary<string, IClass>> Entry in ClassTable)
            {
                string ValidClassName = Entry.Key;

                // List all classes with the same name and no source.
                List<IClass> DuplicateClassList = new List<IClass>();
                foreach (IClass Item in ValidatedClassList)
                    if (Item.ValidClassName == ValidClassName && !Item.FromIdentifier.IsAssigned)
                        DuplicateClassList.Add(Item);

                // If more than one, report an error for each of them.
                if (DuplicateClassList.Count > 1)
                {
                    IsClassNamesValid = false;

                    foreach (IClass Item in DuplicateClassList)
                        ErrorList.AddError(new ErrorSourceRequired((IName)Item.EntityName));
                }
            }

            Debug.Assert(IsClassNamesValid || !ErrorList.IsEmpty);
            return IsClassNamesValid;
        }

        /// <summary></summary>
        protected virtual bool CheckLibraryNames(IRoot root)
        {
            IList<ILibrary> ValidatedLibraryList = new List<ILibrary>();
            bool IsLibraryNamesValid = true;

            // Basic name checks.
            foreach (ILibrary Library in root.LibraryList)
                IsLibraryNamesValid &= Library.CheckLibraryNames(LibraryTable, ValidatedLibraryList, ErrorList);

            foreach (KeyValuePair<string, ISealableDictionary<string, ILibrary>> Entry in LibraryTable)
            {
                string ValidLibraryName = Entry.Key;

                // List all libraries with the same name and no source.
                List<ILibrary> DuplicateLibraryList = new List<ILibrary>();
                foreach (ILibrary Item in ValidatedLibraryList)
                    if (Item.ValidLibraryName == ValidLibraryName && !Item.FromIdentifier.IsAssigned)
                        DuplicateLibraryList.Add(Item);

                // If more than one, report an error for each of them.
                if (DuplicateLibraryList.Count > 1)
                {
                    IsLibraryNamesValid = false;

                    foreach (ILibrary Item in DuplicateLibraryList)
                        ErrorList.AddError(new ErrorSourceRequired((IName)Item.EntityName));
                }
            }

            Debug.Assert(IsLibraryNamesValid || !ErrorList.IsEmpty);
            return IsLibraryNamesValid;
        }

        /// <summary></summary>
        protected virtual bool InitializeLibraries(IRoot root)
        {
            bool Success = true;

            foreach (ILibrary Library in root.LibraryList)
                Success &= Library.InitLibraryTables(ClassTable, ErrorList);

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }

        /// <summary></summary>
        protected virtual bool CheckLibrariesConsistency(IRoot root)
        {
            List<ILibrary> ResolvedLibraryList = new List<ILibrary>();
            List<ILibrary> UnresolvedLibraryList = new List<ILibrary>(root.LibraryList);

            bool Success = true;
            bool Continue = true;
            while (UnresolvedLibraryList.Count > 0 && Success && Continue)
            {
                // Continue while there is a library to process.
                Continue = false;

                foreach (ILibrary Library in UnresolvedLibraryList)
                    Success &= Library.Resolve(LibraryTable, ResolvedLibraryList, ref Continue, ErrorList);

                MoveResolvedLibraries(UnresolvedLibraryList, ResolvedLibraryList, ref Continue);
            }

            // If we're stuck at processing remaining libraries, it's because they are referencing each other in a cycle.
            if (UnresolvedLibraryList.Count > 0 && Success)
            {
                Success = false;

                IList<string> NameList = new List<string>();
                foreach (ILibrary Library in UnresolvedLibraryList)
                    NameList.Add(Library.ValidLibraryName);

                ErrorList.AddError(new ErrorCyclicDependency(NameList, "Library"));
            }

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }

        /// <summary></summary>
        protected virtual void MoveResolvedLibraries(List<ILibrary> unresolvedLibraryList, List<ILibrary> resolvedLibraryList, ref bool continueMove)
        {
            List<ILibrary> ToMove = new List<ILibrary>();

            foreach (ILibrary Library in unresolvedLibraryList)
                if (Library.IsResolved)
                    ToMove.Add(Library);

            if (ToMove.Count > 0)
            {
                foreach (ILibrary LibraryItem in ToMove)
                {
                    unresolvedLibraryList.Remove(LibraryItem);
                    resolvedLibraryList.Add(LibraryItem);
                }

                continueMove = true;
            }
        }

        /// <summary></summary>
        protected virtual bool CheckClassesConsistency(IRoot root)
        {
            bool Success = true;

            foreach (IClass Class in root.ClassList)
                Success &= Class.CheckClassConsistency(LibraryTable, ClassTable, ErrorList);

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }

        private ISealableDictionary<string, ISealableDictionary<string, IClass>> ClassTable = new SealableDictionary<string, ISealableDictionary<string, IClass>>();
        private ISealableDictionary<string, ISealableDictionary<string, ILibrary>> LibraryTable = new SealableDictionary<string, ISealableDictionary<string, ILibrary>>();
        #endregion

        #region Inference
        /// <summary></summary>
        protected virtual bool ResolveIdentifiers(IRoot root)
        {
            return Resolve(root, RuleTemplateSet.Identifiers, "Identifiers");
        }

        /// <summary></summary>
        protected virtual bool ResolveTypes(IRoot root)
        {
            return Resolve(root, RuleTemplateSet.Types, "Types");
        }

        /// <summary></summary>
        protected virtual bool ResolveContract(IRoot root)
        {
            return Resolve(root, RuleTemplateSet.Contract, "Contract");
        }

        /// <summary></summary>
        protected virtual bool ResolveBody(IRoot root)
        {
            return Resolve(root, RuleTemplateSet.Body, "Body");
        }

        /// <summary></summary>
        protected virtual bool Resolve(IRoot root, IRuleTemplateList ruleTemplateList, string passName)
        {
            BuildInferenceSourceList Context = new BuildInferenceSourceList(ruleTemplateList);
            WalkCallbacks<BuildInferenceSourceList> Callbacks = new WalkCallbacks<BuildInferenceSourceList>() { HandlerNode = ListAllSources, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() };
            IList<IClass> ClassList = root.ClassList;

            foreach (IClass Class in ClassList)
                NodeTreeWalk.Walk<BuildInferenceSourceList>((BaseNode.Class)Class, Callbacks, Context);

            IList<ISource> SourceList = Context.SourceList;
            InferenceEngine Engine = new InferenceEngine(ruleTemplateList, SourceList, ClassList, true, InferenceRetries);

            bool Success = Engine.Solve(ErrorList, passName);

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }

        /// <summary></summary>
        protected virtual bool ListAllSources(BaseNode.Node node, BaseNode.Node parentNode, string propertyName, WalkCallbacks<BuildInferenceSourceList> callbacks, BuildInferenceSourceList context)
        {
            ISource Source = node as ISource;
            Debug.Assert(Source != null);

            Source.Reset(context.RuleTemplateList);

#if COVERAGE
            Debug.Assert(!Source.IsResolved(context.RuleTemplateList));
            Debug.Assert(Source.ToString().Length > 0);
#endif

            foreach (IRuleTemplate RuleTemplate in context.RuleTemplateList)
                if (RuleTemplate.NodeType.IsAssignableFrom(Source.GetType()))
                {
                    context.SourceList.Add(Source);
                    break;
                }

            return true;
        }

        /// <summary></summary>
        protected virtual void SealScope(IRoot root)
        {
            object Context = null;
            WalkCallbacks<object> Callbacks = new WalkCallbacks<object>() { HandlerNode = SealAllScopes, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() };
            IList<IClass> ClassList = root.ClassList;

            foreach (IClass Class in ClassList)
                NodeTreeWalk.Walk<object>((BaseNode.Class)Class, Callbacks, Context);
        }

        /// <summary></summary>
        protected virtual bool SealAllScopes(BaseNode.Node node, BaseNode.Node parentNode, string propertyName, WalkCallbacks<object> callbacks, object context)
        {
            ISource Source = node as ISource;
            Debug.Assert(Source != null);

            if (Scope.IsScopeHolder(Source))
            {
                IScopeHolder ScopeHolder = Source as IScopeHolder;
                Debug.Assert(ScopeHolder != null);

                ScopeHolder.FullScope.Seal();
            }

            return true;
        }
        #endregion

        #region Numbers
        private void CheckNumberType(IRoot root)
        {
            bool IsFirstPass = true;

            while (true)
            {
                bool IsChanged = IsFirstPass;
                IsFirstPass = false;

                foreach (IClass Class in root.ClassList)
                    Class.RestartNumberType(ref IsChanged);

                foreach (IClass Class in root.ClassList)
                    Class.CheckNumberType(ref IsChanged);

                if (!IsChanged)
                    break;
            }

            foreach (IClass Class in root.ClassList)
                Class.ValidateNumberType(ErrorList);
        }
        #endregion
    }
}
