namespace EaslyCompiler
{
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
        /// The context for creating nodes.
        /// </summary>
        ICSharpContext Context { get; }

        /// <summary>
        /// The list of using clauses.
        /// </summary>
        IList<IUsingClause> UsingClauseList { get; }

        /// <summary>
        /// The table of implicit delegates.
        /// </summary>
        IHashtableEx<ITypeName, ICompiledType> DelegateTable { get; }

        /// <summary>
        /// The table of explicit delegates.
        /// </summary>
        IHashtableEx<ITypeName, ICompiledType> TypedefDelegateTable { get; }

        /// <summary>
        /// True if the class shares its name with another from a different 'From' source.
        /// </summary>
        bool IsSharedName { get; }

        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="baseClass">The base class.</param>
        /// <param name="classTable">The table of all classes.</param>
        void SetBaseClass(ICSharpClass baseClass, IDictionary<IClass, ICSharpClass> classTable);

        /// <summary>
        /// Sets the list of class features.
        /// </summary>
        /// <param name="featureList">The list of features.</param>
        void SetFeatureList(IList<ICSharpFeature> featureList);

        /// <summary>
        /// Sets the <see cref="Context"/> property.
        /// </summary>
        /// <param name="context">The context.</param>
        void SetContext(ICSharpContext context);

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

            foreach (IGeneric Item in source.GenericList)
            {
                ICSharpGeneric NewGeneric = CSharpGeneric.Create(Item);
                GenericList.Add(NewGeneric);
            }

            foreach (IInheritance Item in source.InheritanceList)
            {
                ICSharpInheritance NewInheritance = CSharpInheritance.Create(Item);
                InheritanceList.Add(NewInheritance);
            }

            foreach (ITypedef Item in source.TypedefList)
            {
                ICSharpTypedef NewTypedef = CSharpTypedef.Create(Item, this);
                TypedefList.Add(NewTypedef);
            }

            Type = CSharpClassType.Create(source.ResolvedClassType.Item);
            Type.SetClass(this);
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
        public ICSharpClassType Type { get; }

        /// <summary>
        /// The base class. Can be null if none.
        /// </summary>
        public ICSharpClass BaseClass { get; private set; }

        /// <summary>
        /// The list of class features.
        /// </summary>
        public IList<ICSharpFeature> FeatureList { get; private set; }

        /// <summary>
        /// The context for creating nodes.
        /// </summary>
        public ICSharpContext Context { get; private set; }

        /// <summary>
        /// The list of using clauses.
        /// </summary>
        public IList<IUsingClause> UsingClauseList { get; } = new List<IUsingClause>();

        /// <summary>
        /// The table of implicit delegates.
        /// </summary>
        public IHashtableEx<ITypeName, ICompiledType> DelegateTable { get; } = new HashtableEx<ITypeName, ICompiledType>();

        /// <summary>
        /// The table of explicit delegates.
        /// </summary>
        public IHashtableEx<ITypeName, ICompiledType> TypedefDelegateTable { get; } = new HashtableEx<ITypeName, ICompiledType>();

        /// <summary>
        /// True if the class shares its name with another from a different 'From' source.
        /// </summary>
        public bool IsSharedName { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the base class.
        /// </summary>
        /// <param name="baseClass">The base class.</param>
        /// <param name="classTable">The table of all classes.</param>
        public void SetBaseClass(ICSharpClass baseClass, IDictionary<IClass, ICSharpClass> classTable)
        {
            Debug.Assert(baseClass != null);
            Debug.Assert(BaseClass == null);
            Debug.Assert(Context == null);

            BaseClass = baseClass;

            foreach (ICSharpInheritance Inheritance in InheritanceList)
            {
                ICSharpClass AncestorClass = classTable[Inheritance.Source.ResolvedClassParentType.Item.BaseClass];
                Inheritance.SetAncestorClass(AncestorClass);
            }
        }

        /// <summary>
        /// Sets the list of class features.
        /// </summary>
        /// <param name="featureList">The list of features.</param>
        public void SetFeatureList(IList<ICSharpFeature> featureList)
        {
            Debug.Assert(featureList != null);
            Debug.Assert(FeatureList == null);
            Debug.Assert(Context == null);

            FeatureList = featureList;
        }

        /// <summary>
        /// Sets the <see cref="Context"/> property.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SetContext(ICSharpContext context)
        {
            Debug.Assert(context != null);
            Debug.Assert(Context == null);

            Context = context;

            foreach (IDiscrete Item in Source.DiscreteList)
            {
                ICSharpDiscrete NewDiscrete = CSharpDiscrete.Create(Item, context);
                DiscreteList.Add(NewDiscrete);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsSharedName"/> property.
        /// </summary>
        public void SetIsSharedName()
        {
            Debug.Assert(Context != null);

            IsSharedName = true;
        }

        /// <summary>
        /// Find features that are overrides and mark them as such.
        /// </summary>
        public void CheckOverrides()
        {
            Debug.Assert(Context != null);

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
            Debug.Assert(Context != null);

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
                ICompiledFeature SourceFeature = Item.Precursor.Feature.Item;
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
                ICompiledFeature SourceFeature = Item.Precursor.Feature.Item;
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
            Debug.Assert(Context != null);

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
            Debug.Assert(Context != null);

            foreach (ICSharpFeature Feature in FeatureList)
                if (Feature is ICSharpPropertyFeature AsPropertyFeature)
                    AsPropertyFeature.CheckInheritSideBySideAttribute(globalFeatureTable);
        }

        /// <summary>
        /// Creates delegates needed by the class.
        /// </summary>
        public void CreateDelegates()
        {
            Debug.Assert(Context != null);

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
                ICompiledType ReferencedType = Entry.Value.ReferencedType.Item;

                switch (ReferencedType)
                {
                    case IFunctionType AsFunctionType:
                        TypedefDelegateTable.Add(AsFunctionType.ResolvedTypeName.Item, AsFunctionType.ResolvedType.Item);
                        break;

                    case IProcedureType AsProcedureType:
                        TypedefDelegateTable.Add(AsProcedureType.ResolvedTypeName.Item, AsProcedureType.ResolvedType.Item);
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
            Debug.Assert(Context != null);

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
            catch
            {
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

            string UsingClause;
            string NakedClassName;
            if (CSharpRootOutput.SplitUsingClause(ValidClassName, out UsingClause, out NakedClassName))
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
                    if (Source.FromIdentifier.Item.Text != "Microsoft .NET")
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
