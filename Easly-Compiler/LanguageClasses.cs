namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Info about language classes.
    /// </summary>
    internal static class LanguageClasses
    {
        public static readonly NameGuidPair Any = new NameGuidPair() { Guid = new Guid("c6297be4-e121-400f-ad78-79df3ecf2858"), Name = "Any" };
        public static readonly NameGuidPair AnyReference = new NameGuidPair() { Guid = new Guid("2e4f80a8-e8f1-41ab-9584-524a284b7efc"), Name = "Any Reference" };
        public static readonly NameGuidPair AnyValue = new NameGuidPair() { Guid = new Guid("abf3d501-1568-47db-b0a4-e5130f0fdfdd"), Name = "Any Value" };
        public static readonly NameGuidPair AttributeEntity = new NameGuidPair() { Guid = new Guid("1a3bcacb-9e25-4b39-bb4a-aaf1d826a578"), Name = "Attribute Entity" };
        public static readonly NameGuidPair Boolean = new NameGuidPair() { Guid = new Guid("0ce78910-608d-41ee-a5e2-666d834bbb86"), Name = "Boolean" };
        public static readonly NameGuidPair Character = new NameGuidPair() { Guid = new Guid("040a43ee-aa73-457f-9a32-022c2d89e041"), Name = "Character" };
        public static readonly NameGuidPair ConstantEntity = new NameGuidPair() { Guid = new Guid("123ac71a-2be4-4677-924e-b5013472d3b0"), Name = "Constant Entity" };
        public static readonly NameGuidPair Entity = new NameGuidPair() { Guid = new Guid("04abab65-fca1-4cc5-8584-c527f20ae95a"), Name = "Entity" };
        public static readonly NameGuidPair ProcedureEntity = new NameGuidPair() { Guid = new Guid("085550a0-9456-4791-b4a4-303b685a2b99"), Name = "Procedure Entity" };
        public static readonly NameGuidPair Event = new NameGuidPair() { Guid = new Guid("296a70a8-d07a-4f9a-ab09-46fbd9a4f08e"), Name = "Event" };
        public static readonly NameGuidPair Number = new NameGuidPair() { Guid = new Guid("337731dc-37ba-47a3-9e85-1d7c2304e36e"), Name = "Number" };
        public static readonly NameGuidPair PropertyEntity = new NameGuidPair() { Guid = new Guid("270a1358-45e3-4d36-86f1-3e150bbf341d"), Name = "Property Entity" };
        public static readonly NameGuidPair IndexerEntity = new NameGuidPair() { Guid = new Guid("3c5d167e-8406-418d-814a-8fc296524513"), Name = "Indexer Entity" };
        public static readonly NameGuidPair LocalEntity = new NameGuidPair() { Guid = new Guid("43b2429b-e9d6-417e-b6fe-942fa261b176"), Name = "Local Entity" };
        public static readonly NameGuidPair OnceReference = new NameGuidPair() { Guid = new Guid("415dd598-bc65-4e74-ae7d-2f9aafd1d975"), Name = "Once Reference" };
        public static readonly NameGuidPair OverLoopSource = new NameGuidPair() { Guid = new Guid("4273b1ee-4b1b-4ae1-bf22-f72563f87c02"), Name = "Over Loop Source" };
        public static readonly NameGuidPair AnyOnceReference = new NameGuidPair() { Guid = new Guid("894e9bd1-f516-42c4-a95f-a40e698eea97"), Name = "Any Once Reference" };
        public static readonly NameGuidPair Enumeration = new NameGuidPair() { Guid = new Guid("56943636-5db9-48f6-8456-9bb593698aa2"), Name = "Enumeration" };
        public static readonly NameGuidPair OptionalReference = new NameGuidPair() { Guid = new Guid("9af0a9f5-820f-4ddb-b221-f7a1b3446676"), Name = "Optional Reference" };
        public static readonly NameGuidPair String = new NameGuidPair() { Guid = new Guid("46a55923-c615-49b8-a4b7-620738d4a941"), Name = "String" };
        public static readonly NameGuidPair DateAndTime = new NameGuidPair() { Guid = new Guid("4f92fa47-41f5-4c8f-8098-3fc89d50c814"), Name = "Date And Time" };
        public static readonly NameGuidPair AnyDetachableReference = new NameGuidPair() { Guid = new Guid("a035583e-30d7-4434-be9a-8c4780029ead"), Name = "Any Detachable Reference" };
        public static readonly NameGuidPair AnyOptionalReference = new NameGuidPair() { Guid = new Guid("ba18c1c9-f75f-4783-b99a-acfe78ff21fc"), Name = "Any Optional Reference" };
        public static readonly NameGuidPair FeatureEntity = new NameGuidPair() { Guid = new Guid("a24ff4d8-4cc7-40ca-9e20-5990537c683a"), Name = "Feature Entity" };
        public static readonly NameGuidPair KeyValuePair = new NameGuidPair() { Guid = new Guid("a531b63d-73cd-48ee-8115-027c4eb21473"), Name = "Key Value Pair" };
        public static readonly NameGuidPair SpecializedTypeEntity = new NameGuidPair() { Guid = new Guid("a3a0be99-7c93-4d88-8afc-14d53cce8345"), Name = "Specialized Type Entity" };
        public static readonly NameGuidPair DetachableReference = new NameGuidPair() { Guid = new Guid("bc6c211d-737b-4de5-bb69-22fa6f145018"), Name = "Detachable Reference" };
        public static readonly NameGuidPair NamedFeatureEntity = new NameGuidPair() { Guid = new Guid("c1555fa7-928a-40ee-b21f-ded92bc7b756"), Name = "Named Feature Entity" };
        public static readonly NameGuidPair StableReference = new NameGuidPair() { Guid = new Guid("c3ba9569-b9c7-4e30-9146-d489f2e49a64"), Name = "Stable Reference" };
        public static readonly NameGuidPair AnyStableReference = new NameGuidPair() { Guid = new Guid("d3c9682c-5b9f-437c-97a2-f92a76dd2904"), Name = "Any Stable Reference" };
        public static readonly NameGuidPair Hashtable = new NameGuidPair() { Guid = new Guid("d50a2e83-7d47-4aa1-9cb3-1e3f91f3e299"), Name = "Hashtable" };
        public static readonly NameGuidPair List = new NameGuidPair() { Guid = new Guid("ce8fb534-1c8b-4563-a7d2-e617571e5333"), Name = "List" };
        public static readonly NameGuidPair Exception = new NameGuidPair() { Guid = new Guid("d7a3c606-3521-46df-8bd5-d1f16074bd60"), Name = "Exception" };
        public static readonly NameGuidPair SealableHashtable = new NameGuidPair() { Guid = new Guid("d6d59964-9b33-434e-8511-2453e84fbd68"), Name = "Sealable Hashtable" };
        public static readonly NameGuidPair TypeEntity = new NameGuidPair() { Guid = new Guid("eda30eb6-27d8-4632-8cf3-394c9bc61eb6"), Name = "Type Entity" };
        public static readonly NameGuidPair BitFieldEnumeration = new NameGuidPair() { Guid = new Guid("fefd8baf-090a-40c1-85e2-ffc6c7236bdd"), Name = "Bit Field Enumeration" };
        public static readonly NameGuidPair FunctionEntity = new NameGuidPair() { Guid = new Guid("f497d05b-32a2-420f-8d0b-add133e17098"), Name = "Function Entity" };
        public static readonly NameGuidPair UniversallyUniqueIdentifier = new NameGuidPair() { Guid = new Guid("f7019a32-e477-427a-b826-25c9d80f10eb"), Name = "Universally Unique Identifier" };

        public static Dictionary<string, Guid> NameToGuid { get; } = new Dictionary<string, Guid>()
        {
            { Any.Name, Any.Guid },
            { AnyReference.Name, AnyReference.Guid },
            { AnyValue.Name, AnyValue.Guid },
            { AttributeEntity.Name, AttributeEntity.Guid },
            { Boolean.Name, Boolean.Guid },
            { Character.Name, Character.Guid },
            { ConstantEntity.Name, ConstantEntity.Guid },
            { Entity.Name, Entity.Guid },
            { ProcedureEntity.Name, ProcedureEntity.Guid },
            { Event.Name, Event.Guid },
            { Number.Name, Number.Guid },
            { PropertyEntity.Name, PropertyEntity.Guid },
            { IndexerEntity.Name, IndexerEntity.Guid },
            { LocalEntity.Name, LocalEntity.Guid },
            { OnceReference.Name, OnceReference.Guid },
            { OverLoopSource.Name, OverLoopSource.Guid },
            { AnyOnceReference.Name, AnyOnceReference.Guid },
            { Enumeration.Name, Enumeration.Guid },
            { OptionalReference.Name, OptionalReference.Guid },
            { String.Name, String.Guid },
            { DateAndTime.Name, DateAndTime.Guid },
            { AnyDetachableReference.Name, AnyDetachableReference.Guid },
            { AnyOptionalReference.Name, AnyOptionalReference.Guid },
            { FeatureEntity.Name, FeatureEntity.Guid },
            { KeyValuePair.Name, KeyValuePair.Guid },
            { SpecializedTypeEntity.Name, SpecializedTypeEntity.Guid },
            { DetachableReference.Name, DetachableReference.Guid },
            { NamedFeatureEntity.Name, NamedFeatureEntity.Guid },
            { StableReference.Name, StableReference.Guid },
            { AnyStableReference.Name, AnyStableReference.Guid },
            { Hashtable.Name, Hashtable.Guid },
            { List.Name, List.Guid },
            { Exception.Name, Exception.Guid },
            { SealableHashtable.Name, SealableHashtable.Guid },
            { TypeEntity.Name, TypeEntity.Guid },
            { BitFieldEnumeration.Name, BitFieldEnumeration.Guid },
            { FunctionEntity.Name, FunctionEntity.Guid },
            { UniversallyUniqueIdentifier.Name, UniversallyUniqueIdentifier.Guid },
        };

        public static readonly string BooleanTrueString = "True";
        public static readonly string BooleanFalseString = "False";
    }
}
