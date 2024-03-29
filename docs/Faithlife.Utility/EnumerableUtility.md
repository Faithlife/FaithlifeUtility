# EnumerableUtility class

Provides methods for manipulating IEnumerable collections.

```csharp
public static class EnumerableUtility
```

## Public Members

| name | description |
| --- | --- |
| static [AreEqual&lt;T&gt;](EnumerableUtility/AreEqual.md)(…) | Returns a value indicating whether the specified sequences are equal. Supports one or both sequences being null. (3 methods) |
| static [AsReadOnlyList&lt;T&gt;](EnumerableUtility/AsReadOnlyList.md)(…) | Represents the sequence as an IReadOnlyList. |
| static [AsSet&lt;T&gt;](EnumerableUtility/AsSet.md)(…) | Returns a set of the elements in the specified sequence. |
| static [CountIsAtLeast&lt;T&gt;](EnumerableUtility/CountIsAtLeast.md)(…) | Returns true if the count is greater than or equal to the specified value. |
| static [CountIsExactly&lt;T&gt;](EnumerableUtility/CountIsExactly.md)(…) | Returns true if the count is as specified. |
| static [CrossProduct&lt;T&gt;](EnumerableUtility/CrossProduct.md)(…) | Returns the Cartesian cross-product of a sequence of sequences. |
| static [CrossProduct&lt;TSequence,TItem&gt;](EnumerableUtility/CrossProduct.md)(…) | Returns the Cartesian cross-product of a sequence of sequences. |
| static [Except&lt;T&gt;](EnumerableUtility/Except.md)(…) | Makes distinct and then removes a single item from a sequence. (2 methods) |
| static [GroupConsecutiveBy&lt;TSource,TKey&gt;](EnumerableUtility/GroupConsecutiveBy.md)(…) | Groups the elements of a sequence according to a specified key selector function. Each group consists of *consecutive* items having the same key. Order is preserved. |
| static [Intersperse&lt;T&gt;](EnumerableUtility/Intersperse.md)(…) | Intersperses the specified value between the elements of the source collection. |
| static [IsSorted&lt;T&gt;](EnumerableUtility/IsSorted.md)(…) | Determines whether the specified sequence is sorted. (2 methods) |
| static [MergeSorted&lt;T&gt;](EnumerableUtility/MergeSorted.md)(…) | Lazily merges two sorted sequences, maintaining sort order. Does not remove duplicates. |
| static [NullableSum](EnumerableUtility/NullableSum.md)(…) | Returns the sum of a sequence of nullable values. (2 methods) |
| static [Order&lt;T&gt;](EnumerableUtility/Order.md)(…) | Sorts the sequence. A shortcut for OrderBy(x =&gt; x). (2 methods) |
| static [SequenceCompare&lt;T&gt;](EnumerableUtility/SequenceCompare.md)(…) | Compares two sequences. (2 methods) |
| static [SequenceHashCode&lt;T&gt;](EnumerableUtility/SequenceHashCode.md)(…) | Gets the hash code for a sequence. (2 methods) |
| static [SplitIntoBins&lt;T&gt;](EnumerableUtility/SplitIntoBins.md)(…) | Splits the *source* sequence into *binCount* equal-sized bins. If *binCount* does not evenly divide the total element count, then the first (total count % *binCount*) bins will have one more element than the following bins. |
| static [TryFirst&lt;T&gt;](EnumerableUtility/TryFirst.md)(…) | Returns true if the sequence is not empty and provides the first element. (2 methods) |
| static [WhereNotNull&lt;T&gt;](EnumerableUtility/WhereNotNull.md)(…) | Enumerates the specified collection, returning all the elements that are not null. (2 methods) |
| static [Zip&lt;T1,T2&gt;](EnumerableUtility/Zip.md)(…) | Combines two same sized sequences. |
| static [ZipTruncate&lt;T1,T2&gt;](EnumerableUtility/ZipTruncate.md)(…) | Combines two sequences. |

## See Also

* namespace [Faithlife.Utility](../Faithlife.Utility.md)
* [EnumerableUtility.cs](https://github.com/Faithlife/FaithlifeUtility/tree/master/src/Faithlife.Utility/EnumerableUtility.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Utility.dll -->
