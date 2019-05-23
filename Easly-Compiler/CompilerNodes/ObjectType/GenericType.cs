namespace CompilerNode
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler IGenericType.
    /// </summary>
    public interface IGenericType : BaseNode.IGenericType, IObjectType, INodeWithReplicatedBlocks
    {
        /// <summary>
        /// Replicated list from <see cref="BaseNode.GenericType.TypeArgumentBlocks"/>.
        /// </summary>
        IList<ITypeArgument> TypeArgumentList { get; }

        /// <summary>
        /// Base class of the generic type.
        /// </summary>
        OnceReference<IClass> BaseClass { get; }

        /// <summary>
        /// The style of type arguments.
        /// </summary>
        TypeArgumentStyles ArgumentStyle { get; }

        /// <summary>
        /// Table of argument identifiers for assignment.
        /// </summary>
        IHashtableEx<string, IIdentifier> ArgumentIdentifierTable { get; }

        /// <summary>
        /// Resolved types for arguments.
        /// </summary>
        OnceReference<IHashtableEx<string, ICompiledType>> ResolvedTypeArgumentTable { get; }

        /// <summary>
        /// Resolved locations for arguments.
        /// </summary>
        OnceReference<IHashtableEx<string, IObjectType>> ResolvedArgumentLocationTable { get; }

        /// <summary>
        /// Sets the style of type arguments.
        /// </summary>
        /// <param name="argumentStyle">The style of type arguments.</param>
        void SetArgumentStyle(TypeArgumentStyles argumentStyle);
    }

    /// <summary>
    /// Compiler IGenericType.
    /// </summary>
    public class GenericType : BaseNode.GenericType, IGenericType
    {
        #region Implementation of INodeWithReplicatedBlocks
        /// <summary>
        /// Replicated list from <see cref="BaseNode.GenericType.TypeArgumentBlocks"/>.
        /// </summary>
        public IList<ITypeArgument> TypeArgumentList { get; } = new List<ITypeArgument>();

        /// <summary>
        /// Fills lists with the result of replication.
        /// </summary>
        /// <param name="propertyTypeArgument">The property name of the block.</param>
        /// <param name="nodeList">The node list.</param>
        public void FillReplicatedList(string propertyTypeArgument, List<BaseNode.INode> nodeList)
        {
            IList TargetList = null;

            switch (propertyTypeArgument)
            {
                case nameof(TypeArgumentBlocks):
                    TargetList = (IList)TypeArgumentList;
                    break;
            }

            Debug.Assert(TargetList != null);
            Debug.Assert(TargetList.Count == 0);

            foreach (BaseNode.INode Node in nodeList)
                TargetList.Add(Node);
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
        public void Reset(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                BaseClass = new OnceReference<IClass>();
                ArgumentStyle = (TypeArgumentStyles)(-1);
                ResolvedTypeName = new OnceReference<ITypeName>();
                ResolvedType = new OnceReference<ICompiledType>();
                ArgumentIdentifierTable = new HashtableEx<string, IIdentifier>();
                ResolvedTypeArgumentTable = new OnceReference<IHashtableEx<string, ICompiledType>>();
                ResolvedArgumentLocationTable = new OnceReference<IHashtableEx<string, IObjectType>>();
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Checks if a rule is resolved for this source.
        /// </summary>
        /// <param name="ruleTemplateList">The list of rule templates that would read the properties to check.</param>
        public virtual bool IsResolved(IList<IRuleTemplate> ruleTemplateList)
        {
            bool IsResolved = false;

            bool IsHandled = false;

            if (ruleTemplateList == RuleTemplateSet.Identifiers || ruleTemplateList == RuleTemplateSet.Contract || ruleTemplateList == RuleTemplateSet.Body)
            {
                IsResolved = false;
                IsHandled = true;
            }
            else if (ruleTemplateList == RuleTemplateSet.Types)
            {
                Debug.Assert(ResolvedTypeName.IsAssigned == ResolvedType.IsAssigned);
                IsResolved = ResolvedType.IsAssigned;
                Debug.Assert(ResolvedArgumentLocationTable.IsAssigned == IsResolved);
                Debug.Assert(ResolvedTypeArgumentTable.IsAssigned == IsResolved);
                Debug.Assert(BaseClass.IsAssigned == IsResolved);
                IsHandled = true;
            }

            Debug.Assert(IsHandled);
            return IsResolved;
        }
        #endregion

        #region Implementation of IObjectType
        /// <summary>
        /// The resolved type name.
        /// </summary>
        public OnceReference<ITypeName> ResolvedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// The resolved type.
        /// </summary>
        public OnceReference<ICompiledType> ResolvedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Compiler
        /// <summary>
        /// Base class of the generic type.
        /// </summary>
        public OnceReference<IClass> BaseClass { get; private set; } = new OnceReference<IClass>();

        /// <summary>
        /// The style of type arguments.
        /// </summary>
        public TypeArgumentStyles ArgumentStyle { get; private set; } = (TypeArgumentStyles)(-1);

        /// <summary>
        /// Table of argument identifiers for assignment.
        /// </summary>
        public IHashtableEx<string, IIdentifier> ArgumentIdentifierTable { get; private set; } = new HashtableEx<string, IIdentifier>();

        /// <summary>
        /// Resolved types for arguments.
        /// </summary>
        public OnceReference<IHashtableEx<string, ICompiledType>> ResolvedTypeArgumentTable { get; private set; } = new OnceReference<IHashtableEx<string, ICompiledType>>();

        /// <summary>
        /// Resolved locations for arguments.
        /// </summary>
        public OnceReference<IHashtableEx<string, IObjectType>> ResolvedArgumentLocationTable { get; private set; } = new OnceReference<IHashtableEx<string, IObjectType>>();

        /// <summary>
        /// Sets the style of type arguments.
        /// </summary>
        /// <param name="argumentStyle">The style of type arguments.</param>
        public void SetArgumentStyle(TypeArgumentStyles argumentStyle)
        {
            Debug.Assert(ArgumentStyle == (TypeArgumentStyles)(-1));
            Debug.Assert(argumentStyle >= TypeArgumentStyles.None);

            ArgumentStyle = argumentStyle;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString { get { return $"{ClassIdentifier.Text}[{TypeArgument.TypeArgumentListToString(TypeArgumentList)}]"; } }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Generic Type '{TypeToString}'";
        }
        #endregion
    }
}
