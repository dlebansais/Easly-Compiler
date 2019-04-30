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
        OnceReference<ICompiledType> ReferencedType2 { get; }
    }

    /// <summary>
    /// Compiler-only ITypedefType.
    /// </summary>
    public class TypedefType : BaseNode.ObjectType, ITypedefType
    {
        #region Compiler
        /// <summary>
        /// Resolved type name of the source.
        /// </summary>
        public OnceReference<ITypeName> ReferencedTypeName { get; private set; } = new OnceReference<ITypeName>();

        /// <summary>
        /// Resolved type of the source.
        /// </summary>
        public OnceReference<ICompiledType> ReferencedType2 { get; private set; } = new OnceReference<ICompiledType>();
        #endregion

        #region Debugging
        /// <summary>
        /// Gets a string representation of the expression.
        /// </summary>
        public string TypeToString
        {
            get
            {
                string NameString = ReferencedTypeName.IsAssigned ? $" {ReferencedTypeName.Item.Name}" : string.Empty;
                string TypeString = ReferencedType2.IsAssigned && ReferencedType2.Item is IObjectType AsObjectType ? $" = {AsObjectType.TypeToString}" : string.Empty;
                return $"typedef{NameString}{TypeString}";
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
