﻿namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
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
        /// <param name="outputNamespace">Namespace for the output code.</param>
        void Write(string folder, string outputNamespace);

        /// <summary>
        /// Gets the full name of a class.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        string FullClassName2CSharpClassName(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);

        /// <summary>
        /// Gets the name of a class.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        string BasicClassName2CSharpClassName(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat);
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
                IClassType BaseType;
                IClass BaseClass;
                ITypeName ResolvedTypeName;
                ICompiledType ResolvedType;

                switch (Item.Value)
                {
                    case IFunctionType AsFunctionType:
                        BaseType = AsFunctionType.ResolvedBaseType.Item;
                        BaseClass = BaseType.BaseClass;

                        if (!FunctionType.TypeTableContaining(DelegateTable, BaseType, AsFunctionType.OverloadList, out ResolvedTypeName, out ResolvedType))
                            DelegateTable.Add(Item.Key, Item.Value);
                        break;

                    case IProcedureType AsProcedureType:
                        BaseType = AsProcedureType.ResolvedBaseType.Item;
                        BaseClass = BaseType.BaseClass;

                        if (!ProcedureType.TypeTableContaining(DelegateTable, BaseType, AsProcedureType.OverloadList, out ResolvedTypeName, out ResolvedType))
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
        /// <param name="folder">The output root folder.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public void Write(string folder, string outputNamespace)
        {
            string OutputFolder;
            if (Source.FromIdentifier.IsAssigned)
            {
                IIdentifier FromIdentifier = (IIdentifier)Source.FromIdentifier.Item;
                OutputFolder = Path.Combine(folder, FromIdentifier.ValidText.Item);
            }
            else
                OutputFolder = folder;

            try
            {
                if (!Directory.Exists(OutputFolder))
                    Directory.CreateDirectory(OutputFolder);

                string FileName = Path.Combine(OutputFolder, $"{ValidClassName}.cs");

                using (FileStream Stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (ICSharpWriter Writer = new CSharpWriter(Stream))
                    {
                        Write(Writer, outputNamespace);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }
        #endregion

        #region Implementation
        private void Write(ICSharpWriter writer, string outputNamespace)
        {
            if (Source.IsEnumeration)
                WriteEnum(writer, outputNamespace);
            else
                WriteClass(writer, outputNamespace);
        }

        private void WriteEnum(ICSharpWriter writer, string outputNamespace)
        {
            writer.WriteIndentedLine($"namespace {outputNamespace}");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            string ClassName = FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

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
                    Line += " " + "=" + " " + ExplicitValue.CSharpText(outputNamespace);
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

        private void WriteClass(ICSharpWriter writer, string outputNamespace)
        {
            writer.WriteIndentedLine("namespace " + outputNamespace);
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();

            if (IsParameterizedSingleton)
                WriteParameterizedSingletonClass(writer, outputNamespace);
            else
                WriteClassInterface(writer, outputNamespace);

            WriteClassImplementation(writer, outputNamespace);

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
        }

        private void WriteParameterizedSingletonClass(ICSharpWriter writer, string outputNamespace)
        {
            string ClassName = BasicClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.AsSingleton, CSharpNamespaceFormats.None);

            writer.WriteIndentedLine($"class {ClassName}");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine($"static  {ClassName}()");
            writer.WriteIndentedLine("{");
            writer.IncreaseIndent();
            writer.WriteIndentedLine("SingletonSet = new Hashtable<string, object>();");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            writer.WriteLine();
            writer.WriteIndentedLine("public static Hashtable<string, object> SingletonSet;");
            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");

            writer.WriteLine();
        }

        private void WriteClassInterface(ICSharpWriter writer, string outputNamespace)
        {
            string InterfaceName = FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

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

                    OtherInterfaceNames += OtherInterface.FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                }

                InterfaceDeclarationLine += $" : {OtherInterfaceNames}";
            }

            writer.WriteIndentedLine(InterfaceDeclarationLine);

            foreach (ICSharpGeneric Generic in GenericList)
                WriteGenericWhereClause(writer, outputNamespace, Generic);

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
                Feature.WriteCSharp(writer, outputNamespace, CSharpFeatureTextTypes.Interface, CSharpExports.None, IsLocal, ref IsFirstFeature, ref IsMultiline);
            }

            writer.DecreaseIndent();
            writer.WriteIndentedLine("}");
            writer.WriteLine();
        }

        private bool IsDirectFeature(ICSharpFeature feature)
        {
            if (feature.Owner == this && feature.Instance.PrecursorList.Count == 0)
                return true;
            else
                return false;
        }

        private void WriteGenericWhereClause(ICSharpWriter writer, string outputNamespace, ICSharpGeneric generic)
        {
            string GenericName = CSharpNames.ToCSharpIdentifier(generic.Name);
            string InterfaceGenericName = $"I{GenericName}";

            string CopyConstraint = CopyConstraintAsString(generic);

            string ParentConstraint = ParentConstraintAsString(generic, outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string InterfaceParentConstraint = ParentConstraintAsString(generic, outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);

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

        private string ParentConstraintAsString(ICSharpGeneric generic, string outputNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            string Result = string.Empty;

            foreach (ICSharpConstraint Constraint in generic.ConstraintList)
            {
                ICSharpType ConstraintType = Constraint.Type;

                if (ConstraintType is ICSharpClassType || ConstraintType is ICSharpFormalGenericType)
                {
                    if (Result.Length > 0)
                        Result += ", ";

                    Result += Constraint.Type.Type2CSharpString(outputNamespace, cSharpTypeFormat, cSharpNamespaceFormat);
                }
            }

            return Result;
        }

        private void WriteClassImplementation(ICSharpWriter writer, string outputNamespace)
        {
            writer.WriteDocumentation(Source);

            string InterfaceName = FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string ClassName = FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
            string SingletonName = BasicClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.AsSingleton, CSharpNamespaceFormats.None);

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
                string ParentClassName = BaseClass.FullClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
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
                WriteGenericWhereClause(writer, outputNamespace, Generic);

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
                        writer.WriteLine();

                    writer.WriteIndentedLine("#region" + " " + BelongingRegion);

                    IsFirstFeature = true;
                    IsMultiline = false;

                    if ((BelongingRegion == InitRegion && ConstructorOverride != null) && Region.Count == 0)
                    {
                        string NameString = BasicClassName2CSharpClassName(outputNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);

                        foreach (ICSharpCommandOverload Item in ConstructorOverride.OverloadList)
                        {
                            string ParameterEntityList, ParameterNameList;
                            CSharpArgument.BuildParameterList(Item.ParameterList, outputNamespace, out ParameterEntityList, out ParameterNameList);

                            if (IsMultiline)
                                writer.WriteLine();

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

                        Feature.WriteCSharp(writer, outputNamespace, CSharpFeatureTextTypes.Implementation, ExportStatus, IsLocal, ref IsFirstFeature, ref IsMultiline);
                        WrittenFeatures++;

                        if (!string.IsNullOrEmpty(Feature.CoexistingPrecursorName) && Instance.PrecursorList.Count > 0)
                        {
                            IPrecursorInstance PrecursorItem = Instance.PrecursorList[0];
                            IFeatureInstance CoexistingPrecursor = PrecursorItem.Precursor;

                            ICompiledFeature SourcePrecursorFeature = CoexistingPrecursor.Feature;
                            IClass SourcePrecursorClass = CoexistingPrecursor.Owner;
                            CSharpExports PrecursorExportStatus = CSharpExports.Private;

                            ICSharpFeature PrecursorFeature = null;
                            PrecursorFeature.WriteCSharp(writer, outputNamespace, CSharpFeatureTextTypes.Implementation, PrecursorExportStatus, false, ref IsFirstFeature, ref IsMultiline);
                            WrittenFeatures++;
                        }
                    }
                }

                if (BelongingRegion == InitRegion)
                {
                    if (IsParameterizedSingleton)
                    {
                        if (WrittenFeatures > 0)
                            writer.WriteLine();
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
                        writer.WriteLine();
                        writer.WriteIndentedLine("if" + " " + "(" + "!" + SingletonName + "." + "SingletonSet" + "." + "ContainsKey" + "(" + "Key" + ")" + ")");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine(ClassName + " " + "NewEntity" + " " + "=" + " " + "new" + " " + ClassName + "(" + ")" + ";");
                        writer.WriteIndentedLine(SingletonName + "." + "SingletonSet" + "." + "Add" + "(" + "Key" + ", " + "NewEntity" + ")" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.WriteLine();
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
                            writer.WriteLine();
                        else
                            writer.WriteIndentedLine("#region" + " " + BelongingRegion);

                        writer.WriteIndentedLine("static" + " " + ClassName + "(" + ")");
                        writer.WriteIndentedLine("{");
                        writer.IncreaseIndent();
                        writer.WriteIndentedLine("_Singleton" + " " + "=" + " " + "new" + " " + "OnceReference" + "<" + ClassName + ">" + "(" + ")" + ";");
                        writer.DecreaseIndent();
                        writer.WriteIndentedLine("}");
                        writer.WriteLine();

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
                        writer.WriteLine();
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
                    writer.WriteLine();

                WriteClonableImplementation(writer, out IsMultiline);
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
        #endregion

        #region Naming
        /// <summary>
        /// Gets the full name of a class.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        public string FullClassName2CSharpClassName(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
        {
            return BasicClassName2CSharpClassName(cSharpNamespace, cSharpTypeFormat, cSharpNamespaceFormat) + Generics2CSharpName(cSharpNamespace);
        }

        /// <summary>
        /// Gets the name of a class.
        /// </summary>
        /// <param name="cSharpNamespace">The current namespace.</param>
        /// <param name="cSharpTypeFormat">The type format to use.</param>
        /// <param name="cSharpNamespaceFormat">The namespace format to use.</param>
        public string BasicClassName2CSharpClassName(string cSharpNamespace, CSharpTypeFormats cSharpTypeFormat, CSharpNamespaceFormats cSharpNamespaceFormat)
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

            if ((ClassNamespace.Length > 0 && ClassNamespace != cSharpNamespace) || cSharpNamespaceFormat.HasFlag(CSharpNamespaceFormats.FullNamespace))
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
        /// <param name="cSharpNamespace">The current namespace.</param>
        private string Generics2CSharpName(string cSharpNamespace)
        {
            string GenericNames = string.Empty;

            foreach (ICSharpGeneric Generic in GenericList)
            {
                if (GenericNames.Length > 0)
                    GenericNames += ", ";

                ICSharpFormalGenericType GenericType = Generic.Type;
                GenericNames += GenericType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
                GenericNames += ", ";
                GenericNames += GenericType.Type2CSharpString(cSharpNamespace, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
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
