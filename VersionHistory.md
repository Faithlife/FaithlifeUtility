# Version History

## Pending

Add changes here when they're committed to the `master` branch. Move them to "Released" once the version number
is updated in preparation for publishing an updated NuGet package.

Prefix the description of the change with `[major]`, `[minor]` or `[patch]` in accordance with [SemVer](http://semver.org).

* [minor] Support `netstandard2.0`
* [patch] Support `BeginRead`, `BeginWrite`, `EndRead`, `EndWrite` in `WrappingStream`, `ReadOnlyStream`, `CachingStream`, `TruncatedStream`.
* [minor] Remove `EnumerableUtility.Append` and `.Prepend`. Equivalent methods exist in .NET Standard 1.6.

## Released

### 0.1.0

* Initial release.
