# WorkState.FromCancellationToken method

Returns an [`IWorkState`](../IWorkState.md) that will be canceled when the specified CancellationToken is canceled.

```csharp
public static IWorkState FromCancellationToken(CancellationToken token)
```

| parameter | description |
| --- | --- |
| token | The cancellation token. |

## Return Value

An [`IWorkState`](../IWorkState.md) wrapping *token*.

## See Also

* interface [IWorkState](../IWorkState.md)
* class [WorkState](../WorkState.md)
* namespace [Faithlife.Utility](../../Faithlife.Utility.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->