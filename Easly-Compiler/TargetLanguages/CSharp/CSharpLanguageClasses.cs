namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// C# built-in classes and types.
    /// </summary>
    internal static class CSharpLanguageClasses
    {
        public static Dictionary<Guid, string> GuidToName { get; } = new Dictionary<Guid, string>()
        {
            { LanguageClasses.Any.Guid, "object" },
            { LanguageClasses.AnyDetachableReference.Guid, "AnyDetachableReference" },
            { LanguageClasses.AnyOnceReference.Guid, "AnyOnceReference" },
            { LanguageClasses.AnyOptionalReference.Guid, "AnyOptionalReference" },
            { LanguageClasses.AnyReference.Guid, "object" },
            { LanguageClasses.AnyStableReference.Guid, "AnyStableReference" },
            { LanguageClasses.AnyValue.Guid, "object" },
            { LanguageClasses.AttributeEntity.Guid, "AttributeEntity" },
            { LanguageClasses.BitFieldEnumeration.Guid, "enum" },
            { LanguageClasses.Boolean.Guid, "bool" },
            { LanguageClasses.Character.Guid, "char" },
            { LanguageClasses.ConstantEntity.Guid, "ConstantEntity" },
            { LanguageClasses.DateAndTime.Guid, "DateTime" },
            { LanguageClasses.DetachableReference.Guid, "DetachableReference" },
            { LanguageClasses.Entity.Guid, "Entity" },
            { LanguageClasses.Enumeration.Guid, "enum" },
            { LanguageClasses.Event.Guid, "Event" },
            { LanguageClasses.Exception.Guid, "Exception" },
            { LanguageClasses.FeatureEntity.Guid, "FeatureEntity" },
            { LanguageClasses.FunctionEntity.Guid, "FunctionEntity" },
            { LanguageClasses.Hashtable.Guid, "Dictionary" },
            { LanguageClasses.IndexerEntity.Guid, "IndexerEntity" },
            { LanguageClasses.Integer.Guid, "int" },
            { LanguageClasses.KeyValuePair.Guid, "KeyValuePair" },
            { LanguageClasses.List.Guid, "List" },
            { LanguageClasses.LocalEntity.Guid, "LocalEntity" },
            { LanguageClasses.NamedFeatureEntity.Guid, "NamedFeatureEntity" },
            { LanguageClasses.Number.Guid, "double" },
            { LanguageClasses.OnceReference.Guid, "OnceReference" },
            { LanguageClasses.OptionalReference.Guid, "OptionalReference" },
            { LanguageClasses.OverLoopSource.Guid, "OverLoopSource" },
            { LanguageClasses.ProcedureEntity.Guid, "ProcedureEntity" },
            { LanguageClasses.PropertyEntity.Guid, "PropertyEntity" },
            { LanguageClasses.SealableHashtable.Guid, "SealableHashtable" },
            { LanguageClasses.SpecializedTypeEntity.Guid, "SpecializedTypeEntity" },
            { LanguageClasses.StableReference.Guid, "StableReference" },
            { LanguageClasses.String.Guid, "string" },
            { LanguageClasses.TypeEntity.Guid, "TypeEntity" },
            { LanguageClasses.UniversallyUniqueIdentifier.Guid, "Guid" },
        };

        public static Dictionary<string, string> NameUsingTable { get; } = new Dictionary<string, string>()
        {
            { "AttributeEntity", "Easly" },
            { "ConstantEntity", "Easly" },
            { "Entity", "Easly" },
            { "ProcedureEntity", "Easly" },
            { "Event", "Easly" },
            { "PropertyEntity", "Easly" },
            { "IndexerEntity", "Easly" },
            { "LocalEntity", "Easly" },
            { "OnceReference", "Easly" },
            { "OverLoopSource", "Easly" },
            { "AnyOnceReference", "Easly" },
            { "OptionalReference", "Easly" },
            { "DateTime", "System" },
            { "AnyDetachableReference", "Easly" },
            { "AnyOptionalReference", "Easly" },
            { "FeatureEntity", "Easly" },
            { "KeyValuePair", "System.Collections.Generic" },
            { "SpecializedTypeEntity", "Easly" },
            { "DetachableReference", "Easly" },
            { "NamedFeatureEntity", "Easly" },
            { "StableReference", "Easly" },
            { "AnyStableReference", "Easly" },
            { "Dictionary", "System.Collections.Generic" },
            { "List", "System.Collections.Generic" },
            { "Exception", "System" },
            { "SealableHashtable", "Easly" },
            { "TypeEntity", "Easly" },
            { "FunctionEntity", "Easly" },
            { "Guid", "System" },
        };
    }
}
