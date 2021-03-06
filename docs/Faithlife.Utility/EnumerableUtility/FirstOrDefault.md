# EnumerableUtility.FirstOrDefault&lt;T&gt; method (1 of 2)

Returns the first element of a sequence or a default value if no such element is found.

```csharp
public static T? FirstOrDefault<T>(this IEnumerable<T> source, T? defaultValue)
```

| parameter | description |
| --- | --- |
| T | The type of the elements of *source*. |
| source | An IEnumerable&lt;TSource&gt; to return an element from. |
| defaultValue | The value to return if no element is found. |

## Return Value

*defaultValue* if *source* is empty; otherwise, the first element in *source*.

## See Also

* class [EnumerableUtility](../EnumerableUtility.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

---

# EnumerableUtility.FirstOrDefault&lt;T&gt; method (2 of 2)

Returns the first element of a sequence that satisfies a condition or a default value if no such element is found.

```csharp
public static T? FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, 
    T? defaultValue)
```

| parameter | description |
| --- | --- |
| T | The type of the elements of *source*. |
| source | An IEnumerable&lt;TSource&gt; to return an element from. |
| predicate | A function to test each element for a condition. |
| defaultValue | The value to return if no element satisfies the condition. |

## Return Value

*defaultValue* if *source* is empty or if no element passes the test specified by *predicate*; otherwise, the first element in *source* that passes the test specified by *predicate*.

## See Also

* class [EnumerableUtility](../EnumerableUtility.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
