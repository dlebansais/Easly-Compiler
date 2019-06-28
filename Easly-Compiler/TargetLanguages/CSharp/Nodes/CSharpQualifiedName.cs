namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# qualified name.
    /// </summary>
    public interface ICSharpQualifiedName : ICSharpSource<IQualifiedName>
    {
        /// <summary>
        /// The feature called. Can be null.
        /// </summary>
        ICSharpFeature Feature { get; }

        /// <summary>
        /// The discrete read. Can be null.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// Inherit the side-by-side attribute.
        /// </summary>
        bool InheritBySideAttribute { get; }

        /// <summary>
        /// The list of classes involved along the path.
        /// </summary>
        IList<ICSharpClass> ClassPath { get; }

        /// <summary>
        /// True if the final feature is an attribute with ensure clauses.
        /// </summary>
        bool IsAttributeWithContract { get; }

        /// <summary>
        /// True if the qualified name is simple.
        /// </summary>
        bool IsSimple { get; }

        /// <summary>
        /// Gets the source code corresponding to the qualified name.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="skippedAtEnd">Number of identifiers to skip at the end.</param>
        string CSharpText(ICSharpUsingCollection usingCollection, int skippedAtEnd);

        /// <summary>
        /// Gets the source code corresponding to the qualified name.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="skippedAtEnd">Number of identifiers to skip at the end.</param>
        string DecoratedCSharpText(ICSharpUsingCollection usingCollection, int skippedAtEnd);

        /// <summary>
        /// Gets the source code corresponding to the qualified name setter.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        string CSharpSetter(ICSharpUsingCollection usingCollection);
    }

    /// <summary>
    /// A C# qualified name.
    /// </summary>
    public class CSharpQualifiedName : CSharpSource<IQualifiedName>, ICSharpQualifiedName
    {
        #region Init
        /// <summary>
        /// Create a new C# qualified name.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="feature">The feature at the end of the path. Can be null.</param>
        /// <param name="discrete">The discrete at the end of the path. Can be null.</param>
        /// <param name="inheritBySideAttribute">Inherit the side-by-side attribute.</param>
        public static ICSharpQualifiedName Create(ICSharpContext context, IQualifiedName source, ICSharpFeature feature, ICSharpDiscrete discrete, bool inheritBySideAttribute)
        {
            return new CSharpQualifiedName(context, source, feature, discrete, inheritBySideAttribute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpQualifiedName"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly node from which the C# node is created.</param>
        /// <param name="feature">The feature at the end of the path. Can be null.</param>
        /// <param name="discrete">The discrete at the end of the path. Can be null.</param>
        /// <param name="inheritBySideAttribute">Inherit the side-by-side attribute.</param>
        protected CSharpQualifiedName(ICSharpContext context, IQualifiedName source, ICSharpFeature feature, ICSharpDiscrete discrete, bool inheritBySideAttribute)
            : base(source)
        {
            Debug.Assert((feature != null && discrete == null) || (feature == null && discrete != null));

            Feature = feature;
            Discrete = discrete;
            InheritBySideAttribute = inheritBySideAttribute;
            IsAttributeWithContract = feature is ICSharpAttributeFeature AsAttributeFeature && AsAttributeFeature.Source.EnsureList.Count > 0;

            foreach (IExpressionType Item in source.ValidResultTypePath.Item)
            {
                ICSharpClass ItemClass;

                if (Item.ValueType is IClassType AsClassType)
                    ItemClass = context.GetClass(AsClassType.BaseClass);
                else
                    ItemClass = null;

                ClassPath.Add(ItemClass);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The feature called. Can be null.
        /// </summary>
        public ICSharpFeature Feature { get; }

        /// <summary>
        /// The discrete read. Can be null.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// Inherit the side-by-side attribute.
        /// </summary>
        public bool InheritBySideAttribute { get; }

        /// <summary>
        /// The list of classes involved along the path.
        /// </summary>
        public IList<ICSharpClass> ClassPath { get; } = new List<ICSharpClass>();

        /// <summary>
        /// True if the final feature is an attribute with ensure clauses.
        /// </summary>
        public bool IsAttributeWithContract { get; }

        /// <summary>
        /// True if the qualified name is simple.
        /// </summary>
        public bool IsSimple { get { return ClassPath.Count == 1; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the qualified name.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="skippedAtEnd">Number of identifiers to skip at the end.</param>
        public string CSharpText(ICSharpUsingCollection usingCollection, int skippedAtEnd)
        {
            string Result;
            int i = 0;

            /*
            if (Context.AttachmentAliasTable.ContainsKey(ValidPath.Item[i].ValidText.Item))
            {
                Result = Context.AttachmentAliasTable[ValidPath.Item[i].ValidText.Item];
                i++;
            }
            else*/
                Result = string.Empty;

            for (; i + skippedAtEnd < Source.ValidPath.Item.Count; i++)
            {
                if (Result.Length > 0)
                    Result += ".";

                IIdentifier Item = Source.ValidPath.Item[i];
                ICSharpClass ItemClass = i < ClassPath.Count ? ClassPath[i] : null;
                string ItemText = Item.ValidText.Item;

                if (i == 0 && usingCollection.AttachmentMap.ContainsKey(Item.ValidText.Item))
                    ItemText = usingCollection.AttachmentMap[ItemText];
                else
                    ItemText = CSharpNames.ToCSharpIdentifier(ItemText);

                if (ItemClass != null)
                    if (ItemClass.IsUnparameterizedSingleton && ItemClass.ValidSourceName != "Microsoft .NET")
                    {
                        string TypeText = ItemClass.Type.Type2CSharpString(usingCollection, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None);
                        ItemText = $"{TypeText}.Singleton";
                    }

                Result += ItemText;
            }

            return Result;
        }

        /// <summary>
        /// Gets the source code corresponding to the qualified name.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        /// <param name="skippedAtEnd">Number of identifiers to skip at the end.</param>
        public string DecoratedCSharpText(ICSharpUsingCollection usingCollection, int skippedAtEnd)
        {
            string Result = null;

            string QueryText = CSharpText(usingCollection, skippedAtEnd);

            if (Discrete != null)
                Result = QueryText;
            else
            {
                switch (Feature)
                {
                    case ICSharpPropertyFeature AsPropertyFeature:
                        if (AsPropertyFeature.HasSideBySideAttribute || InheritBySideAttribute)
                            Result = $"_{QueryText}";
                        else
                            Result = QueryText;
                        break;

                    case ICSharpAttributeFeature AsAttributeFeature:
                    case ICSharpConstantFeature AsConstantFeature:
                    case ICSharpScopeAttributeFeature AsScopeAttributeFeature:
                        Result = QueryText;
                        break;

                    case ICSharpFunctionFeature AsFunctionFeature:
                    case ICSharpCreationFeature AsCreationFeature:
                    case ICSharpProcedureFeature AsProcedureFeature:
                        Result = $"{QueryText}()";
                        break;
                }
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary>
        /// Gets the source code corresponding to the qualified name setter.
        /// </summary>
        /// <param name="usingCollection">The collection of using directives.</param>
        public string CSharpSetter(ICSharpUsingCollection usingCollection)
        {
            string StartText;

            if (Source.ValidPath.Item.Count > 1)
            {
                StartText = CSharpText(usingCollection, 1);
                StartText += ".";
            }
            else
                StartText = string.Empty;

            IIdentifier Item = Source.ValidPath.Item[0];
            string ItemText = CSharpNames.ToCSharpIdentifier(Item.ValidText.Item);
            string SetterText = $"{StartText}Set_{ItemText}";

            return SetterText;
        }
        #endregion
    }
}
