namespace EaslyCompiler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Info about language classes.
    /// </summary>
    internal static class LanguageClasses
    {
        public static readonly Guid AnyDetachableReference = new Guid("a035583e-30d7-4434-be9a-8c4780029ead");
        public static readonly Guid AnyOnceReference = new Guid("894e9bd1-f516-42c4-a95f-a40e698eea97");
        public static readonly Guid AnyOptionalReference = new Guid("ba18c1c9-f75f-4783-b99a-acfe78ff21fc");
        public static readonly Guid AnyStableReference = new Guid("d3c9682c-5b9f-437c-97a2-f92a76dd2904");
        public static readonly Guid AttributeEntity = new Guid("1a3bcacb-9e25-4b39-bb4a-aaf1d826a578");
        public static readonly Guid BitFieldEnumeration = new Guid("fefd8baf-090a-40c1-85e2-ffc6c7236bdd");
        public static readonly Guid Boolean = new Guid("0ce78910-608d-41ee-a5e2-666d834bbb86");
        public static readonly Guid Character = new Guid("040a43ee-aa73-457f-9a32-022c2d89e041");
        public static readonly Guid ConstantEntity = new Guid("123ac71a-2be4-4677-924e-b5013472d3b0");
        public static readonly Guid DetachableReference = new Guid("bc6c211d-737b-4de5-bb69-22fa6f145018");
        public static readonly Guid Entity = new Guid("04abab65-fca1-4cc5-8584-c527f20ae95a");
        public static readonly Guid Enumeration = new Guid("56943636-5db9-48f6-8456-9bb593698aa2");
        public static readonly Guid Event = new Guid("296a70a8-d07a-4f9a-ab09-46fbd9a4f08e");
        public static readonly Guid Exception = new Guid("d7a3c606-3521-46df-8bd5-d1f16074bd60");
        public static readonly Guid FeatureEntity = new Guid("a24ff4d8-4cc7-40ca-9e20-5990537c683a");
        public static readonly Guid FunctionEntity = new Guid("f497d05b-32a2-420f-8d0b-add133e17098");
        public static readonly Guid Hashtable = new Guid("d50a2e83-7d47-4aa1-9cb3-1e3f91f3e299");
        public static readonly Guid IndexerEntity = new Guid("3c5d167e-8406-418d-814a-8fc296524513");
        public static readonly Guid KeyValuePair = new Guid("a531b63d-73cd-48ee-8115-027c4eb21473");
        public static readonly Guid List = new Guid("ce8fb534-1c8b-4563-a7d2-e617571e5333");
        public static readonly Guid LocalEntity = new Guid("43b2429b-e9d6-417e-b6fe-942fa261b176");
        public static readonly Guid NamedFeatureEntity = new Guid("c1555fa7-928a-40ee-b21f-ded92bc7b756");
        public static readonly Guid Number = new Guid("337731dc-37ba-47a3-9e85-1d7c2304e36e");
        public static readonly Guid OnceReference = new Guid("415dd598-bc65-4e74-ae7d-2f9aafd1d975");
        public static readonly Guid OptionalReference = new Guid("9af0a9f5-820f-4ddb-b221-f7a1b3446676");
        public static readonly Guid OverLoopSource = new Guid("4273b1ee-4b1b-4ae1-bf22-f72563f87c02");
        public static readonly Guid ProcedureEntity = new Guid("085550a0-9456-4791-b4a4-303b685a2b99");
        public static readonly Guid PropertyEntity = new Guid("270a1358-45e3-4d36-86f1-3e150bbf341d");
        public static readonly Guid SealableHashtable = new Guid("d6d59964-9b33-434e-8511-2453e84fbd68");
        public static readonly Guid SpecializedTypeEntity = new Guid("a3a0be99-7c93-4d88-8afc-14d53cce8345");
        public static readonly Guid StableReference = new Guid("c3ba9569-b9c7-4e30-9146-d489f2e49a64");
        public static readonly Guid String = new Guid("46a55923-c615-49b8-a4b7-620738d4a941");
        public static readonly Guid TypeEntity = new Guid("eda30eb6-27d8-4632-8cf3-394c9bc61eb6");

        public static Dictionary<string, Guid> NameToGuid { get; } = new Dictionary<string, Guid>()
        {
            { "Any Detachable Reference", AnyDetachableReference },
            { "Any Once Reference", AnyOnceReference },
            { "Any Optional Reference", AnyOptionalReference },
            { "Any Stable Reference", AnyStableReference },
            { "Attribute Entity", AttributeEntity },
            { "Bit Field Enumeration", BitFieldEnumeration },
            { "Boolean", Boolean },
            { "Character", Character },
            { "Constant Entity", ConstantEntity },
            { "Detachable Reference", DetachableReference },
            { "Entity", Entity },
            { "Enumeration", Enumeration },
            { "Event", Event },
            { "Exception", Exception },
            { "Feature Entity", FeatureEntity },
            { "Function Entity", FunctionEntity },
            { "Hashtable", Hashtable },
            { "Indexer Entity", IndexerEntity },
            { "Key Value Pair", KeyValuePair },
            { "List", List },
            { "Local Entity", LocalEntity },
            { "Named Feature Entity", NamedFeatureEntity },
            { "Number", Number },
            { "Once Reference", OnceReference },
            { "Optional Reference", OptionalReference },
            { "Over Loop Source", OverLoopSource },
            { "Procedure Entity", ProcedureEntity },
            { "Property Entity", PropertyEntity },
            { "Sealable Hashtable", SealableHashtable },
            { "Specialized Type Entity", SpecializedTypeEntity },
            { "Stable Reference", StableReference },
            { "String", String },
            { "Type Entity", TypeEntity },
        };
    }
}
