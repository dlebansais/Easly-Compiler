namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using BaseNodeHelper;
    using CompilerNode;
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
        void Compile(BaseNode.Root root);

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <param name="stream">The source code to compile.</param>
        void Compile(Stream stream);
    }

    /// <summary>
    /// Process Easly source code to output code for the target language (C# supported only).
    /// </summary>
    public partial class Compiler : ICompiler
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
        public BaseNode.Root Root { get; private set; }

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
        public virtual void Compile(BaseNode.Root root)
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
            Debug.Assert(NodeTreeDiagnostic.IsValid((BaseNode.Root)LanguageRoot));

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
            BaseNode.IBlock<BaseNode.Library> BlockLibrary = languageRoot.LibraryBlocks.NodeBlockList[0];

            Debug.Assert(BlockLibrary.NodeList.Count == 1);
            root.LibraryBlocks.NodeBlockList.Add(BlockLibrary);

            BaseNode.Library LanguageLibrary = BlockLibrary.NodeList[0];
            Debug.Assert(LanguageLibrary.ClassIdentifierBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.Identifier> BlockIdentifier = LanguageLibrary.ClassIdentifierBlocks.NodeBlockList[0];

            Debug.Assert(BlockIdentifier.NodeList.Count == LanguageClasses.NameToGuid.Count);
            Debug.Assert(BlockIdentifier.NodeList.Count == LanguageClasses.GuidToName.Count);
            List<string> IdentifierList = new List<string>();
            foreach (IIdentifier Item in BlockIdentifier.NodeList)
            {
                Debug.Assert(!IdentifierList.Contains(Item.Text));
                IdentifierList.Add(Item.Text);
            }

            Debug.Assert(languageRoot.ClassBlocks.NodeBlockList.Count == 1);
            BaseNode.IBlock<BaseNode.Class> BlockClass = languageRoot.ClassBlocks.NodeBlockList[0];

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
            List<BaseNode.Identifier> IdentifierCopyList = new List<BaseNode.Identifier>(BlockIdentifier.NodeList);
            foreach (IIdentifier Item in IdentifierCopyList)
                if (Item.Text == "Any" || Item.Text == "Any Reference" || Item.Text == "Any Value")
                    BlockIdentifier.NodeList.Remove((BaseNode.Identifier)Item);

            List<BaseNode.Class> ClassCopyList = new List<BaseNode.Class>(BlockClass.NodeList);
            foreach (IClass Item in ClassCopyList)
                if (Item.EntityName.Text == "Any" || Item.EntityName.Text == "Any Reference" || Item.EntityName.Text == "Any Value")
                    BlockClass.NodeList.Remove((BaseNode.Class)Item);
        }

        /// <summary></summary>
        protected virtual bool IsRootValid(IRoot root)
        {
            bool Success = true;

            if (!NodeTreeDiagnostic.IsValid((BaseNode.Root)root, throwOnInvalid: false))
            {
                ErrorList.AddError(new ErrorInputRootInvalid());
                Success = false;
            }

            Debug.Assert(Success || !ErrorList.IsEmpty);
            return Success;
        }
        #endregion
    }
}
