namespace EaslyCompiler
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CompilerNode;

    /// <summary>
    /// A C# expression.
    /// </summary>
    public interface ICSharpEntityExpression : ICSharpExpression
    {
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        new IEntityExpression Source { get; }

        /// <summary>
        /// The source feature for which an entity object is obtained. Can be null.
        /// </summary>
        ICSharpFeature Feature { get; }

        /// <summary>
        /// The source discrete for which an entity object is obtained. Can be null.
        /// </summary>
        ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The query.
        /// </summary>
        ICSharpQualifiedName Query { get; }
    }

    /// <summary>
    /// A C# expression.
    /// </summary>
    public class CSharpEntityExpression : CSharpExpression, ICSharpEntityExpression
    {
        #region Init
        /// <summary>
        /// Creates a new C# expression.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        public static ICSharpEntityExpression Create(ICSharpContext context, IEntityExpression source)
        {
            return new CSharpEntityExpression(context, source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpEntityExpression"/> class.
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <param name="source">The Easly expression from which the C# expression is created.</param>
        protected CSharpEntityExpression(ICSharpContext context, IEntityExpression source)
            : base(context, source)
        {
            if (Source.ResolvedFinalFeature.IsAssigned)
            {
                ICompiledFeature ResolvedFeature = Source.ResolvedFinalFeature.Item;
                if (ResolvedFeature is IScopeAttributeFeature AsScopeAttributeFeature)
                {
                    ICSharpClass Owner = context.GetClass(source.EmbeddingClass);
                    Feature = CSharpScopeAttributeFeature.Create(context, Owner, AsScopeAttributeFeature);
                }
                else
                    Feature = context.GetFeature(Source.ResolvedFinalFeature.Item);
            }

            if (Source.ResolvedFinalDiscrete.IsAssigned)
            {
                ICSharpClass Class = context.GetClass(Source.ResolvedFinalDiscrete.Item.EmbeddingClass);

                foreach (ICSharpDiscrete Item in Class.DiscreteList)
                    if (Item.Source == Source.ResolvedFinalDiscrete.Item)
                    {
                        Debug.Assert(Discrete == null);
                        Discrete = Item;
                    }
            }

            Debug.Assert((Feature != null && Discrete == null) || (Feature == null && Discrete != null));

            Query = CSharpQualifiedName.Create(context, (IQualifiedName)Source.Query, Feature, Discrete, false);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Easly expression from which the C# expression is created.
        /// </summary>
        public new IEntityExpression Source { get { return (IEntityExpression)base.Source; } }

        /// <summary>
        /// The source feature for which an entity object is obtained. Can be null.
        /// </summary>
        public ICSharpFeature Feature { get; }

        /// <summary>
        /// The source discrete for which an entity object is obtained. Can be null.
        /// </summary>
        public ICSharpDiscrete Discrete { get; }

        /// <summary>
        /// The query.
        /// </summary>
        public ICSharpQualifiedName Query { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the source code corresponding to the expression.
        /// </summary>
        /// <param name="writer">The stream on which to write.</param>
        /// <param name="expressionContext">The context.</param>
        /// <param name="isDeclaredInPlace">True if variables must be declared with their type.</param>
        /// <param name="skippedIndex">Index of a destination to skip.</param>
        public override void WriteCSharp(ICSharpWriter writer, ICSharpExpressionContext expressionContext, bool isDeclaredInPlace, int skippedIndex)
        {
            string QueryText = Query.CSharpText(writer, 0);
            string Result = null;

            if (Feature != null)
            {
                switch (Feature)
                {
                    case ICSharpAttributeFeature AsAttributeFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Property" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpConstantFeature AsConstantFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Property" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpCreationFeature AsCreationFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Procedure" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpFunctionFeature AsFunctionFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Function" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpProcedureFeature AsProcedureFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Procedure" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpPropertyFeature AsPropertyFeature:
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Property" + "(" + "\"" + QueryText + "\"" + ")";
                        break;

                    case ICSharpScopeAttributeFeature AsScopeAttributeFeature:
                        /*
                        string FeatureString;
                        IIndexerFeature ThisFeatureAsIndexer;
                        IFeatureWithName ThisFeatureWithName;

                        if ((ThisFeatureAsIndexer = EmbeddingFeature as IIndexerFeature) != null)
                            FeatureString = "null";

                        else if ((ThisFeatureWithName = EmbeddingFeature as IFeatureWithName) != null)
                            FeatureString = "\"" + CSharpRootOutput.ToCSharpIdentifier(ThisFeatureWithName.ValidFeatureName.Item.Name) + "\"";

                        else
                            throw new InvalidCastException();

                        Result = "LocalEntity" + "." + "FromThis" + "(" + "this" + "," + " " + FeatureString + "," + " " + "\"" + QueryText + "\"" + ")";
                        */
                        Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Property" + "(" + "\"" + QueryText + "\"" + ")";
                        break;
                }
            }

            else if (Discrete != null)
            {
                Result = "Entity" + "." + "FromThis" + "(" + "this" + ")" + "." + "Property" + "(" + "\"" + QueryText + "\"" + ")";
            }

            Debug.Assert(Result != null);

            expressionContext.SetSingleReturnValue(Result);
        }
        #endregion
    }
}
