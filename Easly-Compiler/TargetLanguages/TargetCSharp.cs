namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BaseNodeHelper;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// An interface to write down C# source code from compiled Easly nodes.
    /// </summary>
    public interface ITargetCSharp : ITargetLanguage
    {
        /// <summary>
        /// Namespace for the output code.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// The table of classes.
        /// </summary>
        IDictionary<IClass, ICSharpClass> ClassTable { get; }
    }

    /// <summary>
    /// A class to write down C# source code from compiled Easly nodes.
    /// </summary>
    public class TargetCSharp : TargetLanguage, ITargetCSharp
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetCSharp"/> class.
        /// </summary>
        /// <param name="compiler">The compiler object to translate.</param>
        /// <param name="outputNamespace">Namespace for the output code.</param>
        public TargetCSharp(ICompiler compiler, string outputNamespace)
            : base(compiler)
        {
            Namespace = outputNamespace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Namespace for the output code.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// The table of classes.
        /// </summary>
        public IDictionary<IClass, ICSharpClass> ClassTable { get; } = new Dictionary<IClass, ICSharpClass>();
        #endregion

        #region Client Interface
        /// <summary>
        /// Translates nodes from the compiler to the target language.
        /// </summary>
        public override void Translate()
        {
            ErrorList.ClearErrors();
            ClassTable.Clear();

            if (!ClassSplitting.Create(Compiler.LoadedRoot.ClassList, ErrorList, out IClassSplitting Splitting))
                return;

            foreach (IClass Class in Compiler.LoadedRoot.ClassList)
                if (!IsClassFromLibrary(Class))
                {
                    ICSharpClass NewCSharpClass = CSharpClass.Create(Class);
                    ClassTable.Add(Class, NewCSharpClass);
                }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                if (GetBaseClass(Class, Splitting, out ICSharpClass BaseClass))
                    Class.SetBaseClass(BaseClass, ClassTable);
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                CheckRename(Class, ErrorList);
            }
            if (!ErrorList.IsEmpty)
                return;

            IDictionary<ICompiledFeature, ICSharpFeature> GlobalFeatureTable = new Dictionary<ICompiledFeature, ICSharpFeature>();

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                IList<ICSharpFeature> FeatureList = new List<ICSharpFeature>();
                CreateClassFeatures(Class, FeatureList);

                foreach (ICSharpFeature Feature in FeatureList)
                {
                    Debug.Assert(!GlobalFeatureTable.ContainsKey(Feature.Source));
                    GlobalFeatureTable.Add(Feature.Source, Feature);
                }
            }

            foreach (KeyValuePair<IClass, ICSharpClass> ClassEntry in ClassTable)
            {
                ICSharpClass Class = ClassEntry.Value;
                IList<ICSharpFeature> FeatureList = new List<ICSharpFeature>();
                IList<ICSharpFeature> InheritedFeatureList = new List<ICSharpFeature>();

                foreach (KeyValuePair<ICompiledFeature, ICSharpFeature> FeatureEntry in GlobalFeatureTable)
                {
                    ICSharpFeature Feature = FeatureEntry.Value;
                    IFeatureInstance Instance = Feature.Instance;

                    if (Instance.IsDiscontinued)
                        continue;

                    if (FeatureEntry.Value.Owner == Class)
                        FeatureList.Add(Feature);
                    else if (!IsDirectOrNotMainParentFeature(Instance, Class))
                        InheritedFeatureList.Add(Feature);
                }

                Class.SetFeatureList(FeatureList, InheritedFeatureList);
            }

            ICSharpContext Context = new CSharpContext(ClassTable, GlobalFeatureTable);

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.SetContext(Context);
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                CheckSharedName(Class);
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.CheckOverrides();
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.CheckOverrides();
            }

            bool Continue;
            do
            {
                Continue = false;

                foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
                {
                    ICSharpClass Class = Entry.Value;
                    Class.CheckForcedReadWrite(GlobalFeatureTable, ref Continue);
                }
            }
            while (Continue);

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.CheckSideBySideAttributes();
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.CheckInheritSideBySideAttributes(GlobalFeatureTable);
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.CreateDelegates();
            }

            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass Class = Entry.Value;
                Class.Write(OutputRootFolder, Namespace);
            }
        }

        private bool IsClassFromLibrary(IClass baseClass)
        {
            bool Result = false;
            ICollection<Guid> GuidValues = LanguageClasses.NameToGuid.Values;

            Result = GuidValues.Contains(baseClass.ClassGuid) || baseClass.HasExternBody;

            return Result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Finds the base class of a C# class.
        /// </summary>
        /// <param name="cSharpClass">The class for which to find a base class.</param>
        /// <param name="splitting">The class splitting.</param>
        /// <param name="result">The base class upon return successful.</param>
        private bool GetBaseClass(ICSharpClass cSharpClass, IClassSplitting splitting, out ICSharpClass result)
        {
            result = null;
            IClass BaseClass = null;

            // If the class inherits from a class that must be a parent, select it as base class.
            foreach (IInheritance InheritanceItem in cSharpClass.Source.InheritanceList)
            {
                IClassType ClassParentType = InheritanceItem.ResolvedClassParentType.Item;
                IClass ParentClass = ClassParentType.BaseClass;

                if (splitting.MustInherit.Contains(ParentClass))
                {
                    Debug.Assert(ParentClass == null);
                    BaseClass = ParentClass;
                }
            }

            // No such class. Try classes that don't have to be interface.
            if (BaseClass == null)
            {
                IList<IClass> ParentCandidates = new List<IClass>();

                foreach (IInheritance InheritanceItem in cSharpClass.Source.InheritanceList)
                {
                    IClassType ClassParentType = InheritanceItem.ResolvedClassParentType.Item;
                    IClass ParentClass = ClassParentType.BaseClass;

                    if (splitting.OtherParents.Contains(ParentClass))
                        ParentCandidates.Add(ParentClass);
                }

                if (ParentCandidates.Count > 0)
                    BaseClass = ParentCandidates[0];
            }

            if (BaseClass != null)
            {
                foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
                {
                    ICSharpClass Item = Entry.Value;

                    if (Item.Source == BaseClass)
                    {
                        Debug.Assert(result == null);
                        result = Item;
                    }
                }
            }

            return result != null;
        }

        private static bool CheckRename(ICSharpClass cSharpClass, IErrorList errorList)
        {
            bool Result = true;

            foreach (IInheritance InheritanceItem in cSharpClass.Source.InheritanceList)
            {
                IClassType ParentType = InheritanceItem.ResolvedClassParentType.Item;
                IClass ParentClass = ParentType.BaseClass;

                bool BadRename = false;
                foreach (IRename RenameItem in InheritanceItem.RenameList)
                {
                    string ValidSourceText = RenameItem.ValidSourceText.Item;

                    if (!FeatureName.TableContain(ParentClass.FeatureTable, ValidSourceText, out IFeatureName Key, out IFeatureInstance Instance))
                    {
                        BadRename = true;
                        break;
                    }

                    IClass SourceClass = Instance.Owner.Item;
                    ICompiledFeature SourceFeature = Instance.Feature.Item;
                    CSharpExports ExportStatus = GetExportStatus(Key, SourceClass, cSharpClass.Source.ExportTable, (IFeature)SourceFeature);
                    if (ExportStatus == CSharpExports.Public && !(SourceFeature is ICreationFeature))
                    {
                        BadRename = true;
                        break;
                    }
                }

                if (BadRename)
                    errorList.AddError(new ErrorInvalidRename(InheritanceItem));

                Result &= !BadRename;
            }

            return Result;
        }

        private static CSharpExports GetExportStatus(IFeatureName name, IClass sourceClass, IHashtableEx<IFeatureName, IHashtableEx<string, IClass>> exportTable, IFeature sourceFeature)
        {
            bool IsExportedToClient;

            string FeatureExport = sourceFeature.ExportIdentifier.Text;
            if (FeatureExport == "All")
                IsExportedToClient = true;

            else if (FeatureExport == "None" || FeatureExport == "Self")
                IsExportedToClient = false;

            else
            {
                bool IsExported = FeatureName.TableContain(exportTable, FeatureExport, out IFeatureName ExportName, out IHashtableEx<string, IClass> ExportList);
                Debug.Assert(IsExported);
                Debug.Assert(ExportList.Count > 0);

                if (ExportList.Count > 1)
                    IsExportedToClient = true;
                else
                {
                    if (ExportList.ContainsKey(sourceClass.ValidClassName))
                        IsExportedToClient = false; // Export to self = self + descendant = protected
                    else
                        IsExportedToClient = true; // export to another = export to all = public
                }
            }

            if (IsExportedToClient)
                return CSharpExports.Public;

            else if (sourceFeature.Export == BaseNode.ExportStatus.Exported)
                return CSharpExports.Protected;
            else
                return CSharpExports.Private;
        }

        private void CheckSharedName(ICSharpClass cSharpClass)
        {
            foreach (KeyValuePair<IClass, ICSharpClass> Entry in ClassTable)
            {
                ICSharpClass OtherClass = Entry.Value;

                if (cSharpClass.ValidClassName == OtherClass.ValidClassName)
                {
                    cSharpClass.SetIsSharedName();
                    break;
                }
            }
        }

        private static void CreateClassFeatures(ICSharpClass cSharpClass, IList<ICSharpFeature> featureList)
        {
            foreach (KeyValuePair<IFeatureName, IFeatureInstance> Entry in cSharpClass.Source.FeatureTable)
            {
                IFeatureInstance Instance = Entry.Value;
                Debug.Assert(Instance.Feature.IsAssigned && Instance.Owner.IsAssigned);

                // Only create features that belong to this class directly, or are not inherited from a base class.
                if (!IsDirectOrNotMainParentFeature(Instance, cSharpClass))
                    continue;

                bool IsCreated = CreateFeature(cSharpClass, Instance, out ICSharpFeature NewFeature);
                Debug.Assert(IsCreated);

                featureList.Add(NewFeature);
            }
        }

        /// <summary>
        /// Creates a C# feature from an Easly feature.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="result">The created feature upon return.</param>
        private static bool CreateFeature(ICSharpClass owner, IFeatureInstance instance, out ICSharpFeature result)
        {
            result = null;
            bool IsHandled = false;

            ICompiledFeature SourceFeature = instance.Feature.Item;

            switch (SourceFeature)
            {
                case IAttributeFeature AsAttributeFeature:
                    result = CSharpAttributeFeature.Create(owner, instance, AsAttributeFeature);
                    IsHandled = true;
                    break;

                case IConstantFeature AsConstantFeature:
                    result = CSharpConstantFeature.Create(owner, instance, AsConstantFeature);
                    IsHandled = true;
                    break;

                case ICreationFeature AsCreationFeature:
                    result = CSharpCreationFeature.Create(owner, instance, AsCreationFeature);
                    IsHandled = true;
                    break;

                case IFunctionFeature AsFunctionFeature:
                    result = CSharpFunctionFeature.Create(owner, instance, AsFunctionFeature);
                    IsHandled = true;
                    break;

                case IIndexerFeature AsIndexerFeature:
                    result = CSharpIndexerFeature.Create(owner, instance, AsIndexerFeature);
                    IsHandled = true;
                    break;

                case IProcedureFeature AsProcedureFeature:
                    result = CSharpProcedureFeature.Create(owner, instance, AsProcedureFeature);
                    IsHandled = true;
                    break;

                case IPropertyFeature AsPropertyFeature:
                    result = CSharpPropertyFeature.Create(owner, instance, AsPropertyFeature);
                    IsHandled = true;
                    break;

                case IScopeAttributeFeature AsScopeAttributeFeature:
                    result = CSharpScopeAttributeFeature.Create(owner, AsScopeAttributeFeature);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);

            return result != null;
        }

        private static bool IsDirectOrNotMainParentFeature(IFeatureInstance instance, ICSharpClass cSharpClass)
        {
            IClass Owner = instance.Owner.Item;

            // Feature directly implemented in the class?
            if (Owner == cSharpClass.Source)
                return true;

            while (cSharpClass.BaseClass != null)
            {
                cSharpClass = cSharpClass.BaseClass;

                // Feature implemented in one of the base classes?
                if (Owner == cSharpClass.Source)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
