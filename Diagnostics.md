# Diagnostics

| ID | Severity | Description | How to fix |
|---|---|---|---|
| MC0001 | ❌ Error | `[PopupSource]` method is not `static partial`, or has an implementation written | Declare the method as `static partial` without an implementation |
| MC0002 | ❌ Error | `[PopupSource]` method has parameters | Remove the parameters from the method |
| MC0003 | ❌ Error | `[PopupSource]` method does not return `IEnumerable<KeyValuePair<ViewId, Type>>` | Change the return type to `IEnumerable<KeyValuePair<ViewId, Type>>` |
| MC0004 | ❌ Error | `[Popup]` class is file-local or nested in a `private` / `protected` type, so the generated registration cannot refer to it; it is not registered | Make the class visible in the assembly |
| MC0005 | ❌ Error | `[PopupSource]` method or type name differs only in case from another one, so the generated file names collide; only the first (in ordinal order) is generated | Rename one of the methods or types |
