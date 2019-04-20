namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using BaseNodeHelper;
    using Easly;

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
        /// Recursively build a path of properties from a base node to the final property.
        /// </summary>
        /// <param name="type">The current object type.</param>
        /// <param name="path">The remaining path to parse.</param>
        /// <param name="propertyPath">Accumulated properties in the path so far.</param>
        public static void BuildPropertyPath(Type type, string path, IList<PropertyInfo> propertyPath)
        {
            if (path.Length == 0)
                return;

            int Index = path.IndexOf(InferenceEngine.Dot);
            int ThisPathIndex = (Index >= 0) ? Index : path.Length;
            string PropertyName = path.Substring(0, ThisPathIndex);
            int NextPathIndex = (Index >= 0) ? Index + 1 : path.Length;
            string NextPath = path.Substring(NextPathIndex);

            if (type.GetInterface(nameof(IOnceReference)) != null)
            {
                Type[] GenericArguments = type.GetGenericArguments();
                Debug.Assert(GenericArguments.Length == 1);
                type = GenericArguments[0];
            }

            PropertyInfo Property = NodeTreeHelper.GetPropertyOf(type, PropertyName);
            Debug.Assert(Property != null);

            propertyPath.Add(Property);

            Type NestedType = ToCompilerType(Property.PropertyType);
            BuildPropertyPath(NestedType, NextPath, propertyPath);
        }

        /// <summary>
        /// Gets the current value at the end of a property path.
        /// </summary>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TValue">Type of the result.</typeparam>
        /// <param name="source">The node for which the value is requested.</param>
        /// <param name="startingPoint">The starting point to use.</param>
        /// <param name="propertyPath">Path from the starting point to the value to read.</param>
        /// <param name="isInterrupted">True is progressing through the path was interrupted.</param>
        public static TValue GetPropertyPathValue<TSource, TValue>(TSource source, ITemplatePathStart<TSource> startingPoint, IReadOnlyList<PropertyInfo> propertyPath, out bool isInterrupted)
            where TSource : ISource
        {
            isInterrupted = true;
            object IntermediateResult = startingPoint.GetStart(source);

            for (int i = 0; i < propertyPath.Count; i++)
            {
                if (IntermediateResult is IOnceReference AsOnceReference && !AsOnceReference.IsAssigned)
                    return default;

                IntermediateResult = propertyPath[i].GetValue(IntermediateResult);
            }

            TValue Result = default;

            try
            {
                Result = (TValue)IntermediateResult;
                isInterrupted = false;
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }

            return Result;
        }

        /// <summary>
        /// Converts a type that references a base node to the same type referencing a compiler node.
        /// </summary>
        /// <param name="nodeType">The type to convert.</param>
        private static Type ToCompilerType(Type nodeType)
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
