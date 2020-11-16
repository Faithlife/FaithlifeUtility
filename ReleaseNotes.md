# Release Notes

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
