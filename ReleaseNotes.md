# Release Notes

## Unreleased

* Make `ReadOnlySet` implement `IReadOnlySet` on .NET 5.
* Obsolete `EnumerableUtility.TakeLast`.
* **Breaking** Drop support for .NET Core 2.1.
* Support .NET 6.0.
* `DictionaryUtility` key types must be `notnull`.
* Add `Stream.ReadAsync`/`WriteAsync` overloads for `Memory<byte>` on .NET Standard 2.1 and .NET Core.
* Obsolete `DictionaryUtility.CreateKeyValuePair` on .NET Standard 2.1 and .NET Core.
* Obsolete `EnumerableUtility.EnumerateBatches` on .NET 6.
* Obsolete `EnumerableUtility.DistinctBy` on .NET 6.
* Obsolete `EnumerableUtility.FirstOrDefault` on .NET 6.

## 0.11.2

* `InvariantConvert.TryParse` methods allow `null`.

## 0.11.1

* Restrict `EnumUtility` to `Enum`.
* Improve nullability of `FirstOrDefault` and `TryFirst`.

## 0.11.0

* **Potentially Breaking** `InvariantConvert` and `StringBuilderUtility.AppendInvariant` have different behavior for floating-point numbers.
  * The `G17` (or `G9`) format is used (instead of `R`) to guarantee that floating-point numbers round-trip correctly. This may cause numbers to be serialized differently than in previous versions of Faithlife.Utility.
  * Lowercase variants of `-Infinity`, `Infinity`, and `NaN` are accepted in `InvariantConvert.(Try)ParseDouble`. Previously, only those extra strings were accepted.
* Changes for `netstandard2.1` and `net5.0`:
  * `StringCache` is implemented with `HashSet<string>`.
  * `WrappingStream` supports the new `Read(Async)` and `Write(Async)` overloads for `Span<byte>` and `Memory<byte>`.
* `EnumerableUtility.Zip` is no longer an extension method in .NET 5, to avoid conflict with `Enumerable.Zip`.
* Add `StringUtility.ReplaceOrdinal`. (Useful to avoid StyleCop error without passing `StringComparison.Ordinal` to `string.Replace`, which doesn't build before .NET 5.)

## 0.10.2

* Allow `AddIfNotNull` to work with `List<Nullable<T>>`.
* Support .NET Core App 2.1 instead of .NET Core App 2.0.

## 0.10.1

* Add missing `ConfigureAwait`.

## 0.10.0

* Remove dependency on `JetBrains.Annotations`. Use source code copied from ReSharper » Options » Code Inspection » Code Annotations. (This allows the annotations to be seen by ReSharper where the library is used.)

## 0.9.0

* Add `netcoreapp2.0` and `netstandard2.1` target frameworks; remove `net472`.
* Add C# 8 nullable annotations.
* Remove `DictionaryUtility.GetValueOrDefault` and `TryAdd` as extension methods on new frameworks.

## 0.8.0

* Remove dependency on Faithlife.Analyzers.

## 0.7.0

* Update minimum target frameworks to .NET Standard 2.0, .NET 4.7.2.
* Add `InvariantConvert.ToInvariantString(object value)` overload.
* Use `InvariantConvert` in `UriUtility.FromPattern()` for better parameter stringification.

## 0.6.0

* Add `KeyEqualityComparer`.
* Fix typo in StringBuilderUtility summary for AppendFormatInvariant.

## 0.5.0

* **Breaking** Remove `EnumerableUtility.ToHashSet()`.
* **Breaking** Seal `WrappingStream`.
* **Breaking** Introduce `WrappingStreamBase` and make it the base class of `CachingStream`, `ReadOnlyStream`, `RebasedStream`, and `TruncatedStream`. This fixes `CopyToAsync` for `RebasedStream` and `TruncatedStream`.

## 0.4.2

* Add `Optional<T>`.

## 0.4.1

* Update release notes link in NuGet package.

## 0.4.0

* **Breaking** Change .NET Framework minimum version to 4.6.1.
* **Breaking** Remove `PeekEnumerator`, `Scoped`, `StreamImpl`, `TextWriterUtility`, `TimeSpanUtility`, `IWorkState`, `WorkState`.
* **Breaking** Modify and/or remove methods from `CollectionUtility`, `DateTimeUtility`, `DateTimeOffsetUtility`, `DictionaryUtility`, `EnumerableUtility`, `EnumUtility`, `GuidUtility`, `ListUtility`, `UriUtility`, `StreamUtility`, `StringSegment`, `StringUtility`.
* **Breaking** Rename `HashSetUtility` and `ReadOnlyHashSet` to `SetUtility` and `ReadOnlySet`.
* **Breaking** Move `InvariantConvert` to `Faithlife.Utility` namespace.
* **Breaking** Improve parameter names.
* Add `ByteUtility`.
* Add `FileUtility` and `DirectoryUtility`.

## 0.3.0

* **Breaking** Remove `EnumerableUtility.ToReadOnlyCollection`.
* Add `EnumerableUtility.AsReadOnlyList`.

## 0.2.0

* Support `netstandard2.0`
  * Support `BeginRead`, `BeginWrite`, `EndRead`, `EndWrite` in `WrappingStream`, `ReadOnlyStream`, `CachingStream`, `TruncatedStream`.
* **Breaking** Remove `EnumerableUtility.Append` and `.Prepend`. Equivalent methods exist in `netstandard1.6`.

## 0.1.0

* Initial release.
