﻿namespace EaslyCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using BaseNodeHelper;
    using CompilerNode;
    using Easly;
    using FormattedNumber;
    using PolySerializer;

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// True to verify the program after compilation.
        /// </summary>
        bool ActivateVerification { get; set; }

        /// <summary>
        /// The last compiled file name.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// The source code, merged with languages classes and compiled.
        /// </summary>
        IRoot LoadedRoot { get; }

        /// <summary>
        /// Errors in last compilation.
        /// </summary>
        IErrorList ErrorList { get; }

        /// <summary>
        /// Number of retries by the inference engine (debug only).
        /// </summary>
        int InferenceRetries { get; }

        /// <summary>
        /// Compiles the file. The file must contain a serialized Easly Root object.
        /// </summary>
        /// <param name="fileName">The file to compile.</param>
        void Compile(string fileName);

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="root">The source code to compile.</param>
        void Compile(BaseNode.IRoot root);

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="stream">The source code to compile.</param>
        void Compile(Stream stream);
    }

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public class Compiler : ICompiler
    {
        #region Properties
        /// <summary>
        /// True to verify the program after compilation.
        /// </summary>
        public bool ActivateVerification { get; set; }

        /// <summary>
        /// The last compiled file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The last compiled source code.
        /// </summary>
        public BaseNode.IRoot Root { get; private set; }

        /// <summary>
        /// The source code, merged with languages classes and compiled.
        /// </summary>
        public IRoot LoadedRoot { get; private set; }

        /// <summary>
        /// Errors in last compilation.
        /// </summary>
        public IErrorList ErrorList { get; } = new ErrorList();

        /// <summary>
        /// Number of retries by the inference engine (debug only).
        /// </summary>
        public int InferenceRetries { get; set; } = 0;
        #endregion

        #region Client Interface
        /// <summary>
        /// Compiles the file. The file must contain a serialized Easly Root object.
        /// </summary>
        /// <param name="fileName">The file to compile.</param>
        public virtual void Compile(string fileName)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ErrorList.ClearErrors();

            if (File.Exists(FileName))
            {
                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        Compile(fs);
                    }
                }
                catch (Exception e)
                {
                    ErrorList.AddError(new ErrorInputFileInvalid(e));
                }
            }
            else
                ErrorList.AddError(new ErrorInputFileNotFound(FileName));
        }

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="root">The source code to compile.</param>
        public virtual void Compile(BaseNode.IRoot root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            ErrorList.ClearErrors();

            using (MemoryStream ms = new MemoryStream())
            {
                ISerializer s = new Serializer();
                s.Serialize(ms, root);

                ms.Seek(0, SeekOrigin.Begin);
                Compile(ms);
            }
        }

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="stream">The source code to compile.</param>
        public virtual void Compile(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            ErrorList.ClearErrors();

            try
            {
                ISerializer Serializer = CreateCompilerSerializer();
                LoadedRoot = (IRoot)Serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                ErrorList.AddError(new ErrorInputFileInvalid(e));
                return;
            }

            try
            {
                CompileRoot(LoadedRoot);
            }
            catch (Exception e)
            {
                ErrorList.AddError(new ErrorInternal(e));
            }
        }
        #endregion

        #region Implementation
        /// <summary></summary>
        protected virtual ISerializer CreateCompilerSerializer()
        {
            // Convert namespace, assembly and version to use the compiler classes.
            ISerializer Serializer = new Serializer();
            Dictionary<NamespaceDescriptor, NamespaceDescriptor> NamespaceOverrideTable = new Dictionary<NamespaceDescriptor, NamespaceDescriptor>();
            NamespaceOverrideTable.Add(new NamespaceDescriptor("BaseNode", "*", "*", "*", "*"), NamespaceDescriptor.DescriptorFromType(typeof(IRoot)));
            Serializer.NamespaceOverrideTable = NamespaceOverrideTable;
            Serializer.OverrideGenericArguments = false;

            return Serializer;
        }

        /// <summary></summary>
        protected virtual void CompileRoot(IRoot root)
        {
            Debug.Assert(root != null);

            IRoot LanguageRoot = LoadLanguageRoot();
            Debug.Assert(LanguageRoot != null);
            Debug.Assert(NodeTreeDiagnostic.IsValid(LanguageRoot));

            MergeLanguageRoot(root, LanguageRoot);

            if (IsRootValid(root))
            {
                ReplacePhase1Macroes(root);

                if (ReplicateAllBlocks(root))
                {
                    ReplacePhase2Macroes(root);
                    InitializeSources(root);

                    if (CheckClassAndLibraryNames(root))
                    {
                        if (ResolveIdentifiers(root) && ResolveTypes(root))
                        {
                            SealScope(root);

                            if (ResolveContract(root) && ResolveBody(root))
                            {
                                CheckNumberType(root);
                            }
                        }
                    }
                }
            }
        }

        /// <summary></summary>
        protected virtual IRoot LoadLanguageRoot()
        {
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
            using (Stream fs = CurrentAssembly.GetManifestResourceStream("EaslyCompiler.Resources.language.easly"))
            {
                ISerializer Serializer = CreateCompilerSerializer();
                return (IRoot)Serializer.Deserialize(fs);
            }
        }

        /// <summary></summary>
        protected virtual void MergeLanguageRoot(IRoot root, IRoot languageRoot)
        {
            Debug.Assert(languageRoot.LibraryBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.ILibrary, BaseNode.Library> BlockLibrary = languageRoot.LibraryBlocks.NodeBlockList[0];

            Debug.Assert(BlockLibrary.NodeList.Count == 1);
            root.LibraryBlocks.NodeBlockList.Add(BlockLibrary);

            BaseNode.ILibrary LanguageLibrary = BlockLibrary.NodeList[0];
            Debug.Assert(LanguageLibrary.ClassIdentifierBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.IIdentifier, BaseNode.Identifier> BlockIdentifier = LanguageLibrary.ClassIdentifierBlocks.NodeBlockList[0];

            Debug.Assert(BlockIdentifier.NodeList.Count == LanguageClasses.NameToGuid.Count);
            Debug.Assert(BlockIdentifier.NodeList.Count == LanguageClasses.GuidToName.Count);
            List<string> IdentifierList = new List<string>();
            foreach (IIdentifier Item in BlockIdentifier.NodeList)
            {
                Debug.Assert(!IdentifierList.Contains(Item.Text));
                IdentifierList.Add(Item.Text);
            }

            Debug.Assert(languageRoot.ClassBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.IClass, BaseNode.Class> BlockClass = languageRoot.ClassBlocks.NodeBlockList[0];

            Debug.Assert(BlockClass.NodeList.Count == LanguageClasses.NameToGuid.Count);
            Debug.Assert(BlockClass.NodeList.Count == LanguageClasses.GuidToName.Count);
            foreach (IClass Item in BlockClass.NodeList)
            {
                Debug.Assert(LanguageClasses.NameToGuid.ContainsKey(Item.EntityName.Text));
                Debug.Assert(LanguageClasses.NameToGuid[Item.EntityName.Text] == Item.ClassGuid);
                Debug.Assert(LanguageClasses.GuidToName.ContainsKey(Item.ClassGuid));
                Debug.Assert(LanguageClasses.GuidToName[Item.ClassGuid] == Item.EntityName.Text);
                Debug.Assert(IdentifierList.Contains(Item.EntityName.Text));
            }
            root.ClassBlocks.NodeBlockList.Add(BlockClass);

            Debug.Assert(languageRoot.Replicates.Count == 0);

            // Remove hard-coded classes.
            List<BaseNode.IIdentifier> IdentifierCopyList = new List<BaseNode.IIdentifier>(BlockIdentifier.NodeList);
            foreach (IIdentifier Item in IdentifierCopyList)
                if (Item.Text == "Any" || Item.Text == "Any Reference" || Item.Text == "Any Value")
                    BlockIdentifier.NodeList.Remove(Item);

            List<BaseNode.IClass> ClassCopyList = new List<BaseNode.IClass>(BlockClass.NodeList);
            foreach (IClass Item in ClassCopyList)
                if (Item.EntityName.Text == "Any" || Item.EntityName.Text == "Any Reference" || Item.EntityName.Text == "Any Value")
                    BlockClass.NodeList.Remove(Item);
        }

        /// <summary></summary>
        protected virtual bool IsRootValid(IRoot root)
        {
            bool Success = true;

            if (!NodeTreeDiagnostic.IsValid(root, assertValid: false))
            {
                ErrorList.AddError(new ErrorInputRootInvalid());
                Success = false;
            }

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }
        #endregion

        #region Preprocessor, phase 1
        /// <summary></summary>
        protected virtual void ReplacePhase1Macroes(IRoot root)
        {
            Debug.Assert(ErrorList.IsEmpty);

            GenerateCompilationDateTime();
            GenerateCompilationUID();
            GenerateCompilerVersion();
            GenerateConformanceToStandard();
            GenerateDebugging();

            bool Success = NodeTreeWalk<ReplacePhase1MacroContext>.Walk(root, new WalkCallbacks<ReplacePhase1MacroContext>() { HandlerNode = ReplacePhase1Macro, IsRecursive = true }, new ReplacePhase1MacroContext());
            Debug.Assert(Success);
        }

        private IInitializedObjectExpression CompilationDateTime;
        private IUnaryOperatorExpression CompilationUID;
        private IManifestStringExpression CompilerVersion;
        private IKeywordExpression ConformanceToStandard;
        private IKeywordExpression Debugging;

        /// <summary></summary>
        protected virtual void GenerateCompilationDateTime()
        {
            DateTime Now = DateTime.UtcNow;
            string IsoString = Now.ToString("u");
            CompilationDateTime = InitializedStringExpression(LanguageClasses.DateAndTime.Name, nameof(DateAndTime.ToUtcDateAndTime), IsoString);
        }

        /// <summary></summary>
        protected virtual void GenerateCompilationUID()
        {
            string NewGuidDigits = Guid.NewGuid().ToString("N") + IntegerBase.Hexadecimal.Suffix;

            BaseNode.IIdentifier Operator = NodeHelper.CreateSimpleIdentifier("To UUID");
            BaseNode.IManifestNumberExpression NumberExpression = NodeHelper.CreateSimpleManifestNumberExpression(NewGuidDigits);
            BaseNode.IUnaryOperatorExpression Expression = NodeHelper.CreateUnaryOperatorExpression(Operator, NumberExpression);
            CompilationUID = ToCompilerNode<BaseNode.IUnaryOperatorExpression, IUnaryOperatorExpression>(Expression);
        }

        /// <summary></summary>
        protected virtual void GenerateCompilerVersion()
        {
            CompilerVersion = new ManifestStringExpression("Easly 1");
        }

        /// <summary></summary>
        protected virtual void GenerateConformanceToStandard()
        {
            BaseNode.IKeywordExpression Expression = NodeHelper.CreateKeywordExpression(BaseNode.Keyword.True);
            ConformanceToStandard = ToCompilerNode<BaseNode.IKeywordExpression, IKeywordExpression>(Expression);
        }

        /// <summary></summary>
        protected virtual void GenerateDebugging()
        {
            BaseNode.IKeywordExpression Expression = NodeHelper.CreateKeywordExpression(BaseNode.Keyword.False);
            Debugging = ToCompilerNode<BaseNode.IKeywordExpression, IKeywordExpression>(Expression);
        }

        /// <summary></summary>
        protected virtual IInitializedObjectExpression InitializedStringExpression(string className, string identifierName, string initialValue)
        {
            BaseNode.IManifestStringExpression ManifestValue = NodeHelper.CreateManifestStringExpression(initialValue);
            return InitializedExpression(className, identifierName, ManifestValue);
        }

        /// <summary></summary>
        protected virtual IInitializedObjectExpression InitializedExpression(string className, string identifierName, BaseNode.IExpression manifestValue)
        {
            BaseNode.IIdentifier ClassIdentifier = NodeHelper.CreateSimpleIdentifier(className);
            BaseNode.IIdentifier FirstIdentifier = NodeHelper.CreateSimpleIdentifier(identifierName);

            BaseNode.IAssignmentArgument FirstArgument = NodeHelper.CreateAssignmentArgument(new List<BaseNode.IIdentifier>() { FirstIdentifier }, manifestValue);
            BaseNode.IInitializedObjectExpression Expression = NodeHelper.CreateInitializedObjectExpression(ClassIdentifier, new List<BaseNode.IAssignmentArgument>() { FirstArgument });

            IInitializedObjectExpression Result = ToCompilerNode<BaseNode.IInitializedObjectExpression, IInitializedObjectExpression>(Expression);
            return Result;
        }

        /// <summary></summary>
        /// <typeparam name="TBase">BaseNode type.</typeparam>
        /// <typeparam name="TCompiler">Compiler type.</typeparam>
        /// <param name="node">The BaseNode object to convert.</param>
        protected virtual TCompiler ToCompilerNode<TBase, TCompiler>(TBase node)
            where TBase : BaseNode.INode
            where TCompiler : class, INode
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ISerializer s = new Serializer();
                s.Serialize(ms, node);

                ms.Seek(0, SeekOrigin.Begin);

                s = CreateCompilerSerializer();
                return s.Deserialize(ms) as TCompiler;
            }
        }

        /// <summary></summary>
        public bool ReplacePhase1Macro(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<ReplacePhase1MacroContext> callbacks, ReplacePhase1MacroContext context)
        {
            bool Result = true;

            if (node is IClass AsClass)
                context.CurrentClass = AsClass;
            else if (node is ILibrary || node is IGlobalReplicate)
                context.CurrentClass = null;
            else if (node is IPreprocessorExpression AsPreprocessorExpression)
            {
                bool IsHandled = false;

                switch (AsPreprocessorExpression.Value)
                {
                    case BaseNode.PreprocessorMacro.DateAndTime:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilationDateTime);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.CompilationDiscreteIdentifier:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilationUID);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.CompilerVersion:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilerVersion);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.ConformanceToStandard:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ConformanceToStandard);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.DiscreteClassIdentifier:
                        Debug.Assert(context.CurrentClass != null);

                        string GlassGuidDigits = context.CurrentClass.ClassGuid.ToString("N") + IntegerBase.Hexadecimal.Suffix;
                        BaseNode.IIdentifier Operator = NodeHelper.CreateSimpleIdentifier("To UUID");
                        BaseNode.IManifestNumberExpression NumberExpression = NodeHelper.CreateSimpleManifestNumberExpression(GlassGuidDigits);
                        BaseNode.IUnaryOperatorExpression Expression = NodeHelper.CreateUnaryOperatorExpression(Operator, NumberExpression);
                        IExpression ReplacementNode = ToCompilerNode<BaseNode.IUnaryOperatorExpression, IUnaryOperatorExpression>(Expression);

                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.Debugging:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, Debugging);
                        IsHandled = true;
                        break;

                    // Processed in phase 2.
                    case BaseNode.PreprocessorMacro.ClassPath:
                    case BaseNode.PreprocessorMacro.Counter:
                    case BaseNode.PreprocessorMacro.RandomInteger:
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return Result;
        }
        #endregion

        #region Block Replication
        private bool ReplicateAllBlocks(IRoot root)
        {
            Debug.Assert(ErrorList.IsEmpty);

            foreach (IBlock<BaseNode.IClass, BaseNode.Class> Block in root.ClassBlocks.NodeBlockList)
                foreach (IClass Item in Block.NodeList)
                    if (string.IsNullOrEmpty(Item.ClassPath))
                        ErrorList.AddError(new ErrorEmptyClassPath(Item));

            ReplicationContext Replication = new ReplicationContext();
            IWalkCallbacks<ReplicationContext> Callbacks = new WalkCallbacks<ReplicationContext>() { HandlerNode = OnNodeIgnoreReplicates, HandlerBlockList = OnBlockListReplicate, HandlerString = OnStringReplicateText };
            NodeTreeWalk<ReplicationContext>.Walk(root, Callbacks, Replication);

            return ErrorList.IsEmpty;
        }

        private bool OnNodeIgnoreReplicates(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            if (node is IGlobalReplicate)
                return true;
            else if (node is IRoot AsRoot)
            {
                ProcessGlobalReplicates(AsRoot, context);
                return ErrorList.IsEmpty;
            }
            else if (node is IClass AsClass)
            {
                ProcessClassReplicates(AsClass, callbacks, context);
                return ErrorList.IsEmpty;
            }
            else
                return parentNode == null || NodeTreeWalk<ReplicationContext>.Walk(node, callbacks, context);
        }

        private void ProcessGlobalReplicates(IRoot root, ReplicationContext context)
        {
            List<ICompiledReplicate> ReplicateList = new List<ICompiledReplicate>();

            foreach (IGlobalReplicate ReplicateItem in root.Replicates)
            {
                Debug.Assert(ReplicateItem.PatternList.Count == 0);

                foreach (IPattern PatternItem in ReplicateItem.Patterns)
                    ReplicateItem.PatternList.Add(PatternItem);

                ReplicateList.Add(ReplicateItem);
            }

            CheckReplicates(ReplicateList, context);

            Debug.Assert(context.GlobalReplicateTable.Count == 0);
            foreach (KeyValuePair<string, List<string>> Entry in context.ReplicateTable)
                context.GlobalReplicateTable.Add(Entry.Key, Entry.Value);
        }

        private void ProcessClassReplicates(IClass parentClass, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            BaseNode.IBlockList ClassReplicateBlocks = (BaseNode.IBlockList)parentClass.ClassReplicateBlocks;

            List<BaseNode.INode> ReplicatedNodeList = new List<BaseNode.INode>();
            if (ReplicateBlockList(ClassReplicateBlocks, ReplicatedNodeList, callbacks, context))
            {
                parentClass.FillReplicatedList(nameof(IClass.ClassReplicateBlocks), ReplicatedNodeList);

                List<ICompiledReplicate> ReplicateList = new List<ICompiledReplicate>();
                foreach (BaseNode.INode Node in ReplicatedNodeList)
                {
                    IClassReplicate ReplicateItem = Node as IClassReplicate;
                    Debug.Assert(ReplicateItem != null);

                    ReplicateList.Add(ReplicateItem);
                }

                context.ReplicateTable.Clear();
                foreach (KeyValuePair<string, List<string>> Entry in context.GlobalReplicateTable)
                    context.ReplicateTable.Add(Entry.Key, Entry.Value);

                CheckReplicates(ReplicateList, context);
            }
        }

        private void CheckReplicates(IList<ICompiledReplicate> replicateList, ReplicationContext context)
        {
            foreach (ICompiledReplicate Replicate in replicateList)
            {
                IName ReplicateName = (IName)Replicate.ReplicateName;
                string ReplicateNameText = ReplicateName.Text;

                if (!StringValidation.IsValidIdentifier(ReplicateName, ReplicateNameText, out string ValidReplicateName, out IErrorStringValidity StringError))
                    ErrorList.AddError(StringError);
                else
                {
                    if (context.ReplicateTable.ContainsKey(ValidReplicateName))
                        ErrorList.AddError(new ErrorDuplicateName(ReplicateName, ValidReplicateName));
                    else
                    {
                        // If 0, the whole root would not have passed validity check.
                        Debug.Assert(Replicate.PatternList.Count > 0);

                        List<string> ValidPatternList = new List<string>();

                        foreach (IPattern Pattern in Replicate.PatternList)
                        {
                            string PatternText = Pattern.Text;

                            if (!StringValidation.IsValidIdentifier(Pattern, PatternText, out string ValidPatternText, out StringError))
                                ErrorList.AddError(StringError);
                            else
                                ValidPatternList.Add(ValidPatternText);
                        }

                        context.ReplicateTable.Add(ValidReplicateName, ValidPatternList);
                    }
                }
            }
        }

        private bool ReplicateBlockList(BaseNode.IBlockList blockList, List<BaseNode.INode> replicatedNodeList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            bool Result = true;

            foreach (BaseNode.IBlock BlockObject in blockList.NodeBlockList)
                Result &= ReplicateBlock(BlockObject, replicatedNodeList, callbacks, context);

            return Result;
        }

        private bool ReplicateBlock(BaseNode.IBlock block, List<BaseNode.INode> replicatedNodeList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            BaseNode.ReplicationStatus Status = block.Replication;

            if (Status == BaseNode.ReplicationStatus.Replicated)
                return ProcessReplicatedBlock(block, replicatedNodeList, callbacks, context);
            else
                return ProcessNormalBlock(block, replicatedNodeList, callbacks, context);
        }

        private bool ProcessReplicatedBlock(BaseNode.IBlock block, List<BaseNode.INode> replicatedNodeList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            IErrorStringValidity StringError;
            IPattern ReplicationPattern = (IPattern)block.ReplicationPattern;
            IIdentifier SourceIdentifier = (IIdentifier)block.SourceIdentifier;
            string ReplicationPatternText = ReplicationPattern.Text;

            if (!StringValidation.IsValidIdentifier(ReplicationPattern, ReplicationPatternText, out string ValidReplicationPattern, out StringError))
            {
                ErrorList.AddError(StringError);
                return false;
            }
            else
            {
                string SourceIdentifierText = SourceIdentifier.Text;

                if (!StringValidation.IsValidIdentifier(SourceIdentifier, SourceIdentifierText, out string ValidSourceIdentifier, out StringError))
                {
                    ErrorList.AddError(StringError);
                    return false;
                }
                else if (!context.ReplicateTable.ContainsKey(ValidSourceIdentifier))
                {
                    ErrorList.AddError(new ErrorUnknownIdentifier(SourceIdentifier, ValidSourceIdentifier));
                    return false;
                }
                else if (context.PatternTable.ContainsKey(ValidReplicationPattern))
                {
                    ErrorList.AddError(new ErrorPatternAlreadyUsed(ReplicationPattern, ValidReplicationPattern));
                    return false;
                }
                else
                    return ProcessReplicationPatterns(block, replicatedNodeList, ValidReplicationPattern, ValidSourceIdentifier, callbacks, context);
            }
        }

        private bool ProcessReplicationPatterns(BaseNode.IBlock block, List<BaseNode.INode> replicatedNodeList, string replicationPattern, string sourceIdentifier, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            IList NodeList = block.NodeList as IList;
            List<string> PatternList = context.ReplicateTable[sourceIdentifier];

            bool Continue = true;
            for (int i = 0; i < PatternList.Count && Continue; i++)
            {
                string PatternText = PatternList[i];

                context.PatternTable.Add(replicationPattern, PatternText);
                foreach (BaseNode.INode Node in NodeList)
                {
                    BaseNode.INode ClonedNode = NodeHelper.DeepCloneNode(Node, cloneCommentGuid: true);

                    Continue &= NodeTreeWalk<ReplicationContext>.Walk(ClonedNode, callbacks, context);
                    if (Continue)
                    {
                        if (ClonedNode is IClass AsClass)
                            AsClass.SetFullClassPath(replicationPattern, PatternText);

                        replicatedNodeList.Add(ClonedNode);
                    }
                }

                context.PatternTable.Remove(replicationPattern);
            }

            return Continue;
        }

        private bool ProcessNormalBlock(BaseNode.IBlock block, List<BaseNode.INode> replicatedNodeList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            IList NodeList = block.NodeList as IList;

            bool Continue = true;
            for (int i = 0; i < NodeList.Count && Continue; i++)
            {
                BaseNode.INode Node = NodeList[i] as BaseNode.INode;
                Debug.Assert(Node != null);

                Continue &= NodeTreeWalk<ReplicationContext>.Walk(Node, callbacks, context);
                if (Continue)
                {
                    if (Node is IClass AsClass)
                        AsClass.SetFullClassPath();

                    replicatedNodeList.Add(Node);
                }
            }

            return Continue;
        }

        private bool OnStringReplicateText(BaseNode.INode node, string propertyName, ReplicationContext context)
        {
            string Text = NodeTreeHelper.GetString(node, propertyName);

            foreach (KeyValuePair<string, string> Entry in context.PatternTable)
            {
                string ReplicationPattern = Entry.Key;
                string ReplacementPattern = Entry.Value;

                Text = Text.Replace(ReplicationPattern, ReplacementPattern);
            }

            NodeTreeHelper.SetString(node, propertyName, Text);

            return true;
        }

        private bool OnBlockListReplicate(BaseNode.INode node, string propertyName, BaseNode.IBlockList blockList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            if (blockList is IBlockList<BaseNode.IClassReplicate, BaseNode.ClassReplicate>)
                return true;
            else
            {
                List<BaseNode.INode> ReplicatedNodeList = new List<BaseNode.INode>();
                bool Continue = ReplicateBlockList(blockList, ReplicatedNodeList, callbacks, context);
                if (Continue)
                {
                    INodeWithReplicatedBlocks NodeWithReplicatedBlocks = node as INodeWithReplicatedBlocks;
                    Debug.Assert(NodeWithReplicatedBlocks != null);

                    NodeWithReplicatedBlocks.FillReplicatedList(propertyName, ReplicatedNodeList);
                }

                return Continue;
            }
        }

        private KeyValuePair<string, string> CreateBlockSubstitution()
        {
            return new KeyValuePair<string, string>("Blocks", "List");
        }
        #endregion

        #region Preprocessor, phase 2
        /// <summary></summary>
        protected virtual void ReplacePhase2Macroes(IRoot root)
        {
            Debug.Assert(ErrorList.IsEmpty);

            bool Success = NodeTreeWalk<ReplacePhase2MacroContext>.Walk(root, new WalkCallbacks<ReplacePhase2MacroContext>() { HandlerNode = ReplacePhase2Macro, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() }, new ReplacePhase2MacroContext());
            Debug.Assert(Success);
        }

        /// <summary></summary>
        public bool ReplacePhase2Macro(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<ReplacePhase2MacroContext> callbacks, ReplacePhase2MacroContext context)
        {
            bool Result = true;

            if (node is IClass AsClass)
                context.CurrentClass = AsClass;
            else if (node is ILibrary || node is IGlobalReplicate)
                context.CurrentClass = null;
            else if (node is IPreprocessorExpression AsPreprocessorExpression)
            {
                bool IsHandled = false;
                IExpression ReplacementNode;

#if COVERAGE
                Debug.Assert(AsPreprocessorExpression.ToString().Length > 0);
#endif

                switch (AsPreprocessorExpression.Value)
                {
                    case BaseNode.PreprocessorMacro.ClassPath:
                        Debug.Assert(context.CurrentClass != null);
                        ReplacementNode = new ManifestStringExpression(context.CurrentClass.FullClassPath);
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.Counter:
                        Debug.Assert(context.CurrentClass != null);
                        ReplacementNode = new ManifestNumberExpression(context.CurrentClass.ClassCounter);
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);

                        context.CurrentClass.IncrementClassCounter();
                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.RandomInteger:
                        ReplacementNode = CreateRandomInteger();
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return Result;
        }

        /// <summary></summary>
        protected virtual IExpression CreateRandomInteger()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] Data = new byte[8];
            rng.GetBytes(Data);

            string Value = string.Empty;
            foreach (byte b in Data)
                Value += b.ToString("X2");

            return new ManifestNumberExpression(Value + IntegerBase.Hexadecimal.Suffix);
        }
        #endregion

        #region Source Initialization
        /// <summary></summary>
        protected virtual void InitializeSources(IRoot root)
        {
            Debug.Assert(ErrorList.IsEmpty);

            bool Success = NodeTreeWalk<object>.Walk(root, new WalkCallbacks<object>() { HandlerNode = InitializeSource, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() }, null);
            Debug.Assert(Success);
        }

        /// <summary></summary>
        public bool InitializeSource(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<object> callbacks, object context)
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
            IWalkCallbacks<BuildInferenceSourceList> Callbacks = new WalkCallbacks<BuildInferenceSourceList>() { HandlerNode = ListAllSources, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() };
            IList<IClass> ClassList = root.ClassList;

            foreach (IClass Class in ClassList)
                NodeTreeWalk<BuildInferenceSourceList>.Walk(Class, Callbacks, Context);

            IList<ISource> SourceList = Context.SourceList;
            InferenceEngine Engine = new InferenceEngine(ruleTemplateList, SourceList, ClassList, true, InferenceRetries);

            bool Success = Engine.Solve(ErrorList, passName);

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }

        /// <summary></summary>
        protected virtual bool ListAllSources(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<BuildInferenceSourceList> callbacks, BuildInferenceSourceList context)
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
            IWalkCallbacks<object> Callbacks = new WalkCallbacks<object>() { HandlerNode = SealAllScopes, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() };
            IList<IClass> ClassList = root.ClassList;

            foreach (IClass Class in ClassList)
                NodeTreeWalk<object>.Walk(Class, Callbacks, Context);
        }

        /// <summary></summary>
        protected virtual bool SealAllScopes(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<object> callbacks, object context)
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
            for (;;)
            {
                bool IsChanged = false;

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
