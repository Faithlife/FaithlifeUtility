# Version History

## Pending

Describe changes here when they're committed to the `master` branch. Move them to **Released** when the project version number is updated in preparation for publishing an updated NuGet package.

Prefix the description of the change with `[major]`, `[minor]` or `[patch]` in accordance with [Semantic Versioning](https://semver.org/).

* [minor] Add `InvariantConvert.ToInvariantString(object value)` overload.
* [minor] Use `InvariantConvert` in `UriUtility.FromPattern()` for better parameter stringification.

## Released

### 0.8.0

* Remove dependency on Faithlife.Analyzers.

### 0.7.0

* Update minimum target frameworks to .NET Standard 2.0, .NET 4.7.2.

### 0.6.0

* Add `KeyEqualityComparer`.
* Fix typo in StringBuilderUtility summary for AppendFormatInvariant.

### 0.5.0

* [major] Remove `EnumerableUtility.ToHashSet()`.
* [major] Seal `WrappingStream`.
* [major] Introduce `WrappingStreamBase` and make it the base class of `CachingStream`, `ReadOnlyStream`, `RebasedStream`, and `TruncatedStream`. This fixes `CopyToAsync` for `RebasedStream` and `TruncatedStream`.

### 0.4.2

* Add `Optional<T>`.

### 0.4.1

* Update release notes link in NuGet package.

### 0.4.0

* **Breaking** Change .NET Framework minimum version to 4.6.1.
* **Breaking** Remove `PeekEnumerator`, `Scoped`, `StreamImpl`, `TextWriterUtility`, `TimeSpanUtility`, `IWorkState`, `WorkState`.
* **Breaking** Modify and/or remove methods from `CollectionUtility`, `DateTimeUtility`, `DateTimeOffsetUtility`, `DictionaryUtility`, `EnumerableUtility`, `EnumUtility`, `GuidUtility`, `ListUtility`, `UriUtility`, `StreamUtility`, `StringSegment`, `StringUtility`.
* **Breaking** Rename `HashSetUtility` and `ReadOnlyHashSet` to `SetUtility` and `ReadOnlySet`.
* **Breaking** Move `InvariantConvert` to `Faithlife.Utility` namespace.
* **Breaking** Improve parameter names.
* Add `ByteUtility`.
* Add `FileUtility` and `DirectoryUtility`.

### 0.3.0

* **Breaking** Remove `EnumerableUtility.ToReadOnlyCollection`.
* Add `EnumerableUtility.AsReadOnlyList`.

### 0.2.0

* Support `netstandard2.0`
  * Support `BeginRead`, `BeginWrite`, `EndRead`, `EndWrite` in `WrappingStream`, `ReadOnlyStream`, `CachingStream`, `TruncatedStream`.
* **Breaking** Remove `EnumerableUtility.Append` and `.Prepend`. Equivalent methods exist in `netstandard1.6`.

### 0.1.0

* Initial release.
