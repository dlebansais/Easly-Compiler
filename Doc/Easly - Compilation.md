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

## Inference Engine

Once all classes and libraries been validated, they can be inspected for semantic consistency and eventually translated to text in the target language. This large step is decomposed in many little steps, whereby a particular semantic verification is performed, and if successful, enable other verifications. For example, verifying that a generic type is used with the correct number of arguments enable its use as ancestor in class inheritance, for which further verifications are necessary.

The whole process is performed by an inference engine. The engine basically contains a set of rules, and for each rule:
+ Checks that elements that are verified by the rule are ready for verification.
+ Execute the verification code, and upon success modifies nodes with updated information, or upon failure record an error.
+ Ensures modified elements are ready for other rules.

### Sources

Each rules inspects one or more source node to see if their content is ready. For example, if a type has been verified and is valid, the node contains a field updated with this type. While the field is not filled, the source is considered not ready for the purpose of this rule, and thr inference engine either checks another node, or until they have all been checked, moves to the next rule.

Example of sources are:
+ A field filled by the execution of another rule.
+ A sealed list or hash table.
+ A collection of nodes for which all fields must be filled, or the collection is not yet ready.
+ And more

### Destinations

They are fields, lists or tables, that can be filled with information, or sealed. If all destinations of a rule are filled on a given node, it means the rule has been executed and there is no need to wait for sources to be ready. Therefore, the engine mostly looks for destinations not filled, checks if all sources associated to a rule that could fill them are ready, and executes the rule.

### Major steps

If it necessary, to ensure consistency of the language semantic, to split inference in a few steps separated by some barrier. For example, types are evaluated only after duplicate names or invalid manifest constants have been checked. Expressions are evaluated only after all types has been verified, and so no.

The inference steps are as follow:
+ Identifiers: no duplicate name, no invalid string or constant.
+ Types: no missing type argument, no unknown identifier, no use of a type in the wrong context. This is a big and complex step.
+ Contract: boolean types where boolean is expected, respect of exceptions.
+ Body: each instruction has their own requirement and they are checked. 