namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Text;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// A C# class node.
    /// </summary>
    public interface ICSharpClass : ICSharpSource<IClass>
    {
        /// <summary>
        /// The valid class name.
        /// </summary>
        string ValidClassName { get; }

        /// <summary>
        /// The valid source name.
        /// </summary>
        string ValidSourceName { get; }

        /// <summary>
        /// The list of class generics.
        /// </summary>
        IList<ICSharpGeneric> GenericList { get; }

        /// <summary>
        /// The list of class inheritances.
        /// </summary>
        IList<ICSharpInheritance> InheritanceList { get; }

        /// <summary>
        /// The list of class typedefs.
        /// </summary>
        IList<ICSharpTypedef> TypedefList { get; }

        /// <summary>
        /// The list of class discretes.
        /// </summary>
        IList<ICSharpDiscrete> DiscreteList { get; }

        /// <summary>
        /// The list of class invariants.
        /// </summary>
        IList<ICSharpAssertion> InvariantList { get; }

        /// <summary>
        /// The corresponding type.
        /// </summary>
        ICSharpClassType Type { get; }

        /// <summary>
        /// The base class. Can be null if none.
        /// </summary>
        ICSharpClass BaseClass { get; }

        /// <summary>
        /// The list of class features.
        /// </summary>
        IList<ICSharpFeature> FeatureList { get; }

        /// <summary>
        /// The list of inhrited features.
        /// </summary>
        IList<ICSharpFeature> InheritedFeatureList { get; }

        /// <summary>
        /// The list of using clauses.
        /// </summary>
        IList<IUsingClause> UsingClauseList { get; }

        /// <summary>
        /// The table of implicit delegates.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> DelegateTable { get; }

        /// <summary>
        /// The table of explicit delegates.
        /// </summary>
        ISealableDictionary<ITypeName, ICompiledType> TypedefDelegateTable { get; }

        /// <summary>
        /// True if the class shares its name with another from a different 'From' source.
        /// </summary>
        bool IsSharedName { get; }

        /// <summary>
        /// True if the class implements the method to check the class invariant.
        /// </summary>
        bool HasCheckInvariantMethod { get; }

        /// <summary>
        /// True if the class implements or inherits a method to check the class invariant.
        /// </summary>
        bool HasCheckInvariant { get; }

        /// <summary>
        /// True if the class is a singleton with generic parameters.
        /// </summary>
        bool IsParameterizedSingleton { get; }

        /// <summary>
        /// True if the class is a singleton with no generic parameters.
        /// </summary>
        bool IsUnparameterizedSingleton { get; }

        /// <summary>
        /// Gets how many contructors the class has.
        /// </summary>
        CSharpConstructorTypes ClassConstructorType { get; }

        /// <summary>
        /// True if the class is a .NET event.
        /// </summary>
        bool IsDotNetEventClass { get; }

        /// <summary>
        /// True if the class inherits from one of the .NET events.
        /// </summary>
        bool InheritFromDotNetEvent { get; }

        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="baseClass">The base class.</param>
        void SetBaseClass(ICSharpClass baseClass);

        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="classTable">The table of all classes.</param>
        void SetAncestorClasses(IDictionary<IClass, ICSharpClass> classTable);

        /// <summary>
        /// Sets the list of class features.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="featureList">The list of features.</param>
        /// <param name="inheritedFeatureList">The list of inherited features.</param>
        void SetFeatureList(ICSharpContext context, IList<ICSharpFeature> featureList, IList<ICSharpFeature> inheritedFeatureList);

        /// <summary>
        /// Sets the <see cref="IsSharedName"/> property.
        /// </summary>
        void SetIsSharedName();

        /// <summary>
        /// Find features that are overrides and mark them as such.
        /// </summary>
        void CheckOverrides();

        /// <summary>
        /// Checks if a feature in the class must be set as both read and write.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        /// <param name="isChanged">True if a feature was changed.</param>
        void CheckForcedReadWrite(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable, ref bool isChanged);

        /// <summary>
        /// Checks if properties of the class should declare a side-by-side private field of the same type.
        /// </summary>
        void CheckSideBySideAttributes();

        /// <summary>
        /// Checks if properties of the class should declare a side-by-side private field of the same type due to inherited ancestors.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        void CheckInheritSideBySideAttributes(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable);

        /// <summary>
        /// Creates delegates needed by the class.
        /// </summary>
        void CreateDelegates();

        /// <summary>
        /// Writes down the class source code.
        /// </summary>
        /// <param name="folder">The output root folder.</param>
        /// <param name="defaultNamespace">Namespace for the output code.</param>
        void Write(string folder, string defaultNamespace);

        /// <summary>
        /// Gets the full name of a class.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        string FullClassName2CSharpClassName(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);

        /// <summary>
        /// Gets the name of a class.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        string BasicClassName2CSharpClassName(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);
    }

    /// <summary>
    /// A C# class node.
    /// </summary>
    public class CSharpClass : CSharpSource<IClass>, ICSharpClass
    {
        #region Init
        /// <summary>
        /// Creates a new C# class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        public static ICSharpClass Create(IClass source)
        {
            return new CSharpClass(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpClass"/> class.
        /// </summary>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        protected CSharpClass(IClass source)
            : base(source)
        {
            ValidClassName = source.ValidClassName;
            ValidSourceName = source.ValidSourceName;

            foreach (IGeneric Item in Source.GenericList)
            {
                ICSharpGeneric NewGeneric = CSharpGeneric.Create(Item);
                GenericList.Add(NewGeneric);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The valid class name.
        /// </summary>
        public string ValidClassName { get; }

        /// <summary>
        /// The valid source name.
        /// </summary>
        public string ValidSourceName { get; }

        /// <summary>
        /// The list of class generics.
        /// </summary>
        public IList<ICSharpGeneric> GenericList { get; } = new List<ICSharpGeneric>();

        /// <summary>
        /// The list of class inheritances.
        /// </summary>
        public IList<ICSharpInheritance> InheritanceList { get; } = new List<ICSharpInheritance>();

        /// <summary>
        /// The list of class typedefs.
        /// </summary>
        public IList<ICSharpTypedef> TypedefList { get; } = new List<ICSharpTypedef>();

        /// <summary>
        /// The list of class discretes.
        /// </summary>
        public IList<ICSharpDiscrete> DiscreteList { get; } = new List<ICSharpDiscrete>();

        /// <summary>
        /// The list of class invariants.
        /// </summary>
        public IList<ICSharpAssertion> InvariantList { get; } = new List<ICSharpAssertion>();

        /// <summary>
        /// The corresponding type.
        /// </summary>
        public ICSharpClassType Type { get; private set; }

        /// <summary>
        /// The base class. Can be null if none.
        /// </summary>
        public ICSharpClass BaseClass { get; private set; }

        /// <summary>
        /// The list of class features.
        /// </summary>
        public IList<ICSharpFeature> FeatureList { get; private set; }

        /// <summary>
        /// The list of inhrited features.
        /// </summary>
        public IList<ICSharpFeature> InheritedFeatureList { get; private set; }

        /// <summary>
        /// The list of using clauses.
        /// </summary>
        public IList<IUsingClause> UsingClauseList { get; } = new List<IUsingClause>();

        /// <summary>
        /// The table of implicit delegates.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> DelegateTable { get; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// The table of explicit delegates.
        /// </summary>
        public ISealableDictionary<ITypeName, ICompiledType> TypedefDelegateTable { get; } = new SealableDictionary<ITypeName, ICompiledType>();

        /// <summary>
        /// True if the class shares its name with another from a different 'From' source.
        /// </summary>
        public bool IsSharedName { get; private set; }

        /// <summary>
        /// True if the class implements the method to check the class invariant.
        /// </summary>
        public bool HasCheckInvariantMethod { get { return InvariantList.Count > 0; } }

        /// <summary>
        /// True if the class implements or inherits a method to check the class invariant.
        /// </summary>
        public bool HasCheckInvariant { get { return HasCheckInvariantMethod || (BaseClass != null && BaseClass.HasCheckInvariant); } }

        /// <summary>
        /// True if the class is a singleton with generic parameters.
        /// </summary>
        public bool IsParameterizedSingleton { get { return Source.Cloneable == BaseNode.CloneableStatus.Single && Source.GenericTable.Count > 0; } }

        /// <summary>
        /// True if the class is a singleton with no generic parameters.
        /// </summary>
        public bool IsUnparameterizedSingleton { get { return Source.Cloneable == BaseNode.CloneableStatus.Single && Source.GenericTable.Count == 0; } }

        /// <summary>
        /// Gets how many contructors the class has.
        /// </summary>
        public CSharpConstructorTypes ClassConstructorType
        {
            get
            {
                int ConstructorCount = 0;

                foreach (ICSharpFeature Feature in FeatureList)
                    if (Feature is ICSharpCreationFeature)
                        ConstructorCount++;

                if (ConstructorCount == 0)
                    return CSharpConstructorTypes.NoConstructor;
                else if (ConstructorCount == 1)
                    return CSharpConstructorTypes.OneConstructor;
                else
                    return CSharpConstructorTypes.ManyConstructors;
            }
        }

        /// <summary>
        /// True if the class is a .NET event.
        /// </summary>
        public bool IsDotNetEventClass { get { return ValidClassName == ".NET Event" && ValidSourceName == "Microsoft .NET"; } }

        /// <summary>
        /// True if the class inherits from one of the .NET events.
        /// </summary>
        public bool InheritFromDotNetEvent
        {
            get
            {
                if (IsDotNetEventClass)
                    return true;

                foreach (ICSharpInheritance Inheritance in InheritanceList)
                    if (Inheritance.AncestorClass != null)
                        if (Inheritance.AncestorClass.InheritFromDotNetEvent)
                            return true;

                return false;
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a class is one of the language classes.
        /// </summary>
        /// <param name="sourceClass">The class to check.</param>
        public static bool IsLanguageClass(IClass sourceClass)
        {
            return LanguageClasses.GuidToName.ContainsKey(sourceClass.ClassGuid);
        }

        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="baseClass">The base class.</param>
        public void SetBaseClass(ICSharpClass baseClass)
        {
            Debug.Assert(baseClass != null);
            Debug.Assert(BaseClass == null);

            BaseClass = baseClass;
        }

        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="classTable">The table of all classes.</param>
        public void SetAncestorClasses(IDictionary<IClass, ICSharpClass> classTable)
        {
            foreach (IInheritance Item in Source.InheritanceList)
            {
                IClass BaseClass = Item.ResolvedClassParentType.Item.BaseClass;
                Guid ClassGuid = BaseClass.ClassGuid;

                Debug.Assert(classTable.ContainsKey(BaseClass));

                ICSharpClass AncestorClass = classTable[BaseClass];
                ICSharpInheritance NewInheritance = CSharpInheritance.Create(Item, AncestorClass);

                InheritanceList.Add(NewInheritance);
            }
        }

        /// <summary>
        /// Sets the list of class features.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="featureList">The list of features.</param>
        /// <param name="inheritedFeatureList">The list of inherited features.</param>
        public void SetFeatureList(ICSharpContext context, IList<ICSharpFeature> featureList, IList<ICSharpFeature> inheritedFeatureList)
        {
            Debug.Assert(featureList != null);
            Debug.Assert(inheritedFeatureList != null);
            Debug.Assert(FeatureList == null);
            Debug.Assert(InheritedFeatureList == null);

            FeatureList = featureList;
            InheritedFeatureList = inheritedFeatureList;

            Type = CSharpClassType.Create(context, Source.ResolvedClassType.Item);

            foreach (ICSharpGeneric Generic in GenericList)
                Generic.Init(context);

            foreach (ITypedef Item in Source.TypedefList)
            {
                ICSharpTypedef NewTypedef = CSharpTypedef.Create(context, Item, this);
                TypedefList.Add(NewTypedef);
            }

            foreach (IDiscrete Item in Source.DiscreteList)
            {
                ICSharpDiscrete NewDiscrete = CSharpDiscrete.Create(context, Item);
                DiscreteList.Add(NewDiscrete);
            }

            foreach (IAssertion Item in Source.InvariantList)
            {
                ICSharpAssertion NewAssertion = CSharpAssertion.Create(context, Item);
                InvariantList.Add(NewAssertion);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsSharedName"/> property.
        /// </summary>
        public void SetIsSharedName()
        {
            IsSharedName = true;
        }

        /// <summary>
        /// Find features that are overrides and mark them as such.
        /// </summary>
        public void CheckOverrides()
        {
            foreach (ICSharpFeature Feature in FeatureList)
                CheckOverride(Feature);
        }

        private void CheckOverride(ICSharpFeature feature)
        {
            ICompiledFeature SourceFeature = feature.Source;
            IFeatureInstance FeatureInstance = feature.Instance;
            IClass FeatureSourceClass = feature.Owner.Source;

            bool IsRename = false;
            if (SourceFeature is IFeatureWithName && SourceFeature is IFeature AsFeatureWithName)
            {
                foreach (ICSharpInheritance InheritanceItem in InheritanceList)
                {
                    IClassType ParentType = InheritanceItem.Source.ResolvedClassParentType.Item;
                    IClass ParentClass = ParentType.BaseClass;

                    foreach (IRename RenameItem in InheritanceItem.Source.RenameList)
                    {
                        string ValidDestinationText = RenameItem.ValidDestinationText.Item;
                        if (ValidDestinationText == AsFeatureWithName.ValidFeatureName.Item.Name)
                        {
                            IsRename = true;
                            break;
                        }
                    }

                    if (IsRename)
                        break;
                }
            }

            if (!IsRename && FeatureSourceClass == Source && FeatureInstance.PrecursorList.Count > 0)
                if (BaseClass != null)
                {
                    IClass OriginatingClass = FeatureInstance.PrecursorList[0].Ancestor.BaseClass;
                    if (OriginatingClass == BaseClass.Source)
                        feature.MarkAsOverride();
                }
        }

        /// <summary>
        /// Checks if a feature in the class must be set as both read and write.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        /// <param name="isChanged">True if a feature was changed.</param>
        public void CheckForcedReadWrite(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable, ref bool isChanged)
        {
            foreach (ICSharpFeature Feature in FeatureList)
                if (Feature.IsOverride)
                {
                    Debug.Assert(BaseClass != null);

                    switch (Feature)
                    {
                        case ICSharpPropertyFeature AsPropertyFeature:
                            CheckForcedReadWriteProperty(globalFeatureTable, AsPropertyFeature, ref isChanged);
                            break;

                        case ICSharpIndexerFeature AsIndexerFeature:
                            CheckForcedReadWriteIndexer(globalFeatureTable, AsIndexerFeature, ref isChanged);
                            break;
                    }
                }
        }

        private void CheckForcedReadWriteProperty(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable, ICSharpPropertyFeature feature, ref bool isChanged)
        {
            IFeatureInstance Instance = feature.Instance;

            foreach (IPrecursorInstance Item in Instance.PrecursorList)
            {
                ICompiledFeature SourceFeature = Item.Precursor.Feature;
                Debug.Assert(globalFeatureTable.ContainsKey(SourceFeature));

                if (globalFeatureTable[SourceFeature] is ICSharpPropertyFeature AsPropertyPrecursor)
                    if (!AsPropertyPrecursor.IsForcedReadWrite)
                        if (feature.IsForcedReadWrite || feature.Source.PropertyKind != AsPropertyPrecursor.Source.PropertyKind)
                        {
                            AsPropertyPrecursor.MarkAsForcedReadWrite();
                            isChanged = true;
                        }
            }
        }

        private void CheckForcedReadWriteIndexer(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable, ICSharpIndexerFeature feature, ref bool isChanged)
        {
            IFeatureInstance Instance = feature.Instance;

            foreach (IPrecursorInstance Item in Instance.PrecursorList)
            {
                ICompiledFeature SourceFeature = Item.Precursor.Feature;
                Debug.Assert(globalFeatureTable.ContainsKey(SourceFeature));

                if (globalFeatureTable[SourceFeature] is ICSharpIndexerFeature AsIndexerPrecursor)
                    if (!AsIndexerPrecursor.IsForcedReadWrite)
                        if (feature.IsForcedReadWrite || feature.Source.GetterBody.IsAssigned != AsIndexerPrecursor.Source.GetterBody.IsAssigned || feature.Source.SetterBody.IsAssigned != AsIndexerPrecursor.Source.SetterBody.IsAssigned)
                        {
                            AsIndexerPrecursor.MarkAsForcedReadWrite();
                            isChanged = true;
                        }
            }
        }

        /// <summary>
        /// Checks if properties of the class should declare a side-by-side private field of the same type.
        /// </summary>
        public void CheckSideBySideAttributes()
        {
            foreach (ICSharpFeature Feature in FeatureList)
                if (Feature is ICSharpPropertyFeature AsPropertyFeature)
                    AsPropertyFeature.CheckSideBySideAttribute();
        }

        /// <summary>
        /// Checks if properties of the class should declare a side-by-side private field of the same type due to inherited ancestors.
        /// </summary>
        /// <param name="globalFeatureTable">The table of all known features.</param>
        public void CheckInheritSideBySideAttributes(IDictionary<ICompiledFeature, ICSharpFeature> globalFeatureTable)
        {
            foreach (ICSharpFeature Feature in FeatureList)
                if (Feature is ICSharpPropertyFeature AsPropertyFeature)
                    AsPropertyFeature.CheckInheritSideBySideAttribute(globalFeatureTable);
        }

        /// <summary>
        /// Creates delegates needed by the class.
        /// </summary>
        public void CreateDelegates()
        {
            CreateImplicitDelegates();
            CreateExplicitDelegates();
        }

        private void CreateImplicitDelegates()
        {
            // TODO: mark delegate used in program and add them to the final source code
            // TODO: detect delegate call parameters to select the proper overload
            foreach (KeyValuePair<ITypeName, ICompiledType> Item in Source.TypeTable)
            {
                ICompiledTypeWithFeature BaseType;
                ITypeName ResolvedTypeName;

                switch (Item.Value)
                {
                    case IFunctionType AsFunctionType:
                        BaseType = AsFunctionType.ResolvedBaseType.Item;

                        if (!FunctionType.TypeTableContaining(DelegateTable, BaseType, AsFunctionType.OverloadList, out ResolvedTypeName, out IFunctionType ResolvedFunctionType))
                            DelegateTable.Add(Item.Key, Item.Value);
                        break;

                    case IProcedureType AsProcedureType:
                        BaseType = AsProcedureType.ResolvedBaseType.Item;

                        if (!ProcedureType.TypeTableContaining(DelegateTable, BaseType, AsProcedureType.OverloadList, out ResolvedTypeName, out IProcedureType ResolvedProcedureType))
                            DelegateTable.Add(Item.Key, Item.Value);
                        break;
                }
            }
        }

        private void CreateExplicitDelegates()
        {
            foreach (KeyValuePair<IFeatureName, ITypedefType> Entry in Source.LocalTypedefTable)
            {
                ITypeName ReferencedTypeName = Entry.Value.ReferencedTypeName.Item;
                ICompiledType ReferencedType = Entry.Value.ReferencedType.Item;

                switch (ReferencedType)
                {
                    case IFunctionType AsFunctionType:
                    case IProcedureType AsProcedureType:
                        TypedefDelegateTable.Add(ReferencedTypeName, ReferencedType);
                        break;
                }
            }
        }

        /// <summary>
        /// Writes down the class source code.
        /// </summary>
        /// <param name="rootFolder">The output root folder.</param>
        /// <param name="defaultNamespace">Namespace for the output code.</param>
        public void Write(string rootFolder, string defaultNamespace)
        {
            try
            {
                string SourceName = Source.FromIdentifier.IsAssigned ? ((IIdentifier)Source.FromIdentifier.Item).ValidText.Item : null;
                string ClassFileName = $"{ValidClassName}.cs";
                LocateOrCreatePath(rootFolder, SourceName, ClassFileName, out string FilePath);

                string OutputFolder = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(OutputFolder))
                    Directory.CreateDirectory(OutputFolder);

                using (FileStream Stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (CSharpWriter Writer = new CSharpWriter(Stream, defaultNamespace))
                    {
                        Write(Writer);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        private void LocateOrCreatePath(string rootFolder, string sourceName, string classFileName, out string filePath)
        {
            LocateOrCreateSolution(rootFolder, out string SolutionFullPath);
            LocateOrCreateProject(SolutionFullPath, out string ProjectFullPath);

            List<string> LineList = new List<string>();

            using (FileStream fs = new FileStream(ProjectFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.ASCII))
                {
                    for (;;)
                    {
                        string Line = sr.ReadLine();
                        if (Line == null)
                            break;

                        LineList.Add(Line);
                    }
                }
            }

            string FilePath = (sourceName != null) ? Path.Combine(sourceName, classFileName) : classFileName;
            const string Pattern = "<Compile Include=";

            foreach (string Line in LineList)
                if (Line.Contains("\"" + FilePath + "\"") && Line.Contains(Pattern))
                {
                    filePath = Path.Combine(Path.GetDirectoryName(ProjectFullPath), FilePath);
                    return;
                }

            foreach (string Line in LineList)
            {
                if (Line.Contains("\\" + FilePath) && Line.Contains(Pattern))
                {
                    FilePath = Line.Substring(Line.IndexOf(Pattern) + Pattern.Length);
                    while (FilePath[0] == '"')
                        FilePath = FilePath.Substring(1);

                    int FileEndIndex = FilePath.IndexOf('"');
                    if (FileEndIndex > 0)
                        FilePath = FilePath.Substring(0, FileEndIndex);

                    filePath = Path.Combine(Path.GetDirectoryName(ProjectFullPath), FilePath);
                    return;
                }
            }

            if (!IsSharedName)
                foreach (string Line in LineList)
                {
                    if ((Line.Contains("\\" + classFileName) || Line.Contains("\"" + classFileName)) && Line.Contains(Pattern))
                    {
                        FilePath = Line.Substring(Line.IndexOf(Pattern) + Pattern.Length);
                        while (FilePath[0] == '"')
                            FilePath = FilePath.Substring(1);

                        int FileEndIndex = FilePath.IndexOf('"');
                        if (FileEndIndex > 0)
                            FilePath = FilePath.Substring(0, FileEndIndex);

                        filePath = Path.Combine(Path.GetDirectoryName(ProjectFullPath), FilePath);
                        return;
                    }
                }

            filePath = Path.Combine(Path.GetDirectoryName(ProjectFullPath), FilePath);
            string FolderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            int StartIncludeIndex = -1;
            for (int i = 0; i < LineList.Count; i++)
            {
                string Line = LineList[i];
                if (Line.Contains(Pattern))
                    StartIncludeIndex = i;
            }

            LineList.Insert(StartIncludeIndex + 1, "    <Compile Include=\"" + FilePath + "\" />");

            using (FileStream fs = new FileStream(ProjectFullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    foreach (string Line in LineList)
                        sw.WriteLine(Line);
                }
            }
        }

        private static void LocateOrCreateSolution(string rootFolder, out string solutionFullPath)
        {
            string[] Solutions = Directory.GetFiles(rootFolder, "*.sln");
            if (Solutions.Length > 0)
                solutionFullPath = Solutions[0];
            else
                CreateSolution(rootFolder, out solutionFullPath);
        }

        private static void LocateOrCreateProject(string solutionFullPath, out string projectFullPath)
        {
            List<string> LineList = new List<string>();

            using (FileStream fs = new FileStream(solutionFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.ASCII))
                {
                    for (; ; )
                    {
                        string Line = sr.ReadLine();
                        if (Line == null)
                            break;

                        LineList.Add(Line);
                    }
                }
            }

            List<string> ProjectPathList = new List<string>();

            foreach (string Line in LineList)
            {
                const string Pattern = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = ";
                if (Line.StartsWith(Pattern))
                {
                    string[] LineComponents = Line.Split(',');
                    if (LineComponents.Length > 1)
                    {
                        string ProjectPath = LineComponents[1];
                        if (ProjectPath.Contains(".csproj"))
                        {
                            while (ProjectPath[0] == ' ' || ProjectPath[0] == '"')
                                ProjectPath = ProjectPath.Substring(1);

                            while (ProjectPath[ProjectPath.Length - 1] == ' ' || ProjectPath[ProjectPath.Length - 1] == '"')
                                ProjectPath = ProjectPath.Substring(0, ProjectPath.Length - 1);

                            ProjectPathList.Add(ProjectPath);
                        }
                    }
                }
            }

            foreach (string ProjectPath in ProjectPathList)
            {
                string FullPath = Path.Combine(Path.GetDirectoryName(solutionFullPath), ProjectPath);
                if (ProjectIncludesEasly(FullPath))
                {
                    projectFullPath = FullPath;
                    return;
                }
            }

            Guid ProjectGuid;
            CreateProject(Path.GetDirectoryName(solutionFullPath), out projectFullPath, out ProjectGuid);

            string ProjectName = Path.GetFileNameWithoutExtension(projectFullPath);
            string ProjectFile = Path.GetFileName(projectFullPath);

            int StartGlobalIndex = -1;
            int SolutionConfigurationIndex = -1;
            int ProjectConfigurationIndex = -1;
            for (int i = 0; i < LineList.Count; i++)
            {
                string Line = LineList[i];
                if (Line == "Global")
                    StartGlobalIndex = i;
                else if (Line.Trim() == "GlobalSection(SolutionConfigurationPlatforms)")
                    SolutionConfigurationIndex = i;
                else if (Line.Trim() == "GlobalSection(ProjectConfigurationPlatforms)")
                    ProjectConfigurationIndex = i;
            }

            if (StartGlobalIndex == -1)
            {
                LineList.Add("Global");
                StartGlobalIndex = LineList.Count;
                LineList.Add("	GlobalSection(SolutionProperties) = preSolution");
                LineList.Add("		HideSolutionNode = FALSE");
                LineList.Add("	EndGlobalSection");
                LineList.Add("EndGlobal");
            }

            if (SolutionConfigurationIndex == -1)
            {
                LineList.Insert(StartGlobalIndex + 1, "	EndGlobalSection");
                LineList.Insert(StartGlobalIndex + 1, "		Release|x64 = Release|x64");
                LineList.Insert(StartGlobalIndex + 1, "		Debug|x64 = Debug|x64");
                LineList.Insert(StartGlobalIndex + 1, "	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                if (ProjectConfigurationIndex == -1)
                {
                    ProjectConfigurationIndex = StartGlobalIndex + 5;
                    LineList.Insert(ProjectConfigurationIndex, "	EndGlobalSection");
                    LineList.Insert(ProjectConfigurationIndex, "	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                }
            }

            if (ProjectConfigurationIndex == -1)
            {
                LineList.Insert(StartGlobalIndex + 1, "	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                LineList.Insert(StartGlobalIndex + 1, "	EndGlobalSection");
                ProjectConfigurationIndex = StartGlobalIndex + 2;
            }

            LineList.Insert(ProjectConfigurationIndex + 1, string.Empty + ProjectGuid + ".Release|x64.Build.0 = Release|x64");
            LineList.Insert(ProjectConfigurationIndex + 1, string.Empty + ProjectGuid + ".Release|x64.ActiveCfg = Release|x64");
            LineList.Insert(ProjectConfigurationIndex + 1, string.Empty + ProjectGuid + ".Debug|x64.Build.0 = Debug|x64");
            LineList.Insert(ProjectConfigurationIndex + 1, string.Empty + ProjectGuid + ".Debug|x64.ActiveCfg = Debug|x64");

            LineList.Insert(StartGlobalIndex, "EndProject");
            LineList.Insert(StartGlobalIndex, "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + ProjectName + "\", \"" + ProjectFile + "\", \"" + ProjectGuid + "\"");

            using (FileStream fs = new FileStream(solutionFullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    foreach (string Line in LineList)
                        sw.WriteLine(Line);
                }
            }
        }

        private static void CreateSolution(string rootFolder, out string solutionFullPath)
        {
            solutionFullPath = Path.Combine(rootFolder, "CSharpProject.sln");

            string ProjectFullPath;
            Guid ProjectGuid;
            CreateProject(rootFolder, out ProjectFullPath, out ProjectGuid);

            string ProjectName = Path.GetFileNameWithoutExtension(ProjectFullPath);
            string ProjectFile = Path.GetFileName(ProjectFullPath);

            using (FileStream fs = new FileStream(solutionFullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.WriteLine();
                    sw.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
                    sw.WriteLine("# Visual Studio 15");
                    sw.WriteLine("VisualStudioVersion = 15.0.28010.2041");
                    sw.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
                    sw.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + ProjectName + "\", \"" + ProjectFile + "\", \"" + ProjectGuid + "\"");
                    sw.WriteLine("EndProject");
                    sw.WriteLine("Global");
                    sw.WriteLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                    sw.WriteLine("		Debug|x64 = Debug|x64");
                    sw.WriteLine("		Release|x64 = Release|x64");
                    sw.WriteLine("	EndGlobalSection");
                    sw.WriteLine("	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                    sw.WriteLine("		" + ProjectGuid + ".Debug|x64.ActiveCfg = Debug|x64");
                    sw.WriteLine("		" + ProjectGuid + ".Debug|x64.Build.0 = Debug|x64");
                    sw.WriteLine("		" + ProjectGuid + ".Release|x64.ActiveCfg = Release|x64");
                    sw.WriteLine("		" + ProjectGuid + ".Release|x64.Build.0 = Release|x64");
                    sw.WriteLine("	EndGlobalSection");
                    sw.WriteLine("	GlobalSection(SolutionProperties) = preSolution");
                    sw.WriteLine("		HideSolutionNode = FALSE");
                    sw.WriteLine("	EndGlobalSection");
                    sw.WriteLine("EndGlobal");
                }
            }
        }

        private static bool ProjectIncludesEasly(string projectFullPath)
        {
            using (FileStream fs = new FileStream(projectFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.ASCII))
                {
                    for (; ; )
                    {
                        string Line = sr.ReadLine();
                        if (Line == null)
                            break;

                        if (Line.Contains("<Compile Include=\"Language\\DetachableReference.cs\" />"))
                            return true;
                    }
                }
            }

            return false;
        }

        private static void CreateProject(string rootFolder, out string projectFullPath, out Guid projectGuid)
        {
            projectFullPath = Path.Combine(rootFolder, "CSharpProject.csproj");
            projectGuid = Guid.NewGuid();

            string ProjectName = Path.GetFileNameWithoutExtension(projectFullPath);
            string ProjectFile = Path.GetFileName(projectFullPath);

            using (FileStream fs = new FileStream(projectFullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<Project ToolsVersion=\"15.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
                    sw.WriteLine("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />");
                    sw.WriteLine("  <PropertyGroup>");
                    sw.WriteLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
                    sw.WriteLine("    <Platform Condition=\" '$(Platform)' == '' \">x64</Platform>");
                    sw.WriteLine("    <projectGuid>" + projectGuid + "</projectGuid>");
                    sw.WriteLine("    <OutputType>Library</OutputType>");
                    sw.WriteLine("    <AppDesignerFolder>Properties</AppDesignerFolder>");
                    sw.WriteLine("    <RootNamespace>" + ProjectName + "</RootNamespace>");
                    sw.WriteLine("    <AssemblyName>" + ProjectName + "</AssemblyName>");
                    sw.WriteLine("    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>");
                    sw.WriteLine("    <FileAlignment>512</FileAlignment>");
                    sw.WriteLine("  </PropertyGroup>");
                    sw.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|x64' \">");
                    sw.WriteLine("    <DebugSymbols>true</DebugSymbols>");
                    sw.WriteLine("    <DebugType>full</DebugType>");
                    sw.WriteLine("    <Optimize>false</Optimize>");
                    sw.WriteLine("    <OutputPath>bin\\Debug\\</OutputPath>");
                    sw.WriteLine("    <DefineConstants>DEBUG;TRACE</DefineConstants>");
                    sw.WriteLine("    <ErrorReport>prompt</ErrorReport>");
                    sw.WriteLine("    <WarningLevel>4</WarningLevel>");
                    sw.WriteLine("    <LangVersion>7.2</LangVersion>");
                    sw.WriteLine("  </PropertyGroup>");
                    sw.WriteLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|x64' \">");
                    sw.WriteLine("    <DebugType>pdbonly</DebugType>");
                    sw.WriteLine("    <Optimize>true</Optimize>");
                    sw.WriteLine("    <OutputPath>bin\\Release\\</OutputPath>");
                    sw.WriteLine("    <DefineConstants>TRACE</DefineConstants>");
                    sw.WriteLine("    <ErrorReport>prompt</ErrorReport>");
                    sw.WriteLine("    <WarningLevel>4</WarningLevel>");
                    sw.WriteLine("    <LangVersion>7.2</LangVersion>");
                    sw.WriteLine("  </PropertyGroup>");
                    sw.WriteLine("  <ItemGroup>");
                    sw.WriteLine("    <Reference Include=\"PolySerializer-Attributes\">");
                    sw.WriteLine("      <HintPath>.\\PolySerializer\\$(Platform)\\$(Configuration)\\PolySerializer-Attributes.dll</HintPath>");
                    sw.WriteLine("    </Reference>");
                    sw.WriteLine("    <Reference Include=\"PresentationCore\" />");
                    sw.WriteLine("    <Reference Include=\"PresentationFramework\" />");
                    sw.WriteLine("    <Reference Include=\"System\" />");
                    sw.WriteLine("    <Reference Include=\"System.Core\" />");
                    sw.WriteLine("    <Reference Include=\"System.Xaml\" />");
                    sw.WriteLine("    <Reference Include=\"System.Xml.Linq\" />");
                    sw.WriteLine("    <Reference Include=\"System.Data.DataSetExtensions\" />");
                    sw.WriteLine("    <Reference Include=\"Microsoft.CSharp\" />");
                    sw.WriteLine("    <Reference Include=\"System.Data\" />");
                    sw.WriteLine("    <Reference Include=\"System.Xml\" />");
                    sw.WriteLine("    <Reference Include=\"WindowsBase\" />");
                    sw.WriteLine("  </ItemGroup>");
                    sw.WriteLine("  <ItemGroup>");
                    sw.WriteLine("    <Compile Include=\"Language\\DetachableReference.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\OnceReference.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\OptionalReference.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\StableReference.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\SealableDictionary.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\SealableList.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\Entity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\SpecializedTypeEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\TypeEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\FeatureEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\FunctionEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\IndexerEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\NamedFeatureEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\ProcedureEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Language\\Entity\\FeatureEntity\\PropertyEntity.cs\" />");
                    sw.WriteLine("    <Compile Include=\"Properties\\AssemblyInfo.cs\" />");
                    sw.WriteLine("  </ItemGroup>");
                    sw.WriteLine("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
                    sw.WriteLine("  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. ");
                    sw.WriteLine("       Other similar extension points exist, see Microsoft.Common.targets.");
                    sw.WriteLine("  <Target Name=\"BeforeBuild\">");
                    sw.WriteLine("  </Target>");
                    sw.WriteLine("  <Target Name=\"AfterBuild\">");
                    sw.WriteLine("  </Target>");
                    sw.WriteLine("  -->");
                    sw.WriteLine("</Project>");
                }
            }

            string PropertiesFolder = Path.Combine(rootFolder, "Properties");
            if (!Directory.Exists(PropertiesFolder))
                Directory.CreateDirectory(PropertiesFolder);

            using (FileStream fs = new FileStream(Path.Combine(PropertiesFolder, "AssemblyInfo.cs"), FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.WriteLine("using System.Reflection;");
                    sw.WriteLine("using System.Runtime.CompilerServices;");
                    sw.WriteLine("using System.Runtime.InteropServices;");
                    sw.WriteLine();
                    sw.WriteLine("// General Information about an assembly is controlled through the following");
                    sw.WriteLine("// set of attributes. Change these attribute values to modify the information");
                    sw.WriteLine("// associated with an assembly.");
                    sw.WriteLine("[assembly: AssemblyTitle(\"CSharpProject\")]");
                    sw.WriteLine("[assembly: AssemblyDescription(\"\")]");
                    sw.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                    sw.WriteLine("[assembly: AssemblyCompany(\"\")]");
                    sw.WriteLine("[assembly: AssemblyProduct(\"\")]");
                    sw.WriteLine("[assembly: AssemblyCopyright(\"\")]");
                    sw.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                    sw.WriteLine("[assembly: AssemblyCulture(\"\")]");
                    sw.WriteLine();
                    sw.WriteLine("// Setting ComVisible to false makes the types in this assembly not visible");
                    sw.WriteLine("// to COM components.  If you need to access a type in this assembly from");
                    sw.WriteLine("// COM, set the ComVisible attribute to true on that type.");
                    sw.WriteLine("[assembly: ComVisible(false)]");
                    sw.WriteLine();
                    sw.WriteLine("// Version information for an assembly consists of the following four values:");
                    sw.WriteLine("//");
                    sw.WriteLine("//      Major Version");
                    sw.WriteLine("//      Minor Version");
                    sw.WriteLine("//      Build Number");
                    sw.WriteLine("//      Revision");
                    sw.WriteLine("//");
                    sw.WriteLine("// You can specify all the values or you can default the Build and Revision Numbers");
                    sw.WriteLine("// by using the '*' as shown below:");
                    sw.WriteLine("// [assembly: AssemblyVersion(\"1.0.*\")]");
                    sw.WriteLine("[assembly: AssemblyVersion(\"1.0.0.0\")]");
                    sw.WriteLine("[assembly: AssemblyFileVersion(\"1.0.0.0\")]");
                }
            }

            if (!Directory.Exists(Path.Combine(rootFolder, "Language")))
            {
                Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
                string[] ManifestResourceNames = CurrentAssembly.GetManifestResourceNames();

                string ManifestResourceName = null;
                foreach (string s in ManifestResourceNames)
                    if (s.EndsWith("Language.zip"))
                    {
                        ManifestResourceName = s;
                        break;
                    }

                Debug.Assert(ManifestResourceName != null);

                using (Stream fs = CurrentAssembly.GetManifestResourceStream(ManifestResourceName))
                {
                    using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        Archive.ExtractToDirectory(rootFolder);
                    }
                }
            }

            if (!Directory.Exists(Path.Combine(rootFolder, "PolySerializer")))
            {
                Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
                string[] ManifestResourceNames = CurrentAssembly.GetManifestResourceNames();

                string ManifestResourceName = null;
                foreach (string s in ManifestResourceNames)
                    if (s.EndsWith("PolySerializer.zip"))
                    {
                        ManifestResourceName = s;
                        break;
                    }

                Debug.Assert(ManifestResourceName != null);

                using (Stream fs = CurrentAssembly.GetManifestResourceStream(ManifestResourceName))
                {
                    using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        Archive.ExtractToDirectory(rootFolder);
                    }
                }
            }
        }
        #endregion

        #region Implementation
        private void Write(ICSharpWriter writer)
        {
            if (Source.IsEnumeration)
                WriteEnum(writer);
            else
                WriteClass(writer);
        }

        private void WriteEnum(ICSharpWriter writer)
        {
            writer.WriteIndentedLine($"namespace {writer.DefaultNamespace}");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            string ClassName = FullClassName2CSharpClassName(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

            writer.WriteIndentedLine($"public enum {ClassName}");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            for (int i = 0; i < DiscreteList.Count; i++)
            {
                ICSharpDiscrete Item = DiscreteList[i];
                string Line = string.Empty;

                Line += CSharpNames.ToCSharpIdentifier(Item.Name);

                if (Item.ExplicitValue != null)
                {
                    ICSharpExpression ExplicitValue = Item.ExplicitValue;
                    Line += " " + "=" + " " + ExplicitValue.CSharpText(writer);
                }

                if (i + 1 < DiscreteList.Count)
                    Line += ",";

                writer.WriteIndentedLine(Line);
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }

        private void WriteClass(ICSharpWriter writer)
        {
            writer.WriteIndentedLine($"namespace {writer.DefaultNamespace}");
            writer.WriteIndentedLine("{");

            // Next commit will write using directives.
            writer.CommitLines();

            writer.IncreaseIndent();

            if (IsParameterizedSingleton)
                WriteParameterizedSingletonClass(writer);
            else
                WriteClassInterface(writer);

            if (InvariantList.Count > 0)
                WriteClassContract(writer);

            WriteClassImplementation(writer);

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }

        private void WriteParameterizedSingletonClass(ICSharpWriter writer)
        {
            string ClassName = BasicClassName2CSharpClassName(writer, CSharpTypeFormats.AsSingleton, CSharpNamespaceFormats.None);

            writer.WriteIndentedLine($"class {ClassName}");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine($"static  {ClassName}()");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine("SingletonSet = new Hashtable<string, object>();");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            writer.WriteEmptyLine();
            writer.WriteIndentedLine("public static Hashtable<string, object> SingletonSet;");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            writer.WriteEmptyLine();
        }

        private void WriteClassInterface(ICSharpWriter writer)
        {
            string InterfaceName = FullClassName2CSharpClassName(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

            IList<ICSharpClass> InterfaceList = new List<ICSharpClass>();
            if (BaseClass != null)
                InterfaceList.Add(BaseClass);

            foreach (ICSharpInheritance Inheritance in InheritanceList)
            {
                if (Inheritance.Source.Conformance == BaseNode.ConformanceType.NonConformant)
                    continue;

                ICSharpClass AncestorClass = Inheritance.AncestorClass;

                if (AncestorClass != null && AncestorClass != BaseClass)
                    InterfaceList.Add(AncestorClass);
            }

            string InterfaceDeclarationLine = $"public interface {InterfaceName}";

            if (InterfaceList.Count > 0)
            {
                string OtherInterfaceNames = string.Empty;
                foreach (ICSharpClass OtherInterface in InterfaceList)
                {
                    if (OtherInterfaceNames.Length > 0)
                        OtherInterfaceNames += ", ";

                    OtherInterfaceNames += OtherInterface.FullClassName2CSharpClassName(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                }

                InterfaceDeclarationLine += $" : {OtherInterfaceNames}";
            }

            writer.WriteIndentedLine(InterfaceDeclarationLine);

            foreach (ICSharpGeneric Generic in GenericList)
                WriteGenericWhereClause(writer, Generic);

            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            foreach (ICSharpFeature Feature in FeatureList)
            {
                if (!IsDirectFeature(Feature))
                    continue;

                CSharpExports ExportStatus = Feature.GetExportStatus(this);
                bool IsLocal = Feature.Owner == this;
                if (ExportStatus != CSharpExports.Public || Feature is ICSharpCreationFeature)
                    continue;

                bool IsMultiline = false;
                bool IsFirstFeature = true;
                Feature.WriteCSharp(writer, CSharpFeatureTextTypes.Interface, CSharpExports.None, IsLocal, ref IsFirstFeature, ref IsMultiline);
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
            writer.WriteEmptyLine();
        }

        private bool IsDirectFeature(ICSharpFeature feature)
        {
            if (feature.Owner == this && feature.Instance.PrecursorList.Count == 0)
                return true;
            else
                return false;
        }

        private void WriteGenericWhereClause(ICSharpWriter writer, ICSharpGeneric generic)
        {
            string GenericName = CSharpNames.ToCSharpIdentifier(generic.Name);
            string InterfaceGenericName = $"I{GenericName}";

            string CopyConstraint = CopyConstraintAsString(generic);

            string ParentConstraint = ParentConstraintAsString(writer, generic, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string InterfaceParentConstraint = ParentConstraintAsString(writer, generic, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

            string Constraints = string.Empty;
            string InterfaceConstraints = string.Empty;

            if (ParentConstraint.Length == 0)
                Constraints = CopyConstraint;
            else
                Constraints = ParentConstraint;

            if (generic.IsUsedToCreate)
            {
                if (Constraints.Length > 0)
                    Constraints += ", ";

                Constraints += "new()";
            }

            InterfaceConstraints = CopyConstraint;
            if (InterfaceConstraints.Length > 0)
                InterfaceConstraints += ", ";
            InterfaceConstraints += InterfaceParentConstraint;

            if (InterfaceConstraints.Length == 0)
                return;

            writer.IncreaseIndent();
            writer.WriteIndentedLine($"where {InterfaceGenericName} : {InterfaceConstraints}");
            writer.WriteIndentedLine($"where {GenericName} : {Constraints}, {InterfaceGenericName}");
            writer.DecreaseIndent();
        }

        private string CopyConstraintAsString(ICSharpGeneric generic)
        {
            return string.Empty;
        }

        private string ParentConstraintAsString(ICSharpUsingCollection usingCollection, ICSharpGeneric generic, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            string Result = string.Empty;

            foreach (ICSharpConstraint Constraint in generic.ConstraintList)
            {
                ICSharpType ConstraintType = Constraint.Type;

                if (ConstraintType is ICSharpClassType || ConstraintType is ICSharpFormalGenericType)
                {
                    if (Result.Length > 0)
                        Result += ", ";

                    Result += Constraint.Type.Type2CSharpString(usingCollection, cSharpTypeFormat, cSharpNamespaceFormat);
                }
            }

            return Result;
        }

        private void WriteClassContract(ICSharpWriter writer)
        {
            writer.WriteIndentedLine("// Invariant:");

            foreach (ICSharpAssertion Invariant in InvariantList)
            {
                string Tag = Invariant.Tag != null ? $"{Invariant.Tag}: " : string.Empty;
                string Line = Invariant.BooleanExpression.Source.ExpressionToString;
                writer.WriteIndentedLine($"//   {Tag}{Line}");
            }
        }

        private void WriteClassImplementation(ICSharpWriter writer)
        {
            writer.WriteDocumentation(Source);

            string InterfaceName = FullClassName2CSharpClassName(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string ClassName = FullClassName2CSharpClassName(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string SingletonName = BasicClassName2CSharpClassName(writer, CSharpTypeFormats.AsSingleton, CSharpNamespaceFormats.None);

            bool IsAbstract = Source.IsAbstract;

            foreach (ICSharpFeature Feature in FeatureList)
            {
                IFeatureInstance Instance = Feature.Instance;
                if (Instance.IsForgotten)
                {
                    IsAbstract = true;
                    break;
                }
            }

            if (!IsAbstract && Source.Cloneable == BaseNode.CloneableStatus.Cloneable)
                writer.WriteIndentedLine("[System.Serializable]");

            string IsAbstractText = IsAbstract ? "abstract " : string.Empty;
            string InterfaceDeclarationLine = $"public {IsAbstractText}class {ClassName}";
            IList<string> InheritanceLineList = new List<string>();

            if (BaseClass != null)
            {
                string ParentClassName = BaseClass.FullClassName2CSharpClassName(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
                InheritanceLineList.Add(ParentClassName);
            }

            if (Source.Cloneable != BaseNode.CloneableStatus.Single)
                InheritanceLineList.Add(InterfaceName);

            if (Type.Source.IsUsedInCloneOf)
                InheritanceLineList.Add("ICloneable");

            string InheritanceLineString = string.Empty;
            foreach (string s in InheritanceLineList)
            {
                if (InheritanceLineString.Length > 0)
                    InheritanceLineString += ", ";

                InheritanceLineString += s;
            }

            if (InheritanceLineString.Length > 0)
                InterfaceDeclarationLine += $" : {InheritanceLineString}";

            writer.WriteIndentedLine(InterfaceDeclarationLine);

            foreach (ICSharpGeneric Generic in GenericList)
                WriteGenericWhereClause(writer, Generic);

            string InitRegion = "Init";
            string PropertiesRegion = "Properties";
            string ClientInterfaceRegion = "Client Interface";
            string DescendantInterfaceRegion = "Descendant Interface";
            string ImplementationRegion = "Implementation";

            List<string> OrderedRegionList = new List<string>();
            OrderedRegionList.Add(InitRegion);
            OrderedRegionList.Add(PropertiesRegion);
            OrderedRegionList.Add(ClientInterfaceRegion);
            OrderedRegionList.Add(DescendantInterfaceRegion);
            OrderedRegionList.Add(ImplementationRegion);

            ISealableDictionary<string, ISealableDictionary<string, ICSharpFeature>> RegionTable = new SealableDictionary<string, ISealableDictionary<string, ICSharpFeature>>();
            foreach (string Region in OrderedRegionList)
                RegionTable.Add(Region, new SealableDictionary<string, ICSharpFeature>());

            ICSharpCreationFeature ConstructorOverride = null;

            foreach (ICSharpFeature Feature in InheritedFeatureList)
            {
                IFeatureInstance Instance = Feature.Instance;

                if (Feature is ICSharpCreationFeature AsCreationFeature)
                    if (AsCreationFeature.Owner.ClassConstructorType == CSharpConstructorTypes.OneConstructor)
                    {
                        ICreationFeature Constructor = (ICreationFeature)Instance.Feature;
                        foreach (ICommandOverload Overload in Constructor.OverloadList)
                            if (Overload.ParameterList.Count > 0)
                            {
                                ConstructorOverride = AsCreationFeature;
                                break;
                            }
                    }
            }

            foreach (ICSharpFeature Feature in FeatureList)
            {
                ICSharpClass Owner = Feature.Owner;
                CSharpExports ExportStatus = Feature.GetExportStatus(this);
                bool IsLocal = Owner == this;

                string BelongingRegion;
                if (IsLocal)
                {
                    if (Feature is ICSharpCreationFeature)
                        BelongingRegion = InitRegion;

                    else if (Feature is ICSharpPropertyFeature)
                        BelongingRegion = PropertiesRegion;

                    else if (ExportStatus == CSharpExports.Public)
                        BelongingRegion = ClientInterfaceRegion;

                    else if (ExportStatus == CSharpExports.Protected)
                        BelongingRegion = DescendantInterfaceRegion;

                    else
                        BelongingRegion = ImplementationRegion;
                }
                else
                    BelongingRegion = "Implementation of " + Owner.ValidClassName;

                if (!OrderedRegionList.Contains(BelongingRegion))
                {
                    OrderedRegionList.Add(BelongingRegion);
                    RegionTable.Add(BelongingRegion, new SealableDictionary<string, ICSharpFeature>());
                }

                ISealableDictionary<string, ICSharpFeature> Region = RegionTable[BelongingRegion];

                if (Feature is ICSharpFeatureWithName AsFeatureWithName)
                    Region.Add(AsFeatureWithName.Name, Feature);
            }

            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            bool IsFirstRegion = true;
            bool IsFirstFeature = true;
            bool IsMultiline = false;
            foreach (string BelongingRegion in OrderedRegionList)
            {
                int WrittenFeatures = 0;
                ISealableDictionary<string, ICSharpFeature> Region = RegionTable[BelongingRegion];
                if (Region.Count > 0 || (BelongingRegion == InitRegion && ConstructorOverride != null))
                {
                    if (IsFirstRegion)
                        IsFirstRegion = false;
                    else
                        writer.WriteEmptyLine();

                    writer.WriteIndentedLine("#region" + " " + BelongingRegion);

                    IsFirstFeature = true;
                    IsMultiline = false;

                    if ((BelongingRegion == InitRegion && ConstructorOverride != null) && Region.Count == 0)
                    {
                        string NameString = BasicClassName2CSharpClassName(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

                        foreach (ICSharpCommandOverload Item in ConstructorOverride.OverloadList)
                        {
                            string ParameterEntityList, ParameterNameList;
                            CSharpArgument.BuildParameterList(writer, Item.ParameterList, out ParameterEntityList, out ParameterNameList);

                            if (IsMultiline)
                                writer.WriteEmptyLine();

                            writer.WriteIndentedLine(CSharpNames.ComposedExportStatus(false, false, true, CSharpExports.Public) + $" {NameString}({ParameterEntityList})");
                            writer.IncreaseIndent();
                            writer.WriteIndentedLine($": base({ParameterNameList})");
                            writer.DecreaseIndent();
                            writer.WriteIndentedLine("{");
                            writer.WriteIndentedLine("}");
                            IsMultiline = true;
                        }

                        WrittenFeatures++;
                    }

                    foreach (KeyValuePair<string, ICSharpFeature> Entry in Region)
                    {
                        ICSharpFeature Feature = Entry.Value;
                        IFeatureInstance Instance = Feature.Instance;

                        ICompiledFeature SourceFeature = Feature.Source;
                        ICSharpClass Owner = Feature.Owner;
                        CSharpExports ExportStatus = Feature.GetExportStatus(Owner);
                        bool IsLocal = Owner == this;

                        if (SourceFeature is IFeatureWithPrecursor AsFeatureWithPrecursor && AsFeatureWithPrecursor.IsCallingPrecursor)
                        {
                            bool IsFromParent = false;
                            ICSharpClass CurrentClass = this;
                            while (CurrentClass.BaseClass != null)
                            {
                                ICSharpClass ParentClass = CurrentClass.BaseClass;

                                foreach (IPrecursorInstance PrecursorItem in Instance.PrecursorList)
                                    if (PrecursorItem.Precursor.Owner == ParentClass.Source)
                                    {
                                        IsFromParent = true;
                                        break;
                                    }

                                if (IsFromParent)
                                    break;
                                else
                                    CurrentClass = ParentClass;
                            }

                            if (!IsFromParent)
                                Feature.MarkPrecursorAsCoexisting(Entry.Key);
                        }

                        Feature.WriteCSharp(writer, CSharpFeatureTextTypes.Implementation, ExportStatus, IsLocal, ref IsFirstFeature, ref IsMultiline);
                        WrittenFeatures++;

                        if (!string.IsNullOrEmpty(Feature.CoexistingPrecursorName) && Instance.PrecursorList.Count > 0)
                        {
                            IPrecursorInstance PrecursorItem = Instance.PrecursorList[0];
                            IFeatureInstance CoexistingPrecursor = PrecursorItem.Precursor;

                            ICompiledFeature SourcePrecursorFeature = CoexistingPrecursor.Feature;
                            IClass SourcePrecursorClass = CoexistingPrecursor.Owner;
                            CSharpExports PrecursorExportStatus = CSharpExports.Private;

                            ICSharpFeature PrecursorFeature = null;
                            PrecursorFeature.WriteCSharp(writer, CSharpFeatureTextTypes.Implementation, PrecursorExportStatus, false, ref IsFirstFeature, ref IsMultiline);
                            WrittenFeatures++;
                        }
                    }
                }

                if (BelongingRegion == InitRegion)
                {
                    if (IsParameterizedSingleton)
                    {
                        if (WrittenFeatures > 0)
                            writer.WriteEmptyLine();
                        else
                            writer.WriteIndentedLine("#region" + " " + BelongingRegion);

                        writer.WriteIndentedLine("public" + " " + "static" + " " + ClassName + " " + "Singleton");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("get");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("string" + " " + "Key" + " " + "=" + " " + "\"\"" + ";");
                        foreach (ICSharpGeneric Generic in GenericList)
                            writer.WriteIndentedLine("Key" + " " + "+=" + " " + "\"" + "*" + "\"" + " " + "+" + " " + "typeof" + "(" + Generic.Name + ")" + "." + "FullName" + ";");
                        writer.WriteEmptyLine();
                        writer.WriteIndentedLine("if" + " " + "(" + "!" + SingletonName + "." + "SingletonSet" + "." + "ContainsKey" + "(" + "Key" + ")" + ")");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine(ClassName + " " + "NewEntity" + " " + "=" + " " + "new" + " " + ClassName + "(" + ")" + ";");
                        writer.WriteIndentedLine(SingletonName + "." + "SingletonSet" + "." + "Add" + "(" + "Key" + ", " + "NewEntity" + ")" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.WriteEmptyLine();
                        writer.WriteIndentedLine("return" + " " + "(" + ClassName + ")" + SingletonName + "." + "SingletonSet" + "[" + "Key" + "]" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");

                        IsFirstRegion = false;
                        WrittenFeatures++;
                    }

                    else if (IsUnparameterizedSingleton)
                    {
                        if (WrittenFeatures > 0)
                            writer.WriteEmptyLine();
                        else
                            writer.WriteIndentedLine("#region" + " " + BelongingRegion);

                        writer.WriteIndentedLine("static" + " " + ClassName + "(" + ")");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("_Singleton" + " " + "=" + " " + "new" + " " + "OnceReference" + "<" + ClassName + ">" + "(" + ")" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.WriteEmptyLine();

                        writer.WriteIndentedLine("public" + " " + "static" + " " + ClassName + " " + "Singleton");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("get");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("if" + " " + "(" + "!" + "_Singleton" + "." + "IsAssigned" + ")");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("_Singleton" + "." + "Item" + " " + "=" + " " + "new" + " " + ClassName + "(" + ")" + ";");
                        writer.DecreaseIndent();
                        writer.WriteEmptyLine();
                        writer.WriteIndentedLine("return" + " " + "_Singleton" + "." + "Item" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.WriteIndentedLine("private" + " " + "static" + " " + "OnceReference" + "<" + ClassName + ">" + " " + "_Singleton" + ";");

                        IsFirstRegion = false;
                        WrittenFeatures++;
                    }
                }

                if (WrittenFeatures > 0)
                    writer.WriteIndentedLine("#endregion");
            }

            if (Type.Source.IsUsedInCloneOf)
            {
                if (IsMultiline)
                    writer.WriteEmptyLine();

                WriteClonableImplementation(writer, out IsMultiline);
            }

            if (InvariantList.Count > 0)
            {
                if (IsMultiline)
                    writer.WriteEmptyLine();

                WriteClassInvariant(writer, out IsMultiline);
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }

        private void WriteClonableImplementation(ICSharpWriter writer, out bool isMultiline)
        {
            isMultiline = true;

            writer.WriteIndentedLine("#region Implementation of ICloneable");
            writer.WriteIndentedLine("object Clone()");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine("return Easly.DeepCopy(this);");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
            writer.WriteIndentedLine("#endregion");
        }

        private void WriteClassInvariant(ICSharpWriter writer, out bool isMultiline)
        {
            isMultiline = true;

            bool HasPrecursorInvariant = BaseClass != null && BaseClass.HasCheckInvariant;
            string ExportStatus = HasPrecursorInvariant ? "override" : "virtual";

            writer.WriteIndentedLine("#region Invariant");
            writer.WriteIndentedLine($"protected {ExportStatus} void CheckInvariant()");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            if (HasPrecursorInvariant)
            {
                writer.WriteIndentedLine("base.CheckInvariant();");
                writer.WriteEmptyLine();
            }

            foreach (ICSharpAssertion Invariant in InvariantList)
                Invariant.WriteCSharp(writer);

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
            writer.WriteIndentedLine("#endregion");
        }
        #endregion

        #region Naming
        /// <summary>
        /// Gets the full name of a class.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        public string FullClassName2CSharpClassName(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            return BasicClassName2CSharpClassName(usingCollection, cSharpTypeFormat, cSharpNamespaceFormat) + Generics2CSharpName(usingCollection);
        }

        /// <summary>
        /// Gets the name of a class.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        public string BasicClassName2CSharpClassName(ICSharpUsingCollection usingCollection, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            string ClassNamespace;
            string ClassOrInterfaceName;

            if (SplitUsingClause(ValidClassName, out string UsingClause, out string NakedClassName))
            {
                ClassNamespace = string.Empty;
                ClassOrInterfaceName = NakedClassName;
            }
            else
            {
                ClassNamespace = CSharpNames.ToCSharpIdentifier(ValidSourceName);
                ClassOrInterfaceName = CSharpNames.ToCSharpIdentifier(ValidClassName);
            }

            bool IsHandled = false;

            switch (cSharpTypeFormat)
            {
                case CSharpTypeFormats.Normal:
                    IsHandled = true;
                    break;

                case CSharpTypeFormats.AsInterface:
                    if (ValidSourceName != "Microsoft .NET")
                        ClassOrInterfaceName = "I" + ClassOrInterfaceName;
                    IsHandled = true;
                    break;

                case CSharpTypeFormats.AsSingleton:
                    ClassOrInterfaceName = ClassOrInterfaceName + "Internal";
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            if ((ClassNamespace.Length > 0 && ClassNamespace != usingCollection.DefaultNamespace) || cSharpNamespaceFormat.HasFlag(CSharpNamespaceFormats.FullNamespace))
                if (cSharpNamespaceFormat.HasFlag(CSharpNamespaceFormats.OneWord))
                    return ClassNamespace + ClassOrInterfaceName;
                else
                    return ClassNamespace + "." + ClassOrInterfaceName;
            else
                return ClassOrInterfaceName;
        }

        /// <summary>
        /// Gets elements of a using clause from a class name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="usingClause">The using clause.</param>
        /// <param name="nakedClassName">The class name to use if the using clause is set.</param>
        public static bool SplitUsingClause(string className, out string usingClause, out string nakedClassName)
        {
            usingClause = null;
            nakedClassName = null;

            int DotIndex = className.LastIndexOf('.');
            if (DotIndex < 1)
                return false;

            usingClause = className.Substring(0, DotIndex);
            nakedClassName = className.Substring(DotIndex + 1);
            return true;
        }

        /// <summary>
        /// Gets the string corresponding to the enumeration of C# generic parameters.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        private string Generics2CSharpName(ICSharpUsingCollection usingCollection)
        {
            string GenericNames = string.Empty;

            foreach (ICSharpGeneric Generic in GenericList)
            {
                if (GenericNames.Length > 0)
                    GenericNames += ", ";

                ICSharpFormalGenericType GenericType = Generic.Type;
                GenericNames += GenericType.Type2CSharpString(usingCollection, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                GenericNames += ", ";
                GenericNames += GenericType.Type2CSharpString(usingCollection, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            }

            if (GenericNames.Length > 0)
                GenericNames = "<" + GenericNames + ">";

            return GenericNames;
        }
        #endregion

        #region Debugging
        /// <summary></summary>
        public override string ToString()
        {
            return Source.ToString();
        }
        #endregion
    }
}
