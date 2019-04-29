namespace CompilerNode
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Easly;
    using EaslyCompiler;

    /// <summary>
    /// Compiler-only ITypedefType.
    /// </summary>
    public interface ITypedefType : BaseNode.IObjectType
    {
        /// <summary>
        /// Resolved type name of the source.
        /// </summary>
        OnceReference<ITypeName> ReferencedTypeName { get; }

        /// <summary>
        /// Resolved type of the source.
        /// </summary>
        OnceReference<ICompiledType> ReferencedType { get; }
    }

    /// <summary>
    /// Compiler-only ITypedefType.
    /// </summary>
    public class TypedefType : BaseNode.ObjectType, ITypedefType
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedefType"/> class.
        /// This constructor is required for deserialization.
        /// </summary>
        public TypedefType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedefType"/> class.
        /// </summary>
        /// <param name="typeName">Resolved type name of the source.</param>
        /// <param name="type">Resolved type of the source.</param>
        public TypedefType(ITypeName typeName, ICompiledType type)
        {
            ReferencedTypeName.Item = typeName;
            ReferencedType.Item = type;
        }
        #endregion

        #region Compiler
        /// <summary>
        /// Resolved type name of the source.
        /// </summary>
        public OnceReference<ITypeName> ReferencedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type of the source.
        /// </summary>
        public OnceReference<ICompiledType> ReferencedType { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString
        {
            get
            {
                string TypeString = ReferencedType.Item is IObjectType AsObjectType ? $" = {AsObjectType.TypeToString}" : string.Empty;
                return $"typedef {ReferencedTypeName.Item.Name}{TypeString}";
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return $"Typedef Type '{TypeToString}'";
        }
        #endregion
    }
}
