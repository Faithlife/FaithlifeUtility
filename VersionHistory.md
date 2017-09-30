# Version History

## Pending

Add changes here when they're committed to the `master` branch. Move them to "Released" once the version number
is updated in preparation for publishing an updated NuGet package.

Prefix the description of the change with `[major]`, `[minor]` or `[patch]` in accordance with [SemVer](http://semver.org).

* [minor] Add `ByteUtility`.
* Change .NET Framework minimum version to 4.6.1.

## Released

### 0.3.0

* **Breaking** Remove `EnumerableUtility.ToReadOnlyCollection`.
* Add `EnumerableUtility.AsReadOnlyList`.

### 0.2.0

* Support `netstandard2.0`
  * Support `BeginRead`, `BeginWrite`, `EndRead`, `EndWrite` in `WrappingStream`, `ReadOnlyStream`, `CachingStream`, `TruncatedStream`.
* **Breaking** Remove `EnumerableUtility.Append` and `.Prepend`. Equivalent methods exist in `netstandard1.6`.

### 0.1.0

* Initial release.
