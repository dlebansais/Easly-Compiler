namespace EaslyCompiler
{
    using System;
    using System.Collections;
    using System.Diagnostics;

    /// <summary>
    /// Helper class for source and destination templates.
    /// </summary>
    public static class TemplateHelper
    {
        #region Init
        static TemplateHelper()
        {
            string BaseTypeName = typeof(BaseNode.INode).FullName;
            BaseNamespace = BaseTypeName.Substring(0, BaseTypeName.IndexOf('.') + 1);
            string CompilerTypeName = typeof(CompilerNode.INode).FullName;
            CompilerNamespace = CompilerTypeName.Substring(0, CompilerTypeName.IndexOf('.') + 1);
        }

        private static string BaseNamespace;
        private static string CompilerNamespace;
        #endregion

        #region Client Interface
        /// <summary>
        /// Converts a type that references a base node to the same type referencing a compiler node.
        /// </summary>
        /// <param name="nodeType">The type to convert.</param>
        public static Type ToCompilerType(Type nodeType)
        {
            Type ResultType = nodeType;
            string ResultTypeName = ResultType.FullName;

            if (ResultType.IsGenericType)
            {
                string ListTypeName = typeof(System.Collections.Generic.IList<>).FullName;
                string OptionalReferenceTypeName = typeof(Easly.IOptionalReference<>).FullName;

                if (ResultTypeName.StartsWith(ListTypeName))
                {
                    Type[] GenericArguments = ResultType.GetGenericArguments();
                    Debug.Assert(GenericArguments.Length == 1);
                    Type Argument = ToCompilerType(GenericArguments[0]);
                    ResultType = typeof(System.Collections.Generic.IList<>).MakeGenericType(Argument);
                }
                else if (ResultTypeName.StartsWith(OptionalReferenceTypeName))
                {
                    Type[] GenericArguments = ResultType.GetGenericArguments();
                    Debug.Assert(GenericArguments.Length == 1);
                    Type Argument = ToCompilerType(GenericArguments[0]);
                    ResultType = typeof(Easly.IOptionalReference<>).MakeGenericType(Argument);
                }
            }
            else
            {
                string CompilerResultTypeName = ResultTypeName.Replace(BaseNamespace, CompilerNamespace);
                if (CompilerResultTypeName != ResultTypeName)
                {
                    ResultType = typeof(CompilerNode.INode).Assembly.GetType(CompilerResultTypeName);
                    Debug.Assert(ResultType != null);
                }
            }

            return ResultType;
        }
        #endregion
    }
}
