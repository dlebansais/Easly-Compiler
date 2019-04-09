# Compilation

The step-by-step description of the translation of Easly source code to a target language (C#).

## Loading, validating input and preprocessing

This section explains how the code is loaded, validated and prepared through the preprocessing steps.

### Loading

This step assumes the Easly source code is provided as a stream of bytes. If a IRoot object is provided directly, it is serialized into a memory stream first. If provided as a file, the file is loaded as a stream.

The source code is loaded by deserializing the stream to a compiler IRoot object. This is an instance of the class that inherits from the base IRoot language class. This is done by way of the polymorphic serializer PolySerializer.

For the rest of this document, unless specified, all references to Easly nodes assume an instance of a compiler class.

## Merging with built-in language constructs

The source code may refer to language constructs such as Boolean. These are loaded from a static resource stream, and added to the root object classes and libraries. These constructs can be recognized by their Guid, fixed by the language specification.

## Replacing macroes (phase 1)

The following macroes are replaced by their compile time values, instances of IInitializedObjectExpression:
+ Date And Time
+ Compilation Discrete Identifier
+ Compiler Version
+ Conformance To Standard
+ Discrete Class Identifier
+ Debugging

Other macroes are expanded after the block replication step.

## Block Replication

All blocks with a replication status equal to `Replicated` are processed.
Since blocks can be nested, the following order is used:
+ Class replicate blocks are replicated first in each class.
+ Then, each block is replicated, starting from the top level ones, and reprocessing each replicated version to handle nested blocks.

Since maintaining blocks after this step no longer makes sense, all nodes in block lists are grouped into one list (per property).

## Replacing macroes (phase 2)

The following macroes are replaced by their compile time values, instances of IInitializedObjectExpression:
+ Class Path, this includes replication information.
+ Class Counter, each replicated class has its own counter.
+ Random Number, each instance is unique.
 
## Organizing class and library imports

Libraries list classes that other libraries and classes can import. This step ensures that all classes end up with a list of imported classes, after nested library imports and renames.

The following checks are performed.

### String validity

Identifiers must be valid, i.e contain valid characters, be well-formed unicode strings and no start or end with a whitespace. Identifiers included are:
+ Class name and source.
+ Library name and source.
+ Name of classes listed in libraries
+ Name and source of libraries in import clauses
+ Source and destination in rename clauses

### Import Errors

    Class '{NewName}' already imported under name '{OldName}'.
A class must not be imported using two different names after all rename clauses have been processed.

    Cyclic dependencies detected in ...list of classes...
    Class '{ClassName}' cannot import itself under name '{NewName}'.
A class cannot import itself, neither directly or through libraries in a cycle.

    '{SourceName}' has already been renamed as '{DestinationName}'.
A class cannot be imported with its name and with another name through libraries rename clauses.

    Library '{LibraryName}' already imported.
A library should be imported only once. Note that multiple imports with nested libraries are valid, but explicitely importing the same one twice is not.

    Identifier '{Identifier}' already listed.
This error occurs for example when the same class is renamed twice in the same import clause.

    Conflicting import type for class '{ClassName}'.
A class must not be imported using different specification, for example as `stable` in a library and `latest` in another.

    More than one class is imported using the name '{Name}'.
Rename clauses should not be used to rename different classes to the same name.

    Item renamed to the same name '{Name}'.
In a rename clause, the source and destination must be different.

    All items with the same name must have a source.
Different classes can use the same name, but when they are used together they must have a different source name.
