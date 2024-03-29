# RebasedStream class

[`RebasedStream`](./RebasedStream.md) is a stream wrapper that changes the effective origin of the wrapped stream.

```csharp
public sealed class RebasedStream : WrappingStreamBase
```

## Public Members

| name | description |
| --- | --- |
| [RebasedStream](RebasedStream/RebasedStream.md)(…) | Initializes a new instance of the [`RebasedStream`](./RebasedStream.md) class; the current position in *stream* will be the origin of the [`RebasedStream`](./RebasedStream.md). (2 constructors) |
| override [Length](RebasedStream/Length.md) { get; } | Gets the length in bytes of the stream. |
| override [Position](RebasedStream/Position.md) { get; set; } | Gets or sets the position within the current stream. |
| override [Read](RebasedStream/Read.md)(…) | Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read. |
| override [ReadAsync](RebasedStream/ReadAsync.md)(…) | Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests. (2 methods) |
| override [Seek](RebasedStream/Seek.md)(…) | Sets the position within the current stream. |
| override [SetLength](RebasedStream/SetLength.md)(…) | Sets the length of the current stream. |
| override [Write](RebasedStream/Write.md)(…) | Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written. |
| override [WriteAsync](RebasedStream/WriteAsync.md)(…) | Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests. (2 methods) |

## See Also

* class [WrappingStreamBase](./WrappingStreamBase.md)
* namespace [Faithlife.Utility](../Faithlife.Utility.md)
* [RebasedStream.cs](https://github.com/Faithlife/FaithlifeUtility/tree/master/src/Faithlife.Utility/RebasedStream.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
