# Faithlife.Utility assembly

## Faithlife.Utility namespace

| public type | description |
| --- | --- |
| static class [ArrayUtility](Faithlife.Utility/ArrayUtility.md) | Provides methods for manipulating arrays. |
| static class [ByteUtility](Faithlife.Utility/ByteUtility.md) | Provides helper methods for working with Byte. |
| class [CachingStream](Faithlife.Utility/CachingStream.md) | A [`WrappingStream`](Faithlife.Utility/WrappingStream.md) that caches all data read from the underlying Stream. |
| enum [CaseSensitivity](Faithlife.Utility/CaseSensitivity.md) | Indicates whether an operation should be sensitive to case. |
| static class [CollectionImpl](Faithlife.Utility/CollectionImpl.md) | Provides implementations of common methods needed by an implementer of ICollection. |
| static class [CollectionUtility](Faithlife.Utility/CollectionUtility.md) | Methods for manipulating collections. |
| static class [ComparableImpl](Faithlife.Utility/ComparableImpl.md) | Provides methods for implementing IComparable. |
| static class [ComparisonUtility](Faithlife.Utility/ComparisonUtility.md) | Provides methods to simplify comparing instances. |
| static class [DateTimeOffsetUtility](Faithlife.Utility/DateTimeOffsetUtility.md) | Provides methods for manipulating dates. |
| static class [DateTimeUtility](Faithlife.Utility/DateTimeUtility.md) | Provides methods for manipulating dates. |
| static class [DictionaryUtility](Faithlife.Utility/DictionaryUtility.md) | Provides methods for manipulating dictionaries. |
| class [Disposables](Faithlife.Utility/Disposables.md) | A collection of disposable objects, disposed in reverse order when the collection is disposed. |
| static class [DisposableUtility](Faithlife.Utility/DisposableUtility.md) | Provides methods for manipulating disposable objects. |
| static class [EnumerableUtility](Faithlife.Utility/EnumerableUtility.md) | Provides methods for manipulating IEnumerable collections. |
| static class [EnumUtility](Faithlife.Utility/EnumUtility.md) | Provides helper methods for working with enumerated values. |
| static class [Equivalence](Faithlife.Utility/Equivalence.md) | Methods for working with IHasEquivalence. |
| class [EventInfo&lt;TSource,TEventHandler&gt;](Faithlife.Utility/EventInfo-2.md) | Provides methods for adding and removing handlers from an event. |
| static class [EventInfoUtility](Faithlife.Utility/EventInfoUtility.md) | Helper methods for working with EventInfo. |
| class [GenericEventArgs&lt;T&gt;](Faithlife.Utility/GenericEventArgs-1.md) | Event arguments that contain a single value. |
| static class [GuidUtility](Faithlife.Utility/GuidUtility.md) | Helper methods for working with Guid. |
| static class [GzipUtility](Faithlife.Utility/GzipUtility.md) | Methods for working with gzip. |
| static class [HashCodeUtility](Faithlife.Utility/HashCodeUtility.md) | Provides methods for manipulating integers. |
| interface [IHasEquivalence&lt;T&gt;](Faithlife.Utility/IHasEquivalence-1.md) | Implemented by reference classes that do not want to implement IEquatable{T}, but do want to support some form of equivalence. |
| static class [InvariantConvert](Faithlife.Utility/InvariantConvert.md) | Methods for converting to and from strings using the invariant culture. |
| static class [ListUtility](Faithlife.Utility/ListUtility.md) | Methods for manipulating lists. |
| static class [ObjectImpl](Faithlife.Utility/ObjectImpl.md) | Provides methods for manipulating objects. |
| static class [ObjectUtility](Faithlife.Utility/ObjectUtility.md) | Provides methods for manipulating objects. |
| enum [Ownership](Faithlife.Utility/Ownership.md) | Indicates whether an object takes ownership of an item. |
| class [PriorityQueue&lt;T&gt;](Faithlife.Utility/PriorityQueue-1.md) | Implements a priority queue using a binary heap. The priority queue is sorted so that the smallest item is removed from the queue first. |
| static class [ReaderWriterLockSlimUtility](Faithlife.Utility/ReaderWriterLockSlimUtility.md) | Extension methods for ReaderWriterLockSlim |
| class [ReadOnlySet&lt;T&gt;](Faithlife.Utility/ReadOnlySet-1.md) | Implements a read-only wrapper around a HashSet. |
| class [ReadOnlyStream](Faithlife.Utility/ReadOnlyStream.md) | A read-only stream wrapper. |
| class [RebasedStream](Faithlife.Utility/RebasedStream.md) | [`RebasedStream`](Faithlife.Utility/RebasedStream.md) is a [`WrappingStream`](Faithlife.Utility/WrappingStream.md) that changes the effective origin of the wrapped stream. |
| class [Scope](Faithlife.Utility/Scope.md) | Executes the specified delegate when disposed. |
| static class [SetUtility](Faithlife.Utility/SetUtility.md) | Provides methods for working with ISet. |
| static class [StackUtility](Faithlife.Utility/StackUtility.md) | Provides methods for manipulating stacks. |
| static class [StreamUtility](Faithlife.Utility/StreamUtility.md) | Copies data from one stream to another. |
| static class [StringBuilderUtility](Faithlife.Utility/StringBuilderUtility.md) | Provides methods for manipulating StringBuilder objects. |
| class [StringCache](Faithlife.Utility/StringCache.md) | [`StringCache`](Faithlife.Utility/StringCache.md) provides an append-only cache of strings that can be used to reuse the same string object instance when a string is being dynamically created at runtime (e.g., loaded from an XML file or database). |
| struct [StringSegment](Faithlife.Utility/StringSegment.md) | Encapsulates a length of characters from a string starting at a particular offset. |
| static class [StringUtility](Faithlife.Utility/StringUtility.md) | Provides methods for manipulating strings. |
| class [TimeoutTimer](Faithlife.Utility/TimeoutTimer.md) | Tracks the time left for a timeout. |
| class [TruncatedStream](Faithlife.Utility/TruncatedStream.md) | [`TruncatedStream`](Faithlife.Utility/TruncatedStream.md) is a read-only [`WrappingStream`](Faithlife.Utility/WrappingStream.md) that will not read past the specified length. |
| static class [TypeUtility](Faithlife.Utility/TypeUtility.md) | Extension methods allowing portable and non portable libraries to call a single set of reflection based Type methods. |
| enum [UnicodeCharacterClass](Faithlife.Utility/UnicodeCharacterClass.md) | The major class of a Unicode character's general category. |
| static class [UnicodeUtility](Faithlife.Utility/UnicodeUtility.md) | Utility methods for working with Unicode characters. |
| static class [UriBuilderUtility](Faithlife.Utility/UriBuilderUtility.md) | Provides helper methods for working with UriBuilder. |
| static class [UriUtility](Faithlife.Utility/UriUtility.md) | Provides utilities for working with Uris. |
| static class [UrlEncoding](Faithlife.Utility/UrlEncoding.md) | Methods for encoding and decoding URL-style strings. |
| class [UrlEncodingSettings](Faithlife.Utility/UrlEncodingSettings.md) | Stores settings used for encoding and decoding URL-style strings. |
| static class [Verify](Faithlife.Utility/Verify.md) | Provides methods for throwing InvalidOperationException for "impossible" conditions. |
| class [WrappingStream](Faithlife.Utility/WrappingStream.md) | A Stream that wraps another stream. One major feature of [`WrappingStream`](Faithlife.Utility/WrappingStream.md) is that it does not dispose the underlying stream when it is disposed if Ownership.None is used; this is useful when using classes such as BinaryReaderthat take ownership of the stream passed to their constructors. |
| class [ZeroStream](Faithlife.Utility/ZeroStream.md) | A stream of zeroes. |

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
