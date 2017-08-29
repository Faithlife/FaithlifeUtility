# EnumerableUtility class

Provides methods for manipulating IEnumerable collections.

```csharp
public static class EnumerableUtility
```

## Public Members

| name | description |
| --- | --- |
| static [AppendIfNotAlreadyPresent&lt;T&gt;](EnumerableUtility/AppendIfNotAlreadyPresent.md)(…) | Returns a new sequence that conditionally has the specified item appended to the end (appended iff it doesn't equal an existing sequence item) |
| static [AreEqual&lt;T&gt;](EnumerableUtility/AreEqual.md)(…) | Returns a value indicating whether the specified sequences are equal. Supports one or both sequences being null. (3 methods) |
| static [AsSet&lt;T&gt;](EnumerableUtility/AsSet.md)(…) | Returns a set of the elements in the specified sequence. |
| static [CountIsAtLeast&lt;T&gt;](EnumerableUtility/CountIsAtLeast.md)(…) | Returns true if the count is greater than or equal to the specified value. |
| static [CountIsExactly&lt;T&gt;](EnumerableUtility/CountIsExactly.md)(…) | Returns true if the count is as specified. |
| static [CrossProduct&lt;T&gt;](EnumerableUtility/CrossProduct.md)(…) | Returns the Cartesian cross-product of a sequence of sequences. |
| static [CrossProduct&lt;TSequence,TItem&gt;](EnumerableUtility/CrossProduct.md)(…) | Returns the Cartesian cross-product of a sequence of sequences. |
| static [DistinctBy&lt;TSource,TKey&gt;](EnumerableUtility/DistinctBy.md)(…) | Returns distinct elements from a sequence based on a key by using the default equality comparer. (2 methods) |
| static [Downcast&lt;TSource,TDest&gt;](EnumerableUtility/Downcast.md)(…) | Enumerates the specified collection, casting each element to a derived type. |
| static [EmptyIfNull](EnumerableUtility/EmptyIfNull.md)(…) | Returns the specified sequence, or an empty sequence if it is null. |
| static [EmptyIfNull&lt;T&gt;](EnumerableUtility/EmptyIfNull.md)(…) | Returns the specified sequence, or an empty sequence if it is null. |
| static [Enumerate&lt;T&gt;](EnumerableUtility/Enumerate.md)(…) | Enumerates the specified argument. (2 methods) |
| static [EnumerateBatches&lt;T&gt;](EnumerableUtility/EnumerateBatches.md)(…) | Enumerates a sequence of elements in batches. (2 methods) |
| static [ExactlyOneOrDefault&lt;T&gt;](EnumerableUtility/ExactlyOneOrDefault.md)(…) | Like SingleOrDefault, but doesn't throw an exception if there is more than one (instead returns default value). (2 methods) |
| static [Except&lt;T&gt;](EnumerableUtility/Except.md)(…) | Makes distinct and then removes a single item from a sequence. (2 methods) |
| static [FirstOrDefault&lt;T&gt;](EnumerableUtility/FirstOrDefault.md)(…) | Returns the first element of a sequence or a default value if no such element is found. (2 methods) |
| static [ForEach&lt;T&gt;](EnumerableUtility/ForEach.md)(…) | Executes the specified action for each item in the sequence. (2 methods) |
| static [GroupConsecutiveBy&lt;TSource,TKey&gt;](EnumerableUtility/GroupConsecutiveBy.md)(…) | Groups the elements of a sequence according to a specified key selector function. Each group consists of *consecutive* items having the same key. Order is preserved. |
| static [GroupConsecutiveByTimespan&lt;TSource&gt;](EnumerableUtility/GroupConsecutiveByTimespan.md)(…) | Groups the elements of a sequence that occur near each other in time. Items are presumed to be in chronological order and an item is considered part of the group if the time between the previous item is within the provided time span. Thus, the total time between the first and last items in the group may be greater than the time span. |
| static [Intersperse&lt;T&gt;](EnumerableUtility/Intersperse.md)(…) | Intersperses the specified value between the elements of the source collection. |
| static [IsNullOrEmpty](EnumerableUtility/IsNullOrEmpty.md)(…) | Returns `true` if the sequence is null or contains no elements. |
| static [IsNullOrEmpty&lt;T&gt;](EnumerableUtility/IsNullOrEmpty.md)(…) | Returns `true` if the sequence is null or contains no elements. |
| static [IsSorted&lt;T&gt;](EnumerableUtility/IsSorted.md)(…) | Determines whether the specified sequence is sorted. (2 methods) |
| static [LongestCommonSlice&lt;T&gt;](EnumerableUtility/LongestCommonSlice.md)(…) |  |
| static [Max&lt;T&gt;](EnumerableUtility/Max.md)(…) | Finds the maximum element in the specified collection, using the specified comparison. |
| static [Merge&lt;T&gt;](EnumerableUtility/Merge.md)(…) | Lazily merges two sorted sequences, maintaining sort order. Does not remove duplicates. |
| static [NullableSum](EnumerableUtility/NullableSum.md)(…) | Returns the sum of a sequence of nullable values. (2 methods) |
| static [NullIfEmpty&lt;T&gt;](EnumerableUtility/NullIfEmpty.md)(…) | Returns the specified sequence, or null if it is empty. |
| static [Order&lt;T&gt;](EnumerableUtility/Order.md)(…) | Sorts the sequence. A shortcut for OrderBy(x =&gt; x). (2 methods) |
| static [Range&lt;T&gt;](EnumerableUtility/Range.md)(…) | Returns a range of values. |
| static [RangeTo](EnumerableUtility/RangeTo.md)(…) | Returns a range of integers. |
| static [RangeTo&lt;T&gt;](EnumerableUtility/RangeTo.md)(…) | Returns a range of values. |
| static [SequenceCompare&lt;T&gt;](EnumerableUtility/SequenceCompare.md)(…) | Compares two sequences. (2 methods) |
| static [SequenceHashCode&lt;T&gt;](EnumerableUtility/SequenceHashCode.md)(…) | Gets the hash code for a sequence. (2 methods) |
| static [SplitIntoBins&lt;T&gt;](EnumerableUtility/SplitIntoBins.md)(…) | Splits the *seq* sequence into *nBinCount* equal-sized bins. If *nBinCount* does not evenly divide the total element count, then the first (total count % *nBinCount*) bins will have one more element than the following bins. |
| static [TakeLast&lt;T&gt;](EnumerableUtility/TakeLast.md)(…) | Returns the specified number of items from the end of the sequence. |
| static [ToReadOnlyCollection&lt;T&gt;](EnumerableUtility/ToReadOnlyCollection.md)(…) | Represents the sequence as a ReadOnlyCollection. |
| static [ToSet&lt;T&gt;](EnumerableUtility/ToSet.md)(…) | Returns a new set of the elements in the specified sequence. (2 methods) |
| static [ToStrings](EnumerableUtility/ToStrings.md)(…) | Converts the sequence to a sequence of strings by calling ToString on each item. |
| static [ToStrings&lt;T&gt;](EnumerableUtility/ToStrings.md)(…) | Converts the sequence to a sequence of strings by calling ToString on each item. |
| static [TrimEndWhere&lt;T&gt;](EnumerableUtility/TrimEndWhere.md)(…) | Removes items from the end of the specified sequence that match the given predicate. |
| static [TryFirst&lt;T&gt;](EnumerableUtility/TryFirst.md)(…) |  (2 methods) |
| static [Upcast&lt;TSource,TDest&gt;](EnumerableUtility/Upcast.md)(…) | Enumerates the specified collection, casting each element to a base type. |
| static [WhereNotNull&lt;T&gt;](EnumerableUtility/WhereNotNull.md)(…) | Enumerates the specified collection, returning all the elements that are not null. (2 methods) |
| static [Zip&lt;T1,T2&gt;](EnumerableUtility/Zip.md)(…) | Combines two same sized sequences. |
| static [ZipTruncate&lt;T1,T2&gt;](EnumerableUtility/ZipTruncate.md)(…) | Combines two sequences. |

## See Also

* namespace [Faithlife.Utility](../Faithlife.Utility.md)
* [EnumerableUtility.cs](https://github.com/Faithlife/FaithlifeUtility/tree/master/src/Faithlife.Utility/EnumerableUtility.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
