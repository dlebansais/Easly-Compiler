namespace EaslyCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using BaseNodeHelper;
    using CompilerNode;
    using Easly;
    using PolySerializer;

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public partial class Compiler : ICompiler
    {
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

            bool Success = NodeTreeWalk.Walk<ReplacePhase1MacroContext>(root, new WalkCallbacks<ReplacePhase1MacroContext>() { HandlerNode = ReplacePhase1Macro, IsRecursive = true }, new ReplacePhase1MacroContext());
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
            string IsoString = Now.ToString("u", CultureInfo.InvariantCulture);
            CompilationDateTime = InitializedStringExpression(LanguageClasses.DateAndTime.Name, nameof(DateAndTime.ToUtcDateAndTime), IsoString);
        }

        /// <summary></summary>
        protected virtual void GenerateCompilationUID()
        {
            string NewGuidDigits = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture) + ":" + "H";

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

                        string GlassGuidDigits = context.CurrentClass.ClassGuid.ToString("N", CultureInfo.InvariantCulture) + ":" + "H";
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
            NodeTreeWalk.Walk<ReplicationContext>(root, Callbacks, Replication);

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
                return parentNode == null || NodeTreeWalk.Walk<ReplicationContext>(node, callbacks, context);
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

                    Continue &= NodeTreeWalk.Walk<ReplicationContext>(ClonedNode, callbacks, context);
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

                Continue &= NodeTreeWalk.Walk<ReplicationContext>(Node, callbacks, context);
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

            bool Success = NodeTreeWalk.Walk<ReplacePhase2MacroContext>(root, new WalkCallbacks<ReplacePhase2MacroContext>() { HandlerNode = ReplacePhase2Macro, IsRecursive = true, BlockSubstitution = CreateBlockSubstitution() }, new ReplacePhase2MacroContext());
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
                Value += b.ToString("X2", CultureInfo.InvariantCulture);

            return new ManifestNumberExpression(Value + ":" + "H");
        }
        #endregion
    }
}
