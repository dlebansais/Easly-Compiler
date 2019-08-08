namespace EaslyCompiler
{
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# local attribute.
    /// </summary>
    public interface ICSharpScopeAttributeFeature : ICSharpFeature<IScopeAttributeFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IScopeAttributeFeature Source { get; }

        /// <summary>
        /// The class where the feature is declared.
        /// </summary>
        new ICSharpClass Owner { get; }

        /// <summary>
        /// The source feature instance.
        /// </summary>
        new IFeatureInstance Instance { get; }

        /// <summary>
        /// True if this feature as an override of a virtual parent.
        /// </summary>
        new bool IsOverride { get; }

        /// <summary>
        /// The default value. Can be null.
        /// </summary>
        ICSharpExpression DefaultValue { get; }

        /// <summary>
        /// The attribute type.
        /// </summary>
        ICSharpType Type { get; }

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        void WriteCSharp(ICSharpWriter writer);
    }

    /// <summary>
    /// A C# local attribute.
    /// </summary>
    public class CSharpScopeAttributeFeature : CSharpFeature<IScopeAttributeFeature>, ICSharpScopeAttributeFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# local attribute.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpScopeAttributeFeature Create(ICSharpClass owner, IScopeAttributeFeature source)
        {
            return new CSharpScopeAttributeFeature(owner, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpScopeAttributeFeature(ICSharpClass owner, IScopeAttributeFeature source)
            : base(owner, source)
        {
            Name = Source.ValidFeatureName.Item.Name;
        }
        /// <summary>
        /// Create a new C# local attribute.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpScopeAttributeFeature Create(ICSharpContext context, ICSharpClass owner, IScopeAttributeFeature source)
        {
            return new CSharpScopeAttributeFeature(context, owner, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScopeAttributeFeature"/> class.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpScopeAttributeFeature(ICSharpContext context, ICSharpClass owner, IScopeAttributeFeature source)
            : this(owner, source)
        {
            InitOverloadsAndBodies(context);
            InitHierarchy(context);
        }
        #endregion

        #region Properties
        ICompiledFeature ICSharpFeature.Source { get { return Source; } }
        ICSharpClass ICSharpFeature.Owner { get { return Owner; } }
        IFeatureInstance ICSharpFeature.Instance { get { return Instance; } }
        bool ICSharpFeature.IsOverride { get { return IsOverride; } }

        /// <summary>
        /// The feature name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The default value. Can be null.
        /// </summary>
        public ICSharpExpression DefaultValue { get; private set; }

        /// <summary>
        /// The attribute type.
        /// </summary>
        public ICSharpType Type { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            Type = CSharpType.Create(context, Source.ResolvedEffectiveType.Item);

            if (Source.DefaultValue.IsAssigned)
                DefaultValue = CSharpExpression.Create(context, Source.DefaultValue.Item);
        }

        /// <summary>
        /// Initializes the feature precursor hierarchy.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitHierarchy(ICSharpContext context)
        {
        }

        /// <summary>
        /// Gets the feature output format.
        /// </summary>
        /// <param name="selectedOverloadType">The selected overload type.</param>
        /// <param name="outgoingParameterCount">The number of 'out' parameters upon return.</param>
        /// <param name="returnValueIndex">Index of the return value if the feature returns a value, -1 otherwise.</param>
        public override void GetOutputFormat(ICSharpQueryOverloadType selectedOverloadType, out int outgoingParameterCount, out int returnValueIndex)
        {
            Debug.Assert(selectedOverloadType == null);

            outgoingParameterCount = 1;
            returnValueIndex = 0;
        }

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        public void WriteCSharp(ICSharpWriter writer)
        {
            Debug.Assert(WriteDown);

            string NameString = CSharpNames.ToCSharpIdentifier(Name);
            string TypeString = Type.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string DefaultValueText = string.Empty;

            if (DefaultValue != null)
            {
                ICSharpExpressionContext ExpressionContext = new CSharpExpressionContext();
                DefaultValue.WriteCSharp(writer, ExpressionContext, -1);

                DefaultValueText = ExpressionContext.ReturnValue;

                if (DefaultValue.IsComplex)
                    DefaultValueText = $"({DefaultValueText})";
            }

            if (DefaultValueText.Length == 0)
            {
                if (Type.GetSingletonString(writer, CSharpTypeFormats.Normal, CSharpNamespaceFormats.None, out string SingletonString))
                    DefaultValueText = SingletonString;
            }

            if (DefaultValueText.Length == 0)
                DefaultValueText = "default";

            DefaultValueText = $" = {DefaultValueText}";

            writer.WriteIndentedLine($"{TypeString} {NameString}{DefaultValueText};");
        }

        /// <summary>
        /// Writes down the C# feature.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public override void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            WriteCSharp(writer);
        }
        #endregion

        #region Implementation of ICSharpOutputNode
        /// <summary>
        /// Sets the <see cref="ICSharpOutputNode.WriteDown"/> flag.
        /// </summary>
        public override void SetWriteDown()
        {
            if (WriteDown)
                return;

            WriteDown = true;

            if (Owner != null)
                Owner.SetWriteDown();

            if (DefaultValue != null)
                DefaultValue.SetWriteDown();
        }
        #endregion
    }
}
