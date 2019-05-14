namespace EaslyCompiler
{
    using System;
    using System.Diagnostics;
    using CompilerNode;
    using Easly;

    /// <summary>
    /// The distant constant as a starting point.
    /// </summary>
    public interface ITemplateConstantStart : ITemplatePathStart<IClassConstantExpression>
    {
    }

    /// <summary>
    /// The distant constant as a starting point.
    /// </summary>
    public class TemplateConstantStart : ITemplateConstantStart
    {
        #region Init
        static TemplateConstantStart()
        {
            // Create a fake constant that can be reached but is otherwise impossible. Any failed attempt to find a constant will fall back to that one.
            DefaultConstant = new NeutralExpression();
            DefaultConstant.SetExpressionConstant(new AgentLanguageConstant());
        }

        /// <summary>
        /// An instance that can be used in any source template.
        /// </summary>
        public static TemplateConstantStart Default { get; } = new TemplateConstantStart();

        private TemplateConstantStart()
        {
        }

        private static INeutralExpression DefaultConstant { get; }
        #endregion

        #region Properties
        /// <summary>
        /// The type of the starting point.
        /// </summary>
        public Type PropertyType { get { return typeof(IExpression); } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the starting point.
        /// </summary>
        /// <param name="source">The node for which a value is requested.</param>
        public virtual ISource GetStart(IClassConstantExpression source)
        {
            ISource ConstantSource = DefaultConstant;

            IIdentifier ClassIdentifier = (IIdentifier)source.ClassIdentifier;
            IIdentifier ConstantIdentifier = (IIdentifier)source.ConstantIdentifier;
            IClass EmbeddingClass = source.EmbeddingClass;
            string ValidClassText = ClassIdentifier.ValidText.Item;
            string ValidConstantText = ConstantIdentifier.ValidText.Item;

            IHashtableEx<string, IImportedClass> ClassTable = EmbeddingClass.ImportedClassTable;
            if (ClassTable.ContainsKey(ValidClassText))
            {
                IClass BaseClass = ClassTable[ValidClassText].Item;
                if (FeatureName.TableContain(BaseClass.FeatureTable, ValidConstantText, out IFeatureName Key, out IFeatureInstance FeatureInstance))
                {
                    if (FeatureInstance.Feature.Item is IConstantFeature AsConstantFeature)
                        ConstantSource = (IExpression)AsConstantFeature.ConstantValue;
                }
            }

            Debug.Assert(ConstantSource is IExpression);

            return ConstantSource;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        public override string ToString()
        {
            return "/constant";
        }
        #endregion
    }
}
