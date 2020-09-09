using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Faithlife.Utility
{
	/// <summary>
	/// Methods for manipulating lists.
	/// </summary>
	public static class ListUtility
	{
		/// <summary>
		/// Wraps a ReadOnlyCollection around the specified list.
		/// </summary>
		/// <typeparam name="T">The type of item in the list.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns>A ReadOnlyCollection that wraps the list.</returns>
		public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list) => new ReadOnlyCollection<T>(list);

		/// <summary>
		/// Searches a sorted list for a key via binary search. The list must be sorted
		/// by the ordering in the passed comparison.
		/// </summary>
		/// <param name="list">The sorted list to search.</param>
		/// <param name="key">The key to search for.</param>
		/// <param name="compare">The comparison.</param>
		/// <param name="index">Returns the first index at which the key can be found. If the return
		/// value is zero, indicating that <paramref name="key"/> was not present in the list, then this
		/// returns the index at which <paramref name="key"/> could be inserted to maintain the sorted
		/// order of the list.</param>
		/// <returns>The number of keys equal to <paramref name="key"/> that appear in the list.</returns>
		/// <remarks>Modified from similar function in PowerCollections, copyright (c) 2004-2005, Wintellect.</remarks>
		public static int BinarySearchForKey<TItem, TKey>(IReadOnlyList<TItem> list, TKey key, Func<TItem, TKey, int> compare, out int index)
		{
			if (list is null)
				throw new ArgumentNullException(nameof(list));
			if (compare is null)
				throw new ArgumentNullException(nameof(compare));

			var l = 0;
			var r = list.Count;

			while (r > l)
			{
				var m = l + (r - l) / 2;
				var middleItem = list[m];
				var comp = compare(middleItem, key);
				if (comp < 0)
				{
					// middleItem < key
					l = m + 1;
				}
				else if (comp > 0)
				{
					r = m;
				}
				else
				{
					// Found something equal to key at m. Now we need to find the start and end of this run of equal keys.
					int lFound = l, rFound = r, found = m;

					// Find the start of the run.
					l = lFound;
					r = found;
					while (r > l)
					{
						m = l + (r - l) / 2;
						middleItem = list[m];
						comp = compare(middleItem, key);
						if (comp < 0)
						{
							// middleItem < key
							l = m + 1;
						}
						else
						{
							r = m;
						}
					}
					Debug.Assert(l == r, "l == r");
					index = l;

					// Find the end of the run.
					l = found;
					r = rFound;
					while (r > l)
					{
						m = l + (r - l) / 2;
						middleItem = list[m];
						comp = compare(middleItem, key);
						if (comp <= 0)
						{
							// middleItem <= key
							l = m + 1;
						}
						else
						{
							r = m;
						}
					}
					Debug.Assert(l == r, "l == r");
					return l - index;
				}
			}

			// We did not find the key. l and r must be equal. 
			Debug.Assert(l == r, "l == r");
			index = l;
			return 0;
		}

		/// <summary>
		/// Returns the index of the first item that matches the predicate.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="match">The delegate that defines the conditions of the element to search for.</param>
		public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> match) => FindIndex(list, 0, match);

		/// <summary>
		/// Returns the index of the first item that matches the predicate.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="match">The delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		public static int FindIndex<T>(this IReadOnlyList<T> list, int startIndex, Func<T, bool> match)
		{
			// check arguments
			if (list is null)
				throw new ArgumentNullException(nameof(list));
			if (match is null)
				throw new ArgumentNullException(nameof(match));
			if (startIndex < 0 || startIndex > list.Count)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			var count = list.Count;
			for (var index = startIndex; index < count; index++)
				if (match(list[index]))
					return index;
			return -1;
		}

		/// <summary>
		/// Removes items from the list that match the specified predicate.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The predicate that determines the items to remove.</param>
		/// <returns>The number of items removed from the collection.</returns>
		public static int RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
		{
			// check arguments
			if (list is null)
				throw new ArgumentNullException(nameof(list));
			if (predicate is null)
				throw new ArgumentNullException(nameof(predicate));

			// remove items that match
			var originalCount = list.Count;
			var count = originalCount;
			var index = 0;
			while (index < count)
			{
				if (predicate(list[index]))
				{
					list.RemoveAt(index);
					count--;
				}
				else
				{
					index++;
				}
			}

			return originalCount - count;
		}
	}
}
