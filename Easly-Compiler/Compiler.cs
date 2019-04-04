namespace EaslyCompiler
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
    using PolySerializer;

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public class Compiler
    {
        #region Properties
        /// <summary>
        /// Folder where to output the result.
        /// </summary>
        public string OutputRootFolder { get; set; }

        /// <summary>
        /// Namespace for the output code.
        /// </summary>
        public string Namespace { get; set; }

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
        /// Errors in last compilation.
        /// </summary>
        public IList<Error> ErrorList { get; } = new List<Error>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Compiles the file. The file must contain a serialized Easly Root object.
        /// </summary>
        /// <param name="fileName">The file to compile.</param>
        public virtual void Compile(string fileName)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ErrorList.Clear();

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
                    ErrorList.Add(new ErrorInputFileInvalid(e));
                }
            }
            else
                ErrorList.Add(new ErrorInputFileNotFound(FileName));
        }

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="root">The source code to compile.</param>
        public virtual void Compile(BaseNode.IRoot root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            ErrorList.Clear();

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
            ErrorList.Clear();

            try
            {
                ISerializer Serializer = CreateCompilerSerializer();
                IRoot LoadedRoot = (IRoot)Serializer.Deserialize(stream);

                CompileRoot(LoadedRoot);
            }
            catch (Exception e)
            {
                ErrorList.Add(new ErrorInputFileInvalid(e));
            }
        }
        #endregion

        #region Implementation
        /// <summary></summary>
        protected virtual ISerializer CreateCompilerSerializer()
        {
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

            if (!NodeTreeDiagnostic.IsValid(root, assertValid: false))
            {
                ErrorList.Add(new ErrorInputRootInvalid(root));
                return;
            }

            if (!ReplacePhase1Macroes(root))
                return;

            if (!ReplicateAllBlocks(root))
                return;

            if (!ReplacePhase2Macroes(root))
                return;
        }

        /// <summary></summary>
        protected virtual IRoot LoadLanguageRoot()
        {
            try
            {
                Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
                object o = CurrentAssembly.GetManifestResourceNames();
                using (Stream fs = CurrentAssembly.GetManifestResourceStream("EaslyCompiler.Resources.language.easly"))
                {
                    ISerializer Serializer = CreateCompilerSerializer();
                    return (IRoot)Serializer.Deserialize(fs);
                }
            }
            catch
            {
                return null;
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
            List<string> IdentifierList = new List<string>();
            foreach (IIdentifier Item in BlockIdentifier.NodeList)
            {
                Debug.Assert(!IdentifierList.Contains(Item.Text));
                IdentifierList.Add(Item.Text);
            }

            Debug.Assert(languageRoot.ClassBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.IClass, BaseNode.Class> BlockClass = languageRoot.ClassBlocks.NodeBlockList[0];

            Debug.Assert(BlockClass.NodeList.Count == LanguageClasses.NameToGuid.Count);
            foreach (IClass Item in BlockClass.NodeList)
            {
                Debug.Assert(LanguageClasses.NameToGuid.ContainsKey(Item.EntityName.Text));
                Debug.Assert(LanguageClasses.NameToGuid[Item.EntityName.Text] == Item.ClassGuid);
                Debug.Assert(IdentifierList.Contains(Item.EntityName.Text));
            }
            root.ClassBlocks.NodeBlockList.Add(BlockClass);

            Debug.Assert(languageRoot.Replicates.Count == 0);
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
        #endregion

        #region Replication, phase 1
        /// <summary></summary>
        protected virtual bool ReplacePhase1Macroes(IRoot root)
        {
            GenerateCompilationDateTime();
            GenerateCompilationUID();
            GenerateCompilerVersion();
            GenerateConformanceToStandard();
            GenerateDebugging();

            return NodeTreeWalk<ReplacePhase1MacroContext>.Walk(root, new WalkCallbacks<ReplacePhase1MacroContext>() { HandlerNode = ReplacePhase1Macro, IsRecursive = true }, new ReplacePhase1MacroContext());
        }

        private IInitializedObjectExpression CompilationDateTime;
        private IInitializedObjectExpression CompilationUID;
        private IInitializedObjectExpression CompilerVersion;
        private IInitializedObjectExpression ConformanceToStandard;
        private IInitializedObjectExpression Debugging;

        /// <summary></summary>
        protected virtual void GenerateCompilationDateTime()
        {
            CompilationDateTime = InitializedExpression(LanguageClasses.DateAndTime.Name, "0");
        }

        /// <summary></summary>
        protected virtual void GenerateCompilationUID()
        {
            string NewGuidDigits = Guid.NewGuid().ToString("N");
            CompilationUID = InitializedExpression(LanguageClasses.UniversallyUniqueIdentifier.Name, "0x" + NewGuidDigits);
        }

        /// <summary></summary>
        protected virtual void GenerateCompilerVersion()
        {
            CompilerVersion = InitializedExpression(LanguageClasses.String.Name, "Easly 1");
        }

        /// <summary></summary>
        protected virtual void GenerateConformanceToStandard()
        {
            ConformanceToStandard = InitializedExpression(LanguageClasses.Boolean.Name, "True");
        }

        /// <summary></summary>
        protected virtual void GenerateDebugging()
        {
            Debugging = InitializedExpression(LanguageClasses.Boolean.Name, "False");
        }

        /// <summary></summary>
        protected virtual IInitializedObjectExpression InitializedExpression(string className, string initialValue)
        {
            BaseNode.IIdentifier ClassIdentifier = NodeHelper.CreateSimpleIdentifier(className);
            BaseNode.IManifestNumberExpression CurrentTime = NodeHelper.CreateSimpleManifestNumberExpression(initialValue);

            BaseNode.IIdentifier FirstIdentifier = NodeHelper.CreateSimpleIdentifier(string.Empty);
            BaseNode.IAssignmentArgument FirstArgument = NodeHelper.CreateAssignmentArgument(new List<BaseNode.IIdentifier>() { FirstIdentifier }, CurrentTime);
            BaseNode.IInitializedObjectExpression Expression = NodeHelper.CreateInitializedObjectExpression(ClassIdentifier, new List<BaseNode.IAssignmentArgument>() { FirstArgument });

            IInitializedObjectExpression Result = ToCompilerNode<BaseNode.IInitializedObjectExpression, IInitializedObjectExpression>(Expression);
            return Result;
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
                        if (context.CurrentClass != null)
                        {
                            IInitializedObjectExpression ReplacementNode = InitializedExpression(LanguageClasses.UniversallyUniqueIdentifier.Name, context.CurrentClass.ClassGuid.ToString("N"));
                            NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        }
                        else
                        {
                            ErrorList.Add(new ErrorMacroOutOfContext(AsPreprocessorExpression));
                            Result = false;
                        }

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
            foreach (IBlock<BaseNode.IClass, BaseNode.Class> Block in root.ClassBlocks.NodeBlockList)
                foreach (IClass Item in Block.NodeList)
                    if (string.IsNullOrEmpty(Item.ClassPath))
                        ErrorList.Add(new ErrorEmptyClassPath(Item));

            ReplicationContext Replication = new ReplicationContext();
            IWalkCallbacks<ReplicationContext> Callbacks = new WalkCallbacks<ReplicationContext>() { HandlerNode = OnNodeIgnoreReplicates, HandlerBlockList = OnBlockListReplicate, HandlerString = OnStringReplicateText };
            NodeTreeWalk<ReplicationContext>.Walk(root, Callbacks, Replication);

            return ErrorList.Count == 0;
        }

        private bool OnNodeIgnoreReplicates(BaseNode.INode node, BaseNode.INode parentNode, string propertyName, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            if (node is IGlobalReplicate)
                return true;
            else if (node is IRoot AsRoot)
            {
                ProcessGlobalReplicates(AsRoot, context);
                return ErrorList.Count == 0;
            }
            else if (node is IClass AsClass)
            {
                ProcessClassReplicates(AsClass, callbacks, context);
                return ErrorList.Count == 0;
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
                foreach (KeyValuePair<string, List<string>> Entry in context.ReplicateTable)
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

                string ValidReplicateName;
                ErrorStringValidity StringError;
                if (!StringValidation.IsValidIdentifier(ReplicateName, ReplicateNameText, out ValidReplicateName, out StringError))
                    ErrorList.Add(StringError);
                else
                {
                    if (context.ReplicateTable.ContainsKey(ValidReplicateName))
                        ErrorList.Add(new ErrorDuplicateName(ReplicateName, ValidReplicateName));
                    else if (Replicate.PatternList.Count == 0)
                        ErrorList.Add(new ErrorEmptyList((ISource)Replicate, ValidReplicateName));
                    else
                    {
                        List<string> ValidPatternList = new List<string>();

                        foreach (IPattern Pattern in Replicate.PatternList)
                        {
                            string PatternText = Pattern.Text;

                            string ValidPatternText;
                            if (!StringValidation.IsValidIdentifier(Pattern, PatternText, out ValidPatternText, out StringError))
                                ErrorList.Add(StringError);
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
            foreach (BaseNode.IBlock BlockObject in blockList.NodeBlockList)
                if (!ReplicateBlock(BlockObject, replicatedNodeList, callbacks, context))
                    return false;

            return true;
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
            ErrorStringValidity StringError;
            IPattern ReplicationPattern = (IPattern)block.ReplicationPattern;
            IIdentifier SourceIdentifier = (IIdentifier)block.SourceIdentifier;
            string ReplicationPatternText = ReplicationPattern.Text;

            string ValidReplicationPattern;
            if (!StringValidation.IsValidIdentifier(ReplicationPattern, ReplicationPatternText, out ValidReplicationPattern, out StringError))
            {
                ErrorList.Add(StringError);
                return false;
            }
            else
            {
                string SourceIdentifierText = SourceIdentifier.Text;

                string ValidSourceIdentifier;
                if (!StringValidation.IsValidIdentifier(SourceIdentifier, SourceIdentifierText, out ValidSourceIdentifier, out StringError))
                {
                    ErrorList.Add(StringError);
                    return false;
                }
                else if (!context.ReplicateTable.ContainsKey(ValidSourceIdentifier))
                {
                    ErrorList.Add(new ErrorUnknownIdentifier(SourceIdentifier, ValidSourceIdentifier));
                    return false;
                }
                else if (context.PatternTable.ContainsKey(ValidReplicationPattern))
                {
                    ErrorList.Add(new ErrorPatternAlreadyUsed(ReplicationPattern, ValidReplicationPattern));
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

            foreach (string PatternText in PatternList)
            {
                bool Continue = true;

                context.PatternTable.Add(replicationPattern, PatternText);
                foreach (BaseNode.INode Node in NodeList)
                {
                    BaseNode.INode ClonedNode = NodeHelper.DeepCloneNode(Node, cloneCommentGuid: true);

                    if (!NodeTreeWalk<ReplicationContext>.Walk(ClonedNode, callbacks, context))
                    {
                        Continue = false;
                        break;
                    }

                    if (ClonedNode is IClass AsClass)
                        AsClass.SetFullClassPath(replicationPattern, PatternText);

                    replicatedNodeList.Add(ClonedNode);
                }

                context.PatternTable.Remove(replicationPattern);

                if (!Continue)
                    return false;
            }

            return true;
        }

        private bool ProcessNormalBlock(BaseNode.IBlock block, List<BaseNode.INode> replicatedNodeList, IWalkCallbacks<ReplicationContext> callbacks, ReplicationContext context)
        {
            IList NodeList = block.NodeList as IList;

            foreach (BaseNode.INode Node in NodeList)
            {
                if (!NodeTreeWalk<ReplicationContext>.Walk(Node, callbacks, context))
                    return false;

                if (Node is IClass AsClass)
                    AsClass.SetFullClassPath();

                replicatedNodeList.Add(Node);
            }

            return true;
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
                if (!ReplicateBlockList(blockList, ReplicatedNodeList, callbacks, context))
                    return false;

                INodeWithReplicatedBlocks NodeWithReplicatedBlocks = node as INodeWithReplicatedBlocks;
                Debug.Assert(NodeWithReplicatedBlocks != null);

                NodeWithReplicatedBlocks.FillReplicatedList(propertyName, ReplicatedNodeList);
                return true;
            }
        }
        #endregion

        #region Replication, phase 1
        /// <summary></summary>
        protected virtual bool ReplacePhase2Macroes(IRoot root)
        {
            return NodeTreeWalk<ReplacePhase2MacroContext>.Walk(root, new WalkCallbacks<ReplacePhase2MacroContext>() { HandlerNode = ReplacePhase2Macro, BlockSubstitution = new KeyValuePair<string, string>("Blocks", "List") }, new ReplacePhase2MacroContext());
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
                IInitializedObjectExpression ReplacementNode;

                switch (AsPreprocessorExpression.Value)
                {
                    case BaseNode.PreprocessorMacro.ClassPath:
                        if (context.CurrentClass != null)
                        {
                            ReplacementNode = InitializedExpression(LanguageClasses.String.Name, context.CurrentClass.FullClassPath);
                            NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        }
                        else
                        {
                            ErrorList.Add(new ErrorMacroOutOfContext(AsPreprocessorExpression));
                            Result = false;
                        }

                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.Counter:
                        if (context.CurrentClass != null)
                        {
                            ReplacementNode = InitializedExpression(LanguageClasses.Number.Name, context.CurrentClass.ClassCounter.ToString());
                            NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);

                            context.CurrentClass.IncrementClassCounter();
                        }
                        else
                        {
                            ErrorList.Add(new ErrorMacroOutOfContext(AsPreprocessorExpression));
                            Result = false;
                        }

                        IsHandled = true;
                        break;

                    case BaseNode.PreprocessorMacro.RandomInteger:
                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                        byte[] Data = new byte[8];
                        rng.GetBytes(Data);

                        string Value = "0x";
                        foreach (byte b in Data)
                            Value += b.ToString("X2");

                        ReplacementNode = InitializedExpression(LanguageClasses.Number.Name, Value);
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return Result;
        }
        #endregion
    }
}
