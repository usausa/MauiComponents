# Diagnostics

| ID | Severity | Description | How to fix |
|---|---|---|---|
| MC0001 | ❌ Error | Target method is not a partial extension method | Declare the method as a `partial` extension method |
| MC0002 | ❌ Error | Target method has parameters | Remove the parameters from the method |
| MC0003 | ❌ Error | Target method does not return `IEnumerable<KeyValuePair<ViewId, Type>>` | Change the return type to `IEnumerable<KeyValuePair<ViewId, Type>>` |
