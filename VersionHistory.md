# Version History

## Pending

Add changes here when they're committed to the `master` branch. Move them to "Released" once the version number
is updated in preparation for publishing an updated NuGet package.

Prefix the description of the change with `[major]`, `[minor]` or `[patch]` in accordance with [SemVer](http://semver.org).

## Released

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
