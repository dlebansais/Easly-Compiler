namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# constant.
    /// </summary>
    public interface ICSharpConstantFeature : ICSharpFeature<IConstantFeature>, ICSharpFeatureWithName
    {
        /// <summary>
        /// The Easly node from which the C# node is created.
        /// </summary>
        new IConstantFeature Source { get; }

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
        /// The constant type.
        /// </summary>
        ICSharpType Type { get; }

        /// <summary>
        /// The constant value as an expression.
        /// </summary>
        ICSharpExpression ConstantExpression { get; }
    }

    /// <summary>
    /// A C# constant.
    /// </summary>
    public class CSharpConstantFeature : CSharpFeature<IConstantFeature>, ICSharpConstantFeature
    {
        #region Init
        /// <summary>
        /// Create a new C# constant.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        public static ICSharpConstantFeature Create(ICSharpClass owner, IFeatureInstance instance, IConstantFeature source)
        {
            return new CSharpConstantFeature(owner, instance, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConstantFeature"/> class.
        /// </summary>
        /// <param name="owner">The class where the feature is declared.</param>
        /// <param name="instance">The source feature instance.</param>
        /// <param name="source">The source Easly feature.</param>
        protected CSharpConstantFeature(ICSharpClass owner, IFeatureInstance instance, IConstantFeature source)
            : base(owner, instance, source)
        {
            Name = Source.ValidFeatureName.Item.Name;
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
        /// The constant type.
        /// </summary>
        public ICSharpType Type { get; private set; }

        /// <summary>
        /// The constant value as an expression.
        /// </summary>
        public ICSharpExpression ConstantExpression { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Initializes the feature overloads and bodies.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public override void InitOverloadsAndBodies(ICSharpContext context)
        {
            Type = CSharpType.Create(context, Source.ResolvedEntityType.Item);
            ConstantExpression = CSharpExpression.Create(context, (IExpression)Source.ConstantValue);
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
        /// <param name="featureTextType">The write mode.</param>
        /// <param name="exportStatus">The feature export status.</param>
        /// <param name="isLocal">True if the feature is local to the class.</param>
        /// <param name="isFirstFeature">True if the feature is the first in a list.</param>
        /// <param name="isMultiline">True if there is a separating line above.</param>
        public override void WriteCSharp(ICSharpWriter writer, CSharpFeatureTextTypes featureTextType, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            if (!WriteDown)
                return;

            if (isMultiline)
                writer.WriteEmptyLine();

            isFirstFeature = false;
            isMultiline = false;

            if (featureTextType == CSharpFeatureTextTypes.Implementation)
                WriteCSharpImplementation(writer, exportStatus, isLocal, ref isFirstFeature, ref isMultiline);
        }

        private void WriteCSharpImplementation(ICSharpWriter writer, CSharpExports exportStatus, bool isLocal, ref bool isFirstFeature, ref bool isMultiline)
        {
            writer.WriteDocumentation(Source);

            string TypeString = Type.Type2CSharpString(writer, CSharpTypeFormats.AsInterface, CSharpNamespaceFormats.None);
            string AttributeString = CSharpNames.ToCSharpIdentifier(Name);

            ICSharpExpressionAsConstant ExpressionAsConstant = ConstantExpression as ICSharpExpressionAsConstant;
            Debug.Assert(ExpressionAsConstant != null);

            string ValueString;

            if (ExpressionAsConstant.IsDirectConstant)
            {
                ICSharpExpressionContext SourceExpressionContext = new CSharpExpressionContext();
                ConstantExpression.WriteCSharp(writer, SourceExpressionContext, -1);

                ValueString = SourceExpressionContext.ReturnValue;
                Debug.Assert(ValueString != null);
            }
            else
            {
                ICSharpComputableExpression CompilableExpression = ConstantExpression as ICSharpComputableExpression;
                Debug.Assert(CompilableExpression != null);

                CompilableExpression.Compute(writer);
                ValueString = CompilableExpression.ComputedValue;
                Debug.Assert(ValueString != null);

                writer.WriteIndentedLine($"// {ValueString} = {ConstantExpression.Source.ExpressionToString}");
            }

            string ExportStatusText = CSharpNames.ComposedExportStatus(false, false, true, exportStatus);

            bool IsReadOnlyObject;
            if (Type is ICSharpClassType AsClassType)
            {
                Guid ClassGuid = AsClassType.Source.BaseClass.ClassGuid;
                IsReadOnlyObject = ClassGuid != LanguageClasses.Number.Guid && ClassGuid != LanguageClasses.Boolean.Guid && ClassGuid != LanguageClasses.String.Guid && ClassGuid != LanguageClasses.Character.Guid;
            }
            else
                IsReadOnlyObject = true;

            string ConstString = IsReadOnlyObject ? "static readonly" : "const";

            writer.WriteIndentedLine($"{ExportStatusText} {ConstString} {TypeString} {AttributeString} = {ValueString};");
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

            ConstantExpression.SetWriteDown();

            Owner.SetWriteDown();
        }
        #endregion
    }
}
