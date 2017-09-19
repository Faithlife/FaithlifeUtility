using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
		public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
		{
			return new ReadOnlyCollection<T>(list);
		}

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
		public static int BinarySearchForKey<TItem, TKey>(IList<TItem> list, TKey key, Func<TItem, TKey, int> compare, out int index)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (compare == null)
				throw new ArgumentNullException(nameof(compare));

			int l = 0;
			int r = list.Count;

			while (r > l)
			{
				int m = l + (r - l) / 2;
				TItem middleItem = list[m];
				int comp = compare(middleItem, key);
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
					System.Diagnostics.Debug.Assert(l == r, "l == r");
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
					System.Diagnostics.Debug.Assert(l == r, "l == r");
					return l - index;
				}
			}

			// We did not find the key. l and r must be equal. 
			System.Diagnostics.Debug.Assert(l == r, "l == r");
			index = l;
			return 0;
		}

		/// <summary>
		/// Copies a range of elements from the IList{T} to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <typeparam name="T">Type of elements in the list.</typeparam>
		/// <param name="list">The list from which to copy.</param>
		/// <param name="index">The zero-based index in the source IList{T} at which copying begins..</param>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the IList{T}.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <param name="count">The number of elements to copy.</param>
		public static void CopyTo<T>(this IList<T> list, int index, T[] array, int arrayIndex, int count)
		{
			// use List<T>.CopyTo if available
			List<T> sourceList = list as List<T>;
			if (sourceList != null)
			{
				sourceList.CopyTo(index, array, arrayIndex, count);
				return;
			}

			// use Array.Copy if possible
			T[] sourceArray = list as T[];
			if (sourceArray != null)
			{
				Array.Copy(sourceArray, index, array, arrayIndex, count);
				return;
			}

			// fall back to a manual implementation
			CollectionImpl.CheckCopyToParameters(array, arrayIndex, count);
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (list.Count - index < count)
				throw new ArgumentException(OurMessages.Argument_InvalidIndexCount);

			for (int nItem = 0; nItem < count; nItem++)
				array[nItem + arrayIndex] = list[nItem + index];
		}

		/// <summary>
		/// Creates an empty read only collection.
		/// </summary>
		/// <returns>An empty read only collection.</returns>
		public static ReadOnlyCollection<T> CreateReadOnlyCollection<T>()
		{
			return EmptyReadOnlyCollection<T>.Instance;
		}

		/// <summary>
		/// Creates a read only collection of the specified items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <returns>A read only collection of the specified items.</returns>
		/// <remarks>The supplied array is wrapped by the read only collection, so be sure not to modify
		/// the array after it is passed to this method.</remarks>
		public static ReadOnlyCollection<T> CreateReadOnlyCollection<T>(params T[] items)
		{
			return new ReadOnlyCollection<T>(items);
		}

		/// <summary>
		/// Returns the specified read-only collection, or an empty read-only collection if it is null.
		/// </summary>
		/// <typeparam name="T">The type of object in the read-only collection.</typeparam>
		/// <param name="list">The read-only collection.</param>
		/// <returns>The specified read-only collection, or an empty read-only collection if it is null.</returns>
		public static ReadOnlyCollection<T> EmptyIfNull<T>(this ReadOnlyCollection<T> list)
		{
			return list ?? new T[0].AsReadOnly();
		}

		/// <summary>
		/// Returns the index of the first item that matches the predicate.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="match">The delegate that defines the conditions of the element to search for.</param>
		public static int FindIndex<T>(this IList<T> list, Func<T, bool> match)
		{
			return FindIndex(list, 0, match);
		}

		/// <summary>
		/// Returns the index of the first item that matches the predicate.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="match">The delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
		public static int FindIndex<T>(this IList<T> list, int startIndex, Func<T, bool> match)
		{
			// check arguments
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (match == null)
				throw new ArgumentNullException(nameof(match));
			if (startIndex < 0 || startIndex > list.Count)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			int nCount = list.Count;
			for (int nIndex = startIndex; nIndex < nCount; nIndex++)
				if (match(list[nIndex]))
					return nIndex;
			return -1;
		}

		/// <summary>
		/// Yields the items of the list in reverse order.
		/// </summary>
		/// <typeparam name="T">The type of item in the list.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns>A sequence of the items in reverse order.</returns>
		public static IEnumerable<T> InReverse<T>(this IList<T> list)
		{
			int nCount = list.Count;
			for (int nIndex = nCount - 1; nIndex >= 0; nIndex--)
				yield return list[nIndex];
		}

		/// <summary>
		/// Searches a sorted list for a key with a linear search. The list should be sorted by the ordering in the passed comparison.
		/// </summary>
		/// <param name="list">The sorted list to search.</param>
		/// <param name="key">The key to search for.</param>
		/// <param name="compare">The comparison.</param>
		/// <param name="index">Returns the first index at which the key can be found. If the return
		/// value is zero, indicating that <paramref name="key"/> was not present in the list, then this
		/// returns the index at which <paramref name="key"/> could be inserted to maintain the sorted
		/// order of the list.</param>
		/// <returns>The number of keys equal to <paramref name="key"/> that appear in the list.</returns>
		public static int LinearSearchForKey<TItem, TKey>(IList<TItem> list, TKey key, Func<TItem, TKey, int> compare, out int index)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (compare == null)
				throw new ArgumentNullException(nameof(compare));

			// walk list, looking for items
			int nFound = 0;
			for (int nItem = 0; nItem < list.Count; nItem++)
			{
				// compare current item to key
				int nCompare = compare(list[nItem], key);

				// found an item equal to the key
				if (nCompare == 0)
					nFound++;

				// found an item greater than the key; assume no more items can be found
				if (nCompare > 0)
				{
					index = nItem - nFound;
					return nFound;
				}
			}

			// item was not found
			index = list.Count - nFound;
			return nFound;
		}

		/// <summary>
		/// Returns the item at the end of the specified list.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns>The item at the end of the specified list.</returns>
		public static T Peek<T>(this IList<T> list)
		{
			int nCount = list.Count;
			Verify.IsTrue(nCount != 0);
			return list[nCount - 1];
		}

		/// <summary>
		/// Pops an item from the end of the specified list.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns>An item from the end of the specified list.</returns>
		public static T Pop<T>(this IList<T> list)
		{
			int nCount = list.Count;
			Verify.IsTrue(nCount != 0);
			T item = list[nCount - 1];
			list.RemoveAt(nCount - 1);
			return item;
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
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			// remove items that match
			int nOriginalCount = list.Count;
			int nCount = nOriginalCount;
			int nIndex = 0;
			while (nIndex < nCount)
			{
				if (predicate(list[nIndex]))
				{
					list.RemoveAt(nIndex);
					nCount--;
				}
				else
				{
					nIndex++;
				}
			}

			return nOriginalCount - nCount;
		}

		/// <summary>
		/// Transforms the elements of a list in place.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="predicate">The converter.</param>
		public static IList<T> TransformInPlace<T>(this IList<T> list, Func<T, T> predicate)
		{
			// check arguments
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));

			int nCount = list.Count;
			for (int nIndex = 0; nIndex < nCount; nIndex++)
				list[nIndex] = predicate(list[nIndex]);

			return list;
		}

		private sealed class EmptyReadOnlyCollection<T> : ReadOnlyCollection<T>
		{
			public static readonly EmptyReadOnlyCollection<T> Instance = new EmptyReadOnlyCollection<T>();

			private EmptyReadOnlyCollection()
				: base(new T[0])
			{
			}
		}
	}
}
