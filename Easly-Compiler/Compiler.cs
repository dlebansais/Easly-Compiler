namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using BaseNode;
    using BaseNodeHelper;
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

            if (!NodeTreeDiagnostic.IsValid(root))
            {
                ErrorList.Add(new ErrorInputRootInvalid(root));
                return;
            }

            if (!ReplacePhase1Macroes(root))
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
                    Serializer Serializer = new Serializer();
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
            foreach (IBlock<ILibrary, Library> Block in languageRoot.LibraryBlocks.NodeBlockList)
                root.LibraryBlocks.NodeBlockList.Add(Block);

            foreach (IBlock<IClass, Class> Block in languageRoot.ClassBlocks.NodeBlockList)
            {
                foreach (IClass Item in Block.NodeList)
                {
                    Debug.Assert(LanguageClasses.NameToGuid.ContainsKey(Item.EntityName.Text));
                    Debug.Assert(LanguageClasses.NameToGuid[Item.EntityName.Text] == Item.ClassGuid);
                }

                root.ClassBlocks.NodeBlockList.Add(Block);
            }

            Debug.Assert(languageRoot.Replicates.Count == 0);
        }

        /// <summary></summary>
        protected virtual bool ReplacePhase1Macroes(IRoot root)
        {
            GenerateCompilationDateTime();
            GenerateCompilationUID();
            GenerateCompilerVersion();
            GenerateConformanceToStandard();
            GenerateDebugging();

            return NodeTreeWalk.Walk(root, ReplacePhase1Macro, (INode node, string propertyName, Type type) => true);
        }

        private IInitializedObjectExpression CompilationDateTime;
        private IInitializedObjectExpression CompilationUID;
        private IInitializedObjectExpression CompilerVersion;
        private IInitializedObjectExpression ConformanceToStandard;
        private IInitializedObjectExpression Debugging;

        /// <summary></summary>
        protected virtual void GenerateCompilationDateTime()
        {
            CompilationDateTime = InitializedExpression("Date And Time", "0");
        }

        /// <summary></summary>
        protected virtual void GenerateCompilationUID()
        {
            string NewGuidDigits = Guid.NewGuid().ToString("N");
            CompilationUID = InitializedExpression("Globally Discrete Identifier", "0x" + NewGuidDigits);
        }

        /// <summary></summary>
        protected virtual void GenerateCompilerVersion()
        {
            CompilerVersion = InitializedExpression("String", "Easly 1");
        }

        /// <summary></summary>
        protected virtual void GenerateConformanceToStandard()
        {
            ConformanceToStandard = InitializedExpression("Boolean", "1");
        }

        /// <summary></summary>
        protected virtual void GenerateDebugging()
        {
            Debugging = InitializedExpression("Boolean", "0");
        }

        /// <summary></summary>
        protected virtual IInitializedObjectExpression InitializedExpression(string className, string initialValue)
        {
            IIdentifier ClassIdentifier = NodeHelper.CreateSimpleIdentifier(className);
            IManifestNumberExpression CurrentTime = NodeHelper.CreateSimpleManifestNumberExpression(initialValue);

            IIdentifier FirstIdentifier = NodeHelper.CreateSimpleIdentifier(string.Empty);
            IAssignmentArgument FirstArgument = NodeHelper.CreateAssignmentArgument(new List<IIdentifier>() { FirstIdentifier }, CurrentTime);
            IInitializedObjectExpression Expression = NodeHelper.CreateInitializedObjectExpression(ClassIdentifier, new List<IAssignmentArgument>() { FirstArgument });

            return Expression;
        }

        /// <summary></summary>
        protected virtual bool ReplacePhase1Macro(INode node, INode parentNode, string propertyName, IClass parentClass)
        {
            if (node is IPreprocessorExpression AsPreprocessorExpression)
            {
                bool IsHandled = false;

                switch (AsPreprocessorExpression.Value)
                {
                    case PreprocessorMacro.DateAndTime:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilationDateTime);
                        IsHandled = true;
                        break;

                    case PreprocessorMacro.CompilationDiscreteIdentifier:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilationUID);
                        IsHandled = true;
                        break;

                    case PreprocessorMacro.CompilerVersion:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, CompilerVersion);
                        IsHandled = true;
                        break;

                    case PreprocessorMacro.ConformanceToStandard:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ConformanceToStandard);
                        IsHandled = true;
                        break;

                    case PreprocessorMacro.DiscreteClassIdentifier:
                        if (parentClass == null)
                        {
                            ErrorList.Add(new ErrorMacroOutOfContext(AsPreprocessorExpression));
                            return false;
                        }
                        else
                        {
                            IInitializedObjectExpression ReplacementNode = InitializedExpression("Guid", parentClass.ClassGuid.ToString("N"));
                            NodeTreeHelperChild.SetChildNode(parentNode, propertyName, ReplacementNode);
                        }

                        IsHandled = true;
                        break;

                    case PreprocessorMacro.Debugging:
                        NodeTreeHelperChild.SetChildNode(parentNode, propertyName, Debugging);
                        IsHandled = true;
                        break;

                    // Processed in phase 2.
                    case PreprocessorMacro.ClassPath:
                    case PreprocessorMacro.Counter:
                    case PreprocessorMacro.RandomInteger:
                        IsHandled = true;
                        break;
                }

                Debug.Assert(IsHandled);
            }

            return true;
        }
        #endregion
    }
}
