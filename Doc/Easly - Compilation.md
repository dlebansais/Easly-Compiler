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
 
