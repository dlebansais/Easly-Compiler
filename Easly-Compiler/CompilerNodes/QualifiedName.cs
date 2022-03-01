﻿namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IQualifiedName.
    /// </summary>
    public interface IQualifiedName : INode, ISource
    {
        /// <summary>
        /// Gets or sets the list of feature identifiers to follow to reach the destination feature.
        /// </summary>
        IList<BaseNode.Identifier> Path { get; }

        /// <summary>
        /// The valid value of <see cref="BaseNode.IQualifiedName.Path"/>.
        /// </summary>
        OnceReference<IList<IIdentifier>> ValidPath { get; }

        /// <summary>
        /// Result types of features along the path.
        /// </summary>
        OnceReference<IList<IExpressionType>> ValidResultTypePath { get; }

        /// <summary>
        /// Raw path as string.
        /// </summary>
        string PathToString { get; }
    }

    /// <summary>
    /// Compiler IQualifiedName.
    /// </summary>
    public class QualifiedName : BaseNode.QualifiedName, IQualifiedName
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedName"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public QualifiedName()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedName"/> class.
        /// </summary>
        /// <param name="source">The pre-constructed source.</param>
        /// <param name="sourceType">The qualified name resolved type.</param>
        public QualifiedName(BaseNode.QualifiedName source, IExpressionType sourceType)
        {
            Debug.Assert(source.Path.Count == 1);

            BaseNode.Identifier Identifier = source.Path[0];
            IIdentifier NewIdentifier = new Identifier(Identifier.Text);

            ValidPath.Item = new List<IIdentifier>();
            ValidPath.Item.Add(NewIdentifier);
            ValidResultTypePath.Item = new List<IExpressionType>();
            ValidResultTypePath.Item.Add(sourceType);
        }
        #endregion

        #region Implementation of ISource
        /// <summary>
        /// The parent node, null if root.
        /// </summary>
        public ISource ParentSource { get; private set; }

        /// <summary>
        /// The parent class, null if none.
        /// </summary>
        public IClass EmbeddingClass { get; private set; }

        /// <summary>
        /// The parent feature, null if none.
        /// </summary>
        public IFeature EmbeddingFeature { get; private set; }

        /// <summary>
        /// The parent overload, null if none.
        /// </summary>
        public IOverload EmbeddingOverload { get; private set; }

        /// <summary>
        /// The parent body, null if none.
        /// </summary>
        public IBody EmbeddingBody { get; private set; }

        /// <summary>
        /// The parent assertion, null if none.
        /// </summary>
        public IAssertion EmbeddingAssertion { get; private set; }

        /// <summary>
        /// Initializes parents based on the provided <paramref name="parentSource"/> node.
        /// </summary>
        /// <param name="parentSource">The parent node.</param>
        public virtual void InitializeSource(ISource parentSource)
        {
            ParentSource = parentSource;

            EmbeddingClass = parentSource is IClass AsClass ? AsClass : parentSource?.EmbeddingClass;
            EmbeddingFeature = parentSource is IFeature AsFeature ? AsFeature : parentSource?.EmbeddingFeature;
            EmbeddingOverload = parentSource is IOverload AsOverload ? AsOverload : parentSource?.EmbeddingOverload;
            EmbeddingBody = parentSource is IBody AsBody ? AsBody : parentSource?.EmbeddingBody;
            EmbeddingAssertion = parentSource is IAssertion AsAssertion ? AsAssertion : parentSource?.EmbeddingAssertion;
        }

        /// <summary>
        /// Reset some intermediate results.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to reset.</param>
        public void Reset(IRuleTemplateList ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                ValidPath = new OnceReference<IList<IIdentifier>>();
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                ValidResultTypePath = new OnceReference<IList<IExpressionType>>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IRuleTemplateList ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers)
            {
                IsResolved = ValidPath.IsAssigned;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Contract)
            {
                IsResolved = ValidResultTypePath.IsAssigned;
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// The valid value of <see cref="BaseNode.IQualifiedName.Path"/>.
        /// </summary>
        public OnceReference<IList<IIdentifier>> ValidPath { get; private set; } = new OnceReference<IList<IIdentifier>>();

        /// <summary>
        /// Result types of features along the path.
        /// </summary>
        public OnceReference<IList<IExpressionType>> ValidResultTypePath { get; private set; } = new OnceReference<IList<IExpressionType>>();

        /// <summary>
        /// Compares two qualified names.
        /// </summary>
        /// <param name="qualifiedName1">The first qualified name.</param>
        /// <param name="qualifiedName2">The second qualified name.</param>
        public static bool IsQualifiedNameEqual(IQualifiedName qualifiedName1, IQualifiedName qualifiedName2)
        {
            bool Result = true;

            Result &= qualifiedName1.Path.Count == qualifiedName2.Path.Count;

            for (int i = 0; i < qualifiedName1.Path.Count && i < qualifiedName2.Path.Count; i++)
            {
                IIdentifier Path1 = (IIdentifier)qualifiedName1.Path[i];
                IIdentifier Path2 = (IIdentifier)qualifiedName2.Path[i];
                Result &= Path1.ValidText.Item != Path2.ValidText.Item;
            }

            return Result;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Raw path as string.
        /// </summary>
        public string PathToString
        {
            get
            {
                string Result = Path[0].Text;
                for (int i = 1; i < Path.Count; i++)
                    Result += $".{Path[i].Text}";

                return Result;
            }
        }

        /// <summary>
        /// Gets a string representation of a list of qualified names.
        /// </summary>
        /// <param name="qualifiedNameList">The list of qualified names.</param>
        public static string QualifiedNameListToString(IEnumerable qualifiedNameList)
        {
            string Result = string.Empty;

            foreach (IQualifiedName QualifiedName in qualifiedNameList)
            {
                if (Result.Length > 0)
                    Result += ", ";
                Result += QualifiedName.PathToString;
            }

            return Result;
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Qualified Name '{PathToString}'";
        }
        #endregion
    }
}
