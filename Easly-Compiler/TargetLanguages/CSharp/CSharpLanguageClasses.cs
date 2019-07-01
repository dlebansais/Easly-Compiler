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
            { LanguageClasses.AnyReference.Guid, "object" },
            { LanguageClasses.AnyValue.Guid, "object" },
            { LanguageClasses.AttributeEntity.Guid, "AttributeEntity" },
            { LanguageClasses.Boolean.Guid, "bool" },
            { LanguageClasses.Character.Guid, "char" },
            { LanguageClasses.ConstantEntity.Guid, "ConstantEntity" },
            { LanguageClasses.Entity.Guid, "Entity" },
            { LanguageClasses.ProcedureEntity.Guid, "ProcedureEntity" },
            { LanguageClasses.Event.Guid, "Event" },
            { LanguageClasses.Number.Guid, "int" },
            { LanguageClasses.PropertyEntity.Guid, "PropertyEntity" },
            { LanguageClasses.IndexerEntity.Guid, "IndexerEntity" },
            { LanguageClasses.LocalEntity.Guid, "LocalEntity" },
            { LanguageClasses.OnceReference.Guid, "OnceReference" },
            { LanguageClasses.OverLoopSource.Guid, "OverLoopSource" },
            { LanguageClasses.AnyOnceReference.Guid, "AnyOnceReference" },
            { LanguageClasses.Enumeration.Guid, "enum" },
            { LanguageClasses.OptionalReference.Guid, "OptionalReference" },
            { LanguageClasses.String.Guid, "string" },
            { LanguageClasses.DateAndTime.Guid, "DateTime" },
            { LanguageClasses.AnyDetachableReference.Guid, "AnyDetachableReference" },
            { LanguageClasses.AnyOptionalReference.Guid, "AnyOptionalReference" },
            { LanguageClasses.FeatureEntity.Guid, "FeatureEntity" },
            { LanguageClasses.KeyValuePair.Guid, "KeyValuePair" },
            { LanguageClasses.SpecializedTypeEntity.Guid, "SpecializedTypeEntity" },
            { LanguageClasses.DetachableReference.Guid, "DetachableReference" },
            { LanguageClasses.NamedFeatureEntity.Guid, "NamedFeatureEntity" },
            { LanguageClasses.StableReference.Guid, "StableReference" },
            { LanguageClasses.AnyStableReference.Guid, "AnyStableReference" },
            { LanguageClasses.Hashtable.Guid, "Dictionary" },
            { LanguageClasses.List.Guid, "List" },
            { LanguageClasses.Exception.Guid, "Exception" },
            { LanguageClasses.SealableHashtable.Guid, "SealableHashtable" },
            { LanguageClasses.TypeEntity.Guid, "TypeEntity" },
            { LanguageClasses.BitFieldEnumeration.Guid, "enum" },
            { LanguageClasses.FunctionEntity.Guid, "FunctionEntity" },
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
