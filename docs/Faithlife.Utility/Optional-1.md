# Optional&lt;T&gt; structure

Represents an optional value.

```csharp
public struct Optional<T> : IEquatable<Optional>, IOptional
```

| parameter | description |
| --- | --- |
| T | The type of the optional value. |

## Public Members

| name | description |
| --- | --- |
| [Optional](Optional-1/Optional.md)(…) | Initializes a new instance of the Optional{T} structure to the specified value. |
| [HasValue](Optional-1/HasValue.md) { get; } | Gets a value indicating whether the current Optional{T} object has a value. |
| [Value](Optional-1/Value.md) { get; } | Gets the value of the current Optional{T} value. |
| override [Equals](Optional-1/Equals.md)(…) | Indicates whether the current object is equal to another object of the same type. |
| [Equals](Optional-1/Equals.md)(…) |  |
| override [GetHashCode](Optional-1/GetHashCode.md)() | Retrieves the hash code of the object returned by the Value property. |
| [GetValueOrDefault](Optional-1/GetValueOrDefault.md)() | Retrieves the value of the current Optional{T} object, or the object's default value. |
| [GetValueOrDefault](Optional-1/GetValueOrDefault.md)(…) | Retrieves the value of the current Optional{T} object, or the specified default value. |
| override [ToString](Optional-1/ToString.md)() | Returns the text representation of the value of the current Optional{T} object. |
| [operator ==](Optional-1/op_Equality.md) |  |
| [explicit operator](Optional-1/op_Explicit.md) |  |
| [implicit operator](Optional-1/op_Implicit.md) |  |
| [operator !=](Optional-1/op_Inequality.md) |  |

## See Also

* interface [IOptional](IOptional.md)
* namespace [Faithlife.Utility](../Faithlife.Utility.md)
* [Optional.cs](https://github.com/Faithlife/FaithlifeUtility/tree/master/src/Faithlife.Utility/Optional.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->